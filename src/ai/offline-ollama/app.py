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
        return ["llama3.1"]

def extract_content(content):
    """Helper function to cleanly extract a plain string from Gradio message objects."""
    if isinstance(content, str):
        return content
    if isinstance(content, list):
        return "".join([block.get("text", "") for block in content if isinstance(block, dict) and block.get("type") == "text"])
    return str(content)

def predict(message, history, model_name):
    messages = []
    
    for msg in history:
        if isinstance(msg, dict):
            role = msg.get('role')
            content = extract_content(msg.get('content', ''))
            if role and content:
                messages.append({'role': role, 'content': content})
        elif isinstance(msg, (list, tuple)) and len(msg) == 2:
            user_msg, ai_msg = msg
            if user_msg:
                messages.append({'role': 'user', 'content': extract_content(user_msg)})
            if ai_msg:
                messages.append({'role': 'assistant', 'content': extract_content(ai_msg)})
                
    messages.append({'role': 'user', 'content': extract_content(message)})
    
    try:
        response = client.chat(model=model_name, messages=messages, stream=True)
        partial_message = ""
        for chunk in response:
            if 'message' in chunk and 'content' in chunk['message']:
                partial_message += chunk['message']['content']
                yield partial_message
    except Exception as e:
        yield f"Error running '{model_name}': {str(e)}"

available_models = get_installed_models()
default_model = available_models if available_models else "llama3.1"

# FIX: CSS expanded to enforce 100% viewport height and strip wrapper constraints
head_style = """
<style>
    /* Force container to match browser viewport sizing */
    html, body, div.gradio-container, .gradio-container {
        max-width: unset !important;
        width: 100% !important;
        height: 100vh !important;
        margin: 0 !important;
        padding: 0 !important;
    }
    /* Let the inner block layouts expand completely */
    .contain {
        display: flex !important;
        flex-direction: column !important;
        height: 100% !important;
    }
</style>
"""

# FIX: Main wrapper configured with fill_height=True
with gr.Blocks(title="Local GPU LLM Chat", fill_width=True, fill_height=True, head=head_style) as demo:
    
    # Using a modern sidebar design to optimize layout and preserve vertical height
    with gr.Sidebar(label="Controls", open=True):
        gr.Markdown("# 🦙 Local LLM Chat")
        model_dropdown = gr.Dropdown(
            choices=available_models,
            value=default_model,
            label="Select Active Model",
            info="Choose a model to process your prompts",
            interactive=True
        )
        
    # FIX: Explicitly sizing ChatInterface to fill all horizontal and vertical canvas room
    chat = gr.ChatInterface(
        fn=predict,
        additional_inputs=[model_dropdown],
        fill_width=True,
        fill_height=True
    )

if __name__ == "__main__":
    demo.launch(server_name="0.0.0.0", server_port=8080, share=False)
