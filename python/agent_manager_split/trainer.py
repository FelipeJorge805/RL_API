
import time
import torch
import torch.optim as optim
from torch.nn import MSELoss
import threading
from config import BATCH_SIZE, TRAIN_INTERVAL, SAVE_INTERVAL, MODEL_PATH
import agent

# Settings
global_model = agent.TerrariaAgent(155)
optimizer = optim.Adam(global_model.parameters(), lr=1e-4)
loss_fn = MSELoss()
experience_buffer = []
buffer_lock = threading.Lock()

# Trains the model using Batching, and Settings above
def train_batch(batch):
    for (obs0, act0, rew0, obs1) in batch:
        features0 = torch.tensor(obs0, dtype=torch.float32).unsqueeze(0)
        move_logits, action_logits, cursor_delta, shift_logit = global_model(features0)
        target = torch.tensor([[rew0]], dtype=torch.float32)
        output = move_logits.mean().unsqueeze(0).unsqueeze(0)
        loss = loss_fn(output, target)
        optimizer.zero_grad()
        loss.backward()
        optimizer.step()

# Config: TRAIN_INTERVAL, BATCh_SIZE, SAVE_INTERVAL
# Trainer loop that sleeps on TRAIN_INTERVAL
# Uses buffer_lock to read from the training buffer with BATCH_SIZE
# Trains the model and saves every SAVE_INTERVAL
def trainer_loop():
    last_save = time.time()
    while True:
        time.sleep(TRAIN_INTERVAL)
        with buffer_lock:
            if len(experience_buffer) >= BATCH_SIZE:
                batch = experience_buffer[:BATCH_SIZE]
                del experience_buffer[:BATCH_SIZE]
            else:
                batch = []
        if batch:
            print(f"[Trainer] Training on batch of {len(batch)} samples.")
            train_batch(batch)

        if time.time() - last_save > SAVE_INTERVAL:
            save_model()
            last_save = time.time()

# Config: MODEL_PATH
# Saves the model to MODEL_PATH
def save_model():
    torch.save(global_model.state_dict(), MODEL_PATH)
    print(f"[Trainer] Model saved to {MODEL_PATH}")

# Config: MODEL_PATH
# Attempts to load the model from MODEL_PATH
def load_model():
    try:
        global_model.load_state_dict(torch.load(MODEL_PATH))
        print(f"[Trainer] Loaded model from {MODEL_PATH}")
    except FileNotFoundError:
        print(f"[Trainer] No saved model found. Starting fresh.")
