import os
import subprocess
import time

'''
UNTESTED YET - AI GENERATED
'''

def launch_terraria_instance(port):
    # Replace with your actual tModLoader/Terraria exe path
    terraria_path = r"C:\Path\To\tModLoader\start-tModLoader.bat"

    # Launch Terraria client with environment variable to tell it which port to use
    env = dict(os.environ)
    env["TERRALPHA_PORT"] = str(port)

    # Start the process
    return subprocess.Popen(terraria_path, env=env)

def start_multiple_instances(num_agents, base_port=5000):
    processes = []
    for i in range(num_agents):
        port = base_port + i
        proc = launch_terraria_instance(port)
        processes.append(proc)
        time.sleep(5)  # small delay between launches to avoid crashing startup
    return processes

if __name__ == "__main__":
    agents = start_multiple_instances(num_agents=4)
    print(f"Launched {len(agents)} Terraria clients.")
