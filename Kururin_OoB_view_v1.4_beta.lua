-- Kuru Kuru Kururin - OoB Viewer v1.4 by ThunderAxe31 & E-Sh4rk
-- TODO:
-- Find all wall tile types OR find a cleaner way to do draw it (game graphics, pixels with collision, ...)
-- Find why ending zone are offset from walls in OOB (left: from 1px, top: not even here?)

local addr_map_x_size = 0x313C -- Can also be found at EWRAM+0x0
local addr_map_y_size = 0x313E -- Can also be found at EWRAM+0x2
local addr_x_pos      = 0x4546 -- Actually at 0x4544 on 4 bytes, but we are only interested in the 2 most significant bytes
local addr_y_pos      = 0x454A -- Actually at 0x4548 on 4 bytes, but we are only interested in the 2 most significant bytes
local addr_rotate     = 0x4572

local x_end = 32
local y_end = 32
local tile_size = 8
local helirin_radius = 32
local helirin_x_screen = 128 -- Should be between 127 and 128, seems to be ceiled in the game
local helirin_y_screen = 128 -- Should be between 127 and 128, seems to be ceiled in the game

local view_win = gui.createcanvas(x_end*tile_size, y_end*tile_size)
view_win.SetTitle("Out of Bounds Viewer") 
view_win.ClearImageCache()

while true do
	local map_x_size = memory.read_u16_le(addr_map_x_size, "IWRAM") -- IWRAM = 0x03000000
	local map_y_size = memory.read_u16_le(addr_map_y_size, "IWRAM")
	
	view_win.Clear(0xFFFFFFFF)
	-- If we are not in a level, we do nothing
	if map_x_size <= 32 and map_y_size <= 32
	then
		view_win.DrawText(x_end*tile_size/2 - 68, y_end*tile_size/2 - 8, "Not in a level...")
	else
		-- Position seems to be considered unsigned by the game: effects of the overflow are visible on some maps around position 0. (e.g. MachineLand1, or other maps such that the width is not a power of 2) 
		local x_pos = memory.read_u16_le(addr_x_pos, "IWRAM")
		local y_pos = memory.read_u16_le(addr_y_pos, "IWRAM")
		-- Position for the top left corner of the screen
		x_pos = x_pos - (x_end*tile_size/2)
		y_pos = y_pos - (y_end*tile_size/2)

		local x_mod = x_pos%tile_size
		local y_mod = y_pos%tile_size
		local uint16_max = 2^16
		
		for y=0, y_end do
			for x=0, x_end do
				-- Adjusted position, the modulo simulates overflow
				local x_pos2 = (x_pos + x*tile_size) % uint16_max
				local y_pos2 = (y_pos + y*tile_size) % uint16_max
				
				local x_pos_floor = math.floor(x_pos2/tile_size)
				local y_pos_floor = math.floor(y_pos2/tile_size)
				
				-- Map is stored at the very beggining of EWRAM. The 2 first dwords contain the size of the map.
				local tile_addr = (x_pos_floor %map_x_size)*2 +(y_pos_floor %map_y_size)*map_x_size*2 + 4
				local tile_type = memory.read_u16_le(tile_addr, "EWRAM") -- EWRAM = 0x02000000
				local x_tile = x*tile_size -x_mod
				local y_tile = y*tile_size -y_mod
				
				-- We draw the tile depending on its type
				local tile_index = tile_type % 0x1000 -- Alternative for: tile_type & 0xFFF
				local tile_id = tile_index % 0x400 -- Alternative for: tile_type & 0x3FF
				if tile_id ~= 0 and tile_id <= 130 and tile_id ~= 23 and tile_id ~= 26 and tile_id ~= 56 and tile_id ~= 125 then
					view_win.DrawImage("sprites/" .. tostring(tile_index) .. ".bmp", x_tile, y_tile)
				end
			end
		end
		
		-- We draw the helirin
		local helirin_rot = memory.read_u16_le(addr_rotate, "IWRAM")
		view_win.DrawAxis(helirin_x_screen,helirin_y_screen,6)
		view_win.DrawEllipse(helirin_x_screen-helirin_radius,helirin_y_screen-helirin_radius,helirin_radius*2,helirin_radius*2)
		local angle = helirin_rot * 2*math.pi / uint16_max -- Rotation in game is stored using the full range of the 16bits variable (from 0 to 2^16-1)
		local x1 = helirin_x_screen+math.sin(angle)*helirin_radius
		local y1 = helirin_y_screen-math.cos(angle)*helirin_radius
		local x2 = helirin_x_screen-math.sin(angle)*helirin_radius
		local y2 = helirin_y_screen+math.cos(angle)*helirin_radius
		view_win.DrawLine(x1,y1,x2,y2)
	end
	
	view_win.Refresh()
	emu.frameadvance()
end
