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
    enum Direction1
    {
        Backward=-1,
        None=0,
        Forward=1
    }
    enum Speed
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
        public static string action_to_string(Action a)
        {
            string res = "";

            if (a != Action.NoButton)
            {
                int i = (int)a - 1;
                // Speed
                Speed speed = (Speed)(i / 8);
                if (speed == Speed.Speed1)
                    res += "A";
                else if (speed == Speed.Speed2)
                    res += "AB";
                // Direction
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
            }

            return res;
        }
    }
}
