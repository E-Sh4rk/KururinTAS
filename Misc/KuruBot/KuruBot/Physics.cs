using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KuruBot
{
    public struct HelirinState
    {
        public HelirinState(uint xpos, uint ypos, ushort rot, short rot_srate)
        {
            this.xpos = xpos;
            this.ypos = ypos;
            this.rot = rot;
            this.rot_srate = rot_srate;
            xb = 0;
            yb = 0;
            rot_rate = rot_srate;
        }

        public uint xpos;
        public uint ypos;
        public short xb;
        public short yb;
        public ushort rot;
        public short rot_rate;
        public short rot_srate;
    }

    class Physics
    {
        public static MapControl.GraphicalHelirin ToGraphicalHelirin(HelirinState h)
        {
            float angle = (float)(h.rot * 2 * Math.PI / 0x10000);
            int xpix = (int)h.xpos >> 16;
            int ypix = (int)h.ypos >> 16;
            return new MapControl.GraphicalHelirin(xpix, ypix, angle);
        }

        Map map = null;

        public Physics(Map map)
        {
            this.map = map;
        }

        public HelirinState Next(HelirinState st)
        {
            // TODO
            return st;
        }

    }
}
