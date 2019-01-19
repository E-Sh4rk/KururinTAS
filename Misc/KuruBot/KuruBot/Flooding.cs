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
        const float oob_malus = 0f; // Malus applied everytime we go from a wall to an oob zone, in order to capture the fact that leaving the oob zone could be expensive.

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
        }

        Pixel start;
        Pixel end;
        Map m;
        bool[,] legal_zones;

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

        public float[,] ComputeCostMap(float gwb_multiplier, float oobm_multiplier, bool disallow_wall_clip, float disallow_oob_threshold)
        {
            float gwb = ground_wall_bonus * gwb_multiplier;
            float oobm = oob_malus * oobm_multiplier;

            int width = end.x - start.x + 1;
            int height = end.y - start.y + 1;
            float[,] res = new float[height,width];
            HashSet<Pixel> already_visited = new HashSet<Pixel>();
            SimplePriorityQueue<Pixel> q = new SimplePriorityQueue<Pixel>();

            // Init
            for (short y = start.y; y <= end.y; y++)
            {
                for (short x = start.x; x <= end.x; x++)
                {
                    if (m.IsPixelInZone(x,y) == Map.Zone.Ending)
                    {
                        res[y - start.y, x - start.x] = 0;
                        q.Enqueue(new Pixel(x, y), 0);
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

        public float[,] ComputeDistanceToAWall()
        {
            throw new NotImplementedException();
        }

    }
}
