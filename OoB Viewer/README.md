# OoB Viewer

NOTE: If you want the script to be faster, you can change the LUA interpreter in the settings of Bizhawks: `Config -> Customize... -> Advanced`, check `Lua+LuaInterface`, then restart Bizhawks.

## v1.3 (faster?)

Changelog:
 - Code refactored
 - Display of the helirin added
 - Position of walls fixed in some levels (e.g. Machine Land 1)
 - You can set some parameters at the beginning of the script
 - Add shortcuts to activate (SHIFT+V) or deactivate (CONTROL+V) the viewer
 
Known issues:
 - Some tiles are missing

## v1.4 (prettier)

Installation note:
 - The directory `sprites` is required. It must be placed in the same folder as the script.

Changelog:
 - Use real game tiles for walls rendering
 - Add a window zoom parameter
 
## v1.5 (smarter)

Installation note:
 - The directory `sprites` is required. It must be placed in the same folder as the script.

Changelog:
 - Ingame rendering has been improved. In particular, it now implements a 2-frame delay and a 'CamHack' in order to perfectly intergate walls.
 - Wait for the level to finish loading before drawing walls.

Credits to ThunderAxe31 and E-Sh4rk.
