using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        public static float[,] ComputeCostMap(Map m, bool wall_clip)
        {
            float[,] res = new float[m.HeightPx,m.WidthPx];
            for (int y = 0; y < m.HeightPx; y++)
                for (int x = 0; x < m.WidthPx; x++)
                    res[y, x] = float.PositiveInfinity;

            return res;
        }
    }
}
