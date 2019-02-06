using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KuruBot
{
    public enum GameState
    {
        InGame = 0,
        Win,
        Loose
    }
    // /!\ For efficiency reason, we use a class instead of a struct. Copies need to be performed manually when needed.
    public class HelirinState : IEquatable<HelirinState>
    {
        public HelirinState(int xpos, int ypos, int xb, int yb, short rot, short rot_rate, short rot_srate, byte life, sbyte invul)
        {
            this.xpos = xpos;
            this.ypos = ypos;
            this.xb = xb;
            this.yb = yb;
            this.rot = rot;
            this.rot_rate = rot_rate;
            this.rot_srate = rot_srate;
            this.life = life;
            this.invul = invul;
            gs = GameState.InGame;
        }

        public HelirinState ShallowCopy()
        {
            return (HelirinState)MemberwiseClone();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as HelirinState);
        }

        public bool Equals(HelirinState other)
        {
            return other != null &&
                   xpos == other.xpos &&
                   ypos == other.ypos &&
                   rot == other.rot &&
                   xb == other.xb &&
                   yb == other.yb &&
                   rot_rate == other.rot_rate &&
                   rot_srate == other.rot_srate &&
                   invul == other.invul &&
                   life == other.life &&
                   gs == other.gs;
        }

        public override int GetHashCode()
        {
            var hashCode = 2000477822;
            hashCode = hashCode * -1521134295 + xpos.GetHashCode();
            hashCode = hashCode * -1521134295 + ypos.GetHashCode();
            hashCode = hashCode * -1521134295 + rot.GetHashCode();
            hashCode = hashCode * -1521134295 + xb.GetHashCode();
            hashCode = hashCode * -1521134295 + yb.GetHashCode();
            hashCode = hashCode * -1521134295 + rot_rate.GetHashCode();
            hashCode = hashCode * -1521134295 + rot_srate.GetHashCode();
            hashCode = hashCode * -1521134295 + invul.GetHashCode();
            hashCode = hashCode * -1521134295 + life.GetHashCode();
            hashCode = hashCode * -1521134295 + gs.GetHashCode();
            return hashCode;
        }

        public int xpos;
        public int ypos;
        public int xb;
        public int yb;
        public short rot;
        public short rot_rate;
        public short rot_srate;

        public byte life;
        public sbyte invul;
        public GameState gs;

        public static bool operator ==(HelirinState state1, HelirinState state2)
        {
            return EqualityComparer<HelirinState>.Default.Equals(state1, state2);
        }

        public static bool operator !=(HelirinState state1, HelirinState state2)
        {
            return !(state1 == state2);
        }
    }

    class Physics
    {
        static float rot_to_angle_approx(short rot) // May be innacurrate! Do not use in the physics engine.
        {
            return (float)(2 * Math.PI * rot / 0x10000);
        }
        static short angle_to_rot_approx(float angle) // May be innacurrate! Do not use in the physics engine.
        {
            return (short)(angle / (2 * Math.PI) * 0x10000);
        }
        static int px_to_pos_approx(short px) // May be innacurrate! Do not use in the physics engine.
        {
            return px << 16;
        }
        public static short pos_to_px(int pos)
        {
            return (short)(pos >> 16);
        }

        public static MapControl.GraphicalHelirin ToGraphicalHelirin(HelirinState h)
        {
            if (h == null)
                return new MapControl.GraphicalHelirin();
            float angle = rot_to_angle_approx(h.rot);
            int xpix = pos_to_px(h.xpos);
            int ypix = pos_to_px(h.ypos);
            return new MapControl.GraphicalHelirin(xpix, ypix, angle);
        }

        public static HelirinState FromGraphicalHelirin(MapControl.GraphicalHelirin h, bool clockwise)
        {
            short rot = angle_to_rot_approx(h.angle);
            int xpos = px_to_pos_approx((short)h.pixelX);
            int ypos = px_to_pos_approx((short)h.pixelY);
            short srate = clockwise ? default_srate : (short)(-default_srate);
            return new HelirinState(xpos, ypos, 0, 0, rot, srate, srate, Settings.full_life, 0);
        }

        Map map = null;
        KuruMath math = null;

        // Public constants
        public const short default_srate = 182;
        public const sbyte invul_frames = 20;
        //public const byte full_life = 3; // Moved to Settings!

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
        short[] helirin_pixels_spring = new short[] { -31, 31, -24, 24, -12, 12, 0 }; // Order is important!
        const int middle_mask = 0x7;
        short[] helirin_points = null; // Automatically initialized
        uint up_mask = 0; // Automatically initialized
        uint down_mask = 0; // Automatically initialized

        // Bump speeds
        const short rot_bump_rate = 1024;
        const int auto_bump_speed = 2 * 0x10000;
        const int input_bump_speed = 4 * 0x10000;
        const int input_bump_speed_2 = 185344;
        const short rot_bump_rate_spring = 768;
        const int bump_speed_spring = 0x10000;

        public Physics(Map map)
        {
            this.map = map;
            math = new KuruMath();
            // Set helirin physical points
            up_mask = 0; down_mask = 0;
            helirin_points = new short[1 + down_helirin_pixels_asc.Length * 2];
            helirin_points[0] = 0;
            for (int i = 0; i < down_helirin_pixels_asc.Length; i++)
            {
                down_mask += (uint)1 << (2 * i + 1);
                helirin_points[2 * i + 1] = down_helirin_pixels_asc[i];
                up_mask += (uint)1 << (2 * i + 2);
                helirin_points[2 * i + 2] = (short)(-down_helirin_pixels_asc[i]);
            }
        }

        static int DecreaseBumpSpeed(int bs)
        {
            int diff = bs / bump_speed_diff_frac;
            if (bs % bump_speed_diff_frac != 0)
                diff += Math.Sign(bs);
            return bs - diff;
        }
        static short AngleOfSpring(Map.SpringType t)
        {
            if (t == Map.SpringType.Down)
                return 0;
            else if (t == Map.SpringType.Left)
                return 0x10000 / 4;
            else if (t == Map.SpringType.Up)
                return -(0x10000 / 2);
            else
                return -(0x10000 / 4);
        }

        public HelirinState Next(HelirinState st, Action a)
        {
            if (st == null)
                return null;
            if (st.gs != GameState.InGame)
                return st;

            ActionEffect e = Controller.action_to_effect(a);
            st = st.ShallowCopy();

            // 1. Set input speed (XS and YS) depending on inputs
            int[] speeds = input_speeds;
            if (e.x != Direction1.None && e.y != Direction1.None)
                speeds = input_speeds_2;
            int speed = speeds[(int)e.speed];
            int xs = (int)e.x * speed;
            int ys = (int)e.y * speed;

            // 2. Reduce bump speed / bump rotation (XB, YB and Rot_rate) / invulnerability
            short rot_diff = (short)(st.rot_srate - st.rot_rate);
            if (rot_diff < -rotation_rate_decr)
                rot_diff = -rotation_rate_decr;
            if (rot_diff > rotation_rate_decr)
                rot_diff = rotation_rate_decr;
            st.rot_rate = (short)(st.rot_rate + rot_diff);
            st.xb = DecreaseBumpSpeed(st.xb);
            st.yb = DecreaseBumpSpeed(st.yb);
            if (st.invul > 0)
                st.invul--;

            // 3. Move depending on speed (bump+input), rotate depending on rotation rate (Rot_rate)
            st.xpos += xs + st.xb;
            st.ypos += ys + st.yb;
            st.rot += st.rot_rate;

            // 4. Detection of healing/ending zones
            // Position seems to be converted to px with a shift: subpixels seem to be ignored even in negative positions.
            bool safe_zone = false;
            short xpix = pos_to_px(st.xpos);
            short ypix = pos_to_px(st.ypos);
            Map.Zone zone = map.IsPixelInZone(xpix, ypix);
            if (zone == Map.Zone.Healing || zone == Map.Zone.Starting)
            {
                safe_zone = true;
                st.life = Settings.full_life;
            }
            if (zone == Map.Zone.Ending)
            {
                st.gs = GameState.Win;
                return st;
            }

            // From this point we create a new state so that we can still access old values of the state.
            HelirinState new_st = st.ShallowCopy();

            // 5. Action of springs
            bool invert_rotation = false;
            HashSet<int> already_visited = new HashSet<int>();
            //SortedSet<int> already_visited = new SortedSet<int>();
            foreach (short radius in helirin_pixels_spring)
            {
                // Position seems to be converted to px BEFORE adding the result of the sin/cos (it seems to ignore subpixels, even in negative positions).
                short px = (short)(xpix - math.factor_by_sin(radius, st.rot));
                short py = (short)(ypix + math.factor_by_cos(radius, st.rot));

                Map.Spring[] springs = map.IsPixelInSpring(px, py);
                foreach (Map.Spring spr in springs)
                {
                    if (!already_visited.Contains(spr.unique_id))
                    {
                        already_visited.Add(spr.unique_id);
                        // Invert rotation if at least one spring is in the right direction
                        if (!invert_rotation && radius != 0)
                        {
                            short spring_angle = AngleOfSpring(spr.type);
                            short helirin_angle = radius > 0 ? st.rot : (short)(st.rot + (0x10000 / 2));
                            short helirin_normal_angle = (short)(helirin_angle + Math.Sign(st.rot_srate) * (0x10000 / 4));
                            short diff = (short)(spring_angle - helirin_normal_angle);
                            if (Math.Abs((int)diff) > 0x10000 / 4)
                                invert_rotation = true;
                        }
                        
                        // Position bump
                        if (spr.type == Map.SpringType.Up)
                        {
                            new_st.xb = 0;
                            new_st.yb = -bump_speed_spring;
                        }
                        if (spr.type == Map.SpringType.Down)
                        {
                            new_st.xb = 0;
                            new_st.yb = bump_speed_spring;
                        }
                        if (spr.type == Map.SpringType.Left)
                        {
                            new_st.xb = -bump_speed_spring;
                            new_st.yb = 0;
                        }
                        if (spr.type == Map.SpringType.Right)
                        {
                            new_st.xb = bump_speed_spring;
                            new_st.yb = 0;
                        }
                        new_st.xpos += new_st.xb;
                        new_st.ypos += new_st.yb;
                    }
                }
            }
            if (invert_rotation)
                new_st.rot_srate = (short)(-st.rot_srate);
            // Update rate
            if (already_visited.Count > 0)
            {
                new_st.rot_rate = (short)(Math.Sign(new_st.rot_srate) * rot_bump_rate_spring);
                if (new_st.rot_rate == 0)
                    new_st.rot_rate = -rot_bump_rate_spring;
            }

            // 6. Compute collision mask
            // TODO: Optionally, use memoisation to avoid recomputing collision mask each time
            uint collision_mask = 0;
            for (int i = 0; i < helirin_points.Length; i++)
            {
                int radius = helirin_points[i];

                // Position seems to be converted to px BEFORE adding the result of the sin/cos (it seems to ignore subpixels, even in negative positions).
                short px = (short)(xpix - math.factor_by_sin(radius, st.rot));
                short py = (short)(ypix + math.factor_by_cos(radius, st.rot));

                if (map.IsPixelInCollision(px, py))
                    collision_mask = collision_mask | ((uint)1 << i);
            }

            if (collision_mask != 0) // If collision
            {
                // 7. Damage
                if (!safe_zone)
                {
                    if (new_st.invul == 0)
                    {
                        new_st.invul = invul_frames;
                        new_st.life--;

                        if (new_st.life == 0)
                        {
                            new_st.gs = GameState.Loose;
                            return new_st;
                        }
                    }
                }

                // 8. Bump action
                //  - Substract input speed (XS and YS) to position
                //  - Modify bump speed and rot rate (XB, YB and Rot_rate) accordingly if relevant
                //  - If modified, apply this newly computed bump speed to position
                new_st.xpos -= xs;
                new_st.ypos -= ys;
                bool up_side = (collision_mask & up_mask) != 0;
                bool down_side = (collision_mask & down_mask) != 0;
                if (up_side && !down_side)
                {
                    new_st.xb = - math.factor_by_sin(auto_bump_speed, st.rot);
                    new_st.yb =   math.factor_by_cos(auto_bump_speed, st.rot);
                }
                if (!up_side && down_side)
                {
                    new_st.xb =   math.factor_by_sin(auto_bump_speed, st.rot);
                    new_st.yb = - math.factor_by_cos(auto_bump_speed, st.rot);
                }
                new_st.rot_rate = (short)(-Math.Sign(st.rot_rate) * rot_bump_rate);
                if (new_st.rot_rate == 0)
                    new_st.rot_rate = rot_bump_rate;
                if (up_side != down_side)
                {
                    new_st.xpos += new_st.xb;
                    new_st.ypos += new_st.yb;
                }

                // 9. If mask has collision at one of the 3 lowest bits :
                //  - Modify bump speed (XB and YB) depending on input (if any)
                //  - If modified, apply this newly computed bump speed to position
                if ((collision_mask & middle_mask) != 0 && (e.x != Direction1.None || e.y != Direction1.None))
                {
                    int bump_speed = input_bump_speed;
                    if (e.x != Direction1.None && e.y != Direction1.None)
                        bump_speed = input_bump_speed_2;
                    new_st.xb = -(int)e.x * bump_speed;
                    new_st.yb = -(int)e.y * bump_speed;
                    new_st.xpos += new_st.xb;
                    new_st.ypos += new_st.yb;
                }
            }

            return new_st;
        }

    }
}
