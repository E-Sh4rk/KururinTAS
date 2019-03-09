# KuruBot

## Instructions

### Prerequisite

- This bot has been designed for Windows, but it might also work on Linux or Mac.
- For Windows systems, .NET Framework 4 (or more) need to be installed. For Linux or Mac, Mono can be used as an alternative.
- This bot does not need to communicate with the game in order to work. However, we recommend to connect it to Bizhawk in order to easily transfer states and visualize solutions.

### Installation

- You just have to download the latest release (be sure to download the binaries and not the source code): `https://github.com/E-Sh4rk/KururinTAS/releases`
- To launch the bot, just run: `KuruBot/KuruBot.exe`
- The folder `KuruBot_Configs` contains some predefined configuration files for the bot
- The folder `KuruBot_Levels` contains the map and initial state of each level

### Connecting the bot to Bizhawk

1. Place the script `kuruCOM.lua` in a folder with r/w permissions.
2. Start Kuru Kuru Kururin in Bizhawk and load this LUA script.
3. Connect the bot by clicking on the `Connect` button and selecting the LUA script (the same file that was loaded by Bizhawk, not a copy).
4. If the game is paused in Bizhawk, the communication will also be paused. In this case, you can advance a frame if you want to complete a pending transfer.

### Configuring the bot

- Depending on what kind of problem you want to solve, you can configure the bot by clicking on `Load config` and choosing the right configuration file
- These configuration files have the extension `.ini`. They contain some parameters for the bot.
- In term of complexity, damageless < noWC < WC. Here is a quick description of the predefined configruations:
  - `config_wc_k`: Full wall clip support. `k` correspond to the precision: higher values of `k` will be more precise (it could perform very tricky wall clips) but will explore less the map.
  - `config_no_wc_strict`: Disable wall clips by forbidding to be in a wall.
  - `config_no_wc_relaxed`: Disable wall clips without forbidding to be in a wall. Useful when the starting state is in a wall, but that no new wall clip should be performed.
  - `config_damageless`: For damageless (of course, no wall clip possible).
  - `decrease_cost_map`: Decrease the importance given to the cost map so that the bot will be less greedy.
  - `increase_cost_map`: Increase the importance given to the cost map so that the bot will be greedier.
  - `disable_waiting`: Decrease rotation precision. Reduce the search space but make the bot unable to deliberately wait (for changing its rotation).
  - `disable_life_prediction`: Load it after another config in order to disable the life prediction optimisation. Can be relevant when the bot must briefly quit the legal zone with very low life and invulnerability.
  - `disable_life`: Load it after another config in order to disable the life constraints. The helirin will be considered as invincible.

### Loading a map

- If the bot is connected to Bizhawk, you can load the current ingame map by clicking on the button `Download map`
- Alternatively, you can load a previously saved map by clicking on the button `LM` (Load Map)
- You can save a map by clicking on the button `SM` (Save Map)
- You can visualize the whole map with out of bounds by checking `Show physical map`

### Loading the state of the helirin

- If the bot is connected to Bizhawk, you can load the current ingame state by clicking on the button `Download state`. If the game is paused and you want to capture the current frame (not the next one), you can click on `Download state`, then save the current state into a dummy slot in Bizhawk, and reload this dummy slot (it will perform the transfer without having to frame advance).
- Alternatively, you can load a previously saved state by clicking on the button `LS` (Load State)
- You can save a state by clicking on the button `SS` (Save State)
- If you want to manually edit a state file, here is the list of values that it contains:  
`x_pos y_pos x_bump_speed y_bump_speed rotation rotation_rate rotation_default_rate hearts invulnerability_frames`
- The buttons `BS` (Backup State) and `RS` (Restore State) are shortcuts to backup the current state and restore it later
- You can also set the current state by clicking on the canvas. The helirin will be initialized with full life. A left click will make it rotate clockwise, and a right click counter-clockwise

### Solving!

The three first steps must be done again if you change the map or the configuration of the bot. You do not need to do it again if you just change the state of the helirin.

1. The first step is to build the solver. If you want to focus the search on the inbound ending, click on `1. Build solver (legal ending)`. Otherwise, click on `1. Build solver (any ending)`.
2. The second step is optionnal. If you want to set a custom target (different than the regular ending zone), you can do it by clicking on `2. Draw`, drawing your custom ending zone on the map (right click to clear) and clicking on `2. Set target to current drawing`. You can also add custom bounds (in order to restrict the search space) by drawing some additional walls around the area to explore, and clicking on `2. Set constraints to current drawing`. The radius of the brush can be changed in the spinbox at the right of the button `2. Draw`.
3. The third step is to build the cost map. Just click on the button `3. Cost map` (it can take some times depending on the size of the map and the configuration of the bot). You can then visualize the cost map by checking `Show cost map` (if you have set custom bounds and/or target in the step 2, you can check that it has been taken into account).
4. Last step: click on `4. Solve` and wait! A message box will appear if a solution is found. The spin box at the left correspond to a minimal "life score" (going below is treated as a death). For instance, setting it to 20 prevents the helirin from having less than 2 hearts. You can abort the search at anytime by clicking on the `Abort` button.

### Visualizing the solution and saving it

- If a solution has been found, you can save it by clicking on `SI` (Save Input). You will be able to load it later by clicking on `LI` (Load Input).
- If you want to play the solution in the game, just click on `Send last inputs` (the bot must be connected to Bizhawk).

### Exporting the solution in bk2

- The simplest way to export the solution to a `.bk2` file is to play it in Bizhawk while recording (menu `File -> Movie` of Bizhawk).
- If you are an expert of the bk2 format, you also convert an input file of the bot (saved with `SI`) into an input file for the bk2 format (usually called `Input Log.txt`) by clicking on the `To bk2` button. The reverse conversion can be achieved with the `From bk2` button.

## Credits

This bot is an helping tool for achieving a Tool Assisted Speedrun of Kuru Kuru Kururin (links to come).

Made by Mickael Laurent (E-Sh4rk).  
Thanks to Mohoc for his precious expertise. https://www.twitch.tv/mohoc7  
Thanks to Matt Shepcar for his advice and his inspiring article: https://medium.com/message/building-a-cheat-bot-f848f199e76b
