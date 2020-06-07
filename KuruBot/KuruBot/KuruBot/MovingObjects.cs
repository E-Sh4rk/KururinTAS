using System;
using System.Collections;
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

    public class Roller
    {
        short x, y;
        int vx, vy;
        short period;
        short startTime;
        short endTime;

        public Rectangle dangerArea;

        private Roller() { }

        public static List<Roller> Create(BitArray walls, int mapWidth, int x, int y, int dir, int startTime, int period, int speed = 0xC000)
        {
            Roller roller = new Roller();
            dir *= 0x2000;
            roller.vx = KuruMath.instance.sin(speed, (short)dir);
            roller.vy = -KuruMath.instance.cos(speed, (short)dir);

            roller.x = (short)(x * 8 + 16);
            if ((dir & 0x2000) != 0)
                roller.x -= 8;
            roller.y = (short)(y * 8 + 16);
            roller.startTime = (short)startTime;
            // Roll through the level until it collides with something
            int colxoffs = KuruMath.instance.sin(15, (short)dir);
            int colyoffs = -KuruMath.instance.cos(15, (short)dir);
            int xpos = roller.x << 16;
            int ypos = roller.y << 16;
            int t = 0;
            bool isPixelInCollision(int xm, int ym)
            {
                int addr = xm + ym * mapWidth;
                return walls[addr];
            }
            while (true)
            {
                xpos += roller.vx;
                ypos += roller.vy;
                
                if (isPixelInCollision((xpos >> 16) + colxoffs, (ypos >> 16) + colyoffs))
                    break;
                t++;
            }

            int minx = Math.Min(roller.x, xpos >> 16) - 14;
            int miny = Math.Min(roller.y, ypos >> 16) - 14;
            int maxx = Math.Max(roller.x, xpos >> 16) + 14;
            int maxy = Math.Max(roller.y, ypos >> 16) + 14;
            roller.dangerArea = Rectangle.FromLTRB(minx, miny, maxx + 1, maxy + 1);

            roller.endTime = (short)t;
            period++;
            int remainder = t % period;
            if (remainder != 0)
                t += period - remainder;
            roller.period = (short)t;

            List<Roller> res = new List<Roller>();
            for (; t > 0; t -= period)
            {
                res.Add(roller);
                roller = (Roller)roller.MemberwiseClone();
                roller.startTime += (short)period;
                roller.endTime += (short)period;
            }
            return res;
        }
    }
}
