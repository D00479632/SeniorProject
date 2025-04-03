from flask import Flask, jsonify, request
import random
import os
import signal
import threading
import time
from marqo import Client
from ollama import chat

app = Flask(__name__)  # Create a new Flask application instance
last_heartbeat = time.time()
TIMEOUT_SECONDS = 90  # Longer than the 30-second interval to allow for some latency

def check_heartbeat():
    while True:
        time.sleep(5)  # Check every 5 seconds
        if time.time() - last_heartbeat > TIMEOUT_SECONDS:
            print("No heartbeat received in 90 seconds. Shutting down server...")
            os.kill(os.getpid(), signal.SIGINT)

@app.route('/heartbeat', methods=['POST'])
def heartbeat():
    global last_heartbeat
    last_heartbeat = time.time()
    return jsonify({'status': 'ok'}), 200

@app.route('/random', methods=['POST'])  # Change to POST method
def get_random_number():
    # Getting the data sent
    data = request.get_json()
    max_number = data.get('max', 0)  # Default to 0 if not provided
    random_number = random.randint(0, max_number)  # Generate a random number between 0 and max_number
    return jsonify({'random_number': random_number})

@app.route('/shutdown', methods=['GET'])  
def shutdown():
    os.kill(os.getpid(), signal.SIGINT)  # Send a SIGINT signal to the current process to shut it down
    return 'Server shutting down...'

def get_ollama_response(question, context=""):
    messages = [
        {"role": "system", "content": """
        ALL RESPONSES SHOULD ONLY PERTAIN TO THE STARDEW VALLEY VIDEO GAME, NOT REAL LIFE.
        You use the text to answer the user better.
        You are a Stardew Valley game assistant. Provide answers that are strictly relevant to the question asked.
        Avoid unnecessary context or elaboration. Focus solely on the question and provide a direct answer.
        For example, if asked 'can I plant tomatoes in the spring?', respond with 'tomatoes only grow in summer, so you cannot plant them in spring.'"""},
        {"role": "user", "content": f"{context} {question}"},
    ]
    # llama3.2:1b
    MODEL = "gemma3:4b"
    response = chat(MODEL, messages=messages)
    return response['message']['content']

@app.route('/ask', methods=['POST'])
def ask_question():
    data = request.get_json()
    question = data.get('query', '')
    
    # Print the received question for debugging
    print(f"Received question: {question}")
    
    index_name = 'stardew-valley-data' 
    # Set up Marqo Client
    mq = Client(url='http://localhost:8882')
    
    try:
        print("Searching marqo")
        # Perform search on Marqo index
        results = mq.index(index_name).search(
            q=question,
            limit=1
        )

        # Prepare context
        context = ''
        for i, hit in enumerate(results['hits']):
            title = hit['Title']
            text = hit['Description']
            context += f"Source {i + 1}) {title} || {text} \n"

        # Get response from Ollama
        print("Asking ollama")
        final_response = get_ollama_response(question, context)
        return jsonify({'answer': final_response})
    except Exception as e:
        return jsonify({'error': str(e)}), 500

if __name__ == '__main__':
    # Start heartbeat monitoring in a background thread
    monitor_thread = threading.Thread(target=check_heartbeat, daemon=True)
    monitor_thread.start()
    
    app.run(host='0.0.0.0', port=8080)  # Run the Flask application on all available IP addresses at port 8080
