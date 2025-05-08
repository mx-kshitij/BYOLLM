# BYOLLM (Bring Your Own Large Language Model)

## Overview
**BYOLLM** is a custom extension for Mendix Studio Pro that integrates AI-powered chat functionality into the development environment. It provides a dockable pane with a web-based interface for interacting with a chat client, enabling developers to send messages, receive responses, and perform tool-based operations.

The project leverages OpenAI's Chat API for natural language processing and integrates with Mendix's Extensions API to provide seamless interaction with the Mendix modeling environment.

---

## Features
- **Chat Integration**:
	- Send user messages and receive AI-generated responses.
	- Support for system messages to guide the AI's behavior.
	- Currently supports OpenAI models on Azure

- **Image Handling**:
	- Accepts image attachments for AI to process and discuss or perform actions

- **Tool Handling**:
	- Executes tools to interact with the Domain model retrieving entities, attributes, associations, etc.

---

## Dependencies
- **.NET 8**: The project targets the latest .NET runtime for improved performance and features.
- **Mendix Extensions API**: Provides integration with the Mendix modeling environment.
- **OpenAI Chat API**: Powers the AI chat functionality.
- **System.Drawing.Common**: Used for image processing.

---

## Known Issues
1. **JPEG Save Error**:
   - The `Image.Save` method may throw a `System.Runtime.InteropServices.ExternalException` for JPEG files. This is resolved by explicitly specifying the encoder and parameters.

2. **Error Handling in Tool Calls**:
   - Ensure proper validation of tool call arguments to avoid runtime errors.

---

## Future Enhancements
1. **Support for more LLMS **:
   - Extend support for more LLMs like Claude, Llama, etc.

2. **Enhanced Tool Functionality**:
   - Add more tools for interacting with the Mendix studio pro environment like generating, evaluting and improving microflows, nanoflows, etc.

---

## How to Use

### Setup
1. Add the extension to Mendix Studio Pro.
2. Configure the OpenAI API key / Entra ID login in the settings.
	- If you use Entra ID, make sure you are logged in to the cmd terminal with ```az login``` and have access to the model in Azure OpenAI space

### Executing Tools
1. Chat with the AI and give as detailed instructions as possible.
2. You can tell the model what to do specifically or discuss with the model and let it use the tools as it sees fit.

---

## Conclusion
The BYOLLM project enhances the Mendix development experience by integrating AI-powered chat and tool functionality. It is designed to be extensible, robust, and user-friendly, making it a valuable addition to the Mendix ecosystem.
