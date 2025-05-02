import os
import subprocess
import json
import time

# Config: Not yet using config file
# Starts a single tModLoader client based on the available folders in /Tmods.
# Expects each desired agent_id to have its own folder: tModLoader1 for agent1, tModLoader2 for agent2, etc.
# Currently writes a config file for the respective folder id, with server_ip, server_port and agent_port
# Starts a subprocess with the configs set
# sleeps between each client spawn
def terraria_instance(agent_id, base_path=r"D:\SteamLibrary\steamapps\common\Tmods", base_folder_name="tModLoader", server_ip="127.0.0.1", server_port=7777, agent_base_port=5000):
    agent_folder = os.path.join(base_path, f"{base_folder_name}{agent_id+1}")  # Folder names: tModLoader1, tModLoader2, etc.
    exe_path = os.path.join(agent_folder, "start-tModLoader.bat")

    if not os.path.exists(exe_path):
        raise FileNotFoundError(f"Cannot find {exe_path}")

    # Write client config
    agent_port = agent_base_port + agent_id
    config = {
        "server_ip": server_ip,
        "server_port": server_port,
        "agent_port": agent_port
    }
    config_path = os.path.join(agent_folder, "client_config.json")
    with open(config_path, "w") as f:
        json.dump(config, f)

    print(f"[Agent {agent_id}] Launching Terraria from {agent_folder} with --agent{agent_id}...")

    # Launch tModLoader.exe directly with --agentX
    subprocess.Popen([
        exe_path,
        f"--agent={agent_id}",
        "-playersave", f"Agent{agent_id}",
        "-ip", "127.0.0.1",
        "-port", "7777",
        "-pass", "mypassword"
    ], cwd=agent_folder)
    
    '''subprocess.Popen([
        "path./start-tModLoader.sh",
        f"--agent{agent_id}",
        "-playersave", f"Agent{agent_id + 1}",
        "-ip", "127.0.0.1",
        "-port", "7777",
        "-pass", "mypassword"
    ], cwd="/home/youruser/tModLoaderFolder")
    '''
    
    time.sleep(10)  # Delay to avoid overloading
