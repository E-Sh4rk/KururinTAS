using Priority_Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KuruBot
{
    class Bot
    {
        Flooding f = null;
        Physics p = null;
        Form1 parent = null;
        float[,] current_cost_map = null;

        public Bot(Form1 parent, Map m, Physics p, Flooding.Pixel start, Flooding.Pixel end)
        {
            this.parent = parent;
            this.p = p;
            f = new Flooding(m, start, end);
        }

        public Flooding.Pixel GetPixelStart() { return f.PixelStart; }
        public Flooding.Pixel GetPixelEnd() { return f.PixelEnd; }

        public void SetTarget(bool[,] target)
        {
            f.SetTarget(target);
        }
        public void SetConstraints(bool[,] constraints)
        {
            f.SetConstraints(constraints);
        }

        public void ComputeNewCostMaps(float gwb_mult, Flooding.WallClipSetting wcs)
        {
            current_cost_map = f.ComputeCostMap(gwb_mult, wcs);
        }

        public float[,] GetCurrentCostMap()
        {
            return current_cost_map;
        }

        // /!\ For efficiency reason, we use a class instead of a struct.
        // It avoid useless copies and allows us to modify some values without accessing again the dictionnary.
        class StateData
        {
            public StateData(HelirinState es, float w, float c, Action? a, HelirinState ps, bool at)
            {
                exact_state = es;
                weight = w;
                cost = c;
                action = a;
                previous_state = ps;
                already_treated = at;
            }
            public HelirinState exact_state;
            public float weight;
            public float cost;
            public Action? action;
            public HelirinState previous_state;
            public bool already_treated;
        }

        HelirinState NormaliseState (HelirinState st)
        {
            st = st.ShallowCopy();

            float wall_dist = f.DistToWall(Physics.pos_to_px(st.xpos), Physics.pos_to_px(st.ypos)) * Settings.reduction_dist_multiplier;
            int add_red = wall_dist == 0 ? Settings.additional_reduction_in_wall : Math.Min((int)wall_dist, Settings.max_additional_reduction);
            int pos_reduction = Settings.pos_reduction + add_red;
            int bump_reduction = Settings.bump_reduction + add_red;

            st.xpos = (st.xpos >> pos_reduction) << pos_reduction;
            st.ypos = (st.ypos >> pos_reduction) << pos_reduction;
            st.xb   = (st.xb >> bump_reduction) << bump_reduction;
            st.yb   = (st.yb >> bump_reduction) << bump_reduction;
            st.rot  = (short)((int)Math.Round((float)st.rot / Settings.rot_precision) * Settings.rot_precision);
            st.rot_rate = (short)((int)Math.Round((float)st.rot_rate / Settings.rot_rate_precision) * Settings.rot_rate_precision);

            return st;
        }

        float GetCost(int xpos, int ypos)
        {
            short xpix = Physics.pos_to_px(xpos);
            short ypix = Physics.pos_to_px(ypos);
            return f.Cost(current_cost_map, xpix, ypix);
        }

        bool IsOutOfSearchSpace(int xpos, int ypos)
        {
            short xpix = Physics.pos_to_px(xpos);
            short ypix = Physics.pos_to_px(ypos);
            return xpix < f.PixelStart.x || xpix > f.PixelEnd.x || ypix < f.PixelStart.y || ypix > f.PixelEnd.y;
        }

        public Action[] Solve (HelirinState init)
        {
            if (current_cost_map == null)
                return null;
            SimplePriorityQueue<HelirinState> q = new SimplePriorityQueue<HelirinState>();
            Dictionary<HelirinState, StateData> data = new Dictionary<HelirinState, StateData>();

            // Init
            HelirinState norm_init = NormaliseState(init);
            float cost = GetCost(init.xpos, init.ypos);
            float weight = 0;
            q.Enqueue(norm_init, cost);
            data.Add(norm_init, new StateData(init, weight, cost, null, null, false));

            // ProgressBar and preview settings
            float init_cost = cost;
            bool[,] preview = new bool[current_cost_map.GetLength(0), current_cost_map.GetLength(1)];
            int since_last_update = 0;

            // A*
            HelirinState result = null;
            while (q.Count > 0 && result == null)
            {
                HelirinState norm_st = q.Dequeue();
                StateData st_data = data[norm_st];
                st_data.already_treated = true;
                weight = st_data.weight + 1;

                // ProgressBar and preview settings
                preview[Physics.pos_to_px(st_data.exact_state.ypos)-f.PixelStart.y, Physics.pos_to_px(st_data.exact_state.xpos)-f.PixelStart.x] = true;
                since_last_update++;
                if (since_last_update >= Settings.number_iterations_before_ui_update)
                {
                    since_last_update = 0;
                    parent.UpdateProgressBarAndHighlight(100 - st_data.cost * 100 / init_cost, preview);
                }

                for (int i = 0; i < 25; i++)
                {
                    Action a = (Action)i;
                    HelirinState nst = p.Next(st_data.exact_state, a);
                    HelirinState norm_nst = NormaliseState(nst);

                    // Out of search space / Loose ?
                    if (nst.gs == GameState.Loose || IsOutOfSearchSpace(nst.xpos, nst.ypos))
                        continue;

                    // Already visited ?
                    StateData old = null;
                    data.TryGetValue(norm_nst, out old);
                    if (!Settings.allow_state_multiple_visits && old != null && old.already_treated)
                        continue;

                    // Better cost ?
                    cost = GetCost(nst.xpos, nst.ypos);
                    float total_cost = cost + weight;
                    if (old == null || total_cost < old.cost + old.weight)
                    {
                        StateData nst_data = new StateData(nst, weight, cost, a, norm_st, false);
                        data[norm_nst] = nst_data;

                        // Target reached ? We look at the cost rather than the game state, because the target can be different than winning
                        if (cost <= 0)
                        {
                            result = norm_nst;
                            break;
                        }
                        
                        if (old == null || old.already_treated)
                            q.Enqueue(norm_nst, total_cost);
                        else
                            q.UpdatePriority(norm_nst, total_cost);
                    }
                }
            }

            // Retrieve full path
            if (result == null)
                return null;
            List<Action> res = new List<Action>();
            while (result != null)
            {
                StateData sd = data[result];
                if (sd.action.HasValue)
                    res.Add(sd.action.Value);
                result = sd.previous_state;
            }
            res.Reverse();
            return res.ToArray();
        }
    }
}
