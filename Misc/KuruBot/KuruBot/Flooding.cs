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
            public short x;
            public short y;
        }

        public Flooding(Map m, Pixel start, Pixel end)
        {
            this.m = m;
            this.start = start;
            this.end = end;
            oob_zones = ComputeOobZones();
        }

        Pixel start;
        Pixel end;
        Map m;
        bool[,] oob_zones;

        bool[,] ComputeOobZones()
        {
            throw new NotImplementedException();
        }

        public float[,] ComputeCostMap(float gwb_multiplier, float oobm_multiplier, bool disallow_wall_clip, float disallow_oob_threshold)
        {
            float gwb = ground_wall_bonus * gwb_multiplier;
            float oobm = oob_malus * oobm_multiplier;

            int width = end.x - start.x + 1;
            int height = end.y - start.y + 1;
            float[,] res = new float[height,width];
            SimplePriorityQueue<Pixel> q = new SimplePriorityQueue<Pixel>();

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
                
            return res;
        }

        public float[,] ComputeDistanceToAWall()
        {
            throw new NotImplementedException();
        }

    }
}
