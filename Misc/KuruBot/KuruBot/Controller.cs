using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace KuruBot
{
    enum Action
    {
        NoButton = 0,

        Up1 = 1,
        Down1,
        Left1,
        Right1,
        UpLeft1,
        UpRight1,
        DownLeft1,
        DownRight1,

        Up2 = 9,
        Down2,
        Left2,
        Right2,
        UpLeft2,
        UpRight2,
        DownLeft2,
        DownRight2,

        Up3 = 17,
        Down3,
        Left3,
        Right3,
        UpLeft3,
        UpRight3,
        DownLeft3,
        DownRight3
    }

    static class Controller
    {
        static string action_to_string(Action a)
        {
            if (a == Action.NoButton)
                return "";
            int i = (int)a - 1;
            string res = "";

            int speed = i / 8;
            if (speed == 1)
                res += "A";
            else if (speed == 2)
                res += "AB";

            int dir = i % 8;
            switch (dir)
            {
                case 0:
                    res += "U";
                    break;
                case 1:
                    res += "D";
                    break;
                case 2:
                    res += "L";
                    break;
                case 3:
                    res += "R";
                    break;
                case 4:
                    res += "UL";
                    break;
                case 5:
                    res += "UR";
                    break;
                case 6:
                    res += "DL";
                    break;
                case 7:
                    res += "DR";
                    break;
            }

            return res;
        }
        static Size action_to_size(Action a)
        {
            string action = action_to_string(a);
            Size u = new Size(0, -1);
            Size l = new Size(-1, 0);
            Size res = new Size(0, 0);
            if (action.Contains("U"))
                res = Size.Add(res, u);
            if (action.Contains("D"))
                res = Size.Subtract(res, u);
            if (action.Contains("L"))
                res = Size.Add(res, l);
            if (action.Contains("R"))
                res = Size.Subtract(res, l);

            if (action.Contains("A") && action.Contains("B"))
                res = Size.Add(Size.Add(res, res), res);
            else if (action.Contains("A") || action.Contains("B"))
                res = Size.Add(res, res);

            return res;
        }
    }
}
