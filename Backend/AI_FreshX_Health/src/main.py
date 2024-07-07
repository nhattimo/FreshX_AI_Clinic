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


# endregion

# region : Tạo hàm để phân tích và xử lý yêu cầu từ người dùng 

# Sử dụng mô hình ngôn ngữ để phân tích câu hỏi và trích xuất thông tin
def get_sql_chain(db):
    # Mẫu (template) này định nghĩa cấu trúc của chuỗi xử lý.
    # <SCHEMA>{schema}</SCHEMA>: Chèn thông tin cấu trúc cơ sở dữ liệu vào đây.
    # Lịch sử hội thoại (chat_history): Giữ ngữ cảnh của các cuộc trò chuyện trước đó.
    # Ví dụ: Cung cấp một vài ví dụ về cách chuyển đổi câu hỏi của người dùng thành truy vấn SQL.
    template = """
    <SCHEMA>{schema}</SCHEMA>

    Lịch sử hội thoại: {chat_history}

    Truy vấn SQL và dựa vào dữ liệu đấy và tham khảo tư vấn về sức khỏe. đưa ra gợi ý về tình trạng bệnh, giải đấp những câu chào hỏi thường ngày, bạn cũng có thể gợi ý thêm và tự sử lý 1 số vấn đề ngoài database

    Ví dụ:
    Câu chào: Chào bạn
    Câu trả lời: Xin chào! Tôi là trợ lý ảo của Phòng khám FreshX. Tên tôi là FreshX. Tôi có thể giúp gì cho bạn hôm nay?

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
    # Tạo một đối tượng từ mẫu đã định nghĩa ở trên. Đối tượng này sẽ được sử dụng để tạo ra lời nhắc cho mô hình ngôn ngữ GPT.
    prompt = ChatPromptTemplate.from_template(template)

    # Hàm này lấy thông tin về cấu trúc các bảng trong cơ sở dữ liệu.
    def get_schema(_):
        return db.get_table_info()
    # RunnablePassthrough.assign(schema=get_schema): Gán thông tin cấu trúc cơ sở dữ liệu vào schema.
    # | prompt: Sử dụng mẫu lời nhắc (prompt) đã tạo.
    # | llm: Sử dụng mô hình ngôn ngữ để xử lý lời nhắc.
    # | StrOutputParser(): Chuyển đổi đầu ra của mô hình thành chuỗi (string).
    return (
        RunnablePassthrough.assign(schema=get_schema)
        | prompt
        | llm
        | StrOutputParser()
    )

# get_response được thiết kế để sử dụng chuỗi xử lý SQL được tạo từ get_sql_chain 
# để xử lý các câu hỏi từ người dùng, truy vấn cơ sở dữ liệu, và sau đó trả về phản hồi tự nhiên.
def get_response(user_query: str, db: SQLDatabase, chat_history: list):
    sql_chain = get_sql_chain(db) # 1. get_sql_chain(db) Tạo ra chuỗi xử lý SQL từ đối tượng cơ sở dữ liệu
                                    # chú thích
                                    # chat_history: Lịch sử hội thoại giữa người dùng và AI.
                                    # user_query: Câu hỏi của người dùng.
                                    # db: Đối tượng cơ sở dữ liệu SQLDatabase.
    # 2. Định nghĩa mẫu phản hồi
    # trong đó chứa các thông tin cần thiết để AI hiểu và xử lý câu hỏi của người dùng, tạo truy vấn SQL và phản hồi lại một cách tự nhiên
    template = """
    Bạn là một trợ lý ảo tên là FreshX tại một phòng khám FreshX . 
    Bạn đang tương tác với một người dùng, người đang hỏi bạn các câu hỏi liên qua đến bệnh tình và các bệnh tình đó sẽ có trong cơ sở dữ liệu của phòng khám để nhận tư vấn sức khỏe, bạn cũng có thể dựa vào cơ sở dữ liệu để tư vấn thêm.
    Dựa trên sơ đồ bảng dưới đây, câu hỏi của người dùng, truy vấn SQL và phản hồi SQL, hãy viết một phản hồi tự nhiên. 
    Bạn có thể tự mình thêm vào những thứ tốt đẹp cho người dùng. phản hồi của bạn phải thật là tốt
    <SCHEMA>{schema}</SCHEMA>

    Lịch sử hội thoại: {chat_history}
    Truy vấn SQL: <SQL>{query}</SQL>
    Câu hỏi của người dùng: {question}
    Phản hồi SQL: {response}

    Phản hồi của bạn:
    """
    # Tạo một đối tượng từ mẫu đã định nghĩa. Đối tượng này sẽ được sử dụng để tạo ra lời nhắc cho mô hình ngôn ngữ GPT
    prompt = ChatPromptTemplate.from_template(template)
    
    # 3. Tạo chuỗi xử lý đầy đủ: Kết hợp lấy thông tin cấu trúc cơ sở dữ liệu, chạy truy vấn SQL, sử dụng mẫu phản hồi và mô hình ngôn ngữ để xử lý.
    chain = (
        RunnablePassthrough.assign(query=sql_chain).assign(
            schema=lambda _: db.get_table_info(),
            response=lambda vars: db.run(vars["query"]),
        )
        | prompt
        | llm
        | StrOutputParser()
    )
    
    # 4. Kích hoạt chuỗi xử lý: Với đầu vào là câu hỏi của người dùng và lịch sử hội thoại, trả về kết quả phản hồi từ AI.
    # chain.invoke: Kích hoạt chuỗi xử lý với đầu vào là câu hỏi của người dùng và lịch sử hội thoại.
    # return: Trả về kết quả phản hồi từ AI.
    return chain.invoke({
        "question": user_query,
        "chat_history": chat_history,
    })

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
    full_response = f"{response}\n\n. {advice}"
    return full_response


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

from datetime import datetime

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


# API Test xem file có chạy đc không
@app.route('/test', methods=['GET'])
def test():
    return "Flask server is running"

if __name__ == '__main__':
    app.run(port=5000, debug=True)

