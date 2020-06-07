using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace KuruBot
{
    public class Piston
    {
        public enum Direction
        {
            Up = 0,
            Down,
            Left,
            Right
        }

        Direction dir;
        const byte w = 4*8, h = 4*8;
        ushort x, y;
        ushort startTime;
        ushort period;
        const ushort midwaitStart = 0x7FFF, midwait = 0;
        short speed;
        const short strokeLength = 0;

        public Rectangle dangerArea;

        public Piston(int x, int y, int dir, int startTime, int waitTime, int speed)
        {
            this.x = (ushort)(x * 8);
            this.y = (ushort)(y * 8);
            this.startTime = (ushort)startTime;
            period = (ushort)waitTime;
            period += (ushort)((0x10000 + speed - 1) / speed);
            this.speed = (short)speed;
            switch (dir)
            {
                case 0:
                    this.dir = Direction.Down;
                    break;
                case 1:
                    this.dir = Direction.Left;
                    break;
                case 2:
                    this.dir = Direction.Up;
                    break;
                case 3:
                    this.dir = Direction.Right;
                    break;
            }

            short hStrokeLength = 32;
            int minx = -1, miny = -1;
            int maxx = w + 1, maxy = h + 1;
            switch (this.dir)
            {
                case Direction.Up: case Direction.Down:
                    miny -= hStrokeLength;
                    maxy += hStrokeLength;
                    break;
                case Direction.Left: case Direction.Right:
                    minx -= hStrokeLength;
                    maxx += hStrokeLength;
                    break;
            }
            minx -= 8; miny -= 8; maxx += 8; maxy += 8;
            dangerArea = Rectangle.FromLTRB(this.x + minx, this.y + miny, this.x + maxx+1, this.y + maxy+1);
        }
    }
}
