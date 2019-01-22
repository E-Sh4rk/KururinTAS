using Priority_Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KuruBot
{
    // TODO: optimize copies of HelirinState (use 'in' keyword, or convert to class BUT make copies when needed)
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
            public StateData(HelirinState es, Action? a, HelirinState? ps)
            {
                exact_state = es;
                action = a;
                previous_state = ps;
            }
            public HelirinState exact_state;
            public Action? action;
            public HelirinState? previous_state;
        }

        const int pos_reduction = 0x10000 / 64; // 1/64 px
        const int bump_reduction = 0x10000 / 64; // 1/64 px/frame
        const short rot_reduction = Physics.default_srate;
        const short rot_rate_reduction = Physics.default_srate;

        HelirinState NormaliseState (HelirinState st)
        {
            int wall_dist = (int)f.DistToWall(Physics.pos_to_px(st.xpos), Physics.pos_to_px(st.ypos)) / Map.tile_size + 1;
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

        float GetCost(int xpos, int ypos)
        {
            short xpix = Physics.pos_to_px(xpos);
            short ypix = Physics.pos_to_px(ypos);
            return f.Cost(current_cost_map, xpix, ypix);
        }

        public Action[] Solve (HelirinState init)
        {
            SimplePriorityQueue<HelirinState> q = new SimplePriorityQueue<HelirinState>();
            Dictionary<HelirinState, StateData> data = new Dictionary<HelirinState, StateData>();

            // Init
            HelirinState norm_init = NormaliseState(init);
            q.Enqueue(norm_init, GetCost(init.xpos, init.ypos));
            data.Add(norm_init, new StateData(init, null, null));

            // TODO
            throw new NotImplementedException();
        }
    }
}
