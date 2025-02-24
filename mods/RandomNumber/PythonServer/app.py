from flask import Flask, jsonify, request
import random
import os
import signal

app = Flask(__name__)  # Create a new Flask application instance

@app.route('/random', methods=['GET'])  # Define a route for the function (getting a random number)
def get_random_number():
    random_number = random.randint(1, 100)  # Generate a random number between 1 and 100
    return jsonify({'random_number': random_number})  # Return the random number as a JSON response

@app.route('/shutdown', methods=['GET'])  # Define a route for shutting down the server
def shutdown():
    os.kill(os.getpid(), signal.SIGINT)  # Send a SIGINT signal to the current process to shut it down
    return 'Server shutting down...'  

if __name__ == '__main__':
    app.run(host='0.0.0.0', port=8080)  # Run the Flask application on all available IP addresses at port 8080
