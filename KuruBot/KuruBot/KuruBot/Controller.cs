using KuruBot.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace KuruBot
{
    public enum Action : byte
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
    enum Direction1 : sbyte
    {
        Backward=-1,
        None=0,
        Forward=1
    }
    enum Speed : byte
    {
        Speed0 = 0,
        Speed1 = 1,
        Speed2 = 2
    }
    struct ActionEffect
    {
        public Direction1 x;
        public Direction1 y;
        public Speed speed;
    }

    static class Controller
    {
        public static Action change_action_speed(Action a, Speed s)
        {
            if (a != Action.NoButton)
                a = (Action)(((int)a - 1) % 8 + 8 * (int)s + 1);
            return a;
        }
        public static ActionEffect string_to_effect(string action)
        {
            ActionEffect e;

            if (action.Contains("U"))
                e.y = Direction1.Backward;
            else if (action.Contains("D"))
                e.y = Direction1.Forward;
            else
                e.y = Direction1.None;


            if (action.Contains("L"))
                e.x = Direction1.Backward;
            else if (action.Contains("R"))
                e.x = Direction1.Forward;
            else
                e.x = Direction1.None;

            if (action.Contains("A") && action.Contains("B"))
                e.speed = Speed.Speed2;
            else if (action.Contains("A") || action.Contains("B"))
                e.speed = Speed.Speed1;
            else
                e.speed = Speed.Speed0;

            return e;
        }
        public static Action effect_to_action(ActionEffect e)
        {
            Action res = Action.NoButton;
            if (e.y == Direction1.Backward)
            {
                if (e.x == Direction1.Backward)
                    res = Action.UpLeft1;
                if (e.x == Direction1.None)
                    res = Action.Up1;
                if (e.x == Direction1.Forward)
                    res = Action.UpRight1;
            }
            if (e.y == Direction1.None)
            {
                if (e.x == Direction1.Backward)
                    res = Action.Left1;
                if (e.x == Direction1.Forward)
                    res = Action.Right1;
            }
            if (e.y == Direction1.Forward)
            {
                if (e.x == Direction1.Backward)
                    res = Action.DownLeft1;
                if (e.x == Direction1.None)
                    res = Action.Down1;
                if (e.x == Direction1.Forward)
                    res = Action.DownRight1;
            }

            res = change_action_speed(res, e.speed);
            return res;
        }
        public static ActionEffect action_to_effect(Action a)
        {
            ActionEffect res = new ActionEffect();
            res.speed = Speed.Speed0;
            res.x = Direction1.None;
            res.y = Direction1.None;

            if (a != Action.NoButton)
            {
                int i = (int)a - 1;
                res.speed = (Speed)(i / 8);
                
                int dir = i % 8;
                switch (dir)
                {
                    case 0:
                        res.y = Direction1.Backward;
                        break;
                    case 1:
                        res.y = Direction1.Forward;
                        break;
                    case 2:
                        res.x = Direction1.Backward;
                        break;
                    case 3:
                        res.x = Direction1.Forward;
                        break;
                    case 4:
                        res.y = Direction1.Backward;
                        res.x = Direction1.Backward;
                        break;
                    case 5:
                        res.y = Direction1.Backward;
                        res.x = Direction1.Forward;
                        break;
                    case 6:
                        res.y = Direction1.Forward;
                        res.x = Direction1.Backward;
                        break;
                    case 7:
                        res.y = Direction1.Forward;
                        res.x = Direction1.Forward;
                        break;
                }
            }
            return res;
        }
        public static string effect_to_string(ActionEffect e)
        {
            string res = "";

            // Speed
            if (e.speed == Speed.Speed1)
                res += "A";
            else if (e.speed == Speed.Speed2)
                res += "AB";

            // Direction
            if (e.y == Direction1.Backward)
                res += "U";
            else if (e.y == Direction1.Forward)
                res += "D";

            if (e.x == Direction1.Backward)
                res += "L";
            else if (e.x == Direction1.Forward)
                res += "R";

            return res;
        }
        public static string[] from_bz2_format(string[] lines)
        {
            List<string> res = new List<string>();
            foreach (string line in lines)
            {
                int i = line.LastIndexOf(',');
                if (i >= 0)
                {
                    try
                    {
                        string str = "";
                        string src = line.Substring(i+1, 11);
                        if (src[0] != '.')
                            str += "U";
                        if (src[1] != '.')
                            str += "D";
                        if (src[2] != '.')
                            str += "L";
                        if (src[3] != '.')
                            str += "R";
                        if (src[6] != '.')
                            str += "B";
                        if (src[7] != '.')
                            str += "A";
                        res.Add(str);
                    }
                    catch { }
                }
            }
            return res.ToArray();
        }
        public static string to_bz2_format(string[] lines, bool header)
        {
            char[] keys = new char[] { 'A', 'B', 'U', 'D', 'L', 'R' };
            StringBuilder sb = new StringBuilder();
            foreach (string line in lines)
            {
                string str = Resources.bk2_line;
                foreach (char k in keys)
                {
                    if (!line.Contains(k))
                        str = str.Replace(k, '.');
                }
                sb.AppendLine(str);
            }
            return header ? Resources.bk2_headers.Replace("$CONTENT$", sb.ToString()) : sb.ToString();
        }
    }
}
