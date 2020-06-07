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
        public static sbyte invul_frames = 20;
        public static bool bonus_required = false;
        public static bool enable_moving_objects = false;

        // ---------- FLOODING SETTINGS ----------

        // All these values must be non-negative
        public static float ground_speed = 3;
        public static float damageless_healzone_bonus = 0;
        public static float damageless_back_before_healzone_malus = 0;
        public static float back_before_bonus_malus = 0;
        public static bool allow_wall_clip = true;
        public static float wall_speed = ground_speed; // Should be equal to ground speed (we can't benefit from wall speed for ever, so a constant bonus is more adapted).
        public static float wall_bonus_per_invul = 1 /*(7 - ground_speed) / ground_speed*/; // Bonus applied to each pixel in a wall (in a post procedure) in order to encourage wall exploration. Unit: weight/frame.
        public static float wall_bonus_required_cost = 2.5f /*7 / ground_speed*/; // Min weight required for a wall to benefit from full bonus. Must be greater than wall_bonus_per_invul. Unit: dist/frame.
        public static float wall_clip_malus = 16; // Malus applied everytime we leave a wall clip, in order to capture the fact that doing the other way could be expensive.
        public static float wall_clip_malus_dist = 16; // Distance from the wall after which the full wall-ground malus is applied. It is applied linearly.
        public static bool restrict_complete_wall_clip_when_one_heart = true; // If set to true, can generate some infinite cost in narrow places.
        public static byte nb_additional_cost_maps = 1;
        public static float complete_wall_clip_max_dist = 24; // Distance of a wall at which the the helirin is not considered performing a wall clip.
        public static int complete_wall_clip_duration = 9; // Invulnerability time needed to benefit from full complete_wall_clip_max_dist.
        public static bool cwc_max_dist_zero_in_legal_zone = true; // Avoid infinite cost in narrow places in the legal zone.

        // ---------- SOLVER SETTINGS ----------

        public static int pos_reduction = 16 - 6; // 0x10000 / 64 : 1/64 px
        public static int bump_reduction = 16 - 6; // 0x10000 / 64 : 1/64 px/frame
        public static int healzone_pos_reduction = 16 - 6;
        public static int healzone_bump_reduction = 16 - 6;
        public static int additional_reduction_in_wall = 6; // 64
        public static float additional_reduction_dist_multiplier = 1f / 8f; // 1/8 shift/pixel
        public static int max_additional_reduction = 6; // 64
        public static float healzone_additional_reduction_dist_multiplier = 1f / 8f;
        public static int healzone_max_additional_reduction = 6;

        public static ushort rot_precision = (ushort)Physics.default_srate;
        public static ushort rot_rate_precision = (ushort)Physics.default_srate;
        public static ushort healzone_rot_precision = (ushort)Physics.default_srate;
        public static ushort healzone_rot_rate_precision = (ushort)Physics.default_srate;
        public static ushort frame_nb_precision = 0xFFFF;
        public static ushort healzone_frame_nb_precision = 0xFFFF;
        // TODO: state reduction for invulnerability?

        public static float cost_multiplier = 1;
        public static int min_ab_speed = 0;
        public static bool allow_state_visit_with_less_life = false;
        public static int nb_iterations_before_ui_update = 25000;

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

                ground_speed = parseFloat(data,"Flooding","ground_speed", ground_speed);
                damageless_healzone_bonus = parseFloat(data, "Flooding", "damageless_healzone_bonus", damageless_healzone_bonus);
                damageless_back_before_healzone_malus = parseFloat(data, "Flooding", "damageless_back_before_healzone_malus", damageless_back_before_healzone_malus);
                back_before_bonus_malus = parseFloat(data, "Flooding", "back_before_bonus_malus", back_before_bonus_malus);
                allow_wall_clip = parseBool(data, "Flooding", "allow_wall_clip", allow_wall_clip);
                wall_speed = parseFloat(data, "Flooding", "wall_speed", wall_speed);
                wall_bonus_per_invul = parseFloat(data, "Flooding", "wall_bonus_per_invul", wall_bonus_per_invul);
                wall_bonus_required_cost = parseFloat(data, "Flooding", "wall_bonus_required_cost", wall_bonus_required_cost);
                wall_clip_malus = parseFloat(data, "Flooding", "wall_clip_malus", wall_clip_malus);
                wall_clip_malus_dist = parseFloat(data, "Flooding", "wall_clip_malus_dist", wall_clip_malus_dist);
                restrict_complete_wall_clip_when_one_heart = parseBool(data, "Flooding", "restrict_complete_wall_clip_when_one_heart", restrict_complete_wall_clip_when_one_heart);
                nb_additional_cost_maps = (byte)parseInt(data, "Flooding", "nb_additional_cost_maps", nb_additional_cost_maps);
                complete_wall_clip_max_dist = parseFloat(data, "Flooding", "complete_wall_clip_max_dist", complete_wall_clip_max_dist);
                complete_wall_clip_duration = parseInt(data, "Flooding", "complete_wall_clip_duration", complete_wall_clip_duration);
                cwc_max_dist_zero_in_legal_zone = parseBool(data, "Flooding", "cwc_max_dist_zero_in_legal_zone", cwc_max_dist_zero_in_legal_zone);

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
                cost_multiplier = parseFloat(data, "Solver", "cost_multiplier", cost_multiplier);
                min_ab_speed = parseInt(data, "Solver", "min_ab_speed", min_ab_speed);
                allow_state_visit_with_less_life = parseBool(data, "Solver", "allow_state_visit_with_less_life", allow_state_visit_with_less_life);
                nb_iterations_before_ui_update = parseInt(data, "Solver", "nb_iterations_before_ui_update", nb_iterations_before_ui_update);
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

                data["Flooding"]["ground_speed"] = ToString(ground_speed);
                data["Flooding"]["damageless_healzone_bonus"] = ToString(damageless_healzone_bonus);
                data["Flooding"]["damageless_back_before_healzone_malus"] = ToString(damageless_back_before_healzone_malus);
                data["Flooding"]["back_before_bonus_malus"] = ToString(back_before_bonus_malus);
                data["Flooding"]["allow_wall_clip"] = ToString(allow_wall_clip);
                data["Flooding"]["wall_speed"] = ToString(wall_speed);
                data["Flooding"]["wall_bonus_per_invul"] = ToString(wall_bonus_per_invul);
                data["Flooding"]["wall_bonus_required_cost"] = ToString(wall_bonus_required_cost);
                data["Flooding"]["wall_clip_malus"] = ToString(wall_clip_malus);
                data["Flooding"]["wall_clip_malus_dist"] = ToString(wall_clip_malus_dist);
                data["Flooding"]["restrict_complete_wall_clip_when_one_heart"] = ToString(restrict_complete_wall_clip_when_one_heart);
                data["Flooding"]["nb_additional_cost_maps"] = ToString(nb_additional_cost_maps);
                data["Flooding"]["complete_wall_clip_max_dist"] = ToString(complete_wall_clip_max_dist);
                data["Flooding"]["complete_wall_clip_duration"] = ToString(complete_wall_clip_duration);
                data["Flooding"]["cwc_max_dist_zero_in_legal_zone"] = ToString(cwc_max_dist_zero_in_legal_zone);

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
                data["Solver"]["cost_multiplier"] = ToString(cost_multiplier);
                data["Solver"]["min_ab_speed"] = ToString(min_ab_speed);
                data["Solver"]["allow_state_visit_with_less_life"] = ToString(allow_state_visit_with_less_life);
                data["Solver"]["nb_iterations_before_ui_update"] = ToString(nb_iterations_before_ui_update);

                FileIniDataParser parser = new FileIniDataParser();
                parser.WriteFile(filename, data);
            }
            catch { }
        }
    }
}
