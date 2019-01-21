using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KuruBot
{
    // TODO: optimize copies of HelirinState (use 'in' keyword, etc)
    class Bot
    {
        Flooding f = null;
        float[,] current_cost_map = null;

        public Bot(Map m, Flooding.Pixel start, Flooding.Pixel end)
        {
            f = new Flooding(m, start, end);
        }

        public Flooding.Pixel GetPixelStart() { return f.GetPixelStart(); }
        public Flooding.Pixel GetPixelEnd() { return f.GetPixelEnd(); }

        public void ComputeNewCostMaps(float gwb_mult, float wgm_mult, Flooding.WallClipSetting wcs)
        {
            current_cost_map = f.ComputeCostMap(gwb_mult, wgm_mult, wcs);
        }

        public float[,] GetCurrentCostMap()
        {
            return current_cost_map;
        }

        class StateData
        {
            HelirinState exact_state;
            Action action;
            HelirinState previous_state;
        }

        const int pos_reduction = 0x10000 / 64; // 1/64 px
        const int bump_reduction = 0x10000 / 64; // 1/64 px/frame
        const short rot_reduction = Physics.default_srate;
        const short rot_rate_reduction = Physics.default_srate;

        HelirinState NormaliseState (HelirinState st)
        {
            int wall_dist = (int)f.DistToWall(Physics.pos_to_px(st.xpos), Physics.pos_to_px(st.ypos)) + 1;
            int pos_reduction = Bot.pos_reduction * wall_dist;
            int bump_reduction = Bot.bump_reduction * wall_dist;

            st.xpos = (st.xpos / pos_reduction) * pos_reduction;
            st.ypos = (st.ypos / pos_reduction) * pos_reduction;
            st.xb   = (st.xb / bump_reduction) * bump_reduction;
            st.yb   = (st.yb / bump_reduction) * bump_reduction;
            st.rot  = (short)((st.rot / rot_reduction) * rot_reduction);
            st.rot_rate = (short)((st.rot_rate / rot_rate_reduction) * rot_rate_reduction);

            return st;
        }

        public Action[] Solve (HelirinState init)
        {
            // TODO
            throw new NotImplementedException();
        }
    }
}
