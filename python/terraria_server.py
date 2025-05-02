import subprocess
import os
import time

# Starts a terraria server using the tModLoader serverconfig file
# Attempts to find files on path server_folder
# Starts a subprocess and keeps it open
def launch_server():
    server_folder = r"C:\Steam\steamapps\common\tModLoader"  # <-- Change this to your server folder
    batch_path = os.path.join(server_folder, "start-tModLoaderServer.bat")  # <-- THIS instead of tModLoader.exe
    config_path = os.path.join(server_folder, "serverconfig.txt")
    
    if not os.path.exists(batch_path):
        raise FileNotFoundError(f"Cannot find {batch_path}")

    print("[Server] Launching Terraria Server...")

    proc = subprocess.Popen(
        [batch_path, "-config", "-nosteam", config_path],
        cwd=server_folder
    )
    '''proc = subprocess.Popen([
        "./start-tModLoaderServer.sh",
        "-port", "7777",
        "-pass", "",
        "-players", "4"
    ], cwd="/home/youruser/tModLoaderFolder")
    '''
    time.sleep(10)  # Give server time to boot
    return proc

if __name__ == "__main__":
    server_proc = launch_server()
    print("[Server] Running... Press Ctrl+C to stop.")

    try:
        while True:
            time.sleep(1)
    except KeyboardInterrupt:
        print("[Server] Stopping...")
        server_proc.terminate()
