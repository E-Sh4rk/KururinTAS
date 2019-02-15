using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KuruBot
{
    public struct Pixel
    {
        public Pixel(short x, short y)
        {
            this.x = x;
            this.y = y;
        }
        public Pixel Add(Pixel p)
        {
            Pixel res = this;
            res.x += p.x;
            res.y += p.y;
            return res;
        }
        public short x;
        public short y;
    }

    public class CostMap
    {
        protected float[,] cmap = null;
        protected float[,] wall_dist = null;
        protected bool add_bonus_to_walls = false;
        protected Pixel pixel_start;

        protected float gwb = 0;
        protected float gwb_mc = 0;

        public CostMap(float[,] cmap, float[,] wall_dist, Pixel pixel_start, bool add_bonus_to_walls)
        {
            this.cmap = cmap;
            this.wall_dist = wall_dist;
            this.pixel_start = pixel_start;
            this.add_bonus_to_walls = add_bonus_to_walls;
            gwb = Settings.ground_wall_bonus;
            gwb_mc = Settings.ground_wall_bonus_min_cost;
        }

        public Pixel PixelStart
        {
            get { return pixel_start; }
        }
        public int Height
        {
            get { return (cmap.GetLength(0)); }
        }
        public int Width
        {
            get { return cmap.GetLength(1); }
        }

        public float CostAtIndex(int x, int y, int invul_frames)
        {
            float cost = cmap[y, x];
            if (add_bonus_to_walls && cost > 0)
            {
                float gwb = this.gwb * invul_frames;
                float gwb_mc = this.gwb_mc * invul_frames;
                if (wall_dist[y, x] <= 0)
                {
                    if (cost >= gwb_mc)
                        cost -= gwb;
                    else
                        cost -= gwb * cost / gwb_mc;
                }
                if (cost <= 0)
                    cost = float.Epsilon;
            }
            return cost;
        }
        public float CostAtPx(short x, short y, int invul_frames)
        {
            return CostAtIndex(x - pixel_start.x, y - pixel_start.y, invul_frames);
        }
        public float GetMaxWeightExceptInfinity(int invul_frames)
        {
            float res = 0;
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    float cost = CostAtIndex(x, y, invul_frames);
                    if (cost < float.PositiveInfinity && cost > res)
                        res = cost;
                }
            }
            return res;
        }
    }
}
