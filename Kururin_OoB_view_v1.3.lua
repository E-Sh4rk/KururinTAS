-- Kuru Kuru Kururin - OoB Viewer v1.3 by ThunderAxe31 & E-Sh4rk
-- TODO:
-- Find all wall tile types OR find a cleaner way to do draw it (game graphics, pixels with collision, ...)
-- Find why ending zone are offset from walls in OOB (left: from 1px, top: not even here?)

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
			-- Position seems to be considered unsigned by the game: effects of the overflow are visible on some maps around position 0. (e.g. MachineLand1, or other maps such that the width is not a power of 2) 
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
					local x_pos2 = (x_pos + x*tile_size) % 0x10000
					local y_pos2 = (y_pos + y*tile_size) % 0x10000
					
					local x_pos_floor = math.floor(x_pos2/tile_size)
					local y_pos_floor = math.floor(y_pos2/tile_size)
					
					-- Map is stored at the very beggining of EWRAM. The 2 first dwords contain the size of the map.
					local tile_addr = (x_pos_floor %map_x_size)*2 +(y_pos_floor %map_y_size)*map_x_size*2 + 4
					local tile_type = memory.read_u16_be(tile_addr, "EWRAM") -- EWRAM = 0x02000000
					local x_tile = x*tile_size -x_mod
					local y_tile = y*tile_size -y_mod
					
					-- We draw the tile depending on its type
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
					elseif (tile_type == 0xFB00) or (tile_type == 0xFB04) or (tile_type == 0xFB08) or (tile_type == 0xFB0C) or (tile_type == 0xFD00) or (tile_type == 0xFD04) or (tile_type == 0xFD08) or (tile_type == 0xFD0C) or (tile_type == 0xEA00) or (tile_type == 0xEA04) or (tile_type == 0xEA08) or (tile_type == 0xEA0C) or (tile_type == 0xED00) or (tile_type == 0xED04) or (tile_type == 0xED08) or (tile_type == 0xED0C) then
						view.DrawRectangle(x_tile, y_tile, 7, 7, 0xFF4040FF)
					elseif (tile_type == 0xFE00) or (tile_type == 0xFE04) or (tile_type == 0xFE08) or (tile_type == 0xFE0C) or (tile_type == 0xFF00) or (tile_type == 0xFF04) or (tile_type == 0xFF08) or (tile_type == 0xFF0C) then
						view.DrawRectangle(x_tile, y_tile, 7, 7, 0xFFD0D000)
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
