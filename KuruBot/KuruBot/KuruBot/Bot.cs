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
        ExtendedCostMap[][] cost_maps = null;

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
            ExtendedCostMap full_life_cost_map;
            if (!Settings.allow_wall_clip)
                full_life_cost_map = f.ComputeExtendedCostMap(Flooding.WallClipSetting.NoWallClip, 0);
            else if (Settings.full_life <= 1 && Settings.restrict_complete_wall_clip_when_one_heart)
                full_life_cost_map = f.ComputeExtendedCostMap(Flooding.WallClipSetting.NoCompleteWallClip, 0);
            else
                full_life_cost_map = f.ComputeExtendedCostMap(Flooding.WallClipSetting.Allow, 0);

            if (!Settings.allow_wall_clip || !Settings.restrict_complete_wall_clip_when_one_heart || Settings.full_life <= 1 || Settings.invul_frames < 0)
            {
                cost_maps = new ExtendedCostMap[1][];
                cost_maps[0] = new ExtendedCostMap[] { full_life_cost_map };
            }
            else
            {
                int total_op = 2 + Settings.nb_additional_cost_maps;
                parent.UpdateProgressBarAndHighlight(100 / total_op, null);

                cost_maps = new ExtendedCostMap[2][];
                cost_maps[1] = new ExtendedCostMap[] { full_life_cost_map };
                
                int current_op = 1;
                ExtendedCostMap[] cms = new ExtendedCostMap[Settings.nb_additional_cost_maps + 1];
                for (int i = 0; i < cms.Length; i++)
                {
                    int invul = i * (Settings.invul_frames+1) / cms.Length;
                    cms[i] = f.ComputeExtendedCostMap(Flooding.WallClipSetting.NoCompleteWallClip, Flooding.GetRealInvul(1,(sbyte)invul));
                    current_op++;
                    parent.UpdateProgressBarAndHighlight(100 * current_op / total_op, null);
                }
                cost_maps[0] = cms;
            }
        }

        public CostMap GetCostMap(byte life, sbyte invul, bool hasBonus)
        {
            if (cost_maps == null || life < 1)
                return null;

            ExtendedCostMap[] cms = cost_maps[Math.Min(cost_maps.Length - 1, life - 1)];
            if (Settings.invul_frames < 0)
                return cms[cms.Length - 1].Get(hasBonus);
            int invul_index = Math.Max(0, (int)invul) * cms.Length / (Settings.invul_frames+1);
            return cms[Math.Min(cms.Length - 1, invul_index)].Get(hasBonus);
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

            if (!Settings.bonus_required && !st.IsTerminal())
                st.gs = GameState.InGame;

            st.frameNumber = (ushort)((st.frameNumber / frame_nb_precision) * frame_nb_precision);
            st.invul = (sbyte)(((st.invul-1) / Settings.invul_precision + 1) * Settings.invul_precision);

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
            float cost = cm.CostAtPx(xpix, ypix, Flooding.GetRealInvul(life, invul));
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
            if (cost_maps == null || init == null)
                return null;
            SimplePriorityQueue<HelirinState> q = new SimplePriorityQueue<HelirinState>();
            Dictionary<HelirinState, StateData> data = new Dictionary<HelirinState, StateData>();
            Dictionary<HelirinState, int> life_data = null;
            if (!Settings.allow_state_visit_with_less_life && Settings.invul_frames >= 0 && Settings.full_life >= 2)
                life_data = new Dictionary<HelirinState, int>();

            // Set range of possible inputs
            int min_input = 0;
            if (Settings.min_ab_speed == 1)
                min_input = 1;
            else if (Settings.min_ab_speed == 2)
                min_input = 9;
            else if (Settings.min_ab_speed == 3)
                min_input = 17;
            else if (Settings.min_ab_speed > 3)
                min_input = 25;

            // Init
            HelirinState norm_init = NormaliseState(init);
            float cost = GetCost(init.xpos, init.ypos, init.life, init.invul, init.HasBonus());
            float weight = 0;
            float total_cost = cost + weight;
            q.Enqueue(norm_init, total_cost);
            data.Add(norm_init, new StateData(init, weight, cost, null, null, false));
            if (life_data != null)
                life_data.Add(ClearLifeDataOfState(norm_init), Flooding.GetRealInvul(init.life, init.invul));

            // ProgressBar and preview settings
            float init_cost = cost;
            bool[,] preview = new bool[cost_maps[0][0].Get(true).Height, cost_maps[0][0].Get(true).Width];
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
                if (since_last_update >= Settings.nb_iterations_before_ui_update)
                {
                    since_last_update = 0;
                    parent.UpdateProgressBarAndHighlight(100 - st_data.cost * 100 / init_cost, preview);
                }

                for (int i = 24; i >= min_input; i--)
                {
                    Action a = (Action)i;
                    HelirinState nst = p.Next(st_data.exact_state, a);
                    HelirinState norm_nst = NormaliseState(nst);

                    // Lose / Not enough life / Out of search space ?
                    int life_score = Flooding.GetRealInvul(nst.life, nst.invul);
                    if (nst.gs == GameState.Lose || (life_score < min_life_score && life_score >= 0) || IsOutOfSearchSpace(nst.xpos, nst.ypos))
                        continue;

                    // Already enqueued with more life ?
                    HelirinState cleared_nst = null;
                    if (life_data!= null)
                    {
                        cleared_nst = ClearLifeDataOfState(norm_nst);
                        int old_life_score;
                        life_data.TryGetValue(cleared_nst, out old_life_score); // Default value for 'old_life_score' (type int) is 0.
                        if (old_life_score > life_score)
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
                    total_cost = cost + weight;
                    if (old != null && total_cost >= old.cost + old.weight)
                        continue;

                    // Is the state terminal without having completed the objective?
                    if (cost > 0 && nst.IsTerminal())
                        continue;

                    // Everything's okay, we add the config to the data and queue

                    StateData nst_data = new StateData(nst, weight, cost, a, norm_st, false);
                    data[norm_nst] = nst_data;
                    if (life_data != null)
                        life_data[cleared_nst] = life_score;

                    // Target reached ? We look at the cost rather than the game state, because the target can be different than winning
                    if (cost <= 0)
                    {
                        result = norm_nst;
                        break;
                    }

                    // We don't use UpdatePriority because it does not change the InsertionIndex (first-in, first-out)
                    if (old != null)
                        q.Remove(norm_nst);
                    q.Enqueue(norm_nst, total_cost);
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
                if (sd.action.HasValue)
                    res.Add(sd.action.Value);
                result = sd.previous_state;
            }
            res.Reverse();
            return res.ToArray();
        }
    }
}
