import signal
import socket
import json
import sys

import torch
import torch.nn.functional as F
import torch.optim as optim
from torch.nn import MSELoss

import parser as p
import agent as a

def signal_handler(sig, frame):
    print('You pressed Ctrl+C!')
    sys.exit(0)

def start_server(host="127.0.0.1", port=5000):
    optimizer = optim.Adam(model.parameters(), lr=1e-4)

    with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as server:
        server.bind((host, port))
        server.listen(1)
        print(f"[Python] Listening on {host}:{port}...")

        conn, addr = server.accept()
        print(f"[Python] Connected by {addr}")

        signal.signal(signal.SIGINT, signal_handler)

        with conn, conn.makefile('rwb') as stream:
            while True:
                line = stream.readline()
                if not line:
                    break

                try:
                    line = line.decode("utf-8")
                    features, reward = p.parse_observation(line)
                    features_tensor = torch.tensor(features, dtype=torch.float32).unsqueeze(0)
                except Exception as e:
                    print(f"[Python] Parse error: {e}")
                    continue

                #print(f"obs: {features_tensor}")
                move_logits, action_logits, cursor_delta, shift_logit = model(features_tensor)


                loss = MSELoss()

                optimizer.zero_grad()
                loss.backward()
                optimizer.step()

                # Movement decision
                move_probs = F.softmax(move_logits, dim=-1)
                move_idx = torch.multinomial(move_probs, num_samples=1).item()
                #print(f"Move probs: {move_probs.detach().numpy()}")

                # Main action decision
                action_probs = F.softmax(action_logits, dim=-1)
                action_idx = torch.multinomial(action_probs, num_samples=1).item()

                # Cursor offset
                cursor_output = torch.tanh(cursor_delta)  # Now (-1, 1) range

                # Shift decision
                shift_active = torch.sigmoid(shift_logit).item() > 0.5

                reply_dict = {
                    "move": a.MOVEMENT_ACTIONS[move_idx],
                    "action": a.MAIN_ACTIONS[action_idx],
                    "cursor": cursor_output,
                    "shift": shift_active
                }
                print(f"reward: {reward:.3f} | reply: {reply_dict}")

                reply = json.dumps(reply_dict) + "\n"
                stream.write(reply.encode())
                stream.flush()
