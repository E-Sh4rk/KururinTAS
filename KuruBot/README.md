# KuruBot

## Instructions

### Prerequisite

- This bot has been designed for Windows, but it might also work on Linux or Mac.
- For Windows systems, .NET Framework 4 (or more) need to be installed. For Linux or Mac, Mono can be used as an alternative.
- This bot does not need to communicate with the game in order to work. However, we recommend to connect it to Bizhawks in order to easily transfer states and visualize solutions.

### Connecting the bot to Bizhawk

1. Place the script `kuruCOM.lua` in a folder with r/w permissions.
2. Start Kuru Kuru Kururin in Bizhawks and load this LUA script.
3. Connect the bot by clicking on the `Connect` button and selecting the LUA script (the same file that was loaded by Bizhawks, not a copy).
4. If the game is paused in Bizhawks, the communication will also be paused. In this case, you can advance a frame if you want to complete a pending transfer.

### Configuring the bot

- Depending on what kind of problem you want to solve, you can configure the bot by clicking on `Load config` and choosing the right configuration file
- These configuration files have the extension `.ini`. They contain some parameters for the bot

### Loading a map

- If the bot is connected to Bizhawks, you can load the current ingame map by clicking on the button `Download map`
- Alternatively, you can load a previously saved map by clicking on the button `LM` (Load Map)
- You can save a map by clicking on the button `SM` (Save Map)

### Loading the state of the helirin

- If the bot is connected to Bizhawks, you can load the current ingame state by clicking on the button `Download state`
- Alternatively, you can load a previously saved state by clicking on the button `LS` (Load State)
- You can save a state by clicking on the button `SS` (Save State)
- If you want to manually edit a state file, here is the list of values that it contains:  
`x_pos y_pos x_bump_speed y_bump_speed rotation rotation_rate rotation_default_rate hearts invulnerability_frames`
- The buttons `BS` (Backup State) and `RS` (Restore State) are shortcuts to backup the current state and restore it later
- You can also set the current state by clicking on the canvas. The helirin will be initialized with full life. A left click will make it rotate clockwise, and a right click counter-clockwise

### In progress

## Credits

This bot is an helping tool for achieving a Tool Assisted Speedrun of Kuru Kuru Kururin (links to come).

Made by Mickael Laurent (E-Sh4rk).  
Thanks to Mohoc for his precious expertise. https://www.twitch.tv/mohoc7  
Thanks to Matt Shepcar for his advice and his inspiring article: https://medium.com/message/building-a-cheat-bot-f848f199e76b
