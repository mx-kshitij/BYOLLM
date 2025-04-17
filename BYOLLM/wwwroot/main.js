﻿function addMessage(text, type = 'user', attachment = null) {
    const messagesContainer = document.getElementById('messages');
    const messageElement = document.createElement('div');
    messageElement.classList.add(
        'p-2', 'rounded', 'max-w-[80%]', 'border',
        type === 'user' ? 'bg-white' : 'bg-gray-200',
        type === 'user' ? 'border-black' : 'border-gray-400',
        type === 'user' ? 'self-end' : 'self-start'
    );

    if (text) {
        const textElement = document.createElement('div');
        textElement.textContent = text;
        messageElement.appendChild(textElement);
    }

    // Add image if there's an attachment
    if (attachment) {
        const imageElement = document.createElement('img');
        imageElement.src = attachment;
        imageElement.classList.add('mt-2', 'max-w-full', 'rounded');
        imageElement.style.maxHeight = '200px';
        messageElement.appendChild(imageElement);
    }

    messagesContainer.appendChild(messageElement);
    messagesContainer.scrollTop = messagesContainer.scrollHeight;

    if (type === 'user') {
        publishMessage("SendNewUserMessage", { Text: text, Attachment: attachment });
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

function applyClassToMessageContainer(classname) {
    const messageContainer = document.getElementById('messages-container-outer');
    messageContainer.classList.add(classname);
}

function removeClassFromMessageContainer(classname) {
    const messageContainer = document.getElementById('messages-container-outer');
    messageContainer.classList.remove(classname);
}

function showThinking() {
    const thinkingIndicator = document.getElementById('thinking-indicator');
    thinkingIndicator.classList.remove('hidden');
    applyClassToMessageContainer('thinking');
}

function hideThinking() {
    const thinkingIndicator = document.getElementById('thinking-indicator');
    thinkingIndicator.classList.add('hidden');
    removeClassFromMessageContainer('thinking');
}

function clearImageAttachment() {
    const imagePreviewContainer = document.getElementById('image-preview-container');
    const imageUploadInput = document.getElementById('image-upload');
    const imagePreview = document.getElementById('image-preview');
    imagePreviewContainer.classList.add('hidden');
    imagePreview.src = '';
    imageUploadInput.value = '';
    currentAttachment = null;
    removeClassFromMessageContainer('imgpreview');
}

function showImageError(message) {
    const imageError = document.getElementById('image-error');
    const imageUploadInput = document.getElementById('image-upload');
    imageError.textContent = message;
    imageError.classList.remove('hidden');
    imageUploadInput.value = '';
}

function hideImageError() {
    const imageError = document.getElementById('image-error');
    imageError.classList.add('hidden');
}

function uploadImageToMessage(e) {
    const file = e.target.files[0];
    if (!file) return;

    // Check if file is an image
    if (!file.type.startsWith('image/')) {
        showImageError('Only image files are allowed.');
        return;
    }

    // Check file size (5MB max)
    const MAX_SIZE = 5 * 1024 * 1024; // 5MB in bytes
    if (file.size > MAX_SIZE) {
        showImageError('Image size must be less than 5MB.');
        return;
    }

    // Clear any previous error
    hideImageError();

    // Preview the image
    const reader = new FileReader();
    reader.onload = (event) => {
        const imagePreviewContainer = document.getElementById('image-preview-container');
        const imagePreview = document.getElementById('image-preview');
        imagePreview.src = event.target.result;
        imagePreviewContainer.classList.remove('hidden');
        applyClassToMessageContainer('imgpreview');
        currentAttachment = {
            data: event.target.result,
            file: file
        };
    };
    reader.readAsDataURL(file);
}

let currentAttachment = null;

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
    const attachImageBtn = document.getElementById('attach-image');
    const imageUploadInput = document.getElementById('image-upload');
    const removeImageBtn = document.getElementById('remove-image');

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
        if (message || currentAttachment) {
            const attachmentData = currentAttachment ? currentAttachment.data : null;

            // Add user message
            addMessage(message, 'user', attachmentData);

            // Clear input and attachment
            messageInput.value = '';
            clearImageAttachment();
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

    imageUploadInput.addEventListener('change', uploadImageToMessage);

    attachImageBtn.addEventListener('click', () => {
        imageUploadInput.click();
    });

    removeImageBtn.addEventListener('click', () => {
        clearImageAttachment();
    });

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
        addMessage(data, 'bot');
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
