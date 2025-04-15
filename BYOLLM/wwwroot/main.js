function toggleConfigPanel(configPanel, configArrow) {
    const isExpanded = configPanel.classList.contains('expanded');

    if (isExpanded) {
        // Collapse
        configPanel.classList.remove('expanded');
        configPanel.classList.add('opacity-0');
        configPanel.style.maxHeight = '0px';
        configArrow.classList.remove('rotate-180');
    } else {
        // Expand
        configPanel.classList.add('expanded');
        configPanel.classList.remove('opacity-0');
        configPanel.style.maxHeight = configPanel.scrollHeight + 'px';
        configArrow.classList.add('rotate-180');
    }
}

// Initialize config panel as expanded by default
function initConfigPanel(configPanel, configArrow) {
    configData = {
        modelUrl: document.getElementById('model-url').value,
        modelName: document.getElementById('model-name').value,
        apiKey: document.getElementById('api-key').value
    }
    if (!configData.modelUrl || configData.modelUrl === '' || !configData.modelName || configData.modelName === '' || !configData.apiKey || configData.apiKey === '') {
        toggleConfigPanel(configPanel, configArrow);
    }
}

// Message Sending
async function addMessage(messagesContainer, text, type = 'user') {
    const messageElement = document.createElement('div');
    messageElement.classList.add(
        'p-2', 'rounded', 'max-w-[80%]', 'border',
        type === 'user' ? 'bg-white' : 'bg-gray-200',
        type === 'user' ? 'border-black' : 'border-gray-400',
        type === 'user' ? 'self-end' : 'self-start'
    );
    messageElement.textContent = text;
    messagesContainer.appendChild(messageElement);
    messagesContainer.scrollTop = messagesContainer.scrollHeight;

    if (type === 'user') {
        var body = JSON.stringify({ message: "SendNewUserMessage", data: text });
        publishMessage("SendNewUserMessage", text);
    }
}

async function checkIfDarkMode() {
    let documentsResponse = await fetch(`./theme`);
    let theme = await documentsResponse.json();
    return theme === "Dark";
}

async function init() {
    const themeToggle = document.getElementById('theme-toggle');
    const messageInput = document.getElementById('message-input');
    const sendBtn = document.getElementById('send-btn');
    const messagesContainer = document.getElementById('messages');
    const connectBtn = document.getElementById('connect-btn');
    const configToggle = document.getElementById('config-toggle');
    const configPanel = document.getElementById('config-panel');
    const configArrow = document.getElementById('config-arrow');

    // Theme Toggle
    themeToggle.addEventListener('click', () => {
        document.documentElement.classList.toggle('dark');
    });

    // Config Panel Toggle
    configToggle.addEventListener('click', () => {
        toggleConfigPanel(configPanel, configArrow)
    });

    sendBtn.addEventListener('click', () => {
        const message = messageInput.value.trim();
        if (message) {
            addMessage(messagesContainer, message, 'user');
            messageInput.value = '';
        }
    });

    messageInput.addEventListener('keypress', (e) => {
        if (e.key === 'Enter') {
            sendBtn.click();
        }
    });

    // Connection Configuration
    connectBtn.addEventListener('click', () => {
        const modelUrl = document.getElementById('model-url').value;
        const modelName = document.getElementById('model-name').value;

        if (modelUrl && modelName) {
            alert(`Connected to:\nURL: ${modelUrl}\nModel: ${modelName}`);
        } else {
            alert('Please enter model URL and name');
        }
    });

    initConfigPanel(configPanel, configArrow);
    let isDarkMode = await checkIfDarkMode();
    if (isDarkMode) {
        document.documentElement.classList.toggle('dark');
    }
}

function publishMessage(message, data) {
    if (window.chrome?.webview) {
        window.chrome.webview.postMessage({ message, data });
        //window.postMessage({ message, data });
    } else if (window.webkit?.messageHandlers.studioPro) {
        window.webkit.messageHandlers.studioPro.postMessage(JSON.stringify({ message, data }))
    }
}

// Register message handler.
if (window.chrome?.webview) {
    window.chrome?.webview.addEventListener("message", (event) => {
        const message = event.data;
        console.info("New message", message);
        this.handleMessage(message);
    });
} else {
    window.WKPostMessage = json => {
        const wkMessage = JSON.parse(json);
        this.handleMessage(wkMessage.data);
    };
}
publishMessage("MessageListenerRegistered");

//window.WKPostMessage = json => {
//    // To avoid issues serializing data, we JSONify the data when posting.
//    const wkMessage = JSON.parse(json);
//    const message = wkMessage["message"];
//    const data = wkMessage["data"];

//    if (typeof message === "string") {
//        handleMessage({ message, data });
//    }
//};

function handleMessage(event) {
    const { message, data } = event.data;
    console.info("Received message:", message, data);
}

document.addEventListener('DOMContentLoaded', async () => {
    await init();
});
