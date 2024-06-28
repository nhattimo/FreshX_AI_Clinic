# region : Thư viện
from flask import Flask, request, jsonify, send_from_directory
import os
from dotenv import load_dotenv
from flask_cors import CORS
from langchain_core.messages import AIMessage, HumanMessage
from langchain_core.prompts import ChatPromptTemplate
from langchain_core.runnables import RunnablePassthrough
from langchain_community.utilities import SQLDatabase
from langchain_core.output_parsers import StrOutputParser
from langchain_openai import ChatOpenAI
import requests
from datetime import datetime
# endregion

# region : Cấu hình
app = Flask(__name__)
CORS(app)  # Bật CORS cho tất cả các route
# Tải các biến môi trường từ tệp .env (Load environment variables)
load_dotenv()

# Hàm khởi tạo kết nối cơ sở dữ liệu. (Initialize the database)
def init_database(user: str, password: str, host: str, port: str, database: str) -> SQLDatabase:
    db_uri = f"mysql+mysqlconnector://{user}:{password}@{host}:{port}/{database}"
    return SQLDatabase.from_uri(db_uri)

# Khởi tạo kết nối cơ sở dữ liệu
db = init_database(
    os.getenv("DB_USER"),
    os.getenv("DB_PASSWORD"),
    os.getenv("DB_HOST"),
    os.getenv("DB_PORT"),
    os.getenv("DB_NAME")
)

# Khởi tạo mô hình ngôn ngữ toàn cục
llm = ChatOpenAI(model="gpt-3.5-turbo")

# endregion

# region : Tạo hàm để phân tích và xử lý yêu cầu từ người dùng 

def get_sql_chain(db):
    template = """
    <SCHEMA>{schema}</SCHEMA>

    Lịch sử hội thoại: {chat_history}

    Chỉ viết truy vấn SQL và tư vấn về sức khỏe.

    Ví dụ:
    Câu hỏi: Tôi đau bụng?
    Truy vấn SQL: SELECT c.mo_ta AS diagnosis_description, l.noi_dung AS advice 
                  FROM chan_doan c
                  JOIN trieu_chung_chan_doan tccd ON c.ma_chan_doan = tccd.ma_chan_doan
                  JOIN loi_khuyen l ON c.ma_chan_doan = l.ma_chan_doan
                  WHERE tccd.ma_trieu_chung IN (SELECT ma_trieu_chung FROM trieu_chung WHERE ten_trieu_chung LIKE '%{schema}%')
                    ;
    Câu hỏi: những món ăn tốt cho rối loạn tiêu hóa
    Truy vấn SQL: SELECT 
    m.ten_mon_an, 
    m.cong_thuc
    FROM 
        mon_an m
    JOIN 
        chan_doan c ON m.ma_chan_doan = c.ma_chan_doan
    JOIN 
        trieu_chung_chan_doan tccd ON c.ma_chan_doan = tccd.ma_chan_doan
    JOIN 
        trieu_chung tc ON tccd.ma_trieu_chung = tc.ma_trieu_chung
    WHERE 
        tc.ten_trieu_chung LIKE '%{schema}%'
    ;

    Lượt của bạn:

    Câu hỏi: {question}
    Truy vấn SQL:
    """
    prompt = ChatPromptTemplate.from_template(template)

    def get_schema(_):
        return db.get_table_info()
    return (
        RunnablePassthrough.assign(schema=get_schema)
        | prompt
        | llm
        | StrOutputParser()
    )

def get_response(user_query: str, db: SQLDatabase, chat_history: list):
    sql_chain = get_sql_chain(db)
    template = """
    Bạn là một trợ lý ảo tên là FreshX tại một phòng khám FreshX . 
    Bạn đang tương tác với một người dùng, người đang hỏi bạn các câu hỏi liên qua đến bệnh tình và các bệnh tình đó sẽ có trong cơ sở dữ liệu của phòng khám để nhận tư vấn sức khỏe.
    Dựa trên sơ đồ bảng dưới đây, câu hỏi của người dùng, truy vấn SQL và phản hồi SQL, hãy viết một phản hồi tự nhiên. 
    Bạn có thể tự mình thêm vào những thứ tốt đẹp cho người dùng. phản hồi của bạn phải thật là tốt
    <SCHEMA>{schema}</SCHEMA>

    Lịch sử hội thoại: {chat_history}
    Truy vấn SQL: <SQL>{query}</SQL>
    Câu hỏi của người dùng: {question}
    Phản hồi SQL: {response}

    Phản hồi của bạn:
    """
    prompt = ChatPromptTemplate.from_template(template)
    
    chain = (
        RunnablePassthrough.assign(query=sql_chain).assign(
            schema=lambda _: db.get_table_info(),
            response=lambda vars: db.run(vars["query"]),
        )
        | prompt
        | llm
        | StrOutputParser()
    )
    
    response = chain.invoke({
        "question": user_query,
        "chat_history": chat_history,
    })

    # Nếu phản hồi không chứa kết quả hợp lệ, trả về câu trả lời mặc định và tự động đặt lịch khám
    if "Không tìm thấy" in response:
        response = """
        Xin lỗi, tôi không thể đưa ra chẩn đoán chính xác cho tình trạng của bạn qua trò chuyện này. Tôi rất tiếc khi dữ liệu của tôi chỉ có giới hạn nhưng bạn đừng lo, tôi có thể giúp bạn.
        Để đảm bảo an toàn và sức khỏe của bạn, tôi khuyên bạn nên đến trực tiếp Phòng khám FreshX tại:
        Địa chỉ: 116 Nguyễn Huy Tưởng, Hòa An, Liên Chiểu, Đà Nẵng
        Số điện thoại: 0857075999
        Bác sĩ tại phòng khám sẽ kiểm tra và đưa ra chẩn đoán cùng phương án điều trị phù hợp nhất cho bạn. Bạn có cần hỗ trợ thêm điều gì khác không?
        """
        auto_schedule_appointment(db, user_query)

    return response

def get_symptoms_advice(symptoms: str, db: SQLDatabase):
    query = f"""
    SELECT 
        c.ten_chan_doan, 
        c.mo_ta AS diagnosis_description, 
        l.noi_dung AS advice,
        b.noi_dung AS exercise,
        m.ten_mon_an, 
        m.cong_thuc AS recipe,
        chu_y.noi_dung AS note
    FROM 
        chan_doan c
    JOIN 
        trieu_chung_chan_doan tccd ON c.ma_chan_doan = tccd.ma_chan_doan
    JOIN 
        loi_khuyen l ON c.ma_chan_doan = l.ma_chan_doan
    LEFT JOIN 
        bai_tap b ON c.ma_chan_doan = b.ma_chan_doan
    LEFT JOIN 
        mon_an m ON c.ma_chan_doan = m.ma_chan_doan
    LEFT JOIN 
        chu_y ON c.ma_chan_doan = chu_y.ma_chan_doan
    WHERE 
        tccd.ma_trieu_chung IN (SELECT ma_trieu_chung FROM trieu_chung WHERE ten_trieu_chung LIKE '%{symptoms}%')
    ;
    """
    return db.run(query)

def get_response_with_advice(user_query: str, db: SQLDatabase, chat_history: list):
    response = get_response(user_query, db, chat_history)
    advice = get_symptoms_advice(user_query, db)
    full_response = f"{response}\n\nTư vấn: {advice}"
    return full_response

def auto_schedule_appointment(db: SQLDatabase, ly_do: str):
    ma_benh_nhan = "default_patient_id"  # Thay thế bằng mã bệnh nhân thực tế hoặc logic để lấy mã bệnh nhân
    trang_thai = "pending"

    # Lấy ngày và giờ hiện tại
    now = datetime.now()
    ngay_kham = now.date()
    gio_kham = now.time()

    query = """
    INSERT INTO dat_lich_kham (ma_benh_nhan, ngay_kham, gio_kham, ly_do, trang_thai)
    VALUES (%s, %s, %s, %s, %s)
    """
    params = (ma_benh_nhan, ngay_kham, gio_kham, ly_do, trang_thai)
    db.run(query, params)

@app.route('/chat', methods=['POST'])
def chat():
    data = request.get_json()
    user_query = data.get('message')
    chat_history = data.get('chat_history', [])

    response = get_response_with_advice(user_query, db, chat_history)
    chat_history.append({"role": "human", "content": user_query})
    chat_history.append({"role": "ai", "content": response})

    return jsonify({
        "response": response,
        "chat_history": chat_history
    })

@app.route('/add_appointment', methods=['POST'])
def add_appointment():
    data = request.get_json()
    ma_benh_nhan = data.get('ma_benh_nhan')
    ly_do = data.get('ly_do')
    trang_thai = data.get('trang_thai', 'pending')  # Default status is pending

    # Lấy ngày và giờ hiện tại
    now = datetime.now()
    ngay_kham = now.date()  # Ngày hiện tại
    gio_kham = now.time()  # Giờ hiện tại

    query = """
    INSERT INTO dat_lich_kham (ma_benh_nhan, ngay_kham, gio_kham, ly_do, trang_thai)
    VALUES (%s, %s, %s, %s, %s)
    """
    params = (ma_benh_nhan, ngay_kham, gio_kham, ly_do, trang_thai)
    db.run(query, params)

    return jsonify({"message": "Lịch hẹn đã được thêm thành công"}), 201

@app.route('/add_diagnosis_note', methods=['POST'])
def add_diagnosis_note():
    data = request.get_json()
    ma_benh_nhan = data.get('ma_benh_nhan')
    ma_chan_doan = data.get('ma_chan_doan')
    tinh_trang_benh = data.get('tinh_trang_benh')

    query = """
    INSERT INTO ghi_chu_benh (ma_benh_nhan, ma_chan_doan, tinh_trang_benh, ngay_ghi_chu)
    VALUES (%s, %s, %s, NOW())
    """
    params = (ma_benh_nhan, ma_chan_doan, tinh_trang_benh)
    db.run(query, params)

    return jsonify({"message": "Ghi chú bệnh đã được thêm thành công"}), 201


@app.route('/test', methods=['GET'])
def test():
    return "Flask server is running"

if __name__ == '__main__':
    app.run(port=5000, debug=True)
