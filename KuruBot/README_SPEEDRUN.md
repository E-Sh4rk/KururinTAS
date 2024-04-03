# KuruBot Speedrun Setup Instructions

## Configuring the bot

You can configure the bot for finding speedrun wall-clip setups by clicking on `Load config` and choosing the right configuration files, located in the `KuruBot_Configs` folder:

1. In the `Bot_configs` folder, load one of the `config_wc_k` files. Higher values of `k` means higher precision, but also larger search space (and thus increased solving time and RAM usage). We recommend you to start with `config_wc_3`, and if it is fast enough, you can then try `config_wc_4` or `config_wc_5`. 
2. Then, in the `Gameplay_configs` folder, load `solve_for_human_setup` (it configures the solver to minimize the number of input changes instead of the number of frames) and `default_target_to_oob`.

NOTE: these two configuration folders are independent, you can thus load a new configuration file in `Bot_configs` without having to reload `Gameplay_configs` configuration files.

NOTE: Some configuration files have an impact on the generation of the cost map. Thus, after loading a configuration, you should recompute the cost map (see below).

## Loading a map and the initial state

- You can load a map by clicking on the button `LM` (Load Map). Maps are located in the `KuruBot_Levels` folder.
- You can load the initial state of the helirin by clicking on the button `LS` (Load State). Initial states are located in the `KuruBot_Levels` folder.
- After loading the initial state, we recommend you to click on the button `BS` (Backup State). This way, you can easily restore the initial state later by clicking on `RS` (Restore State).

## Solving!

The three first steps must be done again if you change the map or the configuration of the bot. You do not need to do it again if you just change the state of the helirin.

1. Build the solver. In our case, we are not interested in the out of bound copies of the map, thus we just click on `1. Build solver (legal ending)`.
2. Optional: change the target area. By default, the solver will search a path to any OOB area. If you want to change this target (for instance, if you want to target an area that is not OOB), you can do it by clicking on `2. Draw`, drawing your custom target zone on the map (right click to clear) and clicking on `2. Set target according to drawing`. The radius of the brush can be changed in the spinbox at the right of the button `2. Draw`. We recommend drawing an area long enough so that the solver can reach it directly after a wall clip without having to change the last inputs (otherwise, if an additional input change is required to reach the target, the search may take significantly longer).
3. Build the cost map. Just click on the button `3. Cost map`. You can then visualize the cost map by checking `Show cost map` (if you have set a custom target in the step 2, you can check that it has been taken into account: your target area should be white).
4. Solve. Just click on `4. Solve` and wait! A message box will appear if a solution is found. You can abort the search at anytime by clicking on the `Abort` button.

## Visualizing the solution and saving it

- If a solution has been found, you can save it by clicking on `SI` (Save Input). You will be able to play it again later by clicking on `LI` (Load Input).
- If you want to import it in TAS studio, you can click on `CI` (Copy Input) and then you should be able to paste the inputs in TAS studio.
- Conversely, you can copy inputs from TAS studio and click on `PI` (Paste Input) to play them in the bot (this can be useful if you want to start the search from a custom state obtained by performing a sequence of moves from the initial state).
