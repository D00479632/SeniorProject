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

@app.route('/shutdown', methods=['GET'])  
def shutdown():
    os.kill(os.getpid(), signal.SIGINT)  # Send a SIGINT signal to the current process to shut it down
    return 'Server shutting down...'

def get_ollama_response(question, context=""):
    question2 = "What gifts does Clint love?"
    context2 = get_marqo_context(question2)
    question3 = "Give me a list of people that like Amethyst."
    context3 = get_marqo_context(question3)
    messages = [
        {"role": "system", "content": """
        ALL RESPONSES SHOULD ONLY PERTAIN TO THE STARDEW VALLEY VIDEO GAME, NOT REAL LIFE.
        You are a Stardew Valley game assistant. Provide answers that are strictly relevant to the question asked.
        You use the information text to answer the user better.
        Avoid unnecessary context or elaboration. Focus solely on the question and provide a direct answer. """},
        {"role": "user", "content": f"Given this information: {context2} please answer the following question: {question2}"}, 
        {"role": "assistant", "content": "Clint loves: Amethyst, Aquamarine, Artichoke Dip, Emerald, Fiddlehead Risotto, Gold Bar, Iridium Bar, Jade, Omni Geode, Ruby, Topaz"}, 
        {"role": "user", "content": f"Given this information: {context3} please answer the following question: {question3}"}, 
        {"role": "assistant", "content": "Like:  Alex •  Caroline •  Demetrius •  Elliott •  Evelyn •  George •  Gus •  Haley •  Harvey •  Jas •  Jodi •  Kent •  Krobus •  Leo •  Lewis •  Marnie •  Maru •  Pam •  Penny •  Robin •  Sam •  Sandy •  Sebastian •  Shane •  Vincent •  Willy •  Wizard"}, 
        {"role": "user", "content": f"Given this information: {context} please answer the following question: {question}"},
    ]
    # llama3.2:1b
    MODEL = "gemma3:4b"
    response = chat(MODEL, messages=messages)
    return response['message']['content']

def get_marqo_context(question):
    index_name = 'stardew-valley-data' 
    # Set up Marqo Client
    mq = Client(url='http://localhost:8882')
    
    print("Searching marqo")
    # Perform search on Marqo index
    results = mq.index(index_name).search(
        q=question,
        limit=2
    )

    # Prepare context
    context = ''
    # Reverse the hits to put the most relevant source at the end
    for i, hit in enumerate(reversed(results['hits'])):
        title = hit['Title']
        text = hit['Description']
        context += f"Source {i + 1}) {title} || {text} \n"
    return context

@app.route('/ask', methods=['POST'])
def ask_question():
    data = request.get_json()
    question = data.get('query', '')

    try: 
        context = get_marqo_context(question)
        
        final_response = get_ollama_response(question, context)
        return jsonify({'answer': final_response})
    except Exception as e:
        return jsonify({'error': str(e)}), 500

if __name__ == '__main__':
    # Start heartbeat monitoring in a background thread
    monitor_thread = threading.Thread(target=check_heartbeat, daemon=True)
    monitor_thread.start()
    
    app.run(host='0.0.0.0', port=8080)  # Run the Flask application on all available IP addresses at port 8080
