-- Kuru Kuru Kururin - OoB Viewer v1.3 by ThunderAxe31 & E-Sh4rk
-- TODO:
-- Support all wall tile types, and rewrite it in a cleaner and more clever way

-- Script parameters
local x_nb_tiles = 30 -- Must be 30 if draw_in_separate_window is set to false. Default: 30
local y_nb_tiles = 20 -- Must be 20 if draw_in_separate_window is set to false. Default: 20
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
local helirin_x_screen = math.ceil (x_nb_tiles*tile_size / 2)
local helirin_y_screen = math.ceil (y_nb_tiles*tile_size / 2)

-- Initialize the view
local view = nil
if draw_in_separate_window
then
	view = gui.createcanvas(x_nb_tiles*tile_size, y_nb_tiles*tile_size)
	view.drawText = view.DrawText
	view.drawPolygon = view.DrawPolygon
	view.drawLine = view.DrawLine
	view.drawAxis = view.DrawAxis
	view.drawEllipse = view.DrawEllipse
	view.drawRectangle = view.DrawRectangle

	view.clearImageCache = view.ClearImageCache
	view.clear = view.Clear
	view.refresh = view.Refresh
	view.clearGraphics = function () end
	view.SetTitle("Out of Bounds Viewer")
	
else
	view = gui
	view.clear = function (x) end
	view.refresh = function () end
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
			view.clearImageCache()
			view.clearGraphics()
		end
	end
	-- Go!
	if activated then
		view.clear(0xFFFFFFFF)

		local map_x_size = memory.read_u16_le(addr_map_x_size, "IWRAM") -- IWRAM = 0x03000000
		local map_y_size = memory.read_u16_le(addr_map_y_size, "IWRAM")
			
		-- If we are not in a level, we do nothing
		if map_x_size <= 32 and map_y_size <= 32
		then
			view.drawText(x_nb_tiles*tile_size/2, y_nb_tiles*tile_size/2, "Not in a level...", nil, nil, 12, nil, nil, "center", "middle")
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
					-- For more details, see https://medium.com/message/building-a-cheat-bot-f848f199e76b
					local tile_addr = x_pos_tile_mod*2 + y_pos_tile_mod*map_x_size*2 + 4
					local tile_type = memory.read_u16_be(tile_addr, "EWRAM") -- EWRAM = 0x02000000
					
					-- We draw the tile depending on its type (code by ThunderAxe31)
					if (tile_type == 0x0300) or (tile_type == 0x0304) or (tile_type == 0x0308) or (tile_type == 0x030C) or (tile_type == 0x0400) or (tile_type == 0x0404) or (tile_type == 0x0408) or (tile_type == 0x040C) or (tile_type == 0x2100) or (tile_type == 0x2104) or (tile_type == 0x2108) or (tile_type == 0x210C) or (tile_type == 0x3300) or (tile_type == 0x3304) or (tile_type == 0x3308) or (tile_type == 0x330C) or (tile_type == 0x4100) or (tile_type == 0x4104) or (tile_type == 0x4108) or (tile_type == 0x410C) or (tile_type == 0x3500) or (tile_type == 0x3504) or (tile_type == 0x3508) or (tile_type == 0x350C) or (tile_type == 0x3000) or (tile_type == 0x3004) or (tile_type == 0x3008) or (tile_type == 0x300C) or (tile_type == 0x2400) or (tile_type == 0x2404) or (tile_type == 0x2408) or (tile_type == 0x240C) or (tile_type == 0x0500) or (tile_type == 0x0504) or (tile_type == 0x0508) or (tile_type == 0x050C) then
						view.DrawRectangle(x_tile, y_tile, 7, 7, 0xFFFF0000)
					elseif (tile_type == 0x6300) or (tile_type == 0x6304) or (tile_type == 0x6308) then
						view.DrawPolygon({{x_tile+4,y_tile+3},{x_tile+7,y_tile},{x_tile+7,y_tile+7},{x_tile+4,y_tile+4}}, 0xFFFF0000)
					elseif (tile_type == 0x630C) then
						view.DrawPolygon({{x_tile,y_tile},{x_tile+3,y_tile+3},{x_tile+3,y_tile+4},{x_tile,y_tile+7}}, 0xFFFF0000)
					elseif (tile_type == 0x0B00) or (tile_type == 0x1000) then
						view.DrawPolygon({{x_tile +7,y_tile},{x_tile +7,y_tile +7},{x_tile,y_tile +7}}, 0xFFFF0000)
					elseif (tile_type == 0x0B04) then
						view.DrawPolygon({{x_tile,y_tile},{x_tile,y_tile +7},{x_tile +7,y_tile +7}}, 0xFFFF0000)
					elseif (tile_type == 0x0B08) then
						view.DrawPolygon({{x_tile,y_tile},{x_tile +7,y_tile},{x_tile +7,y_tile +7}}, 0xFFFF0000)
					elseif (tile_type == 0x0B0C) or (tile_type == 0x100C) then
						view.DrawPolygon({{x_tile,y_tile},{x_tile +7,y_tile},{x_tile,y_tile +7}}, 0xFFFF0000)
					elseif (tile_type == 0x7200) then
						view.DrawPolygon({{x_tile +7,y_tile +4},{x_tile +7,y_tile +7},{x_tile,y_tile +7}}, 0xFFFF0000)
					elseif (tile_type == 0x7204) then
						view.DrawPolygon({{x_tile,y_tile +4},{x_tile +7,y_tile +7},{x_tile,y_tile +7}}, 0xFFFF0000)
					elseif (tile_type == 0x7208) then
						view.DrawPolygon({{x_tile,y_tile},{x_tile +7,y_tile},{x_tile +7,y_tile +3}}, 0xFFFF0000)
					elseif (tile_type == 0x720C) then
						view.DrawPolygon({{x_tile,y_tile},{x_tile +7,y_tile},{x_tile,y_tile +3}}, 0xFFFF0000)
					elseif (tile_type == 0x7300) or (tile_type == 0x4200) then
						view.DrawPolygon({{x_tile,y_tile +3},{x_tile +7,y_tile},{x_tile +7,y_tile +7},{x_tile,y_tile +7}}, 0xFFFF0000)
					elseif (tile_type == 0x7304) or (tile_type == 0x4204) then
						view.DrawPolygon({{x_tile,y_tile},{x_tile +7,y_tile +3},{x_tile +7,y_tile +7},{x_tile,y_tile +7}}, 0xFFFF0000)
					elseif (tile_type == 0x7308) or (tile_type == 0x4208) then
						view.DrawPolygon({{x_tile,y_tile},{x_tile +7,y_tile},{x_tile +7,y_tile +7},{x_tile,y_tile +4}}, 0xFFFF0000)
					elseif (tile_type == 0x730C) or (tile_type == 0x420C) then
						view.DrawPolygon({{x_tile,y_tile},{x_tile +7,y_tile},{x_tile +7,y_tile +4},{x_tile,y_tile +7}}, 0xFFFF0000)
					elseif (tile_type == 0x3D00) or (tile_type == 0x360C) then
						view.DrawPolygon({{x_tile +3,y_tile},{x_tile +7,y_tile},{x_tile +7,y_tile +7},{x_tile,y_tile +7}}, 0xFFFF0000)
					elseif (tile_type == 0x3D04) or (tile_type == 0x3608) then
						view.DrawPolygon({{x_tile,y_tile},{x_tile +4,y_tile},{x_tile +7,y_tile +7},{x_tile,y_tile +7}}, 0xFFFF0000)
					elseif (tile_type == 0x3D08) or (tile_type == 0x3604) then
						view.DrawPolygon({{x_tile,y_tile},{x_tile +7,y_tile},{x_tile +7,y_tile +7},{x_tile +3,y_tile +7}}, 0xFFFF0000)
					elseif (tile_type == 0x3D0C) or (tile_type == 0x3600) then
						view.DrawPolygon({{x_tile,y_tile},{x_tile +7,y_tile},{x_tile +4,y_tile +7},{x_tile,y_tile +7}}, 0xFFFF0000)
					elseif (tile_type == 0x1D00) or (tile_type == 0x3400) then
						view.DrawPolygon({{x_tile +7,y_tile},{x_tile +7,y_tile +7},{x_tile +4,y_tile +7}}, 0xFFFF0000)
					elseif (tile_type == 0x1D04) then
						view.DrawPolygon({{x_tile,y_tile},{x_tile +3,y_tile +7},{x_tile,y_tile +7}}, 0xFFFF0000)
					elseif (tile_type == 0x1D08) then
						view.DrawPolygon({{x_tile +4,y_tile},{x_tile +7,y_tile},{x_tile +7,y_tile +7}}, 0xFFFF0000)
					elseif (tile_type == 0x1D0C) then
						view.DrawPolygon({{x_tile,y_tile},{x_tile +3,y_tile},{x_tile,y_tile +7}}, 0xFFFF0000)
					elseif (tile_type == 0x0900) or (tile_type == 0x0904) then
						view.DrawRectangle(x_tile, y_tile +4, 7, 3, 0xFFFF0000)
					elseif (tile_type == 0x6D00) or (tile_type == 0x6D04) then
						view.DrawRectangle(x_tile, y_tile, 7, 3, 0xFFFF0000)
					elseif (tile_type == 0x6408) then
						view.DrawPolygon({{x_tile,y_tile},{x_tile+4,y_tile+4},{x_tile+7,y_tile+4},{x_tile+7,y_tile+7},{x_tile,y_tile+7}}, 0xFFFF0000)
					elseif (tile_type == 0x640C) then
						view.DrawPolygon({{x_tile,y_tile+4},{x_tile+3,y_tile+4},{x_tile+7,y_tile},{x_tile+7,y_tile+7},{x_tile,y_tile+7}}, 0xFFFF0000)
					elseif (tile_type == 0x6508) then
						view.DrawPolygon({{x_tile,y_tile},{x_tile+7,y_tile},{x_tile+7,y_tile+3},{x_tile+3,y_tile+3}}, 0xFFFF0000)
					elseif (tile_type == 0x650C) then
						view.DrawPolygon({{x_tile,y_tile},{x_tile+7,y_tile},{x_tile+4,y_tile+3},{x_tile,y_tile+3}}, 0xFFFF0000)
					elseif (tile_type == 0x2600) or (tile_type == 0x0904) then
						view.DrawRectangle(x_tile +4, y_tile, 3, 7, 0xFFFF0000)
					elseif (tile_type == 0x2F00) or (tile_type == 0x6D04) then
						view.DrawRectangle(x_tile, y_tile, 3, 7, 0xFFFF0000)
					elseif (tile_type == 0x6704) then
						view.DrawPolygon({{x_tile,y_tile},{x_tile+7,y_tile},{x_tile+7,y_tile+7},{x_tile+4,y_tile+7},{x_tile+4,y_tile+4}}, 0xFFFF0000)
					elseif (tile_type == 0x670C) then
						view.DrawPolygon({{x_tile+4,y_tile},{x_tile+7,y_tile},{x_tile+7,y_tile+7},{x_tile,y_tile+7},{x_tile+4,y_tile+3}}, 0xFFFF0000)
					elseif (tile_type == 0x6604) then
						view.DrawPolygon({{x_tile,y_tile},{x_tile+3,y_tile+3},{x_tile+3,y_tile+7},{x_tile,y_tile+7}}, 0xFFFF0000)
					elseif (tile_type == 0x660C) then
						view.DrawPolygon({{x_tile,y_tile},{x_tile+3,y_tile},{x_tile+3,y_tile+4},{x_tile,y_tile+7}}, 0xFFFF0000)
					elseif (tile_type == 0x6700) then
						view.DrawPolygon({{x_tile,y_tile},{x_tile+7,y_tile},{x_tile+3,y_tile+4},{x_tile+3,y_tile+7},{x_tile,y_tile+7}}, 0xFFFF0000)
					elseif (tile_type == 0x6708) then
						view.DrawPolygon({{x_tile,y_tile},{x_tile+3,y_tile},{x_tile+3,y_tile+3},{x_tile+7,y_tile+7},{x_tile,y_tile+7}}, 0xFFFF0000)
					elseif (tile_type == 0x6600) then
						view.DrawPolygon({{x_tile+7,y_tile},{x_tile+7,y_tile+7},{x_tile+4,y_tile+7},{x_tile+4,y_tile+3}}, 0xFFFF0000)
					elseif (tile_type == 0x6608) then
						view.DrawPolygon({{x_tile+4,y_tile},{x_tile+7,y_tile},{x_tile+7,y_tile+7},{x_tile+4,y_tile+4}}, 0xFFFF0000)
					elseif (tile_type == 0x6400) then
						view.DrawPolygon({{x_tile,y_tile},{x_tile+7,y_tile},{x_tile+7,y_tile+3},{x_tile+4,y_tile+3},{x_tile,y_tile+7}}, 0xFFFF0000)
					elseif (tile_type == 0x6404) then
						view.DrawPolygon({{x_tile,y_tile},{x_tile+7,y_tile},{x_tile+7,y_tile+7},{x_tile+3,y_tile+3},{x_tile,y_tile+3}}, 0xFFFF0000)
					elseif (tile_type == 0x6500) then
						view.DrawPolygon({{x_tile,y_tile+7},{x_tile+4,y_tile+3},{x_tile+7,y_tile+3},{x_tile+7,y_tile+7}}, 0xFFFF0000)
					elseif (tile_type == 0x6504) then
						view.DrawPolygon({{x_tile,y_tile+3},{x_tile+3,y_tile+3},{x_tile+7,y_tile+7},{x_tile,y_tile+7}}, 0xFFFF0000)
					end
					
					-- Now we check collision with healing/ending zones. For that, the same calculus is performed except that position is considered signed and:
					-- Modulo of signed integers act like the C one
					-- Floor act like truncate
					-- If it results in tile_addr offset being negative, no collision is reported
					
					-- The following computation does not exactly match the method described above, but it should give the same results in the situations found in the game.
					
					-- We convert position variables to signed one.
					if x_pos_var >= 0x8000 then x_pos_var = x_pos_var-0x10000 end
					if y_pos_var >= 0x8000 then y_pos_var = y_pos_var-0x10000 end
					
					if y_pos_var >= 0 then -- In the top OOB, no healing/ending zone
						if x_pos_var < 0 then -- If both x_pos_var and y_pos_var are still non-negative, previous computation for walls is still valid for zones.
						
							x_pos_tile = math.floor(x_pos_var/tile_size)+1 -- Note: it is different from truncate (it should be ceiling here). We use floor instead for stability reasons in the rendering.
							x_tile = x_tile + 1 --  This line is here to compensate the approximation above.
							x_pos_tile_mod = -((-x_pos_tile) % map_x_size) -- Simulates the C modulo for negative numbers
							
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
						if tile_id == 0xFE or tile_id == 0xFF then
							view.drawRectangle(x_tile, y_tile, tile_size, tile_size, 0, 0x77D0D000)
						elseif tile_id == 0xFB or tile_id == 0xFC or tile_id == 0xFD or tile_id == 0xEA or tile_id == 0xED then
							view.drawRectangle(x_tile, y_tile, tile_size, tile_size, 0, 0x774040FF)
						end
					end
				end
			end
			
			-- We draw the helirin
			local helirin_rot = memory.read_u16_le(addr_rotate, "IWRAM")
			view.drawAxis(helirin_x_screen,helirin_y_screen,6)
			view.drawEllipse(helirin_x_screen-helirin_radius,helirin_y_screen-helirin_radius,helirin_radius*2,helirin_radius*2)
			local angle = helirin_rot * 2*math.pi / 0x10000 -- Rotation in game is stored using the full range of the 16bits variable
			local x1 = helirin_x_screen+math.sin(angle)*helirin_radius
			local y1 = helirin_y_screen-math.cos(angle)*helirin_radius
			local x2 = helirin_x_screen-math.sin(angle)*helirin_radius
			local y2 = helirin_y_screen+math.cos(angle)*helirin_radius
			view.drawLine(x1,y1,x2,y2)
		end
		
		view.refresh()
	end
	emu.frameadvance()
end
