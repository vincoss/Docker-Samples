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
        return models if models else []
    except Exception:
        # Avoid blocking UI startup if backend container is booting up
        return []

def refresh_models_on_load():
    """Triggered after the UI loads to dynamically populate the dropdown.
    If no models are installed, choices remain empty and nothing is selected."""
    models = get_installed_models()
    if models:
        # Populates the choices and automatically selects the first real available model
        return gr.update(choices=models, value=None)
    else:
        # Strips out placeholder options entirely so nothing is selected
        return gr.update(choices=[], value=None)

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
    # Prevent crashing if user tries to submit a message before choosing/downloading a model
    if not model_name:
        yield "⚠️ Please select a model from the sidebar controls before sending a prompt."
        return

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

    # 2. Process Current User Message & All Appended Text Files
    current_text = ""
    file_contents = ""
    
    if isinstance(message, dict):
        current_text = message.get("text", "")
        files = message.get("files", [])
        
        # Iterates cleanly over multiple files selected simultaneously
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

with gr.Blocks(title="Local GPU LLM Chat", fill_width=True, fill_height=True, head=head_style) as demo:
    
    with gr.Sidebar(label="Controls", open=True):
        gr.Markdown("# 🦙 Gabo")
        model_dropdown = gr.Dropdown(
            choices=[], 
            value=None, 
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
    
    demo.load(fn=refresh_models_on_load, inputs=None, outputs=[model_dropdown])

if __name__ == "__main__":
    demo.launch(server_name="0.0.0.0", server_port=8080, share=False)
