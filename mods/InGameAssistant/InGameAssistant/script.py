from flask import Flask, jsonify, request
import random
import os
import signal
import threading
import time

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

if __name__ == '__main__':
    # Start heartbeat monitoring in a background thread
    monitor_thread = threading.Thread(target=check_heartbeat, daemon=True)
    monitor_thread.start()
    
    app.run(host='0.0.0.0', port=8080)  # Run the Flask application on all available IP addresses at port 8080
