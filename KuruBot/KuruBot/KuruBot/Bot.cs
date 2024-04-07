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
        ExtendedCostMap cost_map = null;
        ExtendedCostMap nc_cost_map = null;

        public Bot(Form1 parent, Map m, Physics p, Pixel start, Pixel end)
        {
            this.parent = parent;
            this.p = p;
            f = new Flooding(m, start, end);
        }

        public Pixel GetPixelStart() { return f.PixelStart; }
        public Pixel GetPixelEnd() { return f.PixelEnd; }

        public void SetTarget(bool[,] target)
        {
            f.SetTarget(target);
        }
        public void SetConstraints(bool[,] constraints)
        {
            f.SetConstraints(constraints);
        }

        public void ComputeNewCostMaps()
        {
            cost_map = f.ComputeExtendedCostMap(false);
            parent.UpdateProgressBarAndHighlight(50, null);
            nc_cost_map = f.ComputeExtendedCostMap(true);
            parent.UpdateProgressBarAndHighlight(100, null);
        }

        public CostMap GetCostMap(byte life, sbyte invul, bool hasBonus)
        {
            if (cost_map == null || nc_cost_map == null || life < 1)
                return null;

            int ri = CostMap.GetRealInvul(life, invul);
            ExtendedCostMap cm = ri >= 0 && ri < Settings.enter_wall_minimum_invul ? nc_cost_map : cost_map;
            return cm.Get(hasBonus);
        }

        // /!\ For efficiency reason, we use a class instead of a struct.
        // It avoid useless copies and allows us to modify some values without accessing again the dictionnary.
        class StateData
        {
            public StateData(HelirinState es, float w, float c, HelirinState ps, bool at)
            {
                exact_state = es;
                weight = w;
                cost = c;
                previous_state = ps;
                already_treated = at;
            }
            public HelirinState exact_state;
            public float weight;
            public float cost;
            public HelirinState previous_state;
            public bool already_treated;
        }

        HelirinState NormaliseState (HelirinState st)
        {
            st = st.ShallowCopy();
            short px = Physics.pos_to_px(st.xpos);
            short py = Physics.pos_to_px(st.ypos);

            int pos_reduction = Settings.pos_reduction;
            int bump_reduction = Settings.bump_reduction;
            ushort rot_precision = Settings.rot_precision;
            ushort rot_rate_precision = Settings.rot_rate_precision;
            ushort frame_nb_precision = Settings.frame_nb_precision;
            float additional_reduction_dist_multiplier = Settings.additional_reduction_dist_multiplier;
            int max_additional_reduction = Settings.max_additional_reduction;
            if (f.IsHealZone(px, py))
            {
                pos_reduction = Settings.healzone_pos_reduction;
                bump_reduction = Settings.healzone_bump_reduction;
                rot_precision = Settings.healzone_rot_precision;
                rot_rate_precision = Settings.healzone_rot_rate_precision;
                frame_nb_precision = Settings.healzone_frame_nb_precision;
                additional_reduction_dist_multiplier = Settings.healzone_additional_reduction_dist_multiplier;
                max_additional_reduction = Settings.healzone_max_additional_reduction;
            }

            float wall_dist = f.DistToWall(px, py) * additional_reduction_dist_multiplier;
            int add_red = wall_dist == 0 ? Settings.additional_reduction_in_wall : Math.Min((int)wall_dist, max_additional_reduction);
            pos_reduction += add_red;
            bump_reduction += add_red;

            st.xpos = (st.xpos >> pos_reduction) << pos_reduction;
            st.ypos = (st.ypos >> pos_reduction) << pos_reduction;
            st.xb   = (st.xb >> bump_reduction) << bump_reduction;
            st.yb   = (st.yb >> bump_reduction) << bump_reduction;
            st.rot  = (short)((int)Math.Round((float)st.rot / rot_precision) * rot_precision);
            st.rot_rate = (short)((int)Math.Round((float)st.rot_rate / rot_rate_precision) * rot_rate_precision);

            if (!Settings.bonus_required)
                st.gs &= 0xFF ^ HelirinState.BONUS_FLAG;
            if (!Settings.reexplore_state_if_timer_started)
                st.gs &= 0xFF ^ HelirinState.TIMER_FLAG;

            st.frameNumber = (ushort)((st.frameNumber / frame_nb_precision) * frame_nb_precision);
            st.invul = (sbyte)(((st.invul-1) / Settings.invul_precision + 1) * Settings.invul_precision);

            if (!Settings.reexplore_state_if_different_last_input)
                st.lastAction = null;

            return st;
        }

        HelirinState ClearLifeDataOfState(HelirinState st)
        {
            st = st.ShallowCopy();
            st.life = 0;
            st.invul = 0;
            return st;
        }

        float GetCost(int xpos, int ypos, byte life, sbyte invul, bool hasBonus)
        {
            CostMap cm = GetCostMap(life, invul, hasBonus);
            if (cm == null)
                return float.PositiveInfinity;
            
            short xpix = Physics.pos_to_px(xpos);
            short ypix = Physics.pos_to_px(ypos);
            float cost = cm.CostAtPx(xpix, ypix, CostMap.GetRealInvul(life, invul));
            float mult_cost = cost * Settings.cost_multiplier;
            return cost > 0 && mult_cost <= 0 ? float.Epsilon : mult_cost;
        }

        bool IsOutOfSearchSpace(int xpos, int ypos)
        {
            short xpix = Physics.pos_to_px(xpos);
            short ypix = Physics.pos_to_px(ypos);
            return xpix < f.PixelStart.x || xpix > f.PixelEnd.x || ypix < f.PixelStart.y || ypix > f.PixelEnd.y;
        }

        public Action[] Solve (HelirinState init, int min_life_score)
        {
            if (cost_map == null || init == null)
                return null;
            SimplePriorityQueue<HelirinState> q = new SimplePriorityQueue<HelirinState>();
            Dictionary<HelirinState, StateData> data = new Dictionary<HelirinState, StateData>();
            Dictionary<HelirinState, int> life_data = null;
            if (!Settings.allow_state_visit_with_less_life && Settings.invul_frames >= 0 && Settings.full_life >= 2)
                life_data = new Dictionary<HelirinState, int>();

            Action[] actions = Settings.GetAllowedActions().Reverse().ToArray();

            // Init
            HelirinState norm_init = NormaliseState(init);
            float cost = GetCost(init.xpos, init.ypos, init.life, init.invul, init.HasBonus());
            q.Enqueue(norm_init, cost);
            data.Add(norm_init, new StateData(init, 0, cost, null, false));
            if (life_data != null)
                life_data.Add(ClearLifeDataOfState(norm_init), CostMap.GetRealInvul(init.life, init.invul));

            // ProgressBar and preview settings
            float init_cost = cost;
            bool[,] preview = new bool[cost_map.Get(true).Height, cost_map.Get(true).Width];
            int since_last_update = 0;

            // A*
            HelirinState result = null;
            while (q.Count > 0)
            {
                HelirinState norm_st = q.Dequeue();
                StateData st_data = data[norm_st];
                st_data.already_treated = true;
                HelirinState st = st_data.exact_state;
                // Not enough life / Out of search space ?
                int life_score = CostMap.GetRealInvul(st.life, st.invul);
                if ((life_score < min_life_score && life_score >= 0) || IsOutOfSearchSpace(st.xpos, st.ypos))
                    continue;
                // Target reached ? We look at the cost rather than the game state, because the target can be different than winning
                if (st_data.cost <= 0)
                {
                    result = norm_st;
                    break;
                }
                // Is the state terminal without having completed the objective?
                if (st.IsTerminal())
                    continue;

                // ProgressBar and preview settings
                preview[Physics.pos_to_px(st.ypos)-f.PixelStart.y, Physics.pos_to_px(st.xpos)-f.PixelStart.x] = true;
                since_last_update++;
                if (since_last_update >= Settings.nb_iterations_before_ui_update)
                {
                    since_last_update = 0;
                    parent.UpdateProgressBarAndHighlight(100 - st_data.cost * 100 / init_cost, preview);
                }

                foreach (Action a in actions)
                {
                    float weight = st_data.weight + Settings.frame_weight;
                    if (a != st.lastAction)
                    {
                        weight += Settings.input_change_weight;
                        if (!st.TimerStarted())
                            weight += Settings.input_change_notimer_additional_weight;
                    }
                    HelirinState nst = p.Next(st, a);
                    HelirinState norm_nst = NormaliseState(nst);

                    // Already enqueued with more life ?
                    HelirinState cleared_nst = null;
                    int nlife_score = 0;
                    if (life_data!= null)
                    {
                        nlife_score = CostMap.GetRealInvul(nst.life, nst.invul);
                        cleared_nst = ClearLifeDataOfState(norm_nst);
                        int old_life_score;
                        life_data.TryGetValue(cleared_nst, out old_life_score); // Default value for 'old_life_score' (type int) is 0.
                        if (old_life_score > nlife_score)
                            continue;
                    }

                    // Already visited ?
                    // If the state was already visited, we should not add it to the queue again! Otherwise it could overwrite the state entry and corrupt some paths.
                    StateData old;
                    data.TryGetValue(norm_nst, out old); // Default value for 'old' (type StateData) is null.
                    if (old != null && old.already_treated)
                        continue;

                    // Keep only if it is a non-infinite better cost
                    cost = GetCost(nst.xpos, nst.ypos, nst.life, nst.invul, nst.HasBonus());
                    if (cost >= float.PositiveInfinity)
                        continue;
                    float queue_weight = cost + weight;
                    if (old != null && queue_weight >= old.cost + old.weight)
                        continue;

                    // Everything's okay, we add the config to the data and queue
                    StateData nst_data = new StateData(nst, weight, cost, norm_st, false);
                    data[norm_nst] = nst_data;
                    if (life_data != null)
                        life_data[cleared_nst] = nlife_score;

                    // We don't use UpdatePriority because it does not change the InsertionIndex (first-in, first-out)
                    if (old != null)
                        q.Remove(norm_nst);
                    q.Enqueue(norm_nst, queue_weight);
                    /*
                    if (old == null)
                        q.Enqueue(norm_nst, total_cost);
                    else
                        q.UpdatePriority(norm_nst, total_cost);
                    */
                }
            }

            // Retrieve full path
            if (result == null)
                return null;
            List<Action> res = new List<Action>();
            while (result != null)
            {
                StateData sd = data[result];
                if (sd.previous_state != null)
                    res.Add(sd.exact_state.lastAction.Value);
                result = sd.previous_state;
            }
            res.Reverse();
            return res.ToArray();
        }
    }
}
