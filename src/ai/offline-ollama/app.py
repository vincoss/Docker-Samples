
import os
import logging
from pathlib import Path

import gradio as gr
import ollama


# ============================================================
# APPLICATION CONFIGURATION
# ============================================================

APP_TITLE = "ChatGABO"

OLLAMA_HOST = os.getenv(
    "OLLAMA_HOST",
    "http://ollama-backend:11434"
)

MAX_CONCURRENT_CHATS = int(
    os.getenv("MAX_CONCURRENT_CHATS", "4")
)

OLLAMA_TIMEOUT = float(
    os.getenv("OLLAMA_TIMEOUT", "300")
)

MAX_UPLOAD_FILES = 5
MAX_TEXT_FILE_SIZE = 5 * 1024 * 1024
MAX_IMAGE_FILE_SIZE = 10 * 1024 * 1024
MAX_EXTRACTED_TEXT = 50000
MAX_HISTORY_MESSAGES = 40

TEXT_EXTENSIONS = {
    ".txt", ".md", ".py", ".csv",
    ".json", ".yaml", ".yml", ".log",
    ".xml", ".html", ".css", ".js",
    ".sql", ".sh", ".ini", ".toml"
}

IMAGE_EXTENSIONS = {
    ".png", ".jpg", ".jpeg", ".webp", ".bmp"
}

logging.basicConfig(
    level=os.getenv("LOG_LEVEL", "INFO"),
    format="%(asctime)s [%(levelname)s] %(message)s"
)

logger = logging.getLogger("ChatGABO")


# ============================================================
# OLLAMA CLIENT
# ============================================================

def get_ollama_client():
    return ollama.Client(
        host=OLLAMA_HOST,
        timeout=OLLAMA_TIMEOUT
    )


# ============================================================
# MODEL MANAGEMENT
# ============================================================

def get_installed_models():

    try:
        client = get_ollama_client()
        result = client.list()

        models = []

        # Current Ollama SDK response format
        model_objects = getattr(result, "models", None)

        if model_objects is not None:

            for model in model_objects:

                name = getattr(model, "model", None)

                if name:
                    models.append(name)

        # Backwards compatibility
        elif isinstance(result, dict):

            for model in result.get("models", []):

                if isinstance(model, dict):
                    name = model.get("model") or model.get("name")
                else:
                    name = getattr(model, "model", None)

                if name:
                    models.append(name)

        return sorted(set(models))

    except Exception:

        logger.exception("Unable to retrieve Ollama models")

        return []


def refresh_models():

    models = get_installed_models()

    if models:

       return (
            gr.update(
                choices=models,
                value=None
            ),
            f"Loaded {len(models)} installed model(s). Please select a model."
        )

    return (
        gr.update(
            choices=[],
            value=None
        ),
        "No models found. Check the Ollama backend."
    )


# ============================================================
# MESSAGE HISTORY
# ============================================================

def extract_content(content):

    if content is None:
        return ""

    if isinstance(content, str):
        return content

    if isinstance(content, list):

        pieces = []

        for block in content:

            if isinstance(block, dict):

                if block.get("type") == "text":
                    pieces.append(block.get("text", ""))

            else:

                text = getattr(block, "text", None)

                if text:
                    pieces.append(text)

        return "".join(pieces)

    return str(content)


def prepare_history(history):

    messages = []

    for item in (history or []):

        if not isinstance(item, dict):
            continue

        role = item.get("role")

        if role not in ("user", "assistant"):
            continue

        content = extract_content(item.get("content"))

        if not content:
            continue

        messages.append({
            "role": role,
            "content": content
        })

    # Keep the most recent conversation messages
    return messages[-MAX_HISTORY_MESSAGES:]


# ============================================================
# FILE PROCESSING
# ============================================================

def get_file_path(file_item):

    if isinstance(file_item, str):
        return file_item

    if isinstance(file_item, dict):
        return file_item.get("path") or file_item.get("name")

    return getattr(file_item, "path", None)


def process_uploaded_files(files):

    text_contents = []
    image_paths = []
    warnings = []

    if not files:
        return "", [], []

    if len(files) > MAX_UPLOAD_FILES:

        warnings.append(
            f"Maximum {MAX_UPLOAD_FILES} files allowed per message."
        )

        files = files[:MAX_UPLOAD_FILES]

    total_text_length = 0

    for file_item in files:

        file_path = get_file_path(file_item)

        if not file_path:
            continue

        path = Path(file_path)

        try:

            if not path.is_file():
                warnings.append("An uploaded file could not be accessed.")
                continue

            extension = path.suffix.lower()
            filename = path.name
            size = path.stat().st_size

            # ----------------------------------------
            # IMAGE PROCESSING
            # ----------------------------------------

            if extension in IMAGE_EXTENSIONS:

                if size > MAX_IMAGE_FILE_SIZE:

                    warnings.append(
                        f"{filename}: image exceeds 10MB limit."
                    )

                    continue

                image_paths.append(str(path.resolve()))
                continue

            # ----------------------------------------
            # TEXT FILE PROCESSING
            # ----------------------------------------

            if extension not in TEXT_EXTENSIONS:

                warnings.append(
                    f"{filename}: unsupported file type."
                )

                continue

            if size > MAX_TEXT_FILE_SIZE:

                warnings.append(
                    f"{filename}: file exceeds 5MB limit."
                )

                continue

            remaining = MAX_EXTRACTED_TEXT - total_text_length

            if remaining <= 0:

                warnings.append(
                    "Maximum extracted text limit reached."
                )

                break

            with path.open(
                "r",
                encoding="utf-8",
                errors="replace"
            ) as file:

                content = file.read(remaining + 1)

            if len(content) > remaining:

                content = content[:remaining]

                warnings.append(
                    "Some uploaded text was truncated."
                )

            total_text_length += len(content)

            text_contents.append(
                f"\n\n"
                f"===== FILE: {filename} =====\n"
                f"{content}\n"
                f"===== END FILE =====\n"
            )

        except Exception:

            logger.exception(
                "File processing failed: %s",
                filename if "filename" in locals() else "unknown"
            )

            warnings.append(
                "An uploaded file could not be processed."
            )

    return (
        "".join(text_contents),
        image_paths,
        warnings
    )


# ============================================================
# CHAT GENERATION
# ============================================================

def predict(message, history, model_name):

    # ----------------------------------------
    # MODEL VALIDATION
    # ----------------------------------------

    if not model_name:

        yield "Please select an Ollama model first."
        return

    # ----------------------------------------
    # MESSAGE EXTRACTION
    # ----------------------------------------

    if isinstance(message, dict):

        current_text = message.get("text", "") or ""
        uploaded_files = message.get("files", []) or []

    else:

        current_text = extract_content(message)
        uploaded_files = []

    # ----------------------------------------
    # FILE PROCESSING
    # ----------------------------------------

    file_text, image_paths, warnings = (
        process_uploaded_files(uploaded_files)
    )

    final_prompt = current_text + file_text

    if not final_prompt.strip() and not image_paths:

        yield "Please enter a message or upload a supported file."
        return

    # ----------------------------------------
    # BUILD CONVERSATION
    # ----------------------------------------

    messages = prepare_history(history)

    user_message = {
        "role": "user",
        "content": final_prompt
    }

    # Ollama vision support
    if image_paths:
        user_message["images"] = image_paths

    messages.append(user_message)

    # ----------------------------------------
    # GENERATION OPTIONS
    # ----------------------------------------

    options = {
        "temperature": 0.7,
        "num_ctx": 8192,
        "num_predict": 2048
    }

    client = get_ollama_client()

    partial_message = ""

    try:

        logger.info(
            "Starting generation | model=%s",
            model_name
        )

        response = client.chat(
            model=model_name,
            messages=messages,
            stream=True,
            options=options
        )

        # Display file warnings
        if warnings:

            warning_text = "\n".join(
                f"⚠️ {warning}" for warning in warnings
            )

            yield warning_text + "\n\nGenerating response...\n"

        # ------------------------------------
        # STREAM RESPONSE
        # ------------------------------------

        for chunk in response:

            chunk_message = getattr(
                chunk,
                "message",
                None
            )

            if chunk_message is not None:

                content = getattr(
                    chunk_message,
                    "content",
                    ""
                )

            elif isinstance(chunk, dict):

                content = chunk.get(
                    "message", {}
                ).get("content", "")

            else:

                content = ""

            if content:

                partial_message += content

                yield partial_message

        if not partial_message:

            yield "The model returned an empty response."

        logger.info(
            "Generation completed | model=%s | characters=%s",
            model_name,
            len(partial_message)
        )

    except Exception as error:

        logger.exception(
            "Ollama generation failed | model=%s",
            model_name
        )

        yield (
            f"⚠️ Unable to generate a response using "
            f"'{model_name}'.\n\n"
            f"Check the Ollama server logs and model availability."
        )


# ============================================================
# USER INTERFACE STYLING
# ============================================================

HEAD_STYLE = """
<style>

html, body, .gradio-container {
    width: 100% !important;
    min-height: 100vh !important;
    margin: 0 !important;
}

.gradio-container {
    max-width: 100% !important;
}

footer {
    display: none !important;
}

</style>
"""


# ============================================================
# GRADIO APPLICATION
# ============================================================

with gr.Blocks(
    title=APP_TITLE,
    fill_width=True,
    fill_height=True
) as demo:

    # ----------------------------------------
    # SIDEBAR
    # ----------------------------------------

    with gr.Sidebar(
        label="ChatGABO Controls",
        open=True
    ):

        gr.Markdown(
            """
            # 🦙 ChatGABO

            ### Local AI Assistant

            Powered by Ollama + NVIDIA GPU
            """
        )

        model_dropdown = gr.Dropdown(
            choices=[],
            value=None,
            label="Active Model",
            interactive=True
        )

        refresh_button = gr.Button(
            "🔄 Refresh Models",
            variant="secondary"
        )

        model_status = gr.Markdown(
            "Loading available models..."
        )

        gr.Markdown("---")

        gr.Markdown(
            """
            ### Supported uploads

            - Text and source-code files
            - CSV, JSON and YAML
            - PNG, JPG and WebP images

            Image understanding requires a vision-capable model.
            """
        )

    # ----------------------------------------
    # MAIN CHAT
    # ----------------------------------------

    chat = gr.ChatInterface(
        fn=predict,

        additional_inputs=[
            model_dropdown
        ],

        multimodal=True,

        fill_width=True,
        fill_height=True,

        concurrency_limit=MAX_CONCURRENT_CHATS,

       chatbot=gr.Chatbot(
            height="100%",
            layout="bubble"
        ),

        textbox=gr.MultimodalTextbox(
            placeholder="Message ChatGABO...",
            file_count="multiple",
            file_types=[
                ".txt", ".md", ".py", ".csv",
                ".json", ".yaml", ".yml", ".log",
                ".xml", ".html", ".css", ".js",
                ".sql", ".sh", ".ini", ".toml",
                ".png", ".jpg", ".jpeg",
                ".webp", ".bmp"
            ],
            sources=["upload"]
        )
    )

    # ----------------------------------------
    # MODEL REFRESH
    # ----------------------------------------

    refresh_button.click(
        fn=refresh_models,
        inputs=None,
        outputs=[
            model_dropdown,
            model_status
        ]
    )

    # Initial model discovery
    demo.load(
        fn=refresh_models,
        inputs=None,
        outputs=[
            model_dropdown,
            model_status
        ]
    )


# ============================================================
# APPLICATION STARTUP
# ============================================================

if __name__ == "__main__":

    demo.queue(
        default_concurrency_limit=MAX_CONCURRENT_CHATS
    )

    demo.launch(
    server_name="0.0.0.0",
    server_port=8080,
    share=False,
    head=HEAD_STYLE,
    max_threads=40,
    theme=gr.themes.Soft()
)
