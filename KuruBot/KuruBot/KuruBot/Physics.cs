using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace KuruBot
{
    public enum GameState
    {
        InGame = 0,
        InGameWithBonus,
        Win,
        WinWithBonus,
        Loose
    }
    // /!\ For efficiency reason, we use a class instead of a struct. Copies need to be performed manually when needed.
    public class HelirinState : IEquatable<HelirinState>
    {
        public HelirinState(int xpos, int ypos, int xb, int yb, short rot, short rot_rate, short rot_srate, byte life, sbyte invul, bool hasBonus, ushort frameNumber)
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
            this.frameNumber = frameNumber;
            gs = hasBonus ? GameState.InGameWithBonus : GameState.InGame;
        }

        public bool IsTerminal()
        {
            return gs != GameState.InGameWithBonus && gs != GameState.InGame;
        }
        public bool HasBonus()
        {
            return gs == GameState.InGameWithBonus || gs == GameState.WinWithBonus;
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
                   frameNumber == other.frameNumber &&
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
            hashCode = hashCode * -1521134295 + frameNumber.GetHashCode();
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
        public ushort frameNumber; // 2 bytes is enough for more than 10 minutes

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
            return new MapControl.GraphicalHelirin(xpix, ypix, angle, h.HasBonus(), h.frameNumber);
        }

        public static HelirinState FromGraphicalHelirin(MapControl.GraphicalHelirin h, bool clockwise)
        {
            short rot = angle_to_rot_approx(h.angle);
            int xpos = px_to_pos_approx((short)h.pixelX);
            int ypos = px_to_pos_approx((short)h.pixelY);
            short srate = clockwise ? default_srate : (short)(-default_srate);
            return new HelirinState(xpos, ypos, 0, 0, rot, srate, srate, Settings.full_life, 0, h.hasBonus, (ushort)h.frameNumber);
        }

        Map map = null;
        KuruMath math = null;

        // Public constants
        public const short default_srate = 182;
        //public const sbyte invul_frames = 20; // Moved to Settings!
        //public const byte full_life = 3; // Moved to Settings!

        // Input speed constants
        const int speed0 = (3 * 0x10000) / 2;
        const int speed1 = (3 * speed0) / 2;
        const int speed2 = 2 * speed0;
        readonly int[] input_speeds = new int[] { speed0, speed1, speed2 };

        const int speed0_2 = 69504;
        const int speed1_2 = (3 * speed0_2) / 2;
        const int speed2_2 = 2 * speed0_2;
        readonly int[] input_speeds_2 = new int[] { speed0_2, speed1_2, speed2_2 };

        // Various constants
        const short rotation_rate_decr = 91;
        const int bump_speed_diff_frac = 4;
        readonly short[] helirin_points = new short[] { 0, 4, -4, 8, -8, 12, -12, 16, -16, 20, -20, 24, -24, 28, -28, 31, -31 };
        readonly int[] helirin_points_order_for_springs = new int[] { 16, 14, 12, 10, 8, 6, 4, 2, 15, 13, 11, 9, 7, 5, 3, 1, 0 };
        const uint up_mask = 0x15554;
        const uint down_mask = 0xAAAA;
        const int middle_mask = 0x7;

        // Bump speeds
        const short rot_bump_rate = 1024;
        const int auto_bump_speed = 2 * 0x10000;
        const int input_bump_speed = 4 * 0x10000;
        const int input_bump_speed_2 = 185344;
        const short rot_bump_rate_spring = 768;
        const int bump_speed_spring = 0x10000;
        const int moving_object_bump_speed = 3 * 0x10000;

        public Physics(Map map)
        {
            this.map = map;
            math = KuruMath.instance;//new KuruMath();
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

        void ObjectHitReact(HelirinState st, uint collision_mask, short px, short py, int objx, int objy)
        {
            short dir = math.atan2(px - objx, py - objy);
            if ((collision_mask & middle_mask) != 0)
            {
                st.xb = math.sin(moving_object_bump_speed, dir);
                st.yb = -math.cos(moving_object_bump_speed, dir);
                st.xpos += st.xb;
                st.ypos += st.yb;
            }
            if ((collision_mask & (up_mask | down_mask)) != 0)
            {
                dir -= (short)((st.rot >> 8) << 8);
                if ((collision_mask & up_mask) != 0)
                    dir = (short)(dir + 0x8000);
                if (dir >= 0)
                    st.rot_rate = -rot_bump_rate;
                else
                    st.rot_rate = rot_bump_rate;
            }
        }

        public HelirinState Next(HelirinState st, Action a)
        {
            if (st == null)
                return null;
            if (st.IsTerminal())
                return st;

            ActionEffect e = Controller.action_to_effect(a);
            st = st.ShallowCopy();

            st.frameNumber += 1;

            if (st.life > Settings.full_life)
                st.life = Settings.full_life;
            if (st.invul > Settings.invul_frames)
                st.invul = Settings.invul_frames;

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
            Map.Zone zone = map.IsPixelInZone(pos_to_px(st.xpos), pos_to_px(st.ypos));
            if (zone == Map.Zone.Healing || zone == Map.Zone.Starting)
            {
                safe_zone = true;
                if (st.life < Settings.full_life)
                    st.life = Settings.full_life;
            }
            if (zone == Map.Zone.Ending)
            {
                st.gs = st.gs == GameState.InGameWithBonus ? GameState.WinWithBonus : GameState.Win;
                return st;
            }

            // At this point, we backup the rotation data (will be needed later)
            short rot_rate_bkp = st.rot_rate; // No need to backup st.rot because it will not change anymore
            // We also precompute all the helirin physical points and the collision mask
            // TODO: Optionally, use memoisation to avoid recomputing collision mask each time
            short[] pxs = new short[helirin_points.Length];
            short[] pys = new short[helirin_points.Length];
            uint collision_mask = 0;
            for (int i = 0; i < helirin_points.Length; i++)
            {
                int radius = helirin_points[i];
                // Position seems to be converted to px BEFORE adding the result of the sin/cos (it seems to ignore subpixels, even in negative positions).
                short px = (short)(pos_to_px(st.xpos) - math.sin(radius, st.rot));
                short py = (short)(pos_to_px(st.ypos) + math.cos(radius, st.rot));
                pxs[i] = px;
                pys[i] = py;
                // Compute collision mask
                if (map.IsPixelInCollision(px, py))
                    collision_mask |= ((uint)1 << i);
            }

            // 5. Action of springs & bonus
            HashSet<int> spring_already_visited = new HashSet<int>();
            bool invert_rotation = false;
            bool update_rot_rate = false;
            foreach (int i in helirin_points_order_for_springs) // Order is important for spring actions.
            {
                int radius = helirin_points[i];
                short px = pxs[i];
                short py = pys[i];

                // Action of springs
                Map.Spring[] springs = map.IsPixelInSpring(px, py);
                foreach (Map.Spring spr in springs)
                {
                    if (!spring_already_visited.Contains(spr.unique_id))
                    {
                        spring_already_visited.Add(spr.unique_id);

                        if (radius != 0)
                        {
                            update_rot_rate = true;
                            // Invert rotation if at least one spring is in the right direction
                            if (!invert_rotation)
                            {
                                short spring_angle = AngleOfSpring(spr.type);
                                short helirin_angle = radius > 0 ? st.rot : (short)(st.rot + (0x10000 / 2));
                                short helirin_normal_angle = (short)(helirin_angle + Math.Sign(st.rot_srate) * (0x10000 / 4));
                                short diff = (short)(spring_angle - helirin_normal_angle);
                                if (Math.Abs((int)diff) > 0x10000 / 4)
                                    invert_rotation = true;
                            }
                        }

                        // Position bump
                        if (spr.type == Map.SpringType.Up)
                        {
                            st.xb = 0;
                            st.yb = -bump_speed_spring;
                        }
                        if (spr.type == Map.SpringType.Down)
                        {
                            st.xb = 0;
                            st.yb = bump_speed_spring;
                        }
                        if (spr.type == Map.SpringType.Left)
                        {
                            st.xb = -bump_speed_spring;
                            st.yb = 0;
                        }
                        if (spr.type == Map.SpringType.Right)
                        {
                            st.xb = bump_speed_spring;
                            st.yb = 0;
                        }
                        st.xpos += st.xb;
                        st.ypos += st.yb;
                    }
                }
                // Action of bonus
                if (map.IsPixelInBonus(px, py) != Map.BonusType.None)
                    st.gs = GameState.InGameWithBonus;
            }
            if (invert_rotation)
                st.rot_srate = (short)(-st.rot_srate);
            if (update_rot_rate)
            {
                st.rot_rate = (short)(Math.Sign(st.rot_srate) * rot_bump_rate_spring);
                if (st.rot_rate == 0)
                    st.rot_rate = -rot_bump_rate_spring;
            }

            // 6. Action of moving objects
            uint object_collision_mask = 0;
            if (Settings.enable_moving_objects)
            {
                List<Roller.Ball> balls = new List<Roller.Ball>();
                foreach (Roller r in map.Rollers)
                {
                    Roller.Ball ball = r.PreciseBoxAtTime(st.frameNumber);
                    if (ball != null)
                        balls.Add(ball);
                }
                balls.Sort((x, y) => x < y ? -1 : (x > y ? 1 : 0));
                foreach (Roller.Ball ball in balls)
                {
                    uint elt_collision_mask = 0;
                    for (int i = 0; i < helirin_points.Length; i++)
                    {
                        // For rollers, position of physical points must be recomputed to take into account last position/rotation changes
                        // EDIT: Seems not... At least, spring actions should not affect it
                        /*int radius = helirin_points[i];
                        short px = (short)(pos_to_px(st.xpos) - math.sin(radius, st.rot));
                        short py = (short)(pos_to_px(st.ypos) + math.cos(radius, st.rot));*/
                        short px = pxs[i];
                        short py = pys[i];
                        if (ball.InCollisionWith(px, py))
                            elt_collision_mask |= ((uint)1 << i);
                    }
                    if (elt_collision_mask != 0)
                    {
                        object_collision_mask |= elt_collision_mask;
                        ObjectHitReact(st, elt_collision_mask, /*pxs[0], pys[0],*/pos_to_px(st.xpos), pos_to_px(st.ypos), ball.cx, ball.cy);
                    }
                }
                foreach (Piston p in map.Pistons)
                {
                    Rectangle? box = null;
                    uint elt_collision_mask = 0;
                    for (int i = 0; i < helirin_points.Length; i++)
                    {
                        short px = pxs[i];
                        short py = pys[i];
                        if (p.dangerArea.Contains(px, py))
                        {
                            if (box == null)
                                box = p.PreciseBoxAtTime(st.frameNumber);
                            if (box.Value.Contains(px, py))
                                elt_collision_mask |= ((uint)1 << i);
                        }
                    }
                    if (elt_collision_mask != 0)
                    {
                        object_collision_mask |= elt_collision_mask;
                        ObjectHitReact(st, elt_collision_mask, /*pxs[0], pys[0],*/pos_to_px(st.xpos), pos_to_px(st.ypos),
                            box.Value.X + (box.Value.Width-1) / 2, box.Value.Y + (box.Value.Height-1) / 2);
                    }
                }
            }

            if (collision_mask != 0 || object_collision_mask != 0) // If collision with a wall OR a moving object
            {
                // 7. Damage and substract input speed (XS and YS) to position
                if (!safe_zone)
                {
                    if (st.invul == 0)
                    {
                        st.invul = Settings.invul_frames;
                        st.life--;
                    }
                }
                st.xpos -= xs;
                st.ypos -= ys;

                if (collision_mask != 0) // If collision with a wall
                {
                    // 8. Bump action
                    // - Modify bump speed and rot rate (XB, YB and Rot_rate) accordingly if relevant
                    // - If modified, apply this newly computed bump speed to position
                    bool up_side = (collision_mask & up_mask) != 0;
                    bool down_side = (collision_mask & down_mask) != 0;
                    if (up_side && !down_side)
                    {
                        st.xb = -math.sin(auto_bump_speed, st.rot);
                        st.yb = math.cos(auto_bump_speed, st.rot);
                    }
                    if (!up_side && down_side)
                    {
                        st.xb = math.sin(auto_bump_speed, st.rot);
                        st.yb = -math.cos(auto_bump_speed, st.rot);
                    }
                    st.rot_rate = (short)(-Math.Sign(rot_rate_bkp) * rot_bump_rate);
                    if (st.rot_rate == 0)
                        st.rot_rate = rot_bump_rate;
                    if (up_side != down_side)
                    {
                        st.xpos += st.xb;
                        st.ypos += st.yb;
                    }

                    // 9. If mask has collision at one of the 3 lowest bits :
                    //  - Modify bump speed (XB and YB) depending on input (if any)
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
            }

            // Loose?
            // TODO: Move up so that some useless computation can be avoided when loosing
            if (st.life == 0)
                st.gs = GameState.Loose;

            return st;
        }

    }
}
