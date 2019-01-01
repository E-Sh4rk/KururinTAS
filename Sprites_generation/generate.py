# Inspired from https://medium.com/message/building-a-cheat-bot-f848f199e76b

from PIL import Image
import os

rom = open('0048 - Kuru Kuru Kururin (E)(Mode7).gba', 'rb')
rom.seek(0x1DA788)
blocks = []
for i in range(0x400):
    stream=bytearray()
    for byte in rom.read(32):
        stream.append(255 - ((byte & 15) << 4))
        stream.append(255 - ((byte >> 4) << 4))
    
    if len([x for x in stream if x != 0xFF]) > 0:
        blocks.append(Image.frombytes('L', (8, 8), bytes(stream)))
    else:
        blocks.append(None)
rom.close()

for i in range(0x400):
    if blocks[i] != None:
        blocks.append(blocks[i].transpose(Image.FLIP_LEFT_RIGHT))
    else:
        blocks.append(None)
for i in range(0x800):
    if blocks[i] != None:
        blocks.append(blocks[i].transpose(Image.FLIP_TOP_BOTTOM))
    else:
        blocks.append(None)

# Output
if not os.path.exists("sprites"):
    os.makedirs("sprites")
		
# Creating the preview
void_sprites = open('sprites/void_sprites.txt', 'w')
img = Image.new('L', (8*0x400, 4*8))
for tile in range(0x1000):
    if blocks[tile] != None:
        img.paste(blocks[tile], ((tile % 0x400) * 8, (tile // 0x400) * 8))
    else:
        void_sprites.write(str(tile) + "\n")
void_sprites.close()
img.save("sprites/preview.bmp", "BMP")
img.save("sprites/preview.png", "PNG")

# Creating all sprites separately
max_id = 130
for tile in range(0x1000):
    if blocks[tile] != None and (tile % 0x400) <= max_id:
        blocks[tile].save("sprites/"+str(tile)+".bmp", "BMP")