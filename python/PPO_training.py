import torch
import torch.nn.functional as F
import torch.optim as optim
from torch.distributions import Categorical, Bernoulli

import socket_Server as ss
import agent as a

# === Hyperparameters ===
GAMMA = 0.99
EPS_CLIP = 0.2
ENTROPY_COEF = 0.01
VALUE_COEF = 0.5
TRAIN_FREQ = 1024  # Train every 1024 transitions
MINIBATCH_SIZE = 64
PPO_EPOCHS = 4

# === Initialization ===
conn = ss.start_rl_server()
model = a.TerrariaAgent(155)
optimizer = optim.Adam(model.parameters(), lr=1e-4)

buffer = []  # Global training buffer
step_count = 0

last_obs = None
last_action = None
last_logprobs = None
last_value = None
last_reward = None

def compute_returns(rewards, dones, values, gamma=GAMMA):
    returns = []
    R = 0
    for r, d in zip(reversed(rewards), reversed(dones)):
        R = r + gamma * R * (1 - d)
        returns.insert(0, R)
    return torch.tensor(returns, dtype=torch.float32)

while True:
    packet = ss.receive_packet(conn)
    obs = packet["Obs"]
    reward = packet["Reward"]
    done = packet.get("isDone", False)

    obs_tensor = torch.tensor(obs, dtype=torch.float32).unsqueeze(0)
    move_logits, action_logits, cursor_delta, shift_prob, value = model(obs_tensor)

    # Sample actions
    move_dist = Categorical(logits=move_logits)
    action_dist = Categorical(logits=action_logits)
    shift_dist = Bernoulli(probs=shift_prob)

    move_idx = move_dist.sample()
    action_idx = action_dist.sample()
    shift_act = shift_dist.sample()

    cursor_output = torch.tanh(cursor_delta).detach().cpu().numpy()[0]

    # Send action to game
    reply_dict = {
        "Move": a.MOVEMENT_ACTIONS[move_idx.item()],
        "Action": a.MAIN_ACTIONS[action_idx.item()],
        "Cursor": cursor_output.tolist(),
        "Shift": bool(shift_act.item())
    }
    ss.send_action(conn, reply_dict)

    # Store transition if not first step
    if last_obs is not None:
        buffer.append({
            "obs": last_obs,
            "action": last_action,
            "logprob": last_logprobs,
            "value": last_value,
            "reward": last_reward,
            "done": float(done),
            "next_obs": obs
        })
        step_count += 1

    # Update for next step
    last_obs = obs
    last_action = (move_idx.item(), action_idx.item(), cursor_output, shift_act.item())
    last_logprobs = (
        move_dist.log_prob(move_idx).item(),
        action_dist.log_prob(action_idx).item(),
        shift_dist.log_prob(shift_act).item()
    )
    last_value = value.item()
    last_reward = reward

    # === Training trigger ===
    if step_count >= TRAIN_FREQ:
        print(f"[TRAINING] Updating policy with {step_count} steps")

        # Convert buffer into tensors
        states = torch.tensor([b["obs"] for b in buffer], dtype=torch.float32)
        actions = torch.tensor([[b["action"][0], b["action"][1], b["action"][3]] for b in buffer], dtype=torch.float32)
        old_logprobs = torch.tensor([b["logprob"] for b in buffer], dtype=torch.float32)
        rewards = [b["reward"] for b in buffer]
        dones = [b["done"] for b in buffer]
        values = torch.tensor([b["value"] for b in buffer], dtype=torch.float32)

        returns = compute_returns(rewards, dones, values)
        advantages = returns - values
        advantages = (advantages - advantages.mean()) / (advantages.std() + 1e-8)

        for _ in range(PPO_EPOCHS):
            for i in range(0, len(states), MINIBATCH_SIZE):
                batch_slice = slice(i, i + MINIBATCH_SIZE)
                s_batch = states[batch_slice]
                a_batch = actions[batch_slice]
                logp_old_batch = old_logprobs[batch_slice]
                adv_batch = advantages[batch_slice]
                ret_batch = returns[batch_slice]

                move_logits, action_logits, cursor_delta, shift_prob, values_pred = model(s_batch)

                move_dist = Categorical(logits=move_logits)
                action_dist = Categorical(logits=action_logits)
                shift_dist = Bernoulli(probs=shift_prob)

                move_logp = move_dist.log_prob(a_batch[:, 0].long())
                action_logp = action_dist.log_prob(a_batch[:, 1].long())
                shift_logp = shift_dist.log_prob(a_batch[:, 2])

                entropy = move_dist.entropy() + action_dist.entropy() + shift_dist.entropy()

                total_logp = move_logp + action_logp + shift_logp
                total_old_logp = logp_old_batch.sum(dim=1)
                ratio = torch.exp(total_logp - total_old_logp)

                surr1 = ratio * adv_batch
                surr2 = torch.clamp(ratio, 1 - EPS_CLIP, 1 + EPS_CLIP) * adv_batch
                policy_loss = -torch.min(surr1, surr2).mean()

                value_loss = F.mse_loss(values_pred.squeeze(), ret_batch)

                loss = policy_loss + VALUE_COEF * value_loss - ENTROPY_COEF * entropy.mean()

                optimizer.zero_grad()
                loss.backward()
                optimizer.step()

        # Clear buffer
        buffer.clear()
        step_count = 0
        last_obs = None
        last_action = None
        last_logprobs = None
        last_value = None
        last_reward = None
