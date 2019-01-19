using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KuruBot
{
    class Bot
    {
        Flooding f = null;
        float[,] current_cost_map = null;

        public Bot(Map m, Flooding.Pixel start, Flooding.Pixel end)
        {
            f = new Flooding(m, start, end);
        }

        public void ComputeNewCostMaps(float gwb_mult, float wgm_mult, Flooding.WallClipSetting wcs)
        {
            current_cost_map = f.ComputeCostMap(gwb_mult, wgm_mult, wcs);
        }

        public float[,] GetCurrentCostMap()
        {
            return current_cost_map;
        }
    }
}
