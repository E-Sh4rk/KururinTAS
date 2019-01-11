using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KuruBot
{
    // /!\ This should remain a STRUCT
    public struct HelirinState
    {
        public HelirinState(int xpos, int ypos, int xb, int yb, short rot, short rot_rate, short rot_srate)
        {
            this.xpos = xpos;
            this.ypos = ypos;
            this.xb = xb;
            this.yb = yb;
            this.rot = rot;
            this.rot_rate = rot_rate;
            this.rot_srate = rot_srate;
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
        static short pos_to_px(int pos)
        {
            return (short)(pos >> 16);
        }
        static int px_to_pos(short px)
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
        short[] down_helirin_pixels_asc = new short[] { 4, 8, 12, 16, 20, 24, 28, 31 };
        const int middle_mask = 0x7;
        int[] helirin_points = null; // Automatically initialized
        uint up_mask = 0; // Automatically initialized
        uint down_mask = 0; // Automatically initialized

        // Bump speeds
        const short rot_bump_rate = 1024;
        const int auto_bump_speed = 2 * 0x10000;
        const int input_bump_speed = 4 * 0x10000;
        const int input_bump_speed_2 = 185344;

        public Physics(Map map)
        {
            this.map = map;
            math = new KuruMath();
            // Set helirin physical points
            up_mask = 0; down_mask = 0;
            helirin_points = new int[1 + down_helirin_pixels_asc.Length * 2];
            helirin_points[0] = 0;
            for (int i = 0; i < down_helirin_pixels_asc.Length; i++)
            {
                down_mask += (uint)1 << (2 * i + 1);
                helirin_points[2*i+1] = px_to_pos(down_helirin_pixels_asc[i]);
                up_mask += (uint)1 << (2 * i + 2);
                helirin_points[2*i+2] = px_to_pos((short)(-down_helirin_pixels_asc[i]));
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
            int xs = (int)e.x * speed;
            int ys = (int)e.y * speed;

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
            // TODO: Optionally, use memoisation to avoid recomputing collision mask each time
            uint collision_mask = 0;
            for (int i = 0; i < helirin_points.Length; i++)
            {
                int radius = helirin_points[i];

                // Position seems to be truncated BEFORE adding result of sin/cos
                /*int pixX = pos_to_px(st.xpos - math.factor_by_sin(radius, st.rot));
                int pixY = pos_to_px(st.ypos + math.factor_by_cos(radius, st.rot));*/
                short pixX = (short)(pos_to_px(st.xpos) - pos_to_px(math.factor_by_sin(radius, st.rot)));
                short pixY = (short)(pos_to_px(st.ypos) + pos_to_px(math.factor_by_cos(radius, st.rot)));

                if (map.IsPixelInCollision(pixX, pixY))
                    collision_mask = collision_mask | ((uint)1 << i);
            }

            // 6. If collision:
            //  - Substract input speed to position
            //  - Modify bump speed (pos & rot) accordingly if relevant
            //  - If modified, apply this newly computed bump speed to position
            if (collision_mask != 0)
            {
                st.xpos -= xs;
                st.ypos -= ys;
                bool up_side = (collision_mask & up_mask) != 0;
                bool down_side = (collision_mask & down_mask) != 0;
                if (up_side && !down_side)
                {
                    st.xb = - math.factor_by_sin(auto_bump_speed, st.rot);
                    st.yb =   math.factor_by_cos(auto_bump_speed, st.rot);
                }
                if (!up_side && down_side)
                {
                    st.xb =   math.factor_by_sin(auto_bump_speed, st.rot);
                    st.yb = - math.factor_by_cos(auto_bump_speed, st.rot);
                }
                if (up_side != down_side)
                {
                    st.xpos += st.xb;
                    st.ypos += st.yb;
                }
                st.rot_rate = (short)(-Math.Sign(st.rot_rate) * rot_bump_rate);
                if (st.rot_rate == 0)
                    st.rot_rate = rot_bump_rate;

                // 7. If mask has collision at one of the 3 lowest bits :
                //  - Modify bump speed (position) depending on input
                //  - If modified, apply this newly computed bump speed to position
                if ((collision_mask & middle_mask) != 0 && (e.x != Direction1.None || e.y != Direction1.None))
                {
                    int bump_speed = input_bump_speed;
                    if (e.x != Direction1.None && e.y != Direction1.None)
                        bump_speed = input_bump_speed_2;
                    st.xb = -(int)e.x * bump_speed;
                    st.yb = -(int)e.y * bump_speed;
                    st.xpos += st.xb;
                    st.ypos += st.yb;
                }
            }

            return st;
        }

    }
}
