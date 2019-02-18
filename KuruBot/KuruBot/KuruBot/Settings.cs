﻿using System;
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

        // ---------- FLOODING SETTINGS ----------

        // All these values must be non-negative
        public static float ground_speed = 3;
        public static bool allow_wall_clip = true;
        public static float wall_speed = ground_speed; // Should be equal to ground speed (we can't benefit from wall speed for ever, so a constant bonus is more adapted).
        public static float ground_wall_bonus = 7 - 2 - ground_speed; // Bonus applied to each pixel in a wall (in a post procedure) in order to encourage wall exploration. Unit: weight/frame.
        public static float ground_wall_bonus_min_cost = 7 - 2; // Min weight required for a wall to benefit from full bonus. Must be greater than ground_wall_bonus. Unit: dist/frame.
        public static float wall_ground_malus = ground_speed * 10; // Malus applied everytime we leave a wall clip, in order to capture the fact that doing the other way could be expensive.
        public static float wall_ground_malus_dist = 6; // Distance from the wall after which the full wall-ground malus is applied. It is applied linearly.
        public static bool restrict_complete_wall_clip_when_one_heart = true; // If set to true, can generate some infinite cost in narrow places.
        public static byte nb_additional_cost_maps = 1;
        public static float complete_wall_clip_max_dist = 25; // Distance of a wall at which the the helirin is not considered performing a wall clip.
        public static int complete_wall_clip_duration = 9; // Invulnerability time needed to benefit from full complete_wall_clip_max_dist.
        public static bool cwc_max_dist_zero_in_legal_zone = true; // Avoid infinite cost in narrow places in the legal zone.

        // ---------- SOLVER SETTINGS ----------

        public static int pos_reduction = 16 - 6; // 0x10000 / 64 : 1/64 px
        public static int bump_reduction = 16 - 6; // 0x10000 / 64 : 1/64 px/frame
        public static int additional_reduction_in_wall = 6; // 64
        public static float additional_reduction_dist_multiplier = 1f / 8f; // 1/8 shift/pixel
        public static int max_additional_reduction = 6; // 64

        public static short rot_precision = Physics.default_srate;
        public static short rot_rate_precision = Physics.default_srate;
        // TODO: state reduction for invulnerability?

        public static float cost_multiplier = 1;
        public static bool allow_state_visit_with_less_life = false;
        public static int nb_iterations_before_ui_update = 25000;

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

                full_life = (byte)parseInt(data, "Game", "full_life", full_life);

                ground_speed = parseFloat(data,"Flooding","ground_speed", ground_speed);
                allow_wall_clip = parseBool(data, "Flooding", "allow_wall_clip", allow_wall_clip);
                wall_speed = parseFloat(data, "Flooding", "wall_speed", wall_speed);
                ground_wall_bonus = parseFloat(data, "Flooding", "ground_wall_bonus", ground_wall_bonus);
                ground_wall_bonus_min_cost = parseFloat(data, "Flooding", "ground_wall_bonus_min_cost", ground_wall_bonus_min_cost);
                wall_ground_malus = parseFloat(data, "Flooding", "wall_ground_malus", wall_ground_malus);
                wall_ground_malus_dist = parseFloat(data, "Flooding", "wall_ground_malus_dist", wall_ground_malus_dist);
                restrict_complete_wall_clip_when_one_heart = parseBool(data, "Flooding", "restrict_complete_wall_clip_when_one_heart", restrict_complete_wall_clip_when_one_heart);
                nb_additional_cost_maps = (byte)parseInt(data, "Flooding", "nb_additional_cost_maps", nb_additional_cost_maps);
                complete_wall_clip_max_dist = parseFloat(data, "Flooding", "complete_wall_clip_max_dist", complete_wall_clip_max_dist);
                complete_wall_clip_duration = parseInt(data, "Flooding", "complete_wall_clip_duration", complete_wall_clip_duration);
                cwc_max_dist_zero_in_legal_zone = parseBool(data, "Flooding", "cwc_max_dist_zero_in_legal_zone", cwc_max_dist_zero_in_legal_zone);

                pos_reduction = parseInt(data, "Solver", "pos_reduction", pos_reduction);
                bump_reduction = parseInt(data, "Solver", "bump_reduction", bump_reduction);
                additional_reduction_in_wall = parseInt(data, "Solver", "additional_reduction_in_wall", additional_reduction_in_wall);
                additional_reduction_dist_multiplier = parseFloat(data, "Solver", "additional_reduction_dist_multiplier", additional_reduction_dist_multiplier);
                max_additional_reduction = parseInt(data, "Solver", "max_additional_reduction", max_additional_reduction);
                rot_precision = (short)parseInt(data, "Solver", "rot_precision", rot_precision);
                rot_rate_precision = (short)parseInt(data, "Solver", "rot_rate_precision", rot_rate_precision);
                cost_multiplier = parseFloat(data, "Solver", "cost_multiplier", cost_multiplier);
                allow_state_visit_with_less_life = parseBool(data, "Solver", "allow_state_visit_with_less_life", allow_state_visit_with_less_life);
                nb_iterations_before_ui_update = parseInt(data, "Solver", "nb_iterations_before_ui_update", nb_iterations_before_ui_update);
            }
            catch { }
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

                data["Game"]["full_life"] = full_life.ToString();

                data["Flooding"]["ground_speed"] = ground_speed.ToString();
                data["Flooding"]["allow_wall_clip"] = allow_wall_clip.ToString();
                data["Flooding"]["wall_speed"] = wall_speed.ToString();
                data["Flooding"]["ground_wall_bonus"] = ground_wall_bonus.ToString();
                data["Flooding"]["ground_wall_bonus_min_cost"] = ground_wall_bonus_min_cost.ToString();
                data["Flooding"]["wall_ground_malus"] = wall_ground_malus.ToString();
                data["Flooding"]["wall_ground_malus_dist"] = wall_ground_malus_dist.ToString();
                data["Flooding"]["restrict_complete_wall_clip_when_one_heart"] = restrict_complete_wall_clip_when_one_heart.ToString();
                data["Flooding"]["nb_additional_cost_maps"] = nb_additional_cost_maps.ToString();
                data["Flooding"]["complete_wall_clip_max_dist"] = complete_wall_clip_max_dist.ToString();
                data["Flooding"]["complete_wall_clip_duration"] = complete_wall_clip_duration.ToString();
                data["Flooding"]["cwc_max_dist_zero_in_legal_zone"] = cwc_max_dist_zero_in_legal_zone.ToString();

                data["Solver"]["pos_reduction"] = pos_reduction.ToString();
                data["Solver"]["bump_reduction"] = bump_reduction.ToString();
                data["Solver"]["additional_reduction_in_wall"] = additional_reduction_in_wall.ToString();
                data["Solver"]["additional_reduction_dist_multiplier"] = additional_reduction_dist_multiplier.ToString();
                data["Solver"]["max_additional_reduction"] = max_additional_reduction.ToString();
                data["Solver"]["rot_precision"] = rot_precision.ToString();
                data["Solver"]["rot_rate_precision"] = rot_rate_precision.ToString();
                data["Solver"]["cost_multiplier"] = cost_multiplier.ToString();
                data["Solver"]["allow_state_visit_with_less_life"] = allow_state_visit_with_less_life.ToString();
                data["Solver"]["nb_iterations_before_ui_update"] = nb_iterations_before_ui_update.ToString();

                FileIniDataParser parser = new FileIniDataParser();
                parser.WriteFile(filename, data);
            }
            catch { }
        }
    }
}