
import json
import time
import torch
import torch.nn.functional as F
import socket
import threading
from trainer import global_model, experience_buffer, buffer_lock, device
from config import AGENT_BASE_PORT, NUM_AGENTS, SERVER_IP
import agent

# Config: None
# Handles the socket connection for each Agent thread.
# Decodes the socket message and passes it to the model
# Maps the model response to a Reply
# Writes the reply back to the tMod client
# Saves the Obs for later Batching
# Handles end of Episode and waiting for new tMod client data
def handle_agent_connection(conn, addr, agent_id):
    c_ip, client_port = addr
    print(f"[Agent {agent_id}:{client_port}] Connected.")

    last_obs = None
    last_action = None
    last_reward = None

    with conn, conn.makefile('rwb') as stream:
        try:
            while True:
                #print("before line read")
                line = stream.readline()
                #print("After line read")
                if not line:
                    print(f"[Agent {agent_id}:{client_port}] Disconnected.")
                    break
                
                packet = json.loads(line.decode('utf-8'))
                #print("packet exists")
                #print(f"[Agent {agent_id}] Received: {line.decode('utf-8').strip()}")
                # Receive input_size # Firt tick only
                if "input_size" in packet:
                    input_size = packet["input_size"]
                    stream.write(json.dumps({ "Move": "Still", "Action": "None", "Cursor": [0, 0], "Shift": False }).encode('utf-8') + b'\n')
                    stream.flush()
                    #print(f"[Agent {agent_id}] Replied to init.")
                    #model = build_model(input_size)
                    continue
                #print("first tick done")
                # Receive data
                obs = packet["Obs"]
                reward = packet["Reward"]
                done = packet.get("isDone", False)

                obs_tensor = torch.tensor(obs, dtype=torch.float32).unsqueeze(0).to(device)
                #print("before mapping")
                # Map model Input
                with torch.no_grad():
                    move_logits, action_logits, cursor_delta, shift_logit, value = global_model(obs_tensor)

                move_idx = torch.multinomial(F.softmax(move_logits, dim=-1), num_samples=1).item()
                action_idx = torch.multinomial(F.softmax(action_logits, dim=-1), num_samples=1).item()
                cursor_output = torch.tanh(cursor_delta).detach().cpu().numpy()[0]
                shift_active = torch.sigmoid(shift_logit).item() > 0.5
                #print("before reply")
                reply = {
                    "Move": agent.MOVEMENT_ACTIONS[move_idx],
                    "Action": agent.MAIN_ACTIONS[action_idx],
                    "Cursor": cursor_output.tolist(),
                    "Shift": shift_active
                }

                stream.write(json.dumps(reply).encode('utf-8') + b'\n')
                stream.flush()
                #print(f"After flush! Reply: {reply}")
                # Save for batching
                if last_obs is not None:
                    with buffer_lock:
                        experience_buffer.append((last_obs, last_action, last_reward, obs))

                last_obs = obs
                last_action = (move_idx, action_idx, cursor_output, shift_active)
                last_reward = reward

                # End of Episode (Death)
                if done:
                    print(f"[Agent {agent_id}] Died. Waiting to respawn...")
                    last_obs = None
                    last_action = None
                    last_reward = None

                    # Keeps waiting for Client response, which happens on respawn
                    while True:
                        line = stream.readline()
                        if not line:
                            print(f"[Agent {agent_id}] Disconnected during respawn.")
                            return

                        packet = json.loads(line.decode('utf-8'))
                        obs = packet["Obs"]
                        done = packet.get("isDone", False)

                        if not done:
                            last_obs = obs
                            break
                        time.sleep(5)
                
                #time.sleep(0.02)  # 20 milliseconds

        except Exception as e:
            print(f"[Agent {agent_id}:{client_port}] Error: {e}")

# Config: NUM_AGENTS, AGENT_BASE_PORT, SERVER_IP
# Returns: a server array of connected sockets
# Creates the socket connection for each Agent, and a unique port per Agent
# Accepts a connection in a new thread using handle_agent_connection()
def start_training_servers():
    servers = []
    for agent_id in range(NUM_AGENTS):
        port = AGENT_BASE_PORT + agent_id
        server = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        server.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
        server.bind((SERVER_IP, port))
        server.listen(1)
        print(f"[Socket] Waiting for Agent {agent_id} on port {port}...")

        conn, addr = server.accept()
        print(f"[Agent {agent_id}:{addr[1]}] Connected.")
        threading.Thread(
            target=handle_agent_connection,
            args=(conn, addr, agent_id),
            daemon=True
        ).start()

        servers.append(server)

    return servers
