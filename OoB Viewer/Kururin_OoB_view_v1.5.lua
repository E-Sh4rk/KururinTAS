-- Kuru Kuru Kururin - OoB Viewer v1.5 by ThunderAxe31 & E-Sh4rk

-- Script parameters
local x_nb_tiles = 30 -- Must be 30 if draw_in_separate_window is set to false. Default: 30
local y_nb_tiles = 20 -- Must be 20 if draw_in_separate_window is set to false. Default: 20
local draw_in_separate_window = true
local window_zoom = nil -- You can specify a scale factor for the separate window (e.g. 2). Default: nil
local keys_activate = {"LeftShift", "V"}
local keys_deactivate = {"LeftControl", "V"}
local activated = true -- Automatically activate the viewer when the script is started?

-- Do not touch that
local addr_map_x_size = 0x313C -- Can also be found at EWRAM+0x0
local addr_map_y_size = 0x313E -- Can also be found at EWRAM+0x2
local addr_x_pos      = 0x4546 -- Actually at 0x4544 on 4 bytes, but we are only interested in the 2 most significant bytes
local addr_y_pos      = 0x454A -- Actually at 0x4548 on 4 bytes, but we are only interested in the 2 most significant bytes
local addr_rotate     = 0x4572

local addr_cam_left   = 0x43CC
local addr_cam_right  = 0x43CE
local addr_cam_top    = 0x43D0
local addr_cam_bottom = 0x43D2

local addr_frame      = 0x4584
local special_frame   = 0xFF

local tile_size = 8
local helirin_radius = 32
local helirin_x_screen = math.ceil (x_nb_tiles*tile_size / 2)
local helirin_y_screen = math.ceil (y_nb_tiles*tile_size / 2)

-- Initialize the view
local view = nil
if draw_in_separate_window
then
	if window_zoom == nil then
		view = gui.createcanvas(x_nb_tiles*tile_size, y_nb_tiles*tile_size)
		view.drawText = view.DrawText
		view.drawImage = view.DrawImage
		view.drawLine = view.DrawLine
		view.drawAxis = view.DrawAxis
		view.drawEllipse = view.DrawEllipse
		view.drawRectFix = view.DrawRectangle
	else
		view = gui.createcanvas(x_nb_tiles*tile_size*window_zoom, y_nb_tiles*tile_size*window_zoom)
		view.drawText = function (a,b,c,d,e,f,g,h,i,j) view.DrawText(a*window_zoom,b*window_zoom,c,d,e,f*window_zoom,g,h,i,j) end
		view.drawImage = function (a,b,c) view.DrawImage(a,b*window_zoom,c*window_zoom,tile_size*window_zoom,tile_size*window_zoom) end
		view.drawLine = function (a,b,c,d) view.DrawLine(a*window_zoom,b*window_zoom,c*window_zoom,d*window_zoom) end
		view.drawAxis = function (a,b,c) view.DrawAxis(a*window_zoom,b*window_zoom,c*window_zoom) end
		view.drawEllipse = function (a,b,c,d) view.DrawEllipse(a*window_zoom,b*window_zoom,c*window_zoom,d*window_zoom) end
		view.drawRectFix = function (a,b,c,d,e,f) view.DrawRectangle(a*window_zoom,b*window_zoom,c*window_zoom,d*window_zoom,e,f) end
	end
	view.clearImageCache = view.ClearImageCache
	view.clear = view.Clear
	view.refresh = view.Refresh
	view.clearGraphics = function () end
	view.SetTitle("Out of Bounds Viewer")
	view.SetDefaultForegroundColor(0xFF000000)
	
else
	view = gui
	view.clear = function (x) end --function (x) view.drawRectangle(0, 0, 239, 159, x, x) end
	view.refresh = function () end
	view.drawRectFix = function (x,y,w,h,c1,c2) view.drawRectangle(x-1,y-1,w+1,h+1,c1,c2) end
	view.defaultForeground(0xFF000000)
	--client.SetGameExtraPadding(0, 168, 0, 0)
end

local must_wait = false
local pos_default = 32 * tile_size
local x_pos_old = pos_default
local y_pos_old = pos_default
local x_pos_older = pos_default
local y_pos_older = pos_default
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
		local map_x_size_px = map_x_size*tile_size
		local map_y_size_px = map_y_size*tile_size
			
		-- If we are not in a level, we do nothing
		if map_x_size <= 32 and map_y_size <= 32
		then
			must_wait = true
			memory.write_u16_le(addr_frame, special_frame, "IWRAM")
			if draw_in_separate_window
			then
				view.drawText(x_nb_tiles*tile_size/2, y_nb_tiles*tile_size/2, "Not in a level...", nil, nil, 12, nil, nil, "center", "middle")
			else
				x_pos_old = pos_default
				y_pos_old = pos_default
				x_pos_older = pos_default
				y_pos_older = pos_default
			end
		else
			if must_wait
			then
				if memory.read_u16_le(addr_frame, "IWRAM") ~= special_frame then must_wait = false end
			end
			
			if not must_wait
			then
				if not draw_in_separate_window
				then
					-- Cam Hack
					memory.write_u16_le(addr_cam_left, 0x8000, "IWRAM")
					memory.write_u16_le(addr_cam_right, 0x7FFF, "IWRAM")
					memory.write_u16_le(addr_cam_top, 0x8000, "IWRAM")
					memory.write_u16_le(addr_cam_bottom, 0x7FFF, "IWRAM")
				end
				
				-- We consider position as an unsigned variable (see below).
				local x_pos = memory.read_u16_le(addr_x_pos, "IWRAM")
				local y_pos = memory.read_u16_le(addr_y_pos, "IWRAM")
				if not draw_in_separate_window
				then
					local x_tmp = x_pos
					local y_tmp = y_pos
					x_pos = x_pos_older
					y_pos = y_pos_older
					x_pos_older = x_pos_old
					y_pos_older = y_pos_old
					x_pos_old = x_tmp
					y_pos_old = y_tmp
				end
				
				-- Position for the top left corner of the screen
				x_pos = x_pos - (x_nb_tiles*tile_size/2)
				y_pos = y_pos - (y_nb_tiles*tile_size/2)

				local x_mod = x_pos%tile_size
				local y_mod = y_pos%tile_size
				
				for y=0, y_nb_tiles do
					for x=0, x_nb_tiles do
					
						local x_pos_var = (x_pos + x*tile_size) % 0x10000
						local y_pos_var = (y_pos + y*tile_size) % 0x10000
						
						if draw_in_separate_window or x_pos_var >= map_x_size_px or y_pos_var >= map_y_size_px
						then
							local x_tile = x*tile_size -x_mod
							local y_tile = y*tile_size -y_mod
						
							-- For walls, the collision check seems to consider position as an unsigned variable:
							-- effects of the overflow are visible on some maps around position 0. (e.g. MachineLand1, or other maps such that the width is not a power of 2) 
							-- The modulo simulates overflow of the position variables. It seems faster than bit.band(..., 0xFFFF). 
							local x_pos_tile = math.floor(x_pos_var/tile_size)
							local y_pos_tile = math.floor(y_pos_var/tile_size)
							local x_pos_tile_mod = x_pos_tile % map_x_size
							local y_pos_tile_mod = y_pos_tile % map_y_size
							
							-- Map is stored at the very beginning of EWRAM. The 2 first dwords contain the size of the map.
							-- For more details, see https://medium.com/message/building-a-cheat-bot-f848f199e76b
							local tile_addr = x_pos_tile_mod*2 + y_pos_tile_mod*map_x_size*2 + 4
							local tile_type = memory.read_u16_le(tile_addr, "EWRAM") -- EWRAM = 0x02000000
							
							-- We draw the wall tile depending on its type
							-- It follows this format: http://problemkaputt.de/gbatek.htm#lcdvrambgscreendataformatbgmap
							-- Bits 0-9: Tile number
							-- Bit 10: Horizontal flip
							-- Bit 11: Vertical flip
							-- Bits 12-15: Palette number
							local tile_index = tile_type % 0x1000 -- Seems faster than bit.band(tile_type, 0xFFF)
							local tile_id = tile_index % 0x400 -- Seems fatser than bit.band(tile_index, 0x3FF)
							if tile_id ~= 0 and tile_id <= 130 and tile_id ~= 23 and tile_id ~= 26 and tile_id ~= 56 and tile_id ~= 125 then
								view.drawImage("sprites/" .. tostring(tile_index) .. ".png", x_tile, y_tile)
							end
							--if x == x_nb_tiles/2 and y == y_nb_tiles/2 then console.log(tile_type) end -- Uncomment to print the tile type under the helirin.
							--view.drawText(x_tile+tile_size/2, y_tile+tile_size/2, tostring(tile_id), nil, nil, 4, nil, nil, "center", "middle") -- Uncomment to print the tile id on each sprite.

							
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
									view.drawRectFix(x_tile, y_tile, tile_size, tile_size, 0, 0xA0F09000)
								elseif tile_id == 0xFB or tile_id == 0xFC or tile_id == 0xFD or tile_id == 0xEA or tile_id == 0xEB or tile_id == 0xEC or tile_id == 0xED or tile_id == 0xEE or tile_id == 0xEF then
									view.drawRectFix(x_tile, y_tile, tile_size, tile_size, 0, 0x774040FF)
								end
							end
						end
					end
				end
				
				-- We draw the helirin
				if draw_in_separate_window
				then
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
			end
		end
		
		view.refresh()
	end
	emu.frameadvance()
end
