
-- Parameters
local working_dir = "tasks"
local in_filename = "in.txt"
local out_filename = "out.txt"

-----------------------------

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
function clean_str(str)
	if (str == nil) then return "" end
  local res = bizstring.replace(str, "\r", "")
  return bizstring.trim(res)
end
-----------------------------

while true
do
	-- Is there a task that awaits?
	if file_exists(in_file)
	then
		local content = lines_from(in_file)
		--for i=1, #content do end
		if #content > 0 then
			if clean_str(content[1]) == "DUMPMAP" then
				local xl = memory.read_u16_le(0x0, "EWRAM")
				local yl = memory.read_u16_le(0x2, "EWRAM")
				local res = tostring(xl) .. " " .. tostring(yl) .. "\n"
				for y=0, yl-1 do
					for x=0, xl-1 do
						local tile = memory.read_u16_le(0x4 + x*2 + y*xl*2 + 4, "EWRAM")
						res = res .. tostring(tile) .. " "
					end
					res = res .. "\n"
				end
				write_file(out_file, res)
				os.remove(in_file)
			end
		end
	end

	emu.frameadvance()
end