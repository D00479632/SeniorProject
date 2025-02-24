from flask import Flask, jsonify, request
import random
import os
import signal
import threading
import time
from transformers import AutoModelForCausalLM, AutoTokenizer, pipeline

app = Flask(__name__)
last_heartbeat = time.time()
TIMEOUT_SECONDS = 90

# Using a smaller model that's more suitable for CPU/M2 chip
model_name = "facebook/opt-125m"  # Much smaller model that can run on CPU
tokenizer = AutoTokenizer.from_pretrained(model_name)
model = AutoModelForCausalLM.from_pretrained(
    model_name,
    device_map="auto",  # This will automatically use the best available device
    torch_dtype="auto"  # This will automatically choose the best dtype
)

# Initialize the pipeline with more modest parameters
pipe = pipeline(
    "text-generation",
    model=model,
    tokenizer=tokenizer,
    max_new_tokens=128,  # Reduced from 512 to improve speed
    do_sample=True,
    temperature=0.7,
    top_p=0.95,
    repetition_penalty=1.15
)

def check_heartbeat():
    while True:
        time.sleep(5)
        if time.time() - last_heartbeat > TIMEOUT_SECONDS:
            print("No heartbeat received in 90 seconds. Shutting down server...")
            os.kill(os.getpid(), signal.SIGINT)

@app.route('/heartbeat', methods=['POST'])
def heartbeat():
    global last_heartbeat
    last_heartbeat = time.time()
    return jsonify({'status': 'ok'}), 200

@app.route('/chat', methods=['POST'])
def chat():
    try:
        data = request.get_json()
        if not data or 'message' not in data:
            return jsonify({'error': 'Please provide a message in the request body'}), 400

        user_message = data['message']
        
        # Generate response
        response = pipe(user_message)
        
        return jsonify({
            'response': response[0]['generated_text']
        })
    except Exception as e:
        return jsonify({'error': str(e)}), 500

@app.route('/shutdown', methods=['GET'])
def shutdown():
    os.kill(os.getpid(), signal.SIGINT)
    return 'Server shutting down...'

if __name__ == '__main__':
    # Start heartbeat monitoring in a background thread
    monitor_thread = threading.Thread(target=check_heartbeat, daemon=True)
    monitor_thread.start()
    
    app.run(host='0.0.0.0', port=8080)