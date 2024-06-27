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

app = Flask(__name__)
CORS(app)  # Bật CORS cho tất cả các route

# Tải các biến môi trường từ tệp .env
load_dotenv()

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

def get_sql_chain(db):
    template = """
    <SCHEMA>{schema}</SCHEMA>

    Lịch sử hội thoại: {chat_history}

    Chỉ viết truy vấn SQL và tư vấn về sức khỏe.

    Ví dụ:
    Câu hỏi: Tôi đau bụng?
    Truy vấn SQL: SELECT c.mo_ta AS diagnosis_description, l.noi_dung AS advice FROM chan_doan c
                  JOIN trieu_chung_benh_nhan tcbn ON c.ma_chan_doan = tcbn.ma_trieu_chung
                  JOIN loi_khuyen l ON c.ma_chan_doan = l.ma_chan_doan
                  WHERE tcbn.ma_trieu_chung IN (SELECT ma_trieu_chung FROM trieu_chung WHERE ten_trieu_chung LIKE '%đau bụng%')
                  LIMIT 1;
    Câu hỏi: Tên của 10 bệnh nhân
    Truy vấn SQL: SELECT ho, ten FROM benh_nhan LIMIT 10;

    Lượt của bạn:

    Câu hỏi: {question}
    Truy vấn SQL:
    """
    
    prompt = ChatPromptTemplate.from_template(template)
    
    llm = ChatOpenAI(model="gpt-3.5-turbo")
    
    def get_schema(_):
        return db.get_table_info()
    
    return (
        RunnablePassthrough.assign(schema=get_schema)
        | prompt
        | llm
        | StrOutputParser()
    )

def get_symptoms_advice(symptoms: str, db: SQLDatabase):
    query = f"""
    SELECT c.ten_chan_doan, c.mo_ta, l.noi_dung AS loi_khuyen, b.noi_dung AS bai_tap, m.ten_mon_an, m.cong_thuc
    FROM chan_doan c
    JOIN trieu_chung_benh_nhan tcbn ON c.ma_chan_doan = tcbn.ma_trieu_chung
    JOIN loi_khuyen l ON c.ma_chan_doan = l.ma_chan_doan
    JOIN bai_tap b ON c.ma_chan_doan = b.ma_chan_doan
    JOIN mon_an m ON c.ma_chan_doan = m.ma_chan_doan
    WHERE tcbn.ma_trieu_chung IN (SELECT ma_trieu_chung FROM trieu_chung WHERE ten_trieu_chung LIKE '%{symptoms}%')
    LIMIT 1;
    """
    return db.run(query)

def get_response(user_query: str, db: SQLDatabase, chat_history: list):
    sql_chain = get_sql_chain(db)
    
    template = """
    Bạn là một trợ lý ảo tại một phòng khám. Bạn đang tương tác với một người dùng, người đang hỏi bạn các câu hỏi về cơ sở dữ liệu của phòng khám để nhận tư vấn sức khỏe.
    Dựa trên sơ đồ bảng dưới đây, câu hỏi của người dùng, truy vấn SQL và phản hồi SQL, hãy viết một phản hồi tự nhiên.
    <SCHEMA>{schema}</SCHEMA>

    Lịch sử hội thoại: {chat_history}
    Truy vấn SQL: <SQL>{query}</SQL>
    Câu hỏi của người dùng: {question}
    Phản hồi SQL: {response}

    Phản hồi của bạn:
    """
    
    prompt = ChatPromptTemplate.from_template(template)
    
    llm = ChatOpenAI(model="gpt-3.5-turbo")
    
    chain = (
        RunnablePassthrough.assign(query=sql_chain).assign(
            schema=lambda _: db.get_table_info(),
            response=lambda vars: db.run(vars["query"]),
        )
        | prompt
        | llm
        | StrOutputParser()
    )
    
    return chain.invoke({
        "question": user_query,
        "chat_history": chat_history,
    })

def get_response_with_advice(user_query: str, db: SQLDatabase, chat_history: list):
    response = get_response(user_query, db, chat_history)
    advice = get_symptoms_advice(user_query, db)
    full_response = f"{response}\n\nTư vấn: {advice}"
    return full_response

@app.route('/')
def index():
    return send_from_directory('.', 'index.html')

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

@app.route('/test', methods=['GET'])
def test():
    return "Flask server is running"

if __name__ == '__main__':
    app.run(port=5000, debug=True)
