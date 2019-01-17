using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Priority_Queue;

namespace KuruBot
{
    static class Flooding
    {
        const float ground_speed = 0f;
        const float wall_speed = ground_speed;
        const float ground_wall_bonus = 0f;
        const float wall_ground_malus = ground_wall_bonus; // Must be greater or equal than ground_wall_bonus, otherwise there would be negative cycles...

        // Bellman-Ford algorithm should be used instead of Djikstra, because weights can be negative.
        // However, for efficiency reasons, we will use Djikstra anyway. Depending on the settings above, the result could still be correct.

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

        public static float[,] ComputeCostMap(Map m, Pixel start, Pixel end, bool wall_clip)
        {
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
    }
}
