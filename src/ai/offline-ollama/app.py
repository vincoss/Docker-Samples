
import os
import gradio as gr
import ollama

# ==========================================
# OLLAMA CONFIGURATION
# ==========================================

OLLAMA_HOST = os.getenv(
    "OLLAMA_HOST",
    "http://ollama-backend:11434"
)

# Maximum simultaneous chat generations
MAX_CONCURRENT_CHATS = 4


def get_ollama_client():
    """Create an independent client for each request."""
    return ollama.Client(host=OLLAMA_HOST)


# ==========================================
# MODEL MANAGEMENT
# ==========================================

def get_installed_models():
    try:
        client = get_ollama_client()
        model_list_data = client.list()

        models = [
            m["model"]
            for m in model_list_data.get("models", [])
        ]

        return models if models else []

    except Exception as e:
        print(f"Model loading error: {e}")
        return []


def refresh_models_on_load():
    models = get_installed_models()

    if models:
        return gr.update(choices=models, value=None)

    return gr.update(choices=[], value=None)


# ==========================================
# MESSAGE PROCESSING
# ==========================================

def extract_content(content):

    if isinstance(content, str):
        return content

    if isinstance(content, list):
        text_pieces = []

        for block in content:
            if isinstance(block, dict):
                if block.get("type") == "text":
                    text_pieces.append(
                        block.get("text", "")
                    )

        return "".join(text_pieces)

    return str(content)


def read_text_file(filepath):

    try:
        with open(
            filepath,
            "r",
            encoding="utf-8",
            errors="ignore"
        ) as f:
            return f.read()

    except Exception as e:
        return f"\n[Error reading file: {str(e)}]\n"


# ==========================================
# CONCURRENT CHAT GENERATION
# ==========================================

def predict(message, history, model_name):

    if not model_name:
        yield "⚠️ Please select a model from the sidebar controls."
        return

    # Build independent conversation history
    messages = []

    for msg in history:

        if isinstance(msg, dict):

            role = msg.get("role")
            content_data = msg.get("content")

            if role in ("user", "assistant"):
                messages.append({
                    "role": role,
                    "content": extract_content(content_data)
                })

        elif isinstance(msg, (list, tuple)) and len(msg) == 2:

            user_msg, ai_msg = msg

            if user_msg:
                messages.append({
                    "role": "user",
                    "content": extract_content(user_msg)
                })

            if ai_msg:
                messages.append({
                    "role": "assistant",
                    "content": extract_content(ai_msg)
                })

    # Process current message and uploaded files
    current_text = ""
    file_contents = ""

    if isinstance(message, dict):

        current_text = message.get("text", "")
        files = message.get("files", [])

        for f in files:

            file_path = (
                f.get("path")
                if isinstance(f, dict)
                else f
            )

            if file_path:

                filename = os.path.basename(file_path)
                raw_text = read_text_file(file_path)

                file_contents += (
                    f"\n\n--- Contents of uploaded file: "
                    f"{filename} ---\n"
                    f"{raw_text}\n"
                    f"-------------------------------------------\n"
                )

    else:
        current_text = extract_content(message)

    final_prompt = current_text + file_contents

    messages.append({
        "role": "user",
        "content": final_prompt
    })

    # Create an independent Ollama client per generation
    client = get_ollama_client()

    try:

        response = client.chat(
            model=model_name,
            messages=messages,
            stream=True
        )

        partial_message = ""

        for chunk in response:

            if (
                "message" in chunk
                and "content" in chunk["message"]
            ):

                partial_message += chunk["message"]["content"]

                yield partial_message

    except Exception as e:

        yield (
            f"Error running execution on "
            f"'{model_name}': {str(e)}"
        )


# ==========================================
# UI STYLING
# ==========================================

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


# ==========================================
# GRADIO APPLICATION
# ==========================================

with gr.Blocks(
    title="ChatGABO",
    fill_width=True,
    fill_height=True
) as demo:

    with gr.Sidebar(label="Controls", open=True):

        gr.Markdown("# 🦙 ChatGABO")

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
        multimodal=True,

        # Allow independent simultaneous generations
        concurrency_limit=MAX_CONCURRENT_CHATS
    )

    demo.load(
        fn=refresh_models_on_load,
        inputs=None,
        outputs=[model_dropdown]
    )


# ==========================================
# START APPLICATION
# ==========================================

if __name__ == "__main__":

    # Explicitly configure the Gradio request queue
    demo.queue(
        default_concurrency_limit=MAX_CONCURRENT_CHATS
    )

    demo.launch(
        server_name="0.0.0.0",
        server_port=8080,
        share=False,
        head=head_style,
        max_threads=40
    )
