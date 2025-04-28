import torch
import torch.nn.functional as F
import torch.optim as optim
import json

from torch.nn import MSELoss

import socket_Server as ss
import agent as a

conn = ss.start_rl_server()
model = a.TerrariaAgent(155)  # Assume already defined
optimizer = optim.Adam(model.parameters(), lr=1e-4)
loss_fn = torch.nn.MSELoss()

last_obs = None
last_action = None
last_reward = None
episode_buffer = []

while True:
    packet = ss.receive_packet(conn)
    obs = packet["Obs"]
    reward = packet["Reward"]
    done = packet.get("isDone", False)

    obs_tensor = torch.tensor(obs, dtype=torch.float32).unsqueeze(0)

    move_logits, action_logits, cursor_delta, shift_logit = model(obs_tensor)

    # Pick actions
    move_probs = F.softmax(move_logits, dim=-1)
    move_idx = torch.multinomial(move_probs, num_samples=1).item()

    action_probs = F.softmax(action_logits, dim=-1)
    action_idx = torch.multinomial(action_probs, num_samples=1).item()

    cursor_output = torch.tanh(cursor_delta).detach().cpu().numpy()[0]
    #print(f"cursor {cursor_output}")# (-1, 1)
    shift_active = torch.sigmoid(shift_logit).item() > 0.5

    reply_dict = {
        "Move": a.MOVEMENT_ACTIONS[move_idx],
        "Action": a.MAIN_ACTIONS[action_idx],
        "Cursor": cursor_output.tolist(),
        "Shift": shift_active
    }
    print(f"reward: {reward:.3f} | reply: {reply_dict}")
    ss.send_action(conn, reply_dict)

    # Store transition if not first step
    if last_obs is not None:
        episode_buffer.append((last_obs, last_action, last_reward, obs))

    # Update buffers
    last_obs = obs
    last_action = (move_idx, action_idx, cursor_output, shift_active)
    last_reward = reward

    # Handle end of episode
    if done:
        print(f"Episode finished, training on {len(episode_buffer)} steps")

        # Training step
        for (obs0, act0, rew0, obs1) in episode_buffer:
            features0 = torch.tensor(obs0, dtype=torch.float32).unsqueeze(0)
            features1 = torch.tensor(obs1, dtype=torch.float32).unsqueeze(0)

            move_logits, action_logits, cursor_delta, shift_logit = model(features0)

            # Dummy simple loss: reward as MSE to zero (placeholder)
            target = torch.tensor([[rew0]], dtype=torch.float32)
            output = move_logits.mean().unsqueeze(0).unsqueeze(0)  # Example: mean move prediction
            loss = MSELoss()

            optimizer.zero_grad()
            loss.backward()
            optimizer.step()

        # Clear episode buffer
        episode_buffer.clear()
        last_obs = None
        last_action = None
        last_reward = None
