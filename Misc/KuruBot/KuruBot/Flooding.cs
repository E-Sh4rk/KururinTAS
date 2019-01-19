using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Priority_Queue;

namespace KuruBot
{
    class Flooding
    {
        // All these values must be non-negative
        const float ground_speed = 0f;
        const float wall_speed = ground_speed; // Should be equal to ground speed (we can't benefit from wall speed for ever, so a constant bonus is more adapted).
        const float ground_wall_bonus = 0f; // Bonus applied to each pixel in a wall (in a post procedure) in order to simulate the fact that velocity is higher in a wall (for some distance).
        const float ground_wall_bonus_min_dist = 0f; // Min weight required for a wall to benefit from full bonus.
        const float wall_ground_malus = 0f; // Malus applied everytime we go from a wall to a free zone, in order to capture the fact that doing the other way could be expensive.
        const float deep_oob_dist = 4f; // Distance from the wall at which the helirin has no control anymore.

        const float sqrt2 = 1.41421356237F;

        // Ground wall bonus is not applied during the Djikstra algorithm because it would generate negative weights.
        // Instead, it will be applied after to each pixel in collision (proportionally if current weight is smaller than gwb min dist).

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

        public Flooding(Map m, Pixel start, Pixel end)
        {
            this.m = m;
            this.start = start;
            this.end = end;
            legal_zones = ComputeLegalZones();
            dist_to_wall = ComputeDistanceToAWall();
        }

        Pixel start;
        Pixel end;
        Map m;
        bool[,] legal_zones;
        float[,] dist_to_wall;

        Pixel[] DiagNeighbors(Pixel p)
        {
            List<Pixel> res = new List<Pixel>();

            if (p.y > start.y && p.x > start.x)
                res.Add(p.Add(new Pixel(-1, -1)));
            if (p.y < end.y && p.x < end.x)
                res.Add(p.Add(new Pixel(1, 1)));
            if (p.y < end.y && p.x > start.x)
                res.Add(p.Add(new Pixel(-1, 1)));
            if (p.y > start.y && p.x < end.x)
                res.Add(p.Add(new Pixel(1, -1)));

            return res.ToArray();
        }

        Pixel[] StraightNeighbors(Pixel p)
        {
            List<Pixel> res = new List<Pixel>();

            if (p.y > start.y)
                res.Add(p.Add(new Pixel(0,-1)));
            if (p.y < end.y)
                res.Add(p.Add(new Pixel(0,1)));
            if (p.x > start.x)
                res.Add(p.Add(new Pixel(-1,0)));
            if (p.x < end.x)
                res.Add(p.Add(new Pixel(1,0)));

            return res.ToArray();
        }

        PixelDist[] Neighbors(Pixel p)
        {
            List<PixelDist> res = new List<PixelDist>();
            foreach (Pixel pix in StraightNeighbors(p))
                res.Add(new PixelDist(pix,1));
            foreach (Pixel pix in DiagNeighbors(p))
                res.Add(new PixelDist(pix,sqrt2));
            return res.ToArray();
        }

        struct PixelDist
        {
            public PixelDist(Pixel px, float dist)
            {
                this.px = px;
                this.dist = dist;
            }
            public Pixel px;
            public float dist;
        }

        bool[,] ComputeLegalZones()
        {
            int width = end.x - start.x + 1;
            int height = end.y - start.y + 1;
            bool[,] res = new bool[height, width];
            Queue<Pixel> q = new Queue<Pixel>();

            // Init
            for (short y = start.y; y <= end.y; y++)
            {
                for (short x = start.x; x <= end.x; x++)
                {
                    res[y - start.y, x - start.x] = false;
                    if (x >= 0 && x < m.WidthPx && y >= 0 && y < m.HeightPx) // We only consider the ending zones in the "legal map"
                        if (m.IsPixelInZone(x, y) == Map.Zone.Ending)
                            q.Enqueue(new Pixel(x, y));
                }
            }

            // Graph walk
            while (q.Count > 0)
            {
                Pixel p = q.Dequeue();
                if (!res[p.y - start.y, p.x - start.x] && !m.IsPixelInCollision(p.x, p.y))
                {
                    res[p.y - start.y, p.x - start.x] = true;
                    Pixel[] neighbors = StraightNeighbors(p);
                    foreach (Pixel np in neighbors)
                        q.Enqueue(np);
                }
            }

            return res;
        }

        float[,] ComputeDistanceToAWall()
        {
            int width = end.x - start.x + 1;
            int height = end.y - start.y + 1;
            float[,] res = new float[height, width];
            SimplePriorityQueue<Pixel> q = new SimplePriorityQueue<Pixel>();

            // Init
            for (short y = start.y; y <= end.y; y++)
            {
                for (short x = start.x; x <= end.x; x++)
                {
                    if (m.IsPixelInCollision(x,y))
                    {
                        q.Enqueue(new Pixel(x, y), 0);
                        res[y - start.y, x - start.x] = 0;
                    }
                    else
                        res[y - start.y, x - start.x] = float.PositiveInfinity;
                }
            }

            // Djikstra
            while (q.Count > 0)
            {
                Pixel p = q.Dequeue();
                float weight = res[p.y - start.y, p.x - start.x];

                PixelDist[] neighbors = Neighbors(p);
                foreach (PixelDist npd in neighbors)
                {
                    float nw = weight + npd.dist;
                    float ow = res[npd.px.y - start.y, npd.px.x - start.x];
                    if (nw < ow)
                    {
                        res[npd.px.y - start.y, npd.px.x - start.x] = nw;
                        if (ow < float.PositiveInfinity)
                            q.UpdatePriority(npd.px, nw);
                        else
                            q.Enqueue(npd.px, nw);
                    }
                }
            }

            return res;
        }

        public float DistToWall(short x, short y)
        {
            return dist_to_wall[y, x];
        }

        public enum WallClipSetting
        {
            NoWallClip = 0, // Only legal zones
            NoOob,          // Only legal zones or pixels in collision
            NoDeepOob,      // Only legal zones or zones near from a wall
            Allow           // All zones
        }
        public float[,] ComputeCostMap(float gwb_multiplier, float wgm_multiplier, WallClipSetting wall_clip_setting)
        {
            float gwb = ground_wall_bonus * gwb_multiplier;
            float wgm = wall_ground_malus * wgm_multiplier;

            int width = end.x - start.x + 1;
            int height = end.y - start.y + 1;
            float[,] res = new float[height,width];
            SimplePriorityQueue<Pixel> q = new SimplePriorityQueue<Pixel>();

            // Init
            for (short y = start.y; y <= end.y; y++)
            {
                for (short x = start.x; x <= end.x; x++)
                {
                    if (m.IsPixelInZone(x,y) == Map.Zone.Ending)
                    {
                        q.Enqueue(new Pixel(x, y), 0);
                        res[y - start.y, x - start.x] = 0;
                    }
                    else
                        res[y - start.y, x - start.x] = float.PositiveInfinity;
                }
            }

            // Djikstra
            throw new NotImplementedException();

            // Post-procedure
                
            return res;
        }

    }
}
