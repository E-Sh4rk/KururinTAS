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
        float[][][,] cost_maps = null;

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

        public void ComputeNewCostMaps(bool no_wall_clip)
        {
            if (no_wall_clip)
            {
                cost_maps = new float[1][][,];
                cost_maps[0] = new float[][,] { f.ComputeCostMap(0, Flooding.WallClipSetting.NoWallClip) };
            }
            else
            {
                int total_op = 1 + (Settings.full_life - 1) * Settings.nb_cost_maps_per_life;
                cost_maps = new float[Settings.nb_cost_maps_per_life > 0 ? Settings.full_life : 1][][,];
                cost_maps[cost_maps.Length - 1] = new float[][,] { f.ComputeCostMap(Physics.invul_frames*(Settings.full_life-1),
                    (Settings.full_life-1) > 0 || Settings.allow_complete_wall_clip_with_one_heart ?
                    Flooding.WallClipSetting.Allow : Flooding.WallClipSetting.NoCompleteWallClip) };
                parent.UpdateProgressBarAndHighlight(100 / total_op, null);

                if (Settings.nb_cost_maps_per_life > 0)
                {
                    int current_op = 1;
                    for (int i = 0; i < Settings.full_life - 1; i++)
                    {
                        float[][,] current_cm = new float[Settings.nb_cost_maps_per_life][,];
                        int min_invul = i * Physics.invul_frames;
                        for (int j = 0; j < current_cm.Length; j++)
                        {
                            int current_invul = min_invul + j * Physics.invul_frames / current_cm.Length;
                            current_cm[j] = f.ComputeCostMap(current_invul, i > 0 || Settings.allow_complete_wall_clip_with_one_heart ?
                                                             Flooding.WallClipSetting.Allow : Flooding.WallClipSetting.NoCompleteWallClip);
                            current_op++;
                            parent.UpdateProgressBarAndHighlight(100 * current_op / total_op, null);
                        }
                        cost_maps[i] = current_cm;
                    }
                }

            }
        }

        public float[,] GetPreviewCostMap(int life, int nb)
        {
            if (cost_maps == null || life < 1 || nb < 1)
                return null;
            
            float[][,] cm = cost_maps[Math.Min(cost_maps.Length - 1, life - 1)];
            return cm[Math.Min(cm.Length - 1, nb - 1)];
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

            float wall_dist = f.DistToWall(Physics.pos_to_px(st.xpos), Physics.pos_to_px(st.ypos)) * Settings.additional_reduction_dist_multiplier;
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

        HelirinState ClearLifeDataOfState(HelirinState st)
        {
            st = st.ShallowCopy();
            st.life = 0;
            st.invul = 0;
            return st;
        }
        int GetLifeScore(HelirinState st)
        {
            return (st.life-1) * (Physics.invul_frames + 1) + st.invul;
        }

        float GetCost(int xpos, int ypos, byte life, sbyte invul)
        {
            if (life == 0)
                return float.PositiveInfinity;
            float[][,] cm1 = cost_maps[Math.Min(cost_maps.Length-1,life-1)];
            int invul_index = (invul < 0 ? Physics.invul_frames : invul) * cm1.Length / Physics.invul_frames;
            float[,] cm2 = cm1[Math.Min(cm1.Length-1,invul_index)];
            short xpix = Physics.pos_to_px(xpos);
            short ypix = Physics.pos_to_px(ypos);
            float cost = f.Cost(cm2, xpix, ypix);
            float mult_cost = cost * Settings.cost_multiplier;
            return cost > 0 && mult_cost <= 0 ? float.Epsilon : mult_cost;
        }

        bool IsOutOfSearchSpace(int xpos, int ypos)
        {
            short xpix = Physics.pos_to_px(xpos);
            short ypix = Physics.pos_to_px(ypos);
            return xpix < f.PixelStart.x || xpix > f.PixelEnd.x || ypix < f.PixelStart.y || ypix > f.PixelEnd.y;
        }

        public Action[] Solve (HelirinState init)
        {
            if (cost_maps == null || init == null)
                return null;
            SimplePriorityQueue<HelirinState> q = new SimplePriorityQueue<HelirinState>();
            Dictionary<HelirinState, StateData> data = new Dictionary<HelirinState, StateData>();
            Dictionary<HelirinState, int> life_data = null;
            if (!Settings.allow_state_visit_with_less_life && init.invul >= 0)
                life_data = new Dictionary<HelirinState, int>();

            // Init
            HelirinState norm_init = NormaliseState(init);
            float cost = GetCost(init.xpos, init.ypos, init.life, init.invul);
            float weight = 0;
            q.Enqueue(norm_init, cost);
            data.Add(norm_init, new StateData(init, weight, cost, null, null, false));
            if (life_data != null)
                life_data.Add(ClearLifeDataOfState(norm_init), GetLifeScore(init));

            // ProgressBar and preview settings
            float init_cost = cost;
            bool[,] preview = new bool[GetPreviewCostMap(1,1).GetLength(0), GetPreviewCostMap(1,1).GetLength(1)];
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

                for (int i = 0; i < 25; i++)
                {
                    Action a = (Action)i;
                    HelirinState nst = p.Next(st_data.exact_state, a);
                    HelirinState norm_nst = NormaliseState(nst);

                    // Out of search space / Loose ?
                    if (nst.gs == GameState.Loose || IsOutOfSearchSpace(nst.xpos, nst.ypos))
                        continue;

                    // Already enqueued with more life ?
                    int life_score = -1;
                    HelirinState cleared_nst = null;
                    if (life_data!= null)
                    {
                        life_score = GetLifeScore(nst);
                        cleared_nst = ClearLifeDataOfState(norm_nst);
                        int old_life_score = -1;
                        life_data.TryGetValue(cleared_nst, out old_life_score);
                        if (old_life_score >= life_score)
                            continue;
                    }

                    // Already visited ?
                    StateData old = null;
                    data.TryGetValue(norm_nst, out old);
                    if (!Settings.allow_state_multiple_visits && old != null && old.already_treated)
                        continue;

                    // Better cost ?
                    cost = GetCost(nst.xpos, nst.ypos, nst.life, nst.invul);
                    float total_cost = cost + weight;
                    if (old == null || total_cost < old.cost + old.weight)
                    {
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
