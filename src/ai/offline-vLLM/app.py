
import os
import stat
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
MAX_EXTRACTED_TEXT = 50000
MAX_HISTORY_MESSAGES = 40

TEXT_EXTENSIONS = {
    ".txt", ".md", ".py", ".csv", ".json",
    ".yaml", ".yml", ".log", ".xml", ".html",
    ".css", ".js", ".sql", ".sh", ".ini", ".toml"
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
    response = client.list()

    return sorted({
        model.model
        for model in response.models
        if model.model
    })


def refresh_models():

    try:
        models = get_models()

        if not models:
            return (
                gr.update(choices=[], value=None),
                "⚠️ No Ollama models installed. "
                "Use `ollama pull MODEL_NAME` to install one."
            )

        return (
            gr.update(choices=models, value=None),
            f"✅ Loaded {len(models)} model(s). Select a model."
        )

    except ollama.ResponseError as exc:

        logger.exception("Ollama model discovery failed")

        return (
            gr.update(choices=[], value=None),
            f"❌ Ollama returned HTTP {exc.status_code}. "
            "Check the Ollama backend."
        )

    except ollama.RequestError:

        logger.exception("Unable to connect to Ollama")

        return (
            gr.update(choices=[], value=None),
            "❌ Cannot connect to Ollama. Check OLLAMA_HOST, "
            "network connectivity and backend availability."
        )

    except Exception:

        logger.exception("Unexpected model discovery error")

        return (
            gr.update(choices=[], value=None),
            "❌ Unexpected error retrieving models. "
            "Check application logs."
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

        text = extract_text(item.get("content", ""))

        if text:
            messages.append({
                "role": role,
                "content": text
            })

    return messages


# ============================================================
# SECURE FILE PROCESSING
# ============================================================

def get_file_path(item):

    if isinstance(item, str):
        return item

    if isinstance(item, dict):
        return item.get("path")

    return getattr(item, "path", None)


def read_uploaded_file(file_path):

    """
    Securely read uploaded files.

    Checks:
    - Reject symbolic links.
    - Require regular files.
    - Reject oversized files.
    - Avoid following final symbolic links.
    - Detect binary content.
    - Enforce UTF-8 text decoding.
    """

    original_path = Path(file_path)

    if original_path.is_symlink():
        raise ValueError("Symbolic links are not permitted.")

    try:
        resolved_path = original_path.resolve(strict=True)

    except (OSError, RuntimeError):
        raise ValueError("File path cannot be resolved.")

    flags = os.O_RDONLY | getattr(os, "O_NOFOLLOW", 0)

    flags |= getattr(os, "O_NONBLOCK", 0)

    file_descriptor = None

    try:

        file_descriptor = os.open(
            str(resolved_path),
            flags
        )

        file_info = os.fstat(file_descriptor)

        if not stat.S_ISREG(file_info.st_mode):
            raise ValueError("Only regular files are permitted.")

        if file_info.st_size > MAX_TEXT_FILE_SIZE:
            raise ValueError("File exceeds the 5 MB size limit.")

        with os.fdopen(file_descriptor, "rb") as file:

            file_descriptor = None

            raw_content = file.read(MAX_TEXT_FILE_SIZE + 1)

        if len(raw_content) > MAX_TEXT_FILE_SIZE:
            raise ValueError("File exceeds the 5 MB size limit.")

        if b"\x00" in raw_content[:8192]:
            raise ValueError("Binary files are not supported.")

        return raw_content.decode(
            "utf-8",
            errors="replace"
        )

    finally:

        if file_descriptor is not None:
            os.close(file_descriptor)


def process_files(files):

    text_contents = []
    warnings = []

    total_chars = 0

    files = files or []

    if not isinstance(files, (list, tuple)):
        return "", ["Invalid upload format."]

    if len(files) > MAX_UPLOAD_FILES:

        warnings.append(
            f"Only the first {MAX_UPLOAD_FILES} files are allowed."
        )

    for item in files[:MAX_UPLOAD_FILES]:

        filename = "unknown"

        try:

            file_path = get_file_path(item)

            if not file_path:
                warnings.append("Invalid uploaded file.")
                continue

            path = Path(file_path)

            filename = path.name

            # Validate extension.
            extension = path.suffix.lower()

            if extension not in TEXT_EXTENSIONS:

                warnings.append(
                    f"{filename}: unsupported file type. "
                    "Only approved text files are allowed."
                )

                continue

            # Read securely.
            content = read_uploaded_file(file_path)

            # Enforce extracted text limit.
            remaining = MAX_EXTRACTED_TEXT - total_chars

            if remaining <= 0:

                warnings.append(
                    "Maximum extracted text limit reached."
                )

                break

            if len(content) > remaining:

                content = content[:remaining]

                warnings.append(
                    f"{filename}: content truncated because "
                    "the maximum extracted text limit was reached."
                )

            total_chars += len(content)

            text_contents.append(
                f"\n\n--- FILE: {filename} ---\n"
                f"{content}\n"
                f"--- END FILE ---"
            )

        except ValueError as exc:

            logger.warning(
                "Upload rejected | filename=%s | reason=%s",
                filename,
                exc
            )

            warnings.append(
                f"{filename}: {exc}"
            )

        except OSError:

            logger.exception(
                "File access failed | filename=%s",
                filename
            )

            warnings.append(
                f"{filename}: unable to safely access file."
            )

        except Exception:

            logger.exception(
                "Unexpected file processing error | filename=%s",
                filename
            )

            warnings.append(
                f"{filename}: unexpected processing error."
            )

    return "\n".join(text_contents), warnings


# ============================================================
# ERROR CLASSIFICATION
# ============================================================

def is_timeout_error(exc):

    current = exc
    visited = set()

    while current is not None and id(current) not in visited:

        visited.add(id(current))

        if "timeout" in type(current).__name__.lower():
            return True

        current = (
            getattr(current, "__cause__", None)
            or getattr(current, "__context__", None)
        )

    return False


def format_generation_error(exc, model_name):

    if isinstance(exc, ollama.ResponseError):

        status = getattr(exc, "status_code", None)

        logger.exception(
            "Ollama response error | model=%s | status=%s",
            model_name,
            status
        )

        if status == 404:

            return (
                f"❌ Model '{model_name}' was not found. "
                "Refresh the model list or install the model "
                "using ollama pull."
            )

        if status == 400:

            return (
                "❌ Ollama rejected the request. "
                "Check the selected model and request configuration."
            )

        if status == 500:

            return (
                "❌ Ollama encountered an internal server error. "
                "Check backend logs and available resources."
            )

        return (
            f"❌ Ollama returned HTTP {status}. "
            "Check the backend logs."
        )

    if isinstance(exc, ollama.RequestError):

        logger.exception(
            "Ollama request failed | model=%s",
            model_name
        )

        if is_timeout_error(exc):

            return (
                "⏱️ Ollama request timed out. "
                "Try a smaller prompt or check backend performance."
            )

        return (
            "❌ Unable to communicate with Ollama. "
            "Check OLLAMA_HOST, network connectivity "
            "and backend availability."
        )

    if is_timeout_error(exc):

        logger.exception(
            "Generation timeout | model=%s",
            model_name
        )

        return (
            "⏱️ Generation timed out. "
            "Try reducing the prompt size or increasing "
            "OLLAMA_TIMEOUT."
        )

    logger.exception(
        "Unexpected generation error | model=%s",
        model_name
    )

    return (
        "❌ An unexpected error occurred during generation. "
        "Check ChatGABO and Ollama application logs."
    )


# ============================================================
# CHAT GENERATION
# ============================================================

def predict(message, history, model_name):

    # Validate model selection.
    if not model_name:

        yield "⚠️ Please select an Ollama model first."

        return

    # Read incoming message.
    if isinstance(message, dict):

        user_text = extract_text(
            message.get("text") or ""
        )

        files = message.get("files") or []

    else:

        user_text = extract_text(message)

        files = []

    # Process uploaded files.
    file_text, warnings = process_files(files)

    prompt = user_text + file_text

    if not prompt.strip():

        yield (
            "⚠️ Please enter a message or upload "
            "a supported text file."
        )

        return

    # Prepare conversation history.
    messages = prepare_history(history)

    messages.append({
        "role": "user",
        "content": prompt
    })

    warning_text = "\n".join(
        f"⚠️ {warning}" for warning in warnings
    )

    response_text = ""

    try:

        logger.info(
            "Starting generation | model=%s | prompt_chars=%s",
            model_name,
            len(prompt)
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

        # Stream response.
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

        # Handle empty responses.
        if not response_text:

            yield (
                warning_text + "\n\n"
                "⚠️ Ollama completed the request but returned "
                "an empty response. Try again or select another model."
            )

        logger.info(
            "Generation completed | model=%s | characters=%s",
            model_name,
            len(response_text)
        )

    except Exception as exc:

        error_message = format_generation_error(
            exc,
            model_name
        )

        yield (
            warning_text + "\n\n" + error_message
            if warning_text
            else error_message
        )


# ============================================================
# UI STYLING
# ============================================================


HEAD_STYLE = """
<style>
html, body {
    width: 100% !important;
    height: 100% !important;
    margin: 0 !important;
    overflow: hidden !important;
}

.gradio-container {
    width: 100% !important;
    max-width: 100% !important;
    height: 100dvh !important;
    min-height: 0 !important;
    overflow: hidden !important;
}

footer {
    display: none !important;
}

/* Main application */
.gradio-container > .main {
    height: 100% !important;
    min-height: 0 !important;
    overflow: hidden !important;
}

/* ChatInterface container */
.gradio-container .chatbot {
    min-height: 0 !important;
}

/* Chat message scrolling */
.gradio-container .bubble-wrap {
    flex: 1 1 0 !important;
    min-height: 0 !important;
    overflow-y: auto !important;
    overflow-x: hidden !important;
}

/* Keep the chat input visible */
.gradio-container .input-container {
    flex: 0 0 auto !important;
    position: relative !important;
    z-index: 10 !important;
    background: var(--background-fill-primary) !important;
    padding: 10px !important;
}

/* Prevent chat layout from expanding */
.gradio-container .wrap {
    min-height: 0 !important;
}

/* Smooth scrolling */
.gradio-container .bubble-wrap {
    scroll-behavior: smooth;
}

/* Hide footer */
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

    # --------------------------------------------------------
    # SIDEBAR
    # --------------------------------------------------------

    with gr.Sidebar(
        label="ChatGABO Controls",
        open=True
    ):

        gr.Markdown("""
        # 🦙 ChatGABO
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

        - TXT and Markdown
        - Python and source-code files
        - CSV and JSON
        - YAML and TOML
        - HTML and CSS
        - SQL, Shell and Log files

        ### Upload restrictions

        - Maximum 5 files per message
        - Maximum 5 MB per file
        - Maximum 50,000 extracted characters
        - Text files only
        - No image or binary uploads
        """)

    # --------------------------------------------------------
    # MAIN CHAT
    # --------------------------------------------------------

    gr.ChatInterface(
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
                ".sql", ".sh", ".ini", ".toml"
            ],

            sources=["upload"]
        )
    )

    # --------------------------------------------------------
    # REFRESH MODELS
    # --------------------------------------------------------

    refresh_button.click(
        fn=refresh_models,
        inputs=None,
        outputs=[
            model_dropdown,
            model_status
        ]
    )

    # --------------------------------------------------------
    # INITIAL MODEL DISCOVERY
    # --------------------------------------------------------

    demo.load(
        fn=refresh_models,
        inputs=None,
        outputs=[
            model_dropdown,
            model_status
        ]
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
