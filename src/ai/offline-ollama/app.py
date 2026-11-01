import os
import gradio as gr
import ollama

# Pull the backend host address from the docker-compose environment variables
OLLAMA_HOST = os.getenv("OLLAMA_HOST", "http://ollama-backend:11434")

# Initialize the Ollama Client
client = ollama.Client(host=OLLAMA_HOST)

def get_installed_models():
    """Fetch the list of downloaded models from the Ollama backend container."""
    try:
        model_list_data = client.list()
        models = [m['model'] for m in model_list_data.get('models', [])]
        return models if models else ["llama3.1"]
    except Exception:
        # Return fallback instantly without hanging the application
        return ["llama3.1"]

def refresh_models_on_load():
    """Triggered after the UI loads to dynamically populate the dropdown without blocking startup."""
    models = get_installed_models()
    if models:
        # Update the choices
        return gr.update(choices=models)
    else:
        # No models found, display a message
        return "No models found."

def extract_content(content):
    """Helper function to cleanly extract text from complex multi-modal Gradio message structures."""
    if isinstance(content, str):
        return content
    if isinstance(content, list):
        text_pieces = []
        for block in content:
            if isinstance(block, dict):
                if block.get("type") == "text":
                    text_pieces.append(block.get("text", ""))
        return "".join(text_pieces)
    return str(content)

def read_text_file(filepath):
    """Safely open and read the contents of a dropped text file."""
    try:
        with open(filepath, "r", encoding="utf-8", errors="ignore") as f:
            return f.read()
    except Exception as e:
        return f"\n[Error reading file: {str(e)}]\n"

def predict(message, history, model_name):
    messages = []
    
    # 1. Process Past Conversation History
    for msg in history:
        if isinstance(msg, dict):
            role = msg.get('role')
            content_data = msg.get('content')
            messages.append({'role': role, 'content': extract_content(content_data)})
        elif isinstance(msg, (list, tuple)) and len(msg) == 2:
            user_msg, ai_msg = msg
            if user_msg:
                messages.append({'role': 'user', 'content': extract_content(user_msg)})
            if ai_msg:
                messages.append({'role': 'assistant', 'content': extract_content(ai_msg)})

    # 2. Process Current User Message & Appended Text Files
    current_text = ""
    file_contents = ""
    
    if isinstance(message, dict):
        current_text = message.get("text", "")
        files = message.get("files", [])
        
        for f in files:
            file_path = f.get("path") if isinstance(f, dict) else f
            if file_path:
                filename = os.path.basename(file_path)
                raw_text = read_text_file(file_path)
                file_contents += f"\n\n--- Contents of uploaded file: {filename} ---\n{raw_text}\n-------------------------------------------\n"
    else:
        current_text = extract_content(message)

    final_prompt = current_text
    if file_contents:
        final_prompt += file_contents
        
    messages.append({'role': 'user', 'content': final_prompt})
    
    try:
        response = client.chat(model=model_name, messages=messages, stream=True)
        partial_message = ""
        for chunk in response:
            if 'message' in chunk and 'content' in chunk['message']:
                partial_message += chunk['message']['content']
                yield partial_message
    except Exception as e:
        yield f"Error running execution on '{model_name}': {str(e)}"

head_style = """
<style>
    html, body, div.gradio-container, .gradio-container {
        max-width: unset !important;
        width: 100% !important;
        height: 100vh !important;
        margin: 0 !important;
        padding: 0 !important;
    }
    .contain {
        display: flex !important;
        flex-direction: column !important;
        height: 100% !important;
    }
</style>
"""

# FIX: Boot immediately with a standard placeholder list, rather than calling the API globally
with gr.Blocks(title="Local GPU LLM Chat", fill_width=True, fill_height=True, head=head_style) as demo:
    
    with gr.Sidebar(label="Controls", open=True):
        gr.Markdown("# 🦙 Local Text File Chat")
        model_dropdown = gr.Dropdown(
            choices=[], # Starts empty instantly
            value=None, # No default selection until background analysis checks out
            label="Select Active Model",
            info="Drop text files (.txt, .md, .py, .csv) directly into the chat prompt area.",
            interactive=True
        )
        
    chat = gr.ChatInterface(
        fn=predict,
        additional_inputs=[model_dropdown],
        fill_width=True,
        fill_height=True,
        multimodal=True
    )
    
    # FIX: Async load hook. When the web app finishes mounting in the browser, 
    # it hits this background event to populate your drop-down options without blocking the initial view.
    demo.load(fn=refresh_models_on_load, inputs=None, outputs=[model_dropdown])

if __name__ == "__main__":
    demo.launch(server_name="0.0.0.0", server_port=8080, share=False)
