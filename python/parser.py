import json

import numpy as np

def parse_observation(data):
    """
    Parse the full compressed RLObservation.

    Args:
        obs_flat: List[float] from Terraria

    Returns:
        dict of structured parsed fields
        :param data:
    """
    obs_packet = json.loads(data)
    obs = obs_packet["obs"]
    reward = obs_packet["reward"]
    obs_flat = np.array(obs, dtype=np.float32)
    idx = 0
    #print(f"obs_flat.shape: {obs_flat.shape}, obs_flat: {obs_flat}")

    return obs_flat, reward
