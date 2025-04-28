import socket
import threading

'''
UNTESTED YET - AI GENERATED
'''

def handle_agent(conn, addr):
    print(f"[Server] Connected: {addr}")
    with conn, conn.makefile('rwb') as stream:
        while True:
            line = stream.readline()
            if not line:
                break
            # parse obs, model forward, send action
            # same as single agent version

def start_server(host="127.0.0.1", base_port=5000, num_agents=4):
    servers = []
    for i in range(num_agents):
        port = base_port + i
        server = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        server.bind((host, port))
        server.listen(1)
        print(f"[Server] Listening on port {port}")

        def accept_connections(server_socket):
            conn, addr = server_socket.accept()
            threading.Thread(target=handle_agent, args=(conn, addr)).start()

        threading.Thread(target=accept_connections, args=(server,)).start()
        servers.append(server)

if __name__ == "__main__":
    start_server()
