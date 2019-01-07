﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace KuruBot
{
    public partial class Form1 : Form
    {
        Com com = null;
        Map map = null;
        MapControl mapc = null;
        Physics phy = null;
        HelirinState? hs = new HelirinState();

        public Form1()
        {
            InitializeComponent();
            mapc = new MapControl(null, showGMap.Checked, showPMap.Checked);
            mapc.Dock = DockStyle.Fill;
            main_panel.Controls.Add(mapc);
        }

        private void connect_Click(object sender, EventArgs e)
        {
            if (kuruComFileDialog.ShowDialog() == DialogResult.OK)
                com = new Com(Path.GetDirectoryName(kuruComFileDialog.FileName));
        }

        private void dlMap_Click(object sender, EventArgs e)
        {
            if (com != null)
            {
                map = com.DownloadMap();
                if (map != null)
                    phy = new Physics(map);
                mapc.SetSettings(map, showGMap.Checked, showPMap.Checked);
            }
        }

        private void showPosition_Click(object sender, EventArgs e)
        {
            if (com != null)
            {
                hs = com.GetHelirin();
                mapc.SetHelirin(hs);
            }
        }

        private void showGPMap_CheckedChanged(object sender, EventArgs e)
        {
            mapc.SetSettings(map, showGMap.Checked, showPMap.Checked);
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar >= 97 && e.KeyChar <= 105)
            {
                e.Handled = true;
                Action a = Action.NoButton;
                switch (e.KeyChar)
                {
                    case (char)97:
                        a = Action.DownLeft1;
                        break;
                    case (char)98:
                        a = Action.Down1;
                        break;
                    case (char)99:
                        a = Action.DownRight1;
                        break;
                    case (char)100:
                        a = Action.Left1;
                        break;
                    case (char)102:
                        a = Action.Right1;
                        break;
                    case (char)103:
                        a = Action.UpLeft1;
                        break;
                    case (char)104:
                        a = Action.Up1;
                        break;
                    case (char)105:
                        a = Action.UpRight1;
                        break;
                }
                Speed speed = Speed.Speed0;
                if (ModifierKeys.HasFlag(Keys.Alt))
                    speed = Speed.Speed2;
                else if (ModifierKeys.HasFlag(Keys.Control))
                    speed = Speed.Speed1;
                a = Controller.change_action_speed(a, speed);

                if (hs.HasValue && phy != null)
                {
                    hs = phy.Next(hs.Value);
                    mapc.SetHelirin(hs);
                }
            }
        }

        private void enterInputString_Click(object sender, EventArgs e)
        {
            // TODO
        }
    }
}
