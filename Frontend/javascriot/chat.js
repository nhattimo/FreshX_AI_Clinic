document.addEventListener("DOMContentLoaded", function () {
    const chatForm = document.querySelector("#chat-form");
    const chatHistoryContainer = document.querySelector("#chat-history");
    let chatHistory = [];

    chatForm.addEventListener("submit", async function (event) {
        event.preventDefault();

        const messageInput = document.querySelector("#message-input");
        const message = messageInput.value.trim();

        if (message === "") return;

        // Append user message to chat history and display
        appendMessage("human", message);
        chatHistory.push({ role: "human", content: message });

        // Send message to server and get response
        const response = await fetch("/chat", {
            method: "POST",
            headers: {
                "Content-Type": "application/x-www-form-urlencoded",
            },
            body: new URLSearchParams({
                message: message,
                chat_history: JSON.stringify(chatHistory),
            }),
        }).then((res) => res.json());

        // Append AI response to chat history and display
        chatHistory.push({ role: "ai", content: response.response });
        appendMessage("ai", response.response);

        messageInput.value = "";
    });

    function appendMessage(role, content) {
        const messageElement = document.createElement("div");
        messageElement.classList.add("chat-message", role);
        messageElement.textContent = content;
        chatHistoryContainer.appendChild(messageElement);
        chatHistoryContainer.scrollTop = chatHistoryContainer.scrollHeight;
    }
});
