import socket
import json

def start_server(host="127.0.0.1", port=5000):
    with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as server:
        server.bind((host, port))
        server.listen(1)
        print(f"[Python] Listening on {host}:{port}...")

        conn, addr = server.accept()
        print(f"[Python] Connected by {addr}")

        with conn, conn.makefile("rwb") as stream:
            while True:
                # Receive a line (JSON observation)
                line = stream.readline()
                if not line:
                    break  # client disconnected

                obs = json.loads(line.decode().strip())
                print(f"[Python] Received observation: {obs}")
                print(line)

                # Compute action (placeholder here)
                random_number = random.randint(1, 2)
                
                action = {"action": "right"} if random_number==1 else action={"action": "left"}

                # Send back the JSON response
                reply = (json.dumps(action) + "\n").encode()
                stream.write(reply)
                stream.flush()

if __name__ == "__main__":
    start_server()
