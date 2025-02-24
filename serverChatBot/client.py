import requests
import threading
import time
import sys

SERVER_URL = "http://localhost:8080"
HEARTBEAT_INTERVAL = 30  # seconds

def send_heartbeat():
    while True:
        try:
            response = requests.post(f"{SERVER_URL}/heartbeat")
            if response.status_code != 200:
                print("Warning: Failed to send heartbeat")
        except Exception as e:
            print(f"Error sending heartbeat: {e}")
        time.sleep(HEARTBEAT_INTERVAL)

def chat_with_model(message):
    try:
        print(f"Sending message to server: {message}")  # Debugging line
        response = requests.post(f"{SERVER_URL}/chat", json={"message": message})
        print(f"Received response: {response.status_code}")  # Debugging line
        if response.status_code == 200:
            return response.json()['response']
        else:
            return f"Error: Server returned status code {response.status_code}"
    except Exception as e:
        return f"Error communicating with server: {e}"

def shutdown_server():
    try:
        response = requests.get(f"{SERVER_URL}/shutdown")
        print("Server shutdown request sent:", response.text)
    except Exception as e:
        print(f"Error shutting down server: {e}")

def main():
    # Start heartbeat in background thread
    heartbeat_thread = threading.Thread(target=send_heartbeat, daemon=True)
    heartbeat_thread.start()

    print("Chat client started. Type 'quit' to exit.")
    print("Connecting to server at", SERVER_URL)
    
    while True:
        try:
            user_input = input("\nYou: ").strip()
            
            if user_input.lower() in ['quit', 'exit']:
                print("Shutting down client and server...")
                shutdown_server()
                sys.exit(0)
                
            if user_input:
                response = chat_with_model(user_input)
                print("\nAssistant:", response)
                
        except KeyboardInterrupt:
            print("\nShutting down client and server...")
            shutdown_server()
            sys.exit(0)
        except Exception as e:
            print(f"Error: {e}")

if __name__ == "__main__":
    main()
