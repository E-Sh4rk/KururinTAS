-- Kuru Kuru Kururin - OoB Viewer v1.4 by ThunderAxe31 & E-Sh4rk

-- Script parameters
local x_nb_tiles = 30 -- Must be 30 if draw_in_separate_window is set to false
local y_nb_tiles = 20 -- Must be 20 if draw_in_separate_window is set to false
local draw_in_separate_window = true
local keys_activate = {"LeftShift", "V"}
local keys_deactivate = {"LeftControl", "V"}
local activated = true -- Automatically activate the viewer when the script is started?

-- Do not touch that
local addr_map_x_size = 0x313C -- Can also be found at EWRAM+0x0
local addr_map_y_size = 0x313E -- Can also be found at EWRAM+0x2
local addr_x_pos      = 0x4546 -- Actually at 0x4544 on 4 bytes, but we are only interested in the 2 most significant bytes
local addr_y_pos      = 0x454A -- Actually at 0x4548 on 4 bytes, but we are only interested in the 2 most significant bytes
local addr_rotate     = 0x4572

local tile_size = 8
local helirin_radius = 32
local helirin_x_screen = math.ceil (x_nb_tiles*tile_size / 2) -- Should be between 127 and 128, seems to be ceiled in the game
local helirin_y_screen = math.ceil (y_nb_tiles*tile_size / 2) -- Should be between 127 and 128, seems to be ceiled in the game

local view = nil
if draw_in_separate_window then
	view = gui.createcanvas(x_nb_tiles*tile_size, y_nb_tiles*tile_size)
	view.SetTitle("Out of Bounds Viewer")
	view.clearGraphics = function () end
else
	view = gui
	view.Clear = function (x) end
	view.Refresh = function () end
	view.DrawText = view.drawText
	view.DrawImage = view.drawImage
	view.DrawLine = view.drawLine
	view.DrawAxis = view.drawAxis
	view.DrawEllipse = view.drawEllipse
	view.DrawPolygon = view.drawPolygon
	view.DrawRectangle = view.drawRectangle
	view.ClearImageCache = view.clearImageCache
end

while true do
	-- Activation?
	local ok = true
	local keys = input.get()
	for i, v in ipairs(keys_activate) do
		if keys[v] == nil then
			ok = false
		end
	end
	if ok then activated = true end
	-- Deactivation?
	ok = true
	for i, v in ipairs(keys_deactivate) do
		if keys[v] == nil then
			ok = false
		end
	end
	if ok then
		if activated then
			activated = false
			view.ClearImageCache()
			view.clearGraphics()
		end
	end
	-- Go!
	if activated then
		view.Clear(0xFFFFFFFF)

		local map_x_size = memory.read_u16_le(addr_map_x_size, "IWRAM") -- IWRAM = 0x03000000
		local map_y_size = memory.read_u16_le(addr_map_y_size, "IWRAM")
			
		-- If we are not in a level, we do nothing
		if map_x_size <= 32 and map_y_size <= 32
		then
			view.DrawText(x_nb_tiles*tile_size/2 - 68, y_nb_tiles*tile_size/2 - 8, "Not in a level...")
		else
			-- We consider position as an unsigned variable (see below).
			local x_pos = memory.read_u16_le(addr_x_pos, "IWRAM")
			local y_pos = memory.read_u16_le(addr_y_pos, "IWRAM")
			-- Position for the top left corner of the screen
			x_pos = x_pos - (x_nb_tiles*tile_size/2)
			y_pos = y_pos - (y_nb_tiles*tile_size/2)

			local x_mod = x_pos%tile_size
			local y_mod = y_pos%tile_size
			
			for y=0, y_nb_tiles do
				for x=0, x_nb_tiles do
				
					local x_tile = x*tile_size -x_mod
					local y_tile = y*tile_size -y_mod
					
					-- For walls, the collision check seems to consider position as an unsigned variable:
					-- effects of the overflow are visible on some maps around position 0. (e.g. MachineLand1, or other maps such that the width is not a power of 2) 
					-- The modulo simulates overflow of the position variables. It seems faster than bit.band(..., 0xFFFF). 
					local x_pos_var = (x_pos + x*tile_size) % 0x10000
					local y_pos_var = (y_pos + y*tile_size) % 0x10000
					local x_pos_tile = math.floor(x_pos_var/tile_size)
					local y_pos_tile = math.floor(y_pos_var/tile_size)
					local x_pos_tile_mod = x_pos_tile % map_x_size
					local y_pos_tile_mod = y_pos_tile % map_y_size
					
					-- Map is stored at the very beggining of EWRAM. The 2 first dwords contain the size of the map.
					local tile_addr = x_pos_tile_mod*2 + y_pos_tile_mod*map_x_size*2 + 4
					local tile_type = memory.read_u16_le(tile_addr, "EWRAM") -- EWRAM = 0x02000000
					
					-- We draw the wall tile depending on its type
					local tile_index = tile_type % 0x1000 -- Seems faster than bit.band(tile_type, 0xFFF)
					local tile_id = tile_index % 0x400 -- Seems fatser than bit.band(tile_index, 0x3FF)
					if tile_id ~= 0 and tile_id <= 130 and tile_id ~= 23 and tile_id ~= 26 and tile_id ~= 56 and tile_id ~= 125 then
						view.DrawImage("sprites/" .. tostring(tile_index) .. ".bmp", x_tile, y_tile)
					end
					
					-- Now we check collision with healing/ending zones. For that, the same calculus is performed except that position is considered signed and:
					-- Modulus of signed integers act like the C one
					-- Floor act like truncate
					-- If it results in tile_addr offset being negative, no collision is reported
					
					-- The following computation does not really match the method described above, but it should gives the same results in the situations found in the game.
					
					-- We convert position variables to signed one.
					if x_pos_var >= 0x8000 then x_pos_var = x_pos_var-0x10000 end
					if y_pos_var >= 0x8000 then y_pos_var = y_pos_var-0x10000 end
					
					if y_pos_var >= 0 then -- In the top OOB, no healing/ending zone
						if x_pos_var < 0 then -- If both x_pos_var and y_pos_var are still non-negative, previous computation for walls is still valid for zones.
						
							x_pos_tile = math.floor(x_pos_var/tile_size)+1 -- Note: it is different from truncate (=ceiling here). We use floor instead for stability reasons in the rendering.
							x_tile = x_tile + 1 --  This line is here to compensate the approximation above.
							x_pos_tile_mod = -((-x_pos_tile) % map_x_size) -- Simulates the C modulus for negative numbers
							
							tile_addr = x_pos_tile_mod*2 + y_pos_tile_mod*map_x_size*2 + 4
							if tile_addr >= 4 then
								tile_type = memory.read_u16_le(tile_addr, "EWRAM")
							else
								tile_type = 0
							end
							
							tile_index = tile_type % 0x1000
							tile_id = tile_index % 0x400
						end
						
						-- We draw the healing/ending zone if there is any
						if tile_id == 0xFB or tile_type == 0xFD or tile_type == 0xEA or tile_type == 0xED then
							view.DrawRectangle(x_tile, y_tile, tile_size-1, tile_size-1, 0xFF4040FF)
						elseif tile_type == 0xFE or tile_type == 0xFF then
							view.DrawRectangle(x_tile, y_tile, tile_size-1, tile_size-1, 0xFFD0D000)
						end
					end
				end
			end
			
			-- We draw the helirin
			local helirin_rot = memory.read_u16_le(addr_rotate, "IWRAM")
			view.DrawAxis(helirin_x_screen,helirin_y_screen,6)
			view.DrawEllipse(helirin_x_screen-helirin_radius,helirin_y_screen-helirin_radius,helirin_radius*2,helirin_radius*2)
			local angle = helirin_rot * 2*math.pi / 0x10000 -- Rotation in game is stored using the full range of the 16bits variable
			local x1 = helirin_x_screen+math.sin(angle)*helirin_radius
			local y1 = helirin_y_screen-math.cos(angle)*helirin_radius
			local x2 = helirin_x_screen-math.sin(angle)*helirin_radius
			local y2 = helirin_y_screen+math.cos(angle)*helirin_radius
			view.DrawLine(x1,y1,x2,y2)
		end
		
		view.Refresh()
	end
	emu.frameadvance()
end
