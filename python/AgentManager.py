import os
import subprocess
import time
import json
import socket
import threading

import torch
import torch.nn.functional as F
import torch.optim as optim
from torch.nn import MSELoss

import agent  # Your TerrariaAgent
from start_terraria_instance import terraria_instance  # Your launch function

NUM_AGENTS = 2  # Change to how many agents you want
SERVER_IP = "127.0.0.1"
SERVER_PORT = 7777
AGENT_BASE_PORT = 5000
BASE_PATH = r"D:\SteamLibrary\steamapps\common\Tmods"  # Where your tModLoader_Agent folders are

def handle_agent_connection(conn, addr, agent_id):
    print(f"[Agent {agent_id}] Connected for training.")

    model = agent.TerrariaAgent(155)  # One model per agent
    optimizer = optim.Adam(model.parameters(), lr=1e-4)
    loss_fn = MSELoss()

    last_obs = None
    last_action = None
    last_reward = None
    episode_buffer = []

    with conn, conn.makefile('rwb') as stream:
        try:
            while True:
                line = stream.readline()
                if not line:
                    print(f"[Agent {agent_id}] Disconnected.")
                    break
                print(f"[Agent {agent_id}] Raw line: {line}")

                packet = json.loads(line.decode('utf-8'))
                obs = packet["Obs"]
                reward = packet["Reward"]
                done = packet.get("isDone", False)

                obs_tensor = torch.tensor(obs, dtype=torch.float32).unsqueeze(0)
                move_logits, action_logits, cursor_delta, shift_logit = model(obs_tensor)

                move_idx = torch.multinomial(F.softmax(move_logits, dim=-1), num_samples=1).item()
                action_idx = torch.multinomial(F.softmax(action_logits, dim=-1), num_samples=1).item()
                cursor_output = torch.tanh(cursor_delta).detach().cpu().numpy()[0]
                shift_active = torch.sigmoid(shift_logit).item() > 0.5

                reply = {
                    "Move": agent.MOVEMENT_ACTIONS[move_idx],
                    "Action": agent.MAIN_ACTIONS[action_idx],
                    "Cursor": cursor_output.tolist(),
                    "Shift": shift_active
                }

                # Send action back
                payload = json.dumps(reply).encode('utf-8') + b'\n'
                stream.write(payload)
                stream.flush()

                # Store transition
                if last_obs is not None:
                    episode_buffer.append((last_obs, last_action, last_reward, obs))

                last_obs = obs
                last_action = (move_idx, action_idx, cursor_output, shift_active)
                last_reward = reward

                if done:
                    print(f"[Agent {agent_id}] Episode finished with {len(episode_buffer)} steps.")

                    for (obs0, act0, rew0, obs1) in episode_buffer:
                        features0 = torch.tensor(obs0, dtype=torch.float32).unsqueeze(0)
                        features1 = torch.tensor(obs1, dtype=torch.float32).unsqueeze(0)

                        move_logits, action_logits, cursor_delta, shift_logit = model(features0)

                        target = torch.tensor([[rew0]], dtype=torch.float32)
                        output = move_logits.mean().unsqueeze(0).unsqueeze(0)

                        loss = loss_fn(output, target)

                        optimizer.zero_grad()
                        loss.backward()
                        optimizer.step()

                    episode_buffer.clear()
                    last_obs = None
                    last_action = None
                    last_reward = None

        except Exception as e:
            print(f"[Agent {agent_id}] Error: {e}")

def start_training_servers(num_agents, agent_base_port):
    servers = []

    for agent_id in range(num_agents):
        port = agent_base_port + agent_id
        server = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        server.bind(("127.0.0.1", port))
        server.listen(1)
        print(f"[Socket] Listening for Agent {agent_id} on port {port}")

        def accept_connection(server_socket, agent_id):
            conn, addr = server_socket.accept()
            threading.Thread(target=handle_agent_connection, args=(conn, addr, agent_id), daemon=True).start()

        threading.Thread(target=accept_connection, args=(server, agent_id), daemon=True).start()
        servers.append(server)

    return servers

if __name__ == "__main__":
    # Launch agent instances first
    for agent_id in range(NUM_AGENTS):
        terraria_instance(
            agent_id=agent_id,
            base_path=BASE_PATH,
            server_ip=SERVER_IP,
            server_port=SERVER_PORT,
            agent_base_port=AGENT_BASE_PORT
        )

    # Start training socket servers
    servers = start_training_servers(NUM_AGENTS, AGENT_BASE_PORT)

    print("[System] Launched all agents and training servers.")

    try:
        while True:
            time.sleep(1)
    except KeyboardInterrupt:
        print("[System] Shutting down...")
        for s in servers:
            s.close()
