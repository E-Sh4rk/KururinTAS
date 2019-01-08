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
    }

    class Physics
    {
        public static MapControl.GraphicalHelirin ToGraphicalHelirin(HelirinState h)
        {
            float angle = (float)(2 * Math.PI * h.rot / 0x10000);
            int xpix = h.xpos >> 16;
            int ypix = h.ypos >> 16;
            return new MapControl.GraphicalHelirin(xpix, ypix, angle);
        }

        Map map = null;

        public Physics(Map map)
        {
            this.map = map;
        }

        // Input speed constants
        const int speed0 = (3*0xFFFF)/2;
        const int speed1 = (3*speed0)/2;
        const int speed2 = 2*speed0;
        int[] input_speeds = new int[] { speed0, speed1, speed2 };

        const int speed0_2 = 69504;
        const int speed1_2 = (3*speed0_2)/2;
        const int speed2_2 = 2*speed0_2;
        int[] input_speeds_2 = new int[] { speed0_2, speed1_2, speed2_2 };

        public HelirinState Next(HelirinState st, Action a)
        {
            // TODO
            ActionEffect e = Controller.action_to_effect(a);
            // 1. Set XS and YS depending on inputs
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

            // TMP
            st.xpos += xs;
            st.ypos += ys;

            return st;
        }

    }
}
