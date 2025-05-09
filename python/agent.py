import torch
import torch.nn as nn
import torch.nn.functional as F

# All available 'moves' 
MOVEMENT_ACTIONS = ["left", "right", "still", "up", "down"]

# All available 'actions' (for now)
MAIN_ACTIONS = [
    "use_item",
    "esc",
    "jump",
    "quick_heal",
    "quick_mana",
    "quick_buff",
    "swap_hotbar",
    "discard",
    "grapple",
    "interact",
    "mount",
    #"hotbar_0",
    #"hotbar_1",
    #"hotbar_2",
    #"hotbar_3",
    #"hotbar_4",
    #"hotbar_5",
    #"hotbar_6",
    #"hotbar_7",
    #"hotbar_8",
    #"hotbar_9",
    "scroll_up",
    "scroll_down",
    "none"
]

# Agent class to be trained.
# Currently 5 heads: movement, action, cursor, shift and value (ppo)
class TerrariaAgent(nn.Module):
    def __init__(self, input_size):
        super().__init__()

        self.shared = nn.Sequential(
            nn.Linear(input_size, 256),
            nn.ReLU(),
            nn.Linear(256, 128),
            nn.ReLU()
        )

        self.movement_head = nn.Linear(128, len(MOVEMENT_ACTIONS))
        self.action_head = nn.Linear(128, len(MAIN_ACTIONS))
        self.cursor_head = nn.Linear(128, 2)
        self.shift_head = nn.Linear(128, 1)
        self.value_head = nn.Linear(128, 1)

    def forward(self, x):
        x = self.shared(x)

        move_logits = self.movement_head(x)
        action_logits = self.action_head(x)
        cursor_delta = self.cursor_head(x)
        shift_logit = self.shift_head(x)
        value_logits = self.value_head(x)

        shift_prob = torch.sigmoid(shift_logit)
        
        return move_logits, action_logits, cursor_delta, shift_prob, value_logits