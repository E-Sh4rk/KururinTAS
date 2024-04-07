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

        protected float wb = 0;
        protected float wb_rc = 0;
        protected float global_malus = 0;

        public CostMap(float[,] cmap, float[,] wall_dist, Pixel pixel_start, bool add_bonus_to_walls)
        {
            this.cmap = cmap;
            this.wall_dist = wall_dist;
            this.pixel_start = pixel_start;
            this.add_bonus_to_walls = add_bonus_to_walls;
            wb = Settings.wall_bonus_per_invul;
            wb_rc = Settings.wall_bonus_required_cost;
        }

        public static int GetRealInvul(byte life, sbyte invul)
        {
            if (Settings.invul_frames < 0)
                return -1;
            return Math.Max(0, life - 1) * Settings.invul_frames + Math.Max(0, invul - 1);
        }

        public float GlobalMalus
        {
            get { return global_malus; }
            set { global_malus = value; }
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
            if (invul_frames < 0 || invul_frames > 1024) invul_frames = 1024;
            float cost = cmap[y, x];
            if (add_bonus_to_walls && cost > 0)
            {
                if (wall_dist[y, x] <= 0)
                {
                    float wb = this.wb * invul_frames;
                    float wb_rc = this.wb_rc * invul_frames;
                    float coef = Math.Min(1f, cost / wb_rc);
                    cost -= wb * coef;
                }
                if (cost <= 0)
                    cost = float.Epsilon;
            }
            return cost + global_malus;
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

    public class ExtendedCostMap
    {
        CostMap targetCM = null;
        CostMap bonusCM = null;
        public ExtendedCostMap(CostMap targetCM, CostMap bonusCM)
        {
            this.targetCM = targetCM;
            this.bonusCM = bonusCM;
        }

        public CostMap Get(bool hasBonus)
        {
            if (!hasBonus && bonusCM != null)
                return bonusCM;
            return targetCM;
        }
    }
}
