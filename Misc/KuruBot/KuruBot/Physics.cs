using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KuruBot
{
    struct HelirinState
    {
        internal HelirinState(uint xpos, uint ypos, ushort rot, short rot_srate)
        {
            this.xpos = xpos;
            this.ypos = ypos;
            this.rot = rot;
            this.rot_srate = rot_srate;
            xb = 0;
            yb = 0;
            rot_rate = rot_srate;
        }

        internal uint xpos;
        internal uint ypos;
        internal short xb;
        internal short yb;
        internal ushort rot;
        internal short rot_rate;
        internal short rot_srate;
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

        Form1 parent = null;

        public Physics(Form1 parent)
        {
            this.parent = parent;
        }



    }
}
