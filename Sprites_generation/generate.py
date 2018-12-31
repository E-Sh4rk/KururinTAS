# Inspired from https://medium.com/message/building-a-cheat-bot-f848f199e76b

from PIL import Image

rom = open('0048 - Kuru Kuru Kururin (E)(Mode7).gba', 'rb')
rom.seek(0x1DA788)
blocks = []
for i in range(0x400):
    stream=bytearray()
    for byte in rom.read(32):
        stream.append((byte & 15) << 4)
        stream.append((byte >> 4) << 4)
    blocks.append(Image.frombytes('L', (8, 8), bytes(stream)))
rom.close()

for i in range(0x400):
    blocks.append(blocks[i].transpose(Image.FLIP_LEFT_RIGHT))
for i in range(0x800):
    blocks.append(blocks[i].transpose(Image.FLIP_TOP_BOTTOM))

img = Image.new('L', (8*0x1000, 8))
for tile in range(0x1000):
        img.paste(blocks[tile], (tile * 8, 0))
#img.show()
img.save("sprites.png", "PNG")