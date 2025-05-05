# 🧠 Terraria RL Environment Mod (tModLoader + Reinforcement Learning)

This project integrates a custom `tModLoader` mod with a Python-based reinforcement learning backend to train agents to play **Terraria** using observation-action cycles.

## 🌟 Goal

Train an AI agent to:

* Navigate the world (movement, exploration)
* Fight enemies (combat, aiming)
* Gather resources
* Survive
* Craft
* Eventually: build

## 🧹 Project Structure

### 🔧 tModLoader Mod (C#)

* Collects game state info (tiles, inventory, HP, biomes, weather, etc.)
* Sends normalized observations to Python via socket
* Receives agent actions (movement, use item, cursor movement, etc.)
* Contains reward shaping logic
* Tries to acheive fake-headless mode by disabling visuals and sound
* Allows for multiple Agents to run concurrently

### 🐍 Python Agents (PyTorch)

* Neural net model with multiple output heads
* PPO training loop with experience buffer
* Socket server per agent instance
* Launch controller for running multiple Terraria clients
* JSON-based communication

## 🛠 How to Run

> ⚠️ Requires a working tModLoader install + Python 3.9+

1. Clone this repo
2. Compile the tModLoader mod (place it in `Mod Sources`)
3. Host a terraria multiplayer server by running `python terraria_server.py`
4. Run the Python backend with `python main.py`
5. The Python script will launch and manage Terraria agent instances automatically
6. Currently, you must join the server manually

## 📱 Communication Protocol

**C# → Python:**

```json
{
  "Obs": [...],
  "Reward": 0.0,
  "NextObs: [...],
  "Done": false
}
```

**Python → C#:**

```json
{
  "Move": "left",
  "Action": "use_item",
  "Cursor": [0.1, -0.2],
  "Shift": false
}
```

## ✅ Features

* Modular reward system (e.g. pickups, kills, survival time)
* Biome and weather awareness
* Buff/debuff vector encoding
* Multi-agent compatibility
* JSON socket protocol with compression
* Extensible for crafting, memory, and strategic play

## 📁 File Overview

| File                  | Purpose                         |
| --------------------- | ------------------------------- |
| `RLObsCollector.cs`   | Collect + normalize game state  |
| `RLObservation.cs`    | Defines the full structured obs |
| `RLCompressedObs.cs`  | Flattens obs into float\[]      |
| `RewardCalculator.cs` | Computes reward from obs deltas |
| `server.py`           | Socket server per agent         |
| `trainer.py`          | PPO trainer                     |
| `agent.py`            | Neural net logic                |
| `main.py`             | Launch controller               |

## 📜 License

This is a research + experimental project. Feel free to use it as long as you credit me.

## 🙏 Credits

Built using:

* [tModLoader](https://github.com/tModLoader/tModLoader)
* [PyTorch](https://pytorch.org/)
