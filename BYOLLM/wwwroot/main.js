﻿// Message Sending
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
        disableSendButton();
        showThinking();
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

function showConnectingSpinner() {
    const connectSpinner = document.getElementById('connect-spinner');
    const connectBtn = document.getElementById('connect-btn');
    connectSpinner.classList.remove('hidden');
    connectBtn.disabled = true;
}

function hideConnectingSpinner() {
    const connectSpinner = document.getElementById('connect-spinner');
    const connectBtn = document.getElementById('connect-btn');
    connectSpinner.classList.add('hidden');
    connectBtn.disabled = false;
}

function initiateConnection() {
    configData = {
        Endpoint: document.getElementById('endpoint').value,
        Deployment: document.getElementById('deployment').value,
        //Version: document.getElementById('version').value,
        UseEntraId: document.getElementById('useEntraID').checked,
        Apikey: document.getElementById('apikey').value,
        SystemPrompt: document.getElementById('systemPrompt').value
    }
    let validConfig = false;
    configData.Endpoint === '' ? validateField('endpoint-validation', true) : validateField('endpoint-validation', false);
    configData.Deployment === '' ? validateField('deployment-validation', true) : validateField('deployment-validation', false);
    //configData.version === '' ? validateField('version-validation', true) : validateField('version-validation', false);
    !configData.useEntraId && configData.apikey === '' ? validateField('apikey-validation', true) : validateField('apikey-validation', false);
    if (configData.endpoint !== '' && configData.deployment !== '' && configData.version !== '' && (configData.useEntraId || configData.apikey !== '')) {
        validConfig = true;
    }
    if (validConfig) {
        publishMessage("InitiateConnection", configData);
        showConnectingSpinner();
    }
}

async function loadConfiguration() {
    let documentsResponse = await fetch(`./getConfiguration`);
    let config = await documentsResponse.json();
    if (config) {
        document.getElementById('endpoint').value = config.Endpoint;
        document.getElementById('deployment').value = config.Deployment;
        //document.getElementById('version').value = config.Version;
        document.getElementById('useEntraID').checked = config.UseEntraId;
        document.getElementById('apikey').value = config.Apikey;
        document.getElementById('systemPrompt').value = config.SystemPrompt;
    }
}

function hideConfigPanel() {
    const configDialog = document.getElementById('config-dialog');
    configDialog.classList.add('hidden');
}

function enableSendButton() {
    const sendBtn = document.getElementById('send-btn');
    sendBtn.disabled = false;
    sendBtn.classList.remove('opacity-50');
}

function disableSendButton() {
    const sendBtn = document.getElementById('send-btn');
    sendBtn.disabled = true;
    sendBtn.classList.add('opacity-50');
}

function showThinking() {
    const thinkingIndicator = document.getElementById('thinking-indicator');
    const messageContainer = document.getElementById('messages-container-outer');
    thinkingIndicator.classList.remove('hidden');
    messageContainer.classList.add('thinking');
}

function hideThinking() {
    const thinkingIndicator = document.getElementById('thinking-indicator');
    const messageContainer = document.getElementById('messages-container-outer');
    thinkingIndicator.classList.add('hidden');
    messageContainer.classList.remove('thinking');
}

async function init() {
    const themeToggle = document.getElementById('theme-toggle');
    const messageInput = document.getElementById('message-input');
    const sendBtn = document.getElementById('send-btn');
    const messagesContainer = document.getElementById('messages');
    const connectBtn = document.getElementById('connect-btn');
    const configToggle = document.getElementById('config-toggle');
    const useEntraID = document.getElementById('useEntraID');
    const closeConfig = document.getElementById('close-config');
    const configDialog = document.getElementById('config-dialog');

    // Theme Toggle
    themeToggle.addEventListener('click', () => {
        document.documentElement.classList.toggle('dark');
    });

    // Config Dialog Toggle
    configToggle.addEventListener('click', () => {
        configDialog.classList.remove('hidden');
    });

    // Close Config Dialog
    closeConfig.addEventListener('click', () => {
        hideConfigPanel();
    });

    // Close dialog when clicking outside the config panel
    configDialog.addEventListener('click', (e) => {
        if (e.target === configDialog) {
            hideConfigPanel();
        }
    });

    sendBtn.addEventListener('click', () => {
        const message = messageInput.value.trim();
        if (message) {
            addMessage(messagesContainer, message, 'user');
            messageInput.value = '';
        }
    });

    messageInput.addEventListener('keypress', (e) => {
        if (e.key === 'Enter' && !sendBtn.disabled) {
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
    configDialog.classList.remove('hidden');
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
        enableSendButton();
        hideThinking();
    }
    else if (message === "ConnectionEstablished") {
        hideConnectingSpinner();
        hideConfigPanel();
        showThinking();
        enableSendButton();
    }
    else if (message === "ConnectionFailed") {
        hideConnectingSpinner();
    }
}

document.addEventListener('DOMContentLoaded', async () => {
    await init();
    registerListener();
    publishMessage("MessageListenerRegistered");
});
