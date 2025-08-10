"use strict";

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub")
    .configureLogging(signalR.LogLevel.Information)
    .build();

const messageInput = document.getElementById("message-input");
const sendButton = document.getElementById("send-button");
const messagesList = document.getElementById("messages-list");
const messageForm = document.getElementById("message-form");

sendButton.disabled = true;

connection.on("ReceiveMessage", (user, message, timestamp) => {
    const li = document.createElement("div");
    li.classList.add("col-start-1", "col-end-8", "p-3", "rounded-lg");

    const flexContainer = document.createElement("div");
    flexContainer.classList.add("flex", "flex-row", "items-center");

    const userAvatar = document.createElement("div");
    userAvatar.classList.add("flex", "items-center", "justify-center", "h-10", "w-10", "rounded-full", "bg-indigo-500", "flex-shrink-0", "text-white");
    userAvatar.textContent = user.substring(0, 1).toUpperCase();

    const messageContainer = document.createElement("div");
    messageContainer.classList.add("relative", "ml-3", "text-sm", "bg-gray-100", "py-2", "px-4", "shadow", "rounded-xl");
    
    const messageHeader = document.createElement("div");
    messageHeader.classList.add("font-bold", "text-gray-800");
    messageHeader.textContent = user;

    const messageText = document.createElement("div");
    messageText.textContent = message;

    messageContainer.appendChild(messageHeader);
    messageContainer.appendChild(messageText);
    flexContainer.appendChild(userAvatar);
    flexContainer.appendChild(messageContainer);
    li.appendChild(flexContainer);

    messagesList.appendChild(li);
    messagesList.scrollTop = messagesList.scrollHeight;
});

connection.start().then(() => {
    sendButton.disabled = false;
}).catch(err => console.error(err.toString()));

messageForm.addEventListener("submit", (event) => {
    event.preventDefault();
    const message = messageInput.value;
    if (message.trim() !== "") {
        connection.invoke("SendMessage", message).catch(err => console.error(err.toString()));
        messageInput.value = "";
        messageInput.focus();
    }
});