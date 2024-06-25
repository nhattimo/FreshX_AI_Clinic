import os
from dotenv import load_dotenv
from langchain_core.messages import AIMessage, HumanMessage
from langchain_core.prompts import ChatPromptTemplate
from langchain_core.runnables import RunnablePassthrough
from langchain_community.utilities import SQLDatabase 
from langchain_core.output_parsers import StrOutputParser
import streamlit as st
from transformers import GPT2LMHeadModel, GPT2Tokenizer
import torch

def init_database(user: str, password: str, host: str, port: str, database: str) -> SQLDatabase:
    db_uri = f"mysql+mysqlconnector://{user}:{password}@{host}:{port}/{database}"
    return SQLDatabase.from_uri(db_uri)

def get_sql_chain(db):
    template = """
    You are a data analyst at a company. You are interacting with a user who is asking you questions about the company's database.
    Based on the table schema below, write a SQL query that would answer the user's question. Take the conversation history into account.
    
    <SCHEMA>{schema}</SCHEMA>
    
    Conversation History: {chat_history}
    
    Write only the SQL query and nothing else. Do not wrap the SQL query in any other text, not even backticks.
    
    For example:
    Question: which 3 artists have the most tracks?
    SQL Query: SELECT ArtistId, COUNT(*) as track_count FROM Track GROUP BY ArtistId ORDER BY track_count DESC LIMIT 3;
    Question: Name 10 artists
    SQL Query: SELECT Name FROM Artist LIMIT 10;
    
    Your turn:
    
    Question: {question}
    SQL Query:
    """
    
    prompt = ChatPromptTemplate.from_template(template)
    
    model_name = "gpt2"
    tokenizer = GPT2Tokenizer.from_pretrained(model_name)
    model = GPT2LMHeadModel.from_pretrained(model_name)
    
    def generate_response(prompt_text):
        if not isinstance(prompt_text, str):
            prompt_text = str(prompt_text)
        
        input_ids = tokenizer.encode(prompt_text, return_tensors="pt")
        
        # Chia nhỏ đầu vào nếu cần thiết
        max_length = 1024
        if input_ids.size(1) > max_length:
            input_ids = input_ids[:, :max_length]
        
        # In ra đoạn text truyền vào model
        print(f"Input text to model: {prompt_text}")
        
        output = model.generate(input_ids, max_length=150, num_return_sequences=1)
        response_text = tokenizer.decode(output[0], skip_special_tokens=True)
        return response_text
    
    def get_schema(_):
        return db.get_table_info()
    
    return (
        RunnablePassthrough.assign(schema=get_schema)
        | prompt
        | generate_response
        | StrOutputParser()
    )

def get_response(user_query: str, db: SQLDatabase, chat_history: list):
    model_name = "gpt2"
    tokenizer = GPT2Tokenizer.from_pretrained(model_name)
    model = GPT2LMHeadModel.from_pretrained(model_name)
    
    def generate_response(prompt_text):
        if not isinstance(prompt_text, str):
            prompt_text = str(prompt_text)
        
        input_ids = tokenizer.encode(prompt_text, return_tensors="pt")
        
        # Chia nhỏ đầu vào nếu cần thiết
        max_length = 1024
        if input_ids.size(1) > max_length:
            input_ids = input_ids[:, :max_length]
        
        # In ra đoạn text truyền vào model
        print(f"Input text to model: {prompt_text}")
        
        output = model.generate(input_ids, max_length=150, num_return_sequences=1)
        response_text = tokenizer.decode(output[0], skip_special_tokens=True)
        return response_text
    
    # Truyền trực tiếp câu hỏi của người dùng vào mô hình
    response_text = generate_response(user_query)
    
    return response_text

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
    st.text_input("Password", type="password", value="", key="Password")
    st.text_input("Database", value="weborderfilm", key="Database")
    
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
            response = get_response(user_query, st.session_state.db, st.session_state.chat_history)
            st.markdown(response)
        
        st.session_state.chat_history.append(AIMessage(content=response))
