-- Kuru Kuru Kururin - OoB Viewer v1.4 by ThunderAxe31 & E-Sh4rk
-- TODO:
-- Check and improve checkpoint detection

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
					-- Adjusted position, the modulo simulates overflow. It seems faster than bit.band(..., 0xFFFF). 
					-- For walls, the collision check seems to consider position as an unsigned variable:
					-- effects of the overflow are visible on some maps around position 0. (e.g. MachineLand1, or other maps such that the width is not a power of 2) 
					local x_pos2 = (x_pos + x*tile_size) % 0x10000
					local y_pos2 = (y_pos + y*tile_size) % 0x10000
					local x_pos_tile = math.floor(x_pos2/tile_size) % map_x_size
					local y_pos_tile = math.floor(y_pos2/tile_size) % map_y_size
					
					-- Map is stored at the very beggining of EWRAM. The 2 first dwords contain the size of the map.
					local tile_addr = x_pos_tile*2 + y_pos_tile*map_x_size*2 + 4
					local tile_type = memory.read_u16_le(tile_addr, "EWRAM") -- EWRAM = 0x02000000
					local x_tile = x*tile_size -x_mod
					local y_tile = y*tile_size -y_mod
					
					-- We draw the wall tile depending on its type
					local tile_index = tile_type % 0x1000 -- Seems faster than bit.band(tile_type, 0xFFF)
					local tile_id = tile_type % 0x400 -- Seems fatser than bit.band(tile_index, 0x3FF)
					if tile_id ~= 0 and tile_id <= 130 and tile_id ~= 23 and tile_id ~= 26 and tile_id ~= 56 and tile_id ~= 125 then
						view.DrawImage("sprites/" .. tostring(tile_index) .. ".bmp", x_tile, y_tile)
					end
					
					-- Now we check collision with checkpoints. For that, the same calculus are performed except that position is considered signed and modulus act like C modulus.
					-- If it results in tile_addr offset being negative, we ignore this case.
					
					-- If the following condition is not satisfied, y_pos_tile could be negative and so the resulting tile_addr offset could be negative...
					-- Actually, it would not always be the case but I believe this simplification will not affect the final result.
					if y_pos2 < 0x8000 and y_pos_tile > 0 then

						-- Adjusted position, for checkpoints. For checkpoints, the collision check seems to consider position as a signed variable.
						if x_pos2 >= 0x8000 then x_pos2 = x_pos2-0x10000 end
						
						-- The following simulates C modulus on negative numbers. If x_pos2 is not negative, we don't need to redo the calculus (it would be equal to the computation done for walls)
						if x_pos2 < 0 then
							x_pos_tile = -((-math.floor(x_pos2/tile_size)) % map_x_size)
							tile_addr = x_pos_tile*2 + y_pos_tile*map_x_size*2 + 4
							tile_type = memory.read_u16_le(tile_addr, "EWRAM")
						end
						
						-- We draw the checkpoint if there is any
						if (tile_type == 0x00FB) or (tile_type == 0x04FB) or (tile_type == 0x08FB) or (tile_type == 0x0CFB) or (tile_type == 0x00FD) or (tile_type == 0x04FD) or (tile_type == 0x08FD) or (tile_type == 0x0CFD) or (tile_type == 0x00EA) or (tile_type == 0x04EA) or (tile_type == 0x08EA) or (tile_type == 0x0CEA) or (tile_type == 0x00ED) or (tile_type == 0x04ED) or (tile_type == 0x08ED) or (tile_type == 0x0CED) then
							view.DrawRectangle(x_tile, y_tile, 7, 7, 0xFF4040FF)
						elseif (tile_type == 0x00FE) or (tile_type == 0x04FE) or (tile_type == 0x08FE) or (tile_type == 0x0CFE) or (tile_type == 0x00FF) or (tile_type == 0x04FF) or (tile_type == 0x08FF) or (tile_type == 0x0CFF) then
							view.DrawRectangle(x_tile, y_tile, 7, 7, 0xFFD0D000)
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
