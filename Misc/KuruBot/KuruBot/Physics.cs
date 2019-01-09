using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KuruBot
{
    // /!\ This should remain a STRUCT
    public struct HelirinState
    {
        public HelirinState(int xpos, int ypos, short rot, short rot_srate)
        {
            this.xpos = xpos;
            this.ypos = ypos;
            this.rot = rot;
            this.rot_srate = rot_srate;
            xb = 0;
            yb = 0;
            rot_rate = rot_srate;
        }

        public int xpos;
        public int ypos;
        public int xb;
        public int yb;
        public short rot;
        public short rot_rate;
        public short rot_srate;

        // TODO: Add life and invicibility
    }

    class Physics
    {
        static float rot_to_angle_exact(short rot)
        {
            return (float)(2 * Math.PI * rot / 0x10000);
        }
        static int pos_to_px(int pos)
        {
            return pos >> 16;
        }
        static int px_to_pos(int px)
        {
            return px << 16;
        }

        public static MapControl.GraphicalHelirin ToGraphicalHelirin(HelirinState h)
        {
            float angle = rot_to_angle_exact(h.rot);
            int xpix = pos_to_px(h.xpos);
            int ypix = pos_to_px(h.ypos);
            return new MapControl.GraphicalHelirin(xpix, ypix, angle);
        }

        Map map = null;
        KuruMath math = null;

        // Input speed constants
        const int speed0 = (3 * 0x10000) / 2;
        const int speed1 = (3 * speed0) / 2;
        const int speed2 = 2 * speed0;
        int[] input_speeds = new int[] { speed0, speed1, speed2 };

        const int speed0_2 = 69504;
        const int speed1_2 = (3 * speed0_2) / 2;
        const int speed2_2 = 2 * speed0_2;
        int[] input_speeds_2 = new int[] { speed0_2, speed1_2, speed2_2 };

        // Various constants
        const short rotation_rate_decr = 91;
        const int bump_speed_diff_frac = 4;
        const int nb_points_semi_helirin = 8;
        int[] helirin_points = null; // Automatically initialized
        const int initial_bump_speed = 2 * 0x10000;

        public Physics(Map map)
        {
            this.map = map;
            math = new KuruMath();
            // Set helirin physical points
            helirin_points = new int[1+nb_points_semi_helirin*2];
            helirin_points[0] = 0;
            for (int i = 0; i < nb_points_semi_helirin; i++)
            {
                helirin_points[2*i+1] = px_to_pos(-(i+1) * (Map.helirin_radius / nb_points_semi_helirin));
                helirin_points[2*i+2] = px_to_pos( (i+1) * (Map.helirin_radius / nb_points_semi_helirin));
            }
        }

        static int DecreaseBumpSpeed(int bs)
        {
            int diff = bs / bump_speed_diff_frac;
            if (bs % bump_speed_diff_frac != 0)
                diff += Math.Sign(bs);
            return bs - diff;
        }

        public HelirinState Next(HelirinState st, Action a)
        {
            ActionEffect e = Controller.action_to_effect(a);

            // 1. Set input speed (XS and YS) depending on inputs
            int[] speeds = input_speeds;
            if (e.x != Direction1.None && e.y != Direction1.None)
                speeds = input_speeds_2;
            int speed = speeds[(int)e.speed];
            int xs = 0;
            if (e.x == Direction1.Backward)
                xs = -speed;
            else if (e.x == Direction1.Forward)
                xs = speed;
            int ys = 0;
            if (e.y == Direction1.Backward)
                ys = -speed;
            else if (e.y == Direction1.Forward)
                ys = speed;

            // 2. Reduce bump speed / bump rotation (XB, YB and Rot_rate)
            short rot_diff = (short)(st.rot_srate - st.rot_rate);
            if (rot_diff < -rotation_rate_decr)
                rot_diff = -rotation_rate_decr;
            if (rot_diff > rotation_rate_decr)
                rot_diff = rotation_rate_decr;
            st.rot_rate = (short)(st.rot_rate + rot_diff);
            st.xb = DecreaseBumpSpeed(st.xb);
            st.yb = DecreaseBumpSpeed(st.yb);

            // 3. Move depending on speed (bump+input), rotate depending on rotation rate (Rot_rate)
            st.xpos += xs + st.xb;
            st.ypos += ys + st.yb;
            st.rot += st.rot_rate;

            // 4. Detection of healing/ending zones
            Map.Zone zone = map.IsPixelInZone(pos_to_px(st.xpos), pos_to_px(st.ypos));
            if (zone == Map.Zone.Healing)
            {
                // TODO
            }
            if (zone == Map.Zone.Ending)
            {
                // TODO
            }

            // 5. Compute collision mask
            uint collision_mask = 0;
            for (int i = 0; i < helirin_points.Length; i++)
            {
                int radius = helirin_points[i];
                int pixX = pos_to_px(st.xpos + math.factor_by_sin(radius, st.rot));
                int pixY = pos_to_px(st.ypos - math.factor_by_cos(radius, st.rot));
                if (map.IsPixelInCollision(pixX, pixY))
                    collision_mask = collision_mask | ((uint)1 << i);
            }

            // 6. If collision:
            //      - Substract input speed (XS,YS) to position
            //      - Modify bump speed accordingly if relevant
            //      - If modified, apply this newly computed bump speed to position

            // TODO

            return st;
        }

    }
}
