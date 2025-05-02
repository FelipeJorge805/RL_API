
import threading
import time
from start_terraria_instance import terraria_instance
from server import start_training_servers
from trainer import trainer_loop, load_model
from config import NUM_AGENTS, SERVER_IP, SERVER_PORT, AGENT_BASE_PORT, BASE_PATH

# Config: NUM_AGENTS, SERVER_IP, SERVER_PORT, AGENT_BASE_PORT, BASE_PATH
# Main function for flow control
# Spawns new Tmod Clients based on configs
# Loads the model data if available
# Starts trainig loops using Threads
if __name__ == "__main__":
    for agent_id in range(NUM_AGENTS):
        terraria_instance(
            agent_id=agent_id,
            base_path=BASE_PATH,
            server_ip=SERVER_IP,
            server_port=SERVER_PORT,
            agent_base_port=AGENT_BASE_PORT
        )

    load_model()
    servers = start_training_servers()
    threading.Thread(target=trainer_loop, daemon=True).start()

    print("[System] All agents and training systems are up.")
    try:
        while True:
            time.sleep(1)
    except KeyboardInterrupt:
        print("[System] Shutdown requested.")
        for s in servers:
            s.close()
