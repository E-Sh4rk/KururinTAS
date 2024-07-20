using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IniParser;
using IniParser.Model;

namespace KuruBot
{
    public static class Settings
    {
        // ---------- GAME SETTINGS ----------

        public static byte full_life = 3;
        public static sbyte invul_frames = 20; // < 0 for infinite
        public static bool bonus_required = false;
        public static bool enable_moving_objects = false;
        public static bool easy_mode = false;

        // ---------- FLOODING SETTINGS ----------

        // All these values must be non-negative
        public static float ground_speed = 3;
        public static float enter_healzone_cost = 0;
        public static float back_before_bonus_malus = 0;
        public static int enter_wall_minimum_invul = 1;
        public static float enter_wall_cost = 10;
        public static float wall_speed = ground_speed; // Should be equal to ground speed (we can't benefit from wall speed for ever, so a constant bonus is more adapted).
        public static float wall_bonus_per_invul = 1 /*(7 - ground_speed) / ground_speed*/; // Bonus applied to each pixel in a wall (in a post procedure) in order to encourage wall exploration. Unit: cost/frame.
        public static float wall_bonus_required_cost = 2 /*7 / ground_speed*/; // Min cost required for a wall to benefit from full bonus. Must be greater than wall_bonus_per_invul. Unit: dist/frame.
        public static bool target_is_oob = false; // Target will be OOB zones instead of ending.
        public static float target_oob_dist = 16;

        // ---------- SOLVER SETTINGS ----------

        public static int pos_reduction = 16 - 6; // 0x10000 / 64 : 1/64 px
        public static int bump_reduction = 16 - 6; // 0x10000 / 64 : 1/64 px/frame
        public static int healzone_pos_reduction = 16 - 6;
        public static int healzone_bump_reduction = 16 - 6;
        public static int additional_reduction_in_wall = 6; // 64
        public static float additional_reduction_dist_multiplier = 1f / 4f; // 1/4 shift/pixel
        public static int max_additional_reduction = 6; // 64
        public static float healzone_additional_reduction_dist_multiplier = 1f / 4f;
        public static int healzone_max_additional_reduction = 6;

        public static ushort rot_precision = (ushort)Physics.default_srate;
        public static ushort rot_rate_precision = (ushort)Physics.default_srate;
        public static ushort healzone_rot_precision = (ushort)Physics.default_srate;
        public static ushort healzone_rot_rate_precision = (ushort)Physics.default_srate;
        public static ushort frame_nb_precision = 0xFFFF;
        public static ushort healzone_frame_nb_precision = 0xFFFF;
        public static ushort invul_precision = 1;

        public static float cost_multiplier = 1;
        public static string allowed_moves = "NUDLRUUDDUDLRUUDDUDLRUUDD";
        public static bool allow_state_visit_with_less_life = false;
        public static int nb_iterations_before_ui_update = 25000;

        public static float input_change_weight = 0;
        public static float input_change_notimer_additional_weight = 0; // Additional weight of an input change if no timer is active
        public static float frame_weight = 1;
        public static bool reexplore_state_if_different_last_input = false;
        public static bool reexplore_state_if_timer_started = false;

        // ---------- ACCESSORS ----------

        public static Action[] GetAllowedActions()
        {
            if (allowed_moves.Length != Enum.GetValues(typeof(Action)).Length) return new Action[0];
            List<Action> actions = new List<Action>();
            foreach (Action a in Enum.GetValues(typeof(Action)))
            {
                if (allowed_moves[(int)a] != '.') actions.Add(a);
            }
            return actions.ToArray();
        }

        // ---------- SAVE/LOAD METHODS ----------

        static float parseFloat(IniData data, string cat, string name, float def)
        {
            try { return float.Parse(data[cat][name], System.Globalization.CultureInfo.InvariantCulture); }
            catch { }
            return def;
        }
        static int parseInt(IniData data, string cat, string name, int def)
        {
            try { return int.Parse(data[cat][name], System.Globalization.CultureInfo.InvariantCulture); }
            catch { }
            return def;
        }
        static bool parseBool(IniData data, string cat, string name, bool def)
        {
            try { return bool.Parse(data[cat][name]); }
            catch { }
            return def;
        }
        static string parseString(IniData data, string cat, string name, string def)
        {
            return data[cat][name] == null ? def : data[cat][name];
        }

        public static void LoadConfig(string filename)
        {
            try
            {
                FileIniDataParser parser = new FileIniDataParser();
                IniData data = parser.ReadFile(filename);

                full_life = (byte)parseInt(data, "Game", "full_life", full_life);
                invul_frames = (sbyte)parseInt(data, "Game", "invul_frames", invul_frames);
                bonus_required = parseBool(data, "Game", "bonus_required", bonus_required);
                enable_moving_objects = parseBool(data, "Game", "enable_moving_objects", enable_moving_objects);
                easy_mode = parseBool(data, "Game", "easy_mode", easy_mode);

                ground_speed = parseFloat(data,"Flooding","ground_speed", ground_speed);
                enter_healzone_cost = parseFloat(data, "Flooding", "enter_healzone_cost", enter_healzone_cost);
                back_before_bonus_malus = parseFloat(data, "Flooding", "back_before_bonus_malus", back_before_bonus_malus);
                enter_wall_minimum_invul = parseInt(data, "Flooding", "enter_wall_minimum_invul", enter_wall_minimum_invul);
                enter_wall_cost = parseFloat(data, "Flooding", "enter_wall_cost", enter_wall_cost);
                wall_speed = parseFloat(data, "Flooding", "wall_speed", wall_speed);
                wall_bonus_per_invul = parseFloat(data, "Flooding", "wall_bonus_per_invul", wall_bonus_per_invul);
                wall_bonus_required_cost = parseFloat(data, "Flooding", "wall_bonus_required_cost", wall_bonus_required_cost);
                target_is_oob = parseBool(data, "Flooding", "target_is_oob", target_is_oob);
                target_oob_dist = parseFloat(data, "Flooding", "target_oob_dist", target_oob_dist);

                pos_reduction = parseInt(data, "Solver", "pos_reduction", pos_reduction);
                bump_reduction = parseInt(data, "Solver", "bump_reduction", bump_reduction);
                healzone_pos_reduction = parseInt(data, "Solver", "healzone_pos_reduction", healzone_pos_reduction);
                healzone_bump_reduction = parseInt(data, "Solver", "healzone_bump_reduction", healzone_bump_reduction);
                additional_reduction_in_wall = parseInt(data, "Solver", "additional_reduction_in_wall", additional_reduction_in_wall);
                additional_reduction_dist_multiplier = parseFloat(data, "Solver", "additional_reduction_dist_multiplier", additional_reduction_dist_multiplier);
                max_additional_reduction = parseInt(data, "Solver", "max_additional_reduction", max_additional_reduction);
                healzone_additional_reduction_dist_multiplier = parseFloat(data, "Solver", "healzone_additional_reduction_dist_multiplier", healzone_additional_reduction_dist_multiplier);
                healzone_max_additional_reduction = parseInt(data, "Solver", "healzone_max_additional_reduction", healzone_max_additional_reduction);
                rot_precision = (ushort)parseInt(data, "Solver", "rot_precision", rot_precision);
                rot_rate_precision = (ushort)parseInt(data, "Solver", "rot_rate_precision", rot_rate_precision);
                healzone_rot_precision = (ushort)parseInt(data, "Solver", "healzone_rot_precision", healzone_rot_precision);
                healzone_rot_rate_precision = (ushort)parseInt(data, "Solver", "healzone_rot_rate_precision", healzone_rot_rate_precision);
                frame_nb_precision = (ushort)parseInt(data, "Solver", "frame_nb_precision", frame_nb_precision);
                healzone_frame_nb_precision = (ushort)parseInt(data, "Solver", "healzone_frame_nb_precision", healzone_frame_nb_precision);
                invul_precision = (ushort)parseInt(data, "Solver", "invul_precision", invul_precision);
                cost_multiplier = parseFloat(data, "Solver", "cost_multiplier", cost_multiplier);
                allowed_moves = parseString(data, "Solver", "allowed_moves", allowed_moves);
                allow_state_visit_with_less_life = parseBool(data, "Solver", "allow_state_visit_with_less_life", allow_state_visit_with_less_life);
                nb_iterations_before_ui_update = parseInt(data, "Solver", "nb_iterations_before_ui_update", nb_iterations_before_ui_update);
                input_change_weight = parseFloat(data, "Solver", "input_change_weight", input_change_weight);
                input_change_notimer_additional_weight = parseFloat(data, "Solver", "input_change_notimer_additional_weight", input_change_notimer_additional_weight);
                frame_weight = parseFloat(data, "Solver", "frame_weight", frame_weight);
                reexplore_state_if_different_last_input = parseBool(data, "Solver", "reexplore_state_if_different_last_input", reexplore_state_if_different_last_input);
                reexplore_state_if_timer_started = parseBool(data, "Solver", "reexplore_state_if_timer_started", reexplore_state_if_timer_started);
            }
            catch { }
        }
        static string ToString(float o)
        {
            return o.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }
        static string ToString(int o)
        {
            return o.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }
        static string ToString(string o)
        {
            return o;
        }
        static string ToString(object o)
        {
            return o.ToString();
        }
        public static void SaveConfig(string filename)
        {
            try
            {
                SectionDataCollection sections = new SectionDataCollection();
                sections.AddSection("Game");
                sections.AddSection("Flooding");
                sections.AddSection("Solver");
                IniData data = new IniData(sections);

                data["Game"]["full_life"] = ToString(full_life);
                data["Game"]["invul_frames"] = ToString(invul_frames);
                data["Game"]["bonus_required"] = ToString(bonus_required);
                data["Game"]["enable_moving_objects"] = ToString(enable_moving_objects);
                data["Game"]["easy_mode"] = ToString(easy_mode);

                data["Flooding"]["ground_speed"] = ToString(ground_speed);
                data["Flooding"]["enter_healzone_cost"] = ToString(enter_healzone_cost);
                data["Flooding"]["back_before_bonus_malus"] = ToString(back_before_bonus_malus);
                data["Flooding"]["enter_wall_minimum_invul"] = ToString(enter_wall_minimum_invul);
                data["Flooding"]["enter_wall_cost"] = ToString(enter_wall_cost);
                data["Flooding"]["wall_speed"] = ToString(wall_speed);
                data["Flooding"]["wall_bonus_per_invul"] = ToString(wall_bonus_per_invul);
                data["Flooding"]["wall_bonus_required_cost"] = ToString(wall_bonus_required_cost);
                data["Flooding"]["target_is_oob"] = ToString(target_is_oob);
                data["Flooding"]["target_oob_dist"] = ToString(target_oob_dist);

                data["Solver"]["pos_reduction"] = ToString(pos_reduction);
                data["Solver"]["bump_reduction"] = ToString(bump_reduction);
                data["Solver"]["healzone_pos_reduction"] = ToString(healzone_pos_reduction);
                data["Solver"]["healzone_bump_reduction"] = ToString(healzone_bump_reduction);
                data["Solver"]["additional_reduction_in_wall"] = ToString(additional_reduction_in_wall);
                data["Solver"]["additional_reduction_dist_multiplier"] = ToString(additional_reduction_dist_multiplier);
                data["Solver"]["max_additional_reduction"] = ToString(max_additional_reduction);
                data["Solver"]["healzone_additional_reduction_dist_multiplier"] = ToString(healzone_additional_reduction_dist_multiplier);
                data["Solver"]["healzone_max_additional_reduction"] = ToString(healzone_max_additional_reduction);
                data["Solver"]["rot_precision"] = ToString(rot_precision);
                data["Solver"]["rot_rate_precision"] = ToString(rot_rate_precision);
                data["Solver"]["healzone_rot_precision"] = ToString(healzone_rot_precision);
                data["Solver"]["healzone_rot_rate_precision"] = ToString(healzone_rot_rate_precision);
                data["Solver"]["frame_nb_precision"] = ToString(frame_nb_precision);
                data["Solver"]["healzone_frame_nb_precision"] = ToString(healzone_frame_nb_precision);
                data["Solver"]["invul_precision"] = ToString(invul_precision);
                data["Solver"]["cost_multiplier"] = ToString(cost_multiplier);
                data["Solver"]["allowed_moves"] = ToString(allowed_moves);
                data["Solver"]["allow_state_visit_with_less_life"] = ToString(allow_state_visit_with_less_life);
                data["Solver"]["nb_iterations_before_ui_update"] = ToString(nb_iterations_before_ui_update);
                data["Solver"]["input_change_weight"] = ToString(input_change_weight);
                data["Solver"]["input_change_notimer_additional_weight"] = ToString(input_change_notimer_additional_weight);
                data["Solver"]["frame_weight"] = ToString(frame_weight);
                data["Solver"]["reexplore_state_if_different_last_input"] = ToString(reexplore_state_if_different_last_input);
                data["Solver"]["reexplore_state_if_timer_started"] = ToString(reexplore_state_if_timer_started);

                FileIniDataParser parser = new FileIniDataParser();
                parser.WriteFile(filename, data);
            }
            catch { }
        }
    }
}
