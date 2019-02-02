using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KuruBot
{
    public static class Settings
    {
        // ---------- FLOODING SETTINGS ----------

        // All these values must be non-negative
        public const float ground_speed = 3;
        public const float wall_speed = ground_speed; // Should be equal to ground speed (we can't benefit from wall speed for ever, so a constant bonus is more adapted).
        public const float ground_wall_bonus = 7 - 2 - ground_speed; // Bonus applied to each pixel in a wall (in a post procedure) in order to encourage wall exploration. Unit: weight/frame.
        public const float ground_wall_bonus_min_dist = 7 - 2; // Min weight required for a wall to benefit from full bonus. Must be greater than ground_wall_bonus. Unit: dist/frame.
        public const float wall_ground_malus = ground_speed * 20; // Malus applied everytime we leave a wall clip, in order to capture the fact that doing the other way could be expensive.
        public const float wall_clip_end_dist = 4; // Distance from the wall at which the helirin has no control anymore.

        // ---------- SOLVER SETTINGS ----------

        public const int pos_reduction = 16 - 6; // 0x10000 / 64 : 1/64 px
        public const int bump_reduction = 16 - 6; // 0x10000 / 64 : 1/64 px/frame
        public const int additional_reduction_in_wall = 6; // 64
        public const float reduction_dist_multiplier = 1f / 8f; // 1/8 shift/pixel
        public const int max_additional_reduction = 6; // 64

        public const short rot_precision = Physics.default_srate;
        public const short rot_rate_precision = Physics.default_srate;

        public const float cost_multiplier = 1;
        public const bool allow_state_multiple_visits = true; // A vertex could be visited many times because the cost function is not always a lower-bound of the real distance.
        // TODO: optimisation parameter for lives system (see bot.txt)
        public const int number_iterations_before_ui_update = 10000;
    }
}
