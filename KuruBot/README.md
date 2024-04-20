# KuruBot

## Install

### Prerequisite

- This bot has been designed for Windows, but it might also work on Linux or Mac.
- For Windows systems, .NET Framework 4.8 needs to be installed. For Linux or Mac, Mono can be used as an alternative.
- Optionaly, the bot can be connected to Bizhawk in order to easily transfer states and visualize solutions.

### Binaries

- You just have to download the latest release: `https://github.com/E-Sh4rk/KururinTAS/releases`
- To launch the bot, just run: `KuruBot/KuruBot.exe`
- The folder `KuruBot_Configs` contains some predefined configuration files for the bot
- The folder `KuruBot_Levels` contains the map and initial state of each level

## Using the bot

If you are using this bot to find wall-clip setups for speedrun (not TAS), you should follow [these instructions](README_SPEEDRUN.md).

### Connecting the bot to Bizhawk (optional)

This section is optional: it is only required if you need to automatically import maps and states from a running Bizhawk instance.

1. Place the script `kuruCOM.lua` in a folder with r/w permissions.
2. Start Kuru Kuru Kururin in Bizhawk and load this LUA script.
3. Connect the bot by clicking on the `Connect` button and selecting the LUA script (the same file that was loaded by Bizhawk, not a copy).
4. If the game is paused in Bizhawk, the communication will also be paused. In this case, you can advance a frame if you want to complete a pending transfer.

### Configuring the bot

Depending on what kind of problem you want to solve, you can configure the bot by clicking on `Load config` and choosing the right configuration file.
These configuration files have the extension `.ini`. They contain some parameters for the bot.

1. You first have to select a base configuration in the `Bot_configs` folder:
     - `config_damageless_xxx`: activate the damageless mode and focus the search on the exploration. `xxx` determines which aspect to prioritize in term of precision (`precise` for both, `approx` for none).
     - `config_no_oob`: no oob shortcut, the bot is encouraged to follow the map.
     - `config_wc_k`: full wall clip support. `k` correspond to the precision: higher values of `k` will be more precise (in order to perform very tricky wall clips) but will explore less the map.
2. Then the `Bot_modifiers` folder allows you to modify the behavior of the bot configuration (you can load multiple configurations there). Reloading a bot configuration will reset these modifiers.
     - `increase_cost_map_k`/`decrease_cost_map_k`: scale the cost map. Increasing it will make the solver more focused in direction of the target, while decreasing it will
     make the solver more likely to explore backward, etc.
     - `checkpoint_at_healzones`: Once an healing zone is reached, stop optimizing what has already been done before (very useful for damageless configurations).
3. The `Gameplay_configs` folder contains some configurations independent from the two previous folders. They can be used to change some behaviors related to the gameplay (you can load multiple configurations there).
Reloading a bot configuration will not reset these modifiers (they are independent).
     - `disable_life`: Disable the life system (the life of the helirin will not be checked, which allows to reduce the search space a lot).
     - `easy_mode`: For the easy mode (shorter helirin).
     - `enable_bonus_xxx`: Asks the bot to collect the bonus of the level (if any) before reaching the target. If `with_checkpoint`, once the bonus is collected, stop optimizing what has already been done before.
     - `enable_moving_objects_xxx`: Take the moving objects into account (except the shooters, which are not supported). It can slow down the search process. If `with_frame_counter`, the search space will be extended to take time into account (can give better results when waiting is needed to pass some moving objects).
     - `default_target_to_oob`: When no custom target is drawn, the default target when building the cost map will be any OOB area (instead of being the ending area).
     - `solve_for_human_setup`: Configure the solver to minimize the number of input changes instead of the number of frames.

NOTE: Some configuration files have an impact on the generation of the cost map. Thus, after loading a configuration, you should recompute the cost map.

### Loading a map

- If the bot is connected to Bizhawk, you can load the current ingame map by clicking on the button `Download map`
- Alternatively, you can load a previously saved map by clicking on the button `LM` (Load Map)
- You can save a map by clicking on the button `SM` (Save Map)
- You can visualize the whole map with out of bounds by checking `Physical map`

### Loading the state of the helirin

- If the bot is connected to Bizhawk, you can load the current ingame state by clicking on the button `Download state`. If the game is paused and you want to capture the current frame (not the next one), you can click on `Download state`, then save the current state into a dummy slot in Bizhawk, and reload this dummy slot (it will perform the transfer without having to frame advance).
- Alternatively, you can load a previously saved state by clicking on the button `LS` (Load State)
- You can save a state by clicking on the button `SS` (Save State)
- If you want to manually edit a state file, here is the list of values that it contains:  
`x_pos y_pos x_bump_speed y_bump_speed rotation rotation_rate rotation_default_rate hearts invulnerability_frames has_bonus frame_number timer_started`
- The buttons `BS` (Backup State) and `RS` (Restore State) are shortcuts to backup the current state and restore it later
- You can also set the current state by clicking on the canvas. The helirin will be initialized with full life. A left click will make it rotate clockwise, and a right click counter-clockwise
- Note about moving objects: in order to correctly initialize the moving objects, the `kuruCOM` script must have been active when the level started (because it must keep track of the global frame number at the start of the level). If you have directly loaded a state in the middle of the level using TAS Studio, please go back a little, play the beginning of the level, and then you are free to load any later state of the level.

### Solving!

The three first steps must be done again if you change the map or the configuration of the bot. You do not need to do it again if you just change the state of the helirin.

1. The first step is to build the solver. If you want to focus the search on the inbound ending, click on `1. Build solver (legal ending)`. Otherwise, click on `1. Build solver (any ending)`.
2. The second step is optionnal. If you want to set a custom target (different than the regular ending zone), you can do it by clicking on `2. Draw`, drawing your custom ending zone on the map (right click to clear) and clicking on `2. Set target according to drawing`. You can also add custom bounds (in order to restrict the search space) by drawing some additional walls around the area to explore, and clicking on `2. Set constraints according to drawing`. The radius of the brush can be changed in the spinbox at the right of the button `2. Draw`.
3. The third step is to build the cost map. Just click on the button `3. Cost map` (it can take some times depending on the size of the map and the configuration of the bot). You can then visualize the cost map by checking `Show cost map` (if you have set custom bounds and/or target in the step 2, you can check that it has been taken into account).
4. Last step: click on `4. Solve` and wait! A message box will appear if a solution is found. The spin box at the left corresponds to the minimal number of invulnerability frames the helirin should have left (going below is treated as a death). If you want the helirin to have at least 2 hearts when reaching the target, you should load the config modifier `start_with_2_hearts`. You can abort the search at anytime by clicking on the `Abort` button.

### Visualizing the solution and saving it

- If a solution has been found, you can save it by clicking on `SI` (Save Input). You will be able to load it later by clicking on `LI` (Load Input).
- If the bot is connected to Bizhawk, you can play the solution in the game: just click on `Send last inputs`. The helirin will automatically be teleported to the good starting position (please ensure the correct level is loaded). Warning: the positions of moving objects will not be initialized, so you must ensure they match with the initial state of your search.
- Alternatively, if you have TAS studio opened, you can click on `CI` (Copy Input) and paste the inputs in TAS studio.
- Conversely, you can copy inputs from TAS studio and click on `PI` (Paste Input) to play them in the bot.

## Credits

This bot is an helping tool for achieving a Tool Assisted Speedrun of Kuru Kuru Kururin (links to come).

Made by Mickael Laurent (E-Sh4rk).  
Thanks to Mohoc for his precious expertise. https://www.twitch.tv/mohoc7  
Thanks to Matt Shepcar for his advice and his inspiring article: https://medium.com/message/building-a-cheat-bot-f848f199e76b
