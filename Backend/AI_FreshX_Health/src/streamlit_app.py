import streamlit as st
import requests

# Thiết lập cấu hình trang
st.set_page_config(page_title="FreshX", page_icon=":speech_balloon:")

st.title("FreshX")

# Khởi tạo lịch sử hội thoại nếu chưa có
if "chat_history" not in st.session_state:
    st.session_state.chat_history = []

# Hiển thị lịch sử hội thoại
for message in st.session_state.chat_history:
    if message['role'] == 'ai':
        with st.chat_message("AI"):
            st.markdown(message['content'])
    elif message['role'] == 'human':
        with st.chat_message("Human"):
            st.markdown(message['content'])

# Nhận đầu vào từ người dùng
user_query = st.chat_input("Type a message...")
if user_query:
    st.session_state.chat_history.append({"role": "human", "content": user_query})
    
    # Gửi yêu cầu tới Flask server
    response = requests.post("http://localhost:5000/chat", json={
        "message": user_query,
        "chat_history": st.session_state.chat_history
    })
    
    if response.status_code == 200:
        data = response.json()
        st.session_state.chat_history = data['chat_history']
        with st.chat_message("AI"):
            st.markdown(data['response'])
    else:
        st.error("Something went wrong with the Flask server")
