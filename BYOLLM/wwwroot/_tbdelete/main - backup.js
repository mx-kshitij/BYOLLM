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
//function initConfigPanel(configPanel, configArrow) {
    //configData = {
    //    endpoint: document.getElementById('endpoint').value,
    //    deployment: document.getElementById('deployment').value,
    //    version: document.getElementById('version').value,
    //    useEntraId: document.getElementById('useEntraID').value,
    //    apikey: document.getElementById('apikey').value
    //}
    //let validConfig = false;
    //if (configData.endpoint !== '' && configData.deployment !== '' && configData.version !== '' && (configData.useEntraID || configData.apikey !== '')) {
    //    toggleConfigPanel(configPanel, configArrow);
    //    validConfig = true;
    //}
//}


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
        publishMessage("SendNewUserMessage", { Text: text });
    }
}

async function checkIfDarkMode() {
    let documentsResponse = await fetch(`./theme`);
    let theme = await documentsResponse.json();
    return theme === "Dark";
}

function toggleUseEntraID() {
    const useEntraID = document.getElementById('useEntraID');
    const apikey = document.getElementById('apikey');
    if (useEntraID.checked) {
        apikey.disabled = true;
        apikey.value = '';
    } else {
        apikey.disabled = false;
    }
}
function validateField(validationElementID, show) {
    const validationElement = document.getElementById(validationElementID);
    if (show) {
        validationElement.style.display = 'block';
    } else {
        validationElement.style.display = 'none';
    }
}
function initiateConnection() {
    configData = {
        Endpoint: document.getElementById('endpoint').value,
        Deployment: document.getElementById('deployment').value,
        Version: document.getElementById('version').value,
        UseEntraId: document.getElementById('useEntraID').checked,
        Apikey: document.getElementById('apikey').value
    }
    let validConfig = false;
    configData.endpoint === '' ? validateField('endpoint-validation', true) : validateField('endpoint-validation', false);
    configData.deployment === '' ? validateField('deployment-validation', true) : validateField('deployment-validation', false);
    configData.version === '' ? validateField('version-validation', true) : validateField('version-validation', false);
    !configData.useEntraId && configData.apikey === '' ? validateField('apikey-validation', true) : validateField('apikey-validation', false);
    if (configData.endpoint !== '' && configData.deployment !== '' && configData.version !== '' && (configData.useEntraId || configData.apikey !== '')) {
        validConfig = true;
    }
    if (validConfig) {
        publishMessage("InitiateConnection", configData);
    }
}

async function loadConfiguration() {
    let documentsResponse = await fetch(`./getConfiguration`);
    let config = await documentsResponse.json();
    if (config) {
        document.getElementById('endpoint').value = config.Endpoint;
        document.getElementById('deployment').value = config.Deployment;
        document.getElementById('version').value = config.Version;
        document.getElementById('useEntraID').checked = config.UseEntraId;
        document.getElementById('apikey').value = config.Apikey;
    }
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
    const useEntraID = document.getElementById('useEntraID');

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

    useEntraID.addEventListener('change', toggleUseEntraID);

    // Connection Configuration
    connectBtn.addEventListener('click', initiateConnection);

    //initConfigPanel(configPanel, configArrow);
    let isDarkMode = await checkIfDarkMode();
    if (isDarkMode) {
        document.documentElement.classList.toggle('dark');
    }

    await loadConfiguration();
}

function publishMessage(message, data) {
    if (window.chrome?.webview) {
        window.chrome.webview.postMessage({ message, data });
    } else if (window.webkit?.messageHandlers.studioPro) {
        window.webkit.messageHandlers.studioPro.postMessage(JSON.stringify({ message, data }))
    }
}

function registerListener() {
    // Register message handler.
    if (window.chrome?.webview) {
        window.chrome?.webview.addEventListener("message", (event) => {
            const message = event.data;
            this.handleMessage(message);
        });
    } else {
        window.WKPostMessage = json => {
            const wkMessage = JSON.parse(json);
            this.handleMessage(wkMessage);
        }

    };
}

function handleMessage(event) {
    const { message, data } = event;
    console.info("Received message:", message, data);
    if (message === "AssistantMessageResponse") {
        const messagesContainer = document.getElementById('messages');
        addMessage(messagesContainer, data, 'bot');
    }
}

document.addEventListener('DOMContentLoaded', async () => {
    await init();
    registerListener();
    publishMessage("MessageListenerRegistered");
});
