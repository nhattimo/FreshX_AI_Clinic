import streamlit as st
from dotenv import load_dotenv
from langchain_core.messages import AIMessage, HumanMessage
from langchain_core.prompts import ChatPromptTemplate
from langchain_core.runnables import RunnablePassthrough
from langchain_community.utilities import SQLDatabase
from langchain_core.output_parsers import StrOutputParser
from langchain_openai import ChatOpenAI  # Sử dụng đúng module cho chat models
import tiktoken

def init_database(user: str, password: str, host: str, port: str, database: str) -> SQLDatabase:
    db_uri = f"mysql+mysqlconnector://{user}:{password}@{host}:{port}/{database}"
    return SQLDatabase.from_uri(db_uri)

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
    
    llm = ChatOpenAI(model="gpt-4-turbo")  # Sử dụng mô hình chi phí thấp hơn
    
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
    
    llm = ChatOpenAI(model="gpt-3.5-turbo")  # Đảm bảo mô hình là phiên bản mà bạn có quyền truy cập
    
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

def schedule_appointment(user_id: int, doctor_id: int, appointment_time: str, db: SQLDatabase):
    query = f"INSERT INTO appointments (user_id, doctor_id, appointment_time) VALUES ({user_id}, {doctor_id}, '{appointment_time}')"
    db.run(query)
    return "Lịch khám của bạn đã được lên lịch thành công."

def count_tokens(text, model_name):
    enc = tiktoken.encoding_for_model(model_name)
    tokens = enc.encode(text)
    return len(tokens)

def calculate_total_tokens(chat_history, user_query, response, model_name):
    schema_tokens = count_tokens("<SCHEMA>{schema}</SCHEMA>", model_name)
    chat_history_tokens = count_tokens(f"Lịch sử hội thoại: {chat_history}", model_name)
    query_tokens = count_tokens(f"Truy vấn SQL: <SQL>{user_query}</SQL>", model_name)
    question_tokens = count_tokens(f"Câu hỏi của người dùng: {user_query}", model_name)
    response_tokens = count_tokens(f"Phản hồi SQL: {response}", model_name)

    return schema_tokens + chat_history_tokens + query_tokens + question_tokens + response_tokens

if "chat_history" not in st.session_state:
    st.session_state.chat_history = [
        AIMessage(content="Chào bạn. Tôi có thể giúp bạn về vấn đề sức khỏe hiện tại"),
    ]

if "db" not in st.session_state:
    st.session_state.db = None

load_dotenv()

st.set_page_config(page_title="FreshX", page_icon=":speech_balloon:")

st.title("FreshX")

with st.sidebar:
    st.subheader("Settings")
    st.write("This is a simple chat application using MySQL. Connect to the database and start chatting.")
    
    st.text_input("Host", value="localhost", key="Host")
    st.text_input("Port", value="3306", key="Port")
    st.text_input("User", value="root", key="User")
    st.text_input("Password", type="password", value="10042004", key="Password")
    st.text_input("Database", value="FreshX_DB", key="Database")
    
    if st.button("Connect"):
        with st.spinner("Connecting to database..."):
            db = init_database(
                st.session_state["User"],
                st.session_state["Password"],
                st.session_state["Host"],
                st.session_state["Port"],
                st.session_state["Database"]
            )
            st.session_state.db = db
            st.success("Connected to database!")
       
for message in st.session_state.chat_history:
    if isinstance(message, AIMessage):
        with st.chat_message("AI"):
            st.markdown(message.content)
    elif isinstance(message, HumanMessage):
        with st.chat_message("Human"):
            st.markdown(message.content)

user_query = st.chat_input("Type a message...")
if user_query is not None and user_query.strip() != "":
    st.session_state.chat_history.append(HumanMessage(content=user_query))
    
    if st.session_state.db is not None:
        with st.chat_message("AI"):
            response = get_response_with_advice(user_query, st.session_state.db, st.session_state.chat_history)
            st.markdown(response)
            st.session_state.chat_history.append(AIMessage(content=response))
        
        # Tính toán số lượng token
        total_tokens = calculate_total_tokens(st.session_state.chat_history, user_query, response, "gpt-3.5-turbo")
        st.write(f"Total tokens: {total_tokens}")

    # In ra dữ liệu mà bạn truyền vào cho API
    st.write("Data sent to API:")
    st.json({
        "question": user_query,
        "chat_history": st.session_state.chat_history
    })

