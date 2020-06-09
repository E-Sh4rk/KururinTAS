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
        short x, y;
        ushort startTime;
        ushort period;
        const ushort midwaitStart = 0x7FFF, midwait = 0;
        short speed;

        public Rectangle dangerArea;

        public Rectangle PreciseBoxAtTime(int gameFrames)
        {
            // TODO: Add memoisation?
            int t = gameFrames - 1 - startTime;
            int angle = 0;
            if (t >= 0)
            {
                t %= period;
                if (t >= midwaitStart)
                {
                    angle = 0x8000;
                    t = Math.Max(0, t - midwaitStart - midwait);
                }
                angle += t * speed;
                if (angle >= 0x10000)
                    angle = 0;
            }
            int offset = KuruMath.instance.cos(0x100, (short)angle) << 13;

            int x = this.x << 16;
            int y = this.y << 16;
            switch (dir)
            {
                case Direction.Down: y += offset; break;
                case Direction.Left: x -= offset; break;
                case Direction.Up: y -= offset; break;
                case Direction.Right: x += offset; break;
            }
            return new Rectangle(x >>= 16, y >>= 16, w+1, h+1);
        }

        public Piston(int x, int y, int dir, int startTime, int waitTime, int speed)
        {
            this.x = (short)(x * 8);
            this.y = (short)(y * 8);
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
        public class Ball // No need for copies, so we use a class instead of a struct.
        {
            public Ball() { }
            public Ball(int cx, int cy, int r, int t)
            {
                this.cx = cx; this.cy = cy; this.r = r; this.t = t;
            }
            public int cx = 0;
            public int cy = 0;
            public int r = 0;
            public int t = 0;
            public static bool operator <(Ball l, Ball f)
            {
                return l.t < f.t;
            }
            public static bool operator >(Ball l, Ball f)
            {
                return l.t > f.t;
            }
            public bool InCollisionWith(int px, int py)
            {
                int sx = px - cx;
                int sy = py - cy;
                return sx * sx + sy * sy <= r * r;
            }
        }

        short x, y;
        int vx, vy;
        ushort period;
        ushort startTime;
        ushort endTime;

        public Rectangle dangerArea;

        public Ball PreciseBoxAtTime(int gameFrames)
        {
            // TODO: Add memoisation?
            int t = gameFrames - startTime;
            if (t >= 0)
            {
                t %= period;
                if (t < endTime)
                {
                    int ballx = x + ((t * vx) >> 16);
                    int bally = y + ((t * vy) >> 16);
                    return new Ball(ballx, bally, 14, t);
                }
            }
            return null;
        }

        private Roller() { }

        public static List<Roller> Create(BitArray walls, int mapWidth, int x, int y, int dir, int startTime, int period, int speed = 0xC0)
        {
            Roller roller = new Roller();
            dir *= 0x2000;
            roller.vx = KuruMath.instance.sin(speed * 0x100, (short)dir);
            roller.vy = -KuruMath.instance.cos(speed * 0x100, (short)dir);

            roller.x = (short)(x * 8 + 16);
            if ((dir & 0x2000) != 0)
                roller.x -= 8;
            roller.y = (short)(y * 8 + 16);
            roller.startTime = (ushort)startTime;
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

            roller.endTime = (ushort)t;
            period++;
            int remainder = t % period;
            if (remainder != 0)
                t += period - remainder;
            roller.period = (ushort)t;

            List<Roller> res = new List<Roller>();
            for (; t > 0; t -= period)
            {
                res.Add(roller);
                roller = (Roller)roller.MemberwiseClone();
                roller.startTime += (ushort)period;
                //roller.endTime += (ushort)period;
            }
            return res;
        }
    }
}
