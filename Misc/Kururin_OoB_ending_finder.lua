-- Kuru Kuru Kururin - OoB Ending Finder by E-Sh4rk
-- Test whether there is an hidden ending zone in the top OOB.
-- The script must be run when Kururin is at the initial position of the level.

local addr_map_x_size = 0x313C
local addr_map_y_size = 0x313E
local addr_x_pos      = 0x4546
local addr_y_pos      = 0x454A
local tile_size       = 8

local map_x_size = memory.read_u16_le(addr_map_x_size, "IWRAM")
local map_y_size = memory.read_u16_le(addr_map_y_size, "IWRAM")

local init_x_pos = memory.read_u16_le(addr_x_pos, "IWRAM")
local init_y_pos = memory.read_u16_le(addr_y_pos, "IWRAM")

for y=-map_y_size, 0 do
	for x=0, map_x_size do
		-- Go to the sprite position
		x_pos = x*tile_size + tile_size/2
		y_pos = y*tile_size + tile_size/2
		memory.write_s16_le(addr_x_pos, x_pos, "IWRAM")
		memory.write_s16_le(addr_y_pos, y_pos, "IWRAM")
		emu.frameadvance()
		-- Go back to initial position to heal
		memory.write_u16_le(addr_x_pos, init_x_pos, "IWRAM")
		memory.write_u16_le(addr_y_pos, init_y_pos, "IWRAM")
		emu.frameadvance()
	end
end
