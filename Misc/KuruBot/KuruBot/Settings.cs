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
        // ---------- FLOODING SETTINGS ----------

        // All these values must be non-negative
        public static float ground_speed = 3;
        public static float wall_speed = ground_speed; // Should be equal to ground speed (we can't benefit from wall speed for ever, so a constant bonus is more adapted).
        public static float ground_wall_bonus = 7 - 2 - ground_speed; // Bonus applied to each pixel in a wall (in a post procedure) in order to encourage wall exploration. Unit: weight/frame.
        public static float ground_wall_bonus_min_dist = 7 - 2; // Min weight required for a wall to benefit from full bonus. Must be greater than ground_wall_bonus. Unit: dist/frame.
        public static float wall_ground_malus = ground_speed * 20; // Malus applied everytime we leave a wall clip, in order to capture the fact that doing the other way could be expensive.
        public static float wall_clip_end_dist = 4; // Distance from the wall at which the helirin has no control anymore.

        // ---------- SOLVER SETTINGS ----------

        public static int pos_reduction = 16 - 6; // 0x10000 / 64 : 1/64 px
        public static int bump_reduction = 16 - 6; // 0x10000 / 64 : 1/64 px/frame
        public static int additional_reduction_in_wall = 6; // 64
        public static float additional_reduction_dist_multiplier = 1f / 8f; // 1/8 shift/pixel
        public static int max_additional_reduction = 6; // 64

        public static short rot_precision = Physics.default_srate;
        public static short rot_rate_precision = Physics.default_srate;

        public static float cost_multiplier = 1;
        public static bool allow_state_multiple_visits = true; // A vertex could be visited many times because the cost function is not always a lower-bound of the real distance.
        // TODO: optimisation parameter for lives system (see bot.txt)
        public static int number_iterations_before_ui_update = 10000;

        // ---------- SAVE/LOAD METHODS ----------

        static float parseFloat(IniData data, string cat, string name, float def)
        {
            try { return float.Parse(data[cat][name]); }
            catch { }
            return def;
        }
        static int parseInt(IniData data, string cat, string name, int def)
        {
            try { return int.Parse(data[cat][name]); }
            catch { }
            return def;
        }
        static bool parseBool(IniData data, string cat, string name, bool def)
        {
            try { return bool.Parse(data[cat][name]); }
            catch { }
            return def;
        }

        public static void LoadConfig(string filename)
        {
            try
            {
                FileIniDataParser parser = new FileIniDataParser();
                IniData data = parser.ReadFile(filename);

                ground_speed = parseFloat(data,"Flooding","ground_speed", ground_speed);
                wall_speed = parseFloat(data, "Flooding", "wall_speed", wall_speed);
                ground_wall_bonus = parseFloat(data, "Flooding", "ground_wall_bonus", ground_wall_bonus);
                ground_wall_bonus_min_dist = parseFloat(data, "Flooding", "ground_wall_bonus_min_dist", ground_wall_bonus_min_dist);
                wall_ground_malus = parseFloat(data, "Flooding", "wall_ground_malus", wall_ground_malus);
                wall_clip_end_dist = parseFloat(data, "Flooding", "wall_clip_end_dist", wall_clip_end_dist);

                pos_reduction = parseInt(data, "Solver", "pos_reduction", pos_reduction);
                bump_reduction = parseInt(data, "Solver", "bump_reduction", bump_reduction);
                additional_reduction_in_wall = parseInt(data, "Solver", "additional_reduction_in_wall", additional_reduction_in_wall);
                additional_reduction_dist_multiplier = parseFloat(data, "Solver", "additional_reduction_dist_multiplier", additional_reduction_dist_multiplier);
                max_additional_reduction = parseInt(data, "Solver", "max_additional_reduction", max_additional_reduction);
                rot_precision = (short)parseInt(data, "Solver", "rot_precision", rot_precision);
                rot_rate_precision = (short)parseInt(data, "Solver", "rot_rate_precision", rot_rate_precision);
                cost_multiplier = parseFloat(data, "Solver", "cost_multiplier", cost_multiplier);
                allow_state_multiple_visits = parseBool(data, "Solver", "allow_state_multiple_visits", allow_state_multiple_visits);
                number_iterations_before_ui_update = parseInt(data, "Solver", "number_iterations_before_ui_update", number_iterations_before_ui_update);
            }
            catch { }
        }
        public static void SaveConfig(string filename)
        {
            try
            {
                SectionDataCollection sections = new SectionDataCollection();
                sections.AddSection("Flooding");
                sections.AddSection("Solver");
                IniData data = new IniData(sections);

                data["Flooding"]["ground_speed"] = ground_speed.ToString();
                data["Flooding"]["wall_speed"] = wall_speed.ToString();
                data["Flooding"]["ground_wall_bonus"] = ground_wall_bonus.ToString();
                data["Flooding"]["ground_wall_bonus_min_dist"] = ground_wall_bonus_min_dist.ToString();
                data["Flooding"]["wall_ground_malus"] = wall_ground_malus.ToString();
                data["Flooding"]["wall_clip_end_dist"] = wall_clip_end_dist.ToString();

                data["Solver"]["pos_reduction"] = pos_reduction.ToString();
                data["Solver"]["bump_reduction"] = bump_reduction.ToString();
                data["Solver"]["additional_reduction_in_wall"] = additional_reduction_in_wall.ToString();
                data["Solver"]["additional_reduction_dist_multiplier"] = additional_reduction_dist_multiplier.ToString();
                data["Solver"]["max_additional_reduction"] = max_additional_reduction.ToString();
                data["Solver"]["rot_precision"] = rot_precision.ToString();
                data["Solver"]["rot_rate_precision"] = rot_rate_precision.ToString();
                data["Solver"]["cost_multiplier"] = cost_multiplier.ToString();
                data["Solver"]["allow_state_multiple_visits"] = allow_state_multiple_visits.ToString();
                data["Solver"]["number_iterations_before_ui_update"] = number_iterations_before_ui_update.ToString();

                FileIniDataParser parser = new FileIniDataParser();
                parser.WriteFile(filename, data);
            }
            catch { }
        }
    }
}
