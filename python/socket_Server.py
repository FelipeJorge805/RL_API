import json
import socket

def start_rl_server(host='127.0.0.1', port=5000):
    server = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    server.bind((host, port))
    server.listen(1)
    print(f"Server listening on {host}:{port}...")

    while True:
        try:
            conn, addr = server.accept()
            print(f"Connected to {addr}")
            conn_file = conn.makefile('rwb')
            return conn_file  # Give back connection for training loop
        except Exception as e:
            print(f"Accept failed: {e}")
            continue


def receive_packet(conn_file):
    try:
        data = conn_file.readline()
        if not data:
            raise ConnectionError("Client disconnected.")
        data = data.decode('utf-8')
        return json.loads(data)
    except Exception as e:
        raise ConnectionError(f"Error receiving: {e}")


def send_action(conn_file, action_packet):
    try:
        payload = json.dumps(action_packet).encode('utf-8') + b'\n'
        conn_file.write(payload)
        conn_file.flush()
    except Exception as e:
        raise ConnectionError(f"Error sending: {e}")
