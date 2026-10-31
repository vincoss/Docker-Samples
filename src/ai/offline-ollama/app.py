import os
import gradio as gr
import ollama

# Pull the backend host address from the docker-compose environment variables
OLLAMA_HOST = os.getenv("OLLAMA_HOST", "http://ollama-backend:11434")

# Initialize the Ollama Client
client = ollama.Client(host=OLLAMA_HOST)

# Using Llama 3.1 8B as a standard local GPU model example
MODEL_NAME = "llama3.1"

def extract_content(content):
    """Helper function to cleanly extract a plain string from Gradio message objects."""
    if isinstance(content, str):
        return content
    if isinstance(content, list):
        # Extract and combine the 'text' keys from block items
        return "".join([block.get("text", "") for block in content if isinstance(block, dict) and block.get("type") == "text"])
    return str(content)

def predict(message, history):
    messages = []
    
    # Dynamically handle both Dictionary and Tuple/List history formats
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
    
    # Append the newest user prompt safely handled
    messages.append({'role': 'user', 'content': extract_content(message)})
    
    try:
        # Stream the response from the local model container
        response = client.chat(model=MODEL_NAME, messages=messages, stream=True)
        partial_message = ""
        for chunk in response:
            if 'message' in chunk and 'content' in chunk['message']:
                partial_message += chunk['message']['content']
                yield partial_message
    except Exception as e:
        yield (
            f"Error connecting to local model: {str(e)}\n\n"
            f"💡 Quick Fix: Enter the ollama container and run: docker exec -it ollama_gpu ollama run {MODEL_NAME}"
        )

# Initialize the web interface
demo = gr.ChatInterface(
    fn=predict, 
    title="Pure Local Offline LLM Chat",
    description="Running entirely on your host GPU via Docker Compose."
)

if __name__ == "__main__":
    demo.launch(server_name="0.0.0.0", server_port=8080, share=False)
