
-- Parameters
local working_dir = "tasks"
local in_filename = "in.txt"
local out_filename = "out.txt"

-----------------------------

local addr_x_pos      = 0x4544
local addr_y_pos      = 0x4548
local addr_xb     = 0x454C
local addr_yb     = 0x4550
local addr_xs     = 0x4554
local addr_ys     = 0x4558
local addr_rotation     = 0x4572
local addr_rotation_rate   = 0x4574
local addr_rotation_srate   = 0x4576


local in_file = working_dir .. "/" .. in_filename
local out_file = working_dir .. "/" .. out_filename

-- Return true if file exists and is readable.
function file_exists(path)
  local file = io.open(path, "rb")
  if file then file:close() end
  return file ~= nil
end
-- Read an entire file.
function read_file(path)
    local file = io.open(path, "rb") -- r read mode and b binary mode
    if not file then return nil end
    local content = file:read "*a" -- *a or *all reads the whole file
    file:close()
    return content
end
-- Get all lines from a file, returns an empty list/table if the file does not exist.
function lines_from(file)
  if not file_exists(file) then return {} end
  local f = read_file(file)
  return bizstring.split(f, "\n")
end
-- Write a string to a file.
function write_file(filename, contents)
  local fh = assert(io.open(filename, "wb"))
  fh:write(contents)
  fh:flush()
  fh:close()
end
-- Read, process file contents, write.
function modify(filename, modify_func)
  local contents = readall(filename)
  contents = modify_func(contents)
  write(filename, contents)
end
-- Clean string
function normalize_str(str)
	if (str == nil) then return "" end
  local res = bizstring.replace(str, "\r", "")
  return bizstring.trim(res)
end
-----------------------------

local current_play_index = 0
local current_play = {}
local current_play_ans = ""

local recording = false
local current_record_ans = ""

function get_pos()
	local xpos = memory.read_u32_le(addr_x_pos, "IWRAM")
	local ypos = memory.read_u32_le(addr_y_pos, "IWRAM")
	local rot = memory.read_u16_le(addr_rotation, "IWRAM")
	local srot = memory.read_s16_le(addr_rotation_srate, "IWRAM")
	return tostring(xpos) .. " " .. tostring(ypos) .. " " .. tostring(rot) .. " " .. tostring(srot) .. "\n"
end

while true
do
	-- Is there a task that awaits?
	if (current_play_index == 0) and (recording == false) and file_exists(in_file)
	then
		local content = lines_from(in_file)
		--for i=1, #content do end
		if #content > 0 then
			local cmd = normalize_str(content[1])
			if cmd == "DUMPMAP" then
				local xl = memory.read_u16_le(0x0, "EWRAM")
				local yl = memory.read_u16_le(0x2, "EWRAM")
				local res = tostring(xl) .. " " .. tostring(yl) .. "\n"
				for y=0, yl-1 do
					for x=0, xl-1 do
						local tile = memory.read_u16_le(0x4 + x*2 + y*xl*2, "EWRAM")
						res = res .. tostring(tile) .. " "
					end
					res = res .. "\n"
				end
				write_file(out_file, res)
				os.remove(in_file)
				
			elseif cmd == "GETPOS" then
				write_file(out_file, get_pos())
				os.remove(in_file)
				
			elseif cmd == "PLAY" then
				if #content >= 2 then
					-- Init
					local init = bizstring.split(normalize_str(content[2]), " ")
					local xpos = tonumber(init[1]) % 0x100000000
					local ypos = tonumber(init[2]) % 0x100000000
					local rot = tonumber(init[3]) % 0x10000
					local srot = tonumber(init[4])
					memory.write_u32_le(addr_x_pos, xpos, "IWRAM")
					memory.write_u32_le(addr_y_pos, ypos, "IWRAM")
					memory.write_u32_le(addr_xb, 0, "IWRAM")
					memory.write_u32_le(addr_yb, 0, "IWRAM")
					memory.write_u32_le(addr_xs, 0, "IWRAM")
					memory.write_u32_le(addr_ys, 0, "IWRAM")
					memory.write_u16_le(addr_rotation, rot, "IWRAM")
					memory.write_s16_le(addr_rotation_rate, srot, "IWRAM")
					memory.write_s16_le(addr_rotation_srate, srot, "IWRAM")
					-- Play inputs
					current_play = content
					current_play_index = 3
					current_play_ans = ""
				end
				
			elseif cmd == "STARTRECORD" then
				recording = true
				current_record_ans = ""
			end
		end
		
	elseif current_play_index > 0 then
	
		current_play_ans = current_play_ans .. get_pos()
		if current_play_index > #current_play then
			-- Play terminated
			write_file(out_file, current_play_ans)
			os.remove(in_file)
			current_play_ans = ""
			current_play = {}
			current_play_index = 0
		else
			-- Play next frame
			local inputs_txt = current_play[current_play_index]
			local inputs = {}
			-- Up|Down|Left|Right|Start|Select|B|A|L|R|Power
			if bizstring.contains(inputs_txt, "A") then inputs["A"] = true end
			if bizstring.contains(inputs_txt, "B") then inputs["B"] = true end
			if bizstring.contains(inputs_txt, "U") then inputs["Up"] = true end
			if bizstring.contains(inputs_txt, "D") then inputs["Down"] = true end
			if bizstring.contains(inputs_txt, "L") then inputs["Left"] = true end
			if bizstring.contains(inputs_txt, "R") then inputs["Right"] = true end
			joypad.set(inputs)
			current_play_index = current_play_index + 1
		end
	end
	
	-- Recording
	if recording then
		-- Log inputs
		local inputs = joypad.get()
		if inputs["A"] == true then current_record_ans = current_record_ans .. "A" end
		if inputs["B"] == true then current_record_ans = current_record_ans .. "B" end
		if inputs["Up"] == true then current_record_ans = current_record_ans .. "U" end
		if inputs["Down"] == true then current_record_ans = current_record_ans .. "D" end
		if inputs["Left"] == true then current_record_ans = current_record_ans .. "L" end
		if inputs["Right"] == true then current_record_ans = current_record_ans .. "R" end
		current_record_ans = current_record_ans .. "\n"
		-- Terminate?
		if inputs["Power"] == true or inputs["Start"] == true or inputs["Select"] == true then
			-- Recording terminated
			write_file(out_file, current_record_ans)
			os.remove(in_file)
			current_record_ans = ""
			recording = false
		end
	end

	emu.frameadvance()
end