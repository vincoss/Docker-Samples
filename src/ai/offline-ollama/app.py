
import os
import logging
from pathlib import Path

import gradio as gr
import ollama


# ============================================================
# CONFIGURATION
# ============================================================

APP_TITLE = "ChatGABO"

OLLAMA_HOST = os.getenv(
    "OLLAMA_HOST",
    "http://ollama-backend:11434"
)

OLLAMA_TIMEOUT = float(os.getenv("OLLAMA_TIMEOUT", "300"))
MAX_CONCURRENT_CHATS = int(os.getenv("MAX_CONCURRENT_CHATS", "4"))

if OLLAMA_TIMEOUT <= 0 or MAX_CONCURRENT_CHATS < 1:
    raise ValueError("Invalid application configuration.")

MAX_UPLOAD_FILES = 5
MAX_TEXT_FILE_SIZE = 5 * 1024 * 1024
MAX_IMAGE_FILE_SIZE = 10 * 1024 * 1024
MAX_EXTRACTED_TEXT = 50000
MAX_HISTORY_MESSAGES = 40

TEXT_EXTENSIONS = {
    ".txt", ".md", ".py", ".csv", ".json",
    ".yaml", ".yml", ".log", ".xml", ".html",
    ".css", ".js", ".sql", ".sh", ".ini", ".toml"
}

IMAGE_EXTENSIONS = {
    ".png", ".jpg", ".jpeg", ".webp", ".bmp"
}

logging.basicConfig(
    level=os.getenv("LOG_LEVEL", "INFO"),
    format="%(asctime)s [%(levelname)s] %(message)s"
)

logger = logging.getLogger("ChatGABO")

client = ollama.Client(
    host=OLLAMA_HOST,
    timeout=OLLAMA_TIMEOUT
)


# ============================================================
# MODEL MANAGEMENT
# ============================================================

def get_models():
    try:
        return sorted({
            model.model
            for model in client.list().models
            if model.model
        })

    except Exception:
        logger.exception("Unable to retrieve Ollama models")
        return []


def refresh_models():
    models = get_models()

    if not models:
        return (
            gr.update(choices=[], value=None),
            "No models found. Check the Ollama backend."
        )

    return (
        gr.update(choices=models, value=None),
        f"Loaded {len(models)} model(s). Select a model."
    )


# ============================================================
# HISTORY MANAGEMENT
# ============================================================

def extract_text(content):
    if isinstance(content, str):
        return content

    if not isinstance(content, list):
        return ""

    parts = []

    for block in content:
        if isinstance(block, str):
            parts.append(block)

        elif isinstance(block, dict) and block.get("type") == "text":
            parts.append(block.get("text", ""))

        elif getattr(block, "type", None) == "text":
            parts.append(getattr(block, "text", ""))

    return "".join(parts)


def prepare_history(history):
    messages = []

    for item in (history or [])[-MAX_HISTORY_MESSAGES:]:
        if not isinstance(item, dict):
            continue

        role = item.get("role")

        if role not in ("user", "assistant"):
            continue

        content = item.get("content", "")
        text = extract_text(content)

        if text:
            messages.append({
                "role": role,
                "content": text
            })

    return messages


# ============================================================
# FILE PROCESSING
# ============================================================

def get_file_path(item):
    if isinstance(item, str):
        return item

    if isinstance(item, dict):
        return item.get("path") or item.get("name")

    return getattr(item, "path", None)


def process_files(files):
    text_contents = []
    image_paths = []
    warnings = []

    total_chars = 0

    if len(files) > MAX_UPLOAD_FILES:
        warnings.append(
            f"Only the first {MAX_UPLOAD_FILES} files are allowed."
        )

    for item in (files or [])[:MAX_UPLOAD_FILES]:
        filename = "unknown"

        try:
            file_path = get_file_path(item)

            if not file_path:
                warnings.append("Invalid uploaded file.")
                continue

            path = Path(file_path)
            filename = path.name

            if not path.is_file():
                warnings.append(f"{filename}: file not accessible.")
                continue

            extension = path.suffix.lower()
            size = path.stat().st_size

            # Images
            if extension in IMAGE_EXTENSIONS:
                if size > MAX_IMAGE_FILE_SIZE:
                    warnings.append(
                        f"{filename}: image exceeds 10MB."
                    )
                else:
                    image_paths.append(str(path.resolve()))

                continue

            # Text files
            if extension not in TEXT_EXTENSIONS:
                warnings.append(
                    f"{filename}: unsupported file type."
                )
                continue

            if size > MAX_TEXT_FILE_SIZE:
                warnings.append(
                    f"{filename}: file exceeds 5MB."
                )
                continue

            remaining = MAX_EXTRACTED_TEXT - total_chars

            if remaining <= 0:
                warnings.append("Maximum extracted text reached.")
                break

            with path.open(
                "r",
                encoding="utf-8",
                errors="replace"
            ) as file:
                content = file.read(remaining + 1)

            if len(content) > remaining:
                content = content[:remaining]
                warnings.append("Uploaded text was truncated.")

            total_chars += len(content)

            text_contents.append(
                f"\n\n--- FILE: {filename} ---\n"
                f"{content}\n"
                f"--- END FILE ---"
            )

        except Exception:
            logger.exception(
                "Failed to process file: %s",
                filename
            )
            warnings.append(
                f"{filename}: unable to process file."
            )

    return "\n".join(text_contents), image_paths, warnings


# ============================================================
# CHAT GENERATION
# ============================================================

def predict(message, history, model_name):

    # Validate model
    if not model_name:
        yield "Please select an Ollama model first."
        return

    # Read message
    if isinstance(message, dict):
        user_text = extract_text(message.get("text") or "")
        files = message.get("files") or []
    else:
        user_text = extract_text(message)
        files = []

    # Process uploads
    file_text, images, warnings = process_files(files)

    prompt = user_text + file_text

    if not prompt.strip() and not images:
        yield "Please enter a message or upload a supported file."
        return

    # Prepare conversation
    messages = prepare_history(history)

    user_message = {
        "role": "user",
        "content": prompt
    }

    if images:
        user_message["images"] = images

    messages.append(user_message)

    # Warning messages
    warning_text = "\n".join(
        f"⚠️ {warning}" for warning in warnings
    )

    response_text = ""

    try:
        logger.info(
            "Starting generation | model=%s",
            model_name
        )

        response = client.chat(
            model=model_name,
            messages=messages,
            stream=True,
            options={
                "temperature": 0.7,
                "num_ctx": 8192,
                "num_predict": 2048
            }
        )

        # Stream response
        for chunk in response:
            content = getattr(
                getattr(chunk, "message", None),
                "content",
                ""
            ) or ""

            if content:
                response_text += content

                yield (
                    warning_text + "\n\n" + response_text
                    if warning_text
                    else response_text
                )

        if not response_text:
            yield (
                warning_text + "\n\n"
                "The model returned an empty response."
            )

        logger.info(
            "Generation completed | model=%s | characters=%s",
            model_name,
            len(response_text)
        )

    except Exception:
        logger.exception(
            "Ollama generation failed | model=%s",
            model_name
        )

        yield (
            warning_text + "\n\n"
            "Unable to generate a response. "
            "Check the Ollama backend and model availability."
        )


# ============================================================
# UI STYLING
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
    fill_height=True,
    theme=gr.themes.Soft()
) as demo:

    # Sidebar
    with gr.Sidebar(
        label="ChatGABO Controls",
        open=True
    ):

        gr.Markdown("""
        # 🦙 ChatGABO

        ### Local AI Assistant

        Powered by Ollama + NVIDIA GPU
        """)

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

        gr.Markdown("""
        ---

        ### Supported uploads

        - Text and source-code files
        - CSV, JSON and YAML
        - PNG, JPG and WebP images

        Image understanding requires a vision-capable model.
        """)

    # Main chat
    gr.ChatInterface(
        fn=predict,
        additional_inputs=[model_dropdown],
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
            file_types=list(TEXT_EXTENSIONS | IMAGE_EXTENSIONS),
            sources=["upload"]
        )
    )

    # Refresh models
    refresh_button.click(
        fn=refresh_models,
        inputs=None,
        outputs=[model_dropdown, model_status]
    )

    # Initial model discovery
    demo.load(
        fn=refresh_models,
        inputs=None,
        outputs=[model_dropdown, model_status]
    )


# ============================================================
# START APPLICATION
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
        max_threads=40
    )
