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

        private void ExecuteInputs(string[] inputs)
        {
            if (inputs == null || !hs.HasValue || phy == null)
                return;
            foreach (string input in inputs)
                hs = phy.Next(hs.Value, Controller.effect_to_action(Controller.string_to_effect(input)));
            mapc.SetHelirin(hs);
        }

        private void enterInputString_Click(object sender, EventArgs e)
        {
            try
            {
                if (inputsFileDialog.ShowDialog() == DialogResult.OK)
                    ExecuteInputs(File.ReadAllLines(inputsFileDialog.FileName));
            } catch { }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.NumPad9)
            {
                e.Handled = true;
                Action a = Action.NoButton;
                switch (e.KeyCode)
                {
                    case Keys.NumPad1:
                        a = Action.DownLeft1;
                        break;
                    case Keys.NumPad2:
                        a = Action.Down1;
                        break;
                    case Keys.NumPad3:
                        a = Action.DownRight1;
                        break;
                    case Keys.NumPad4:
                        a = Action.Left1;
                        break;
                    case Keys.NumPad6:
                        a = Action.Right1;
                        break;
                    case Keys.NumPad7:
                        a = Action.UpLeft1;
                        break;
                    case Keys.NumPad8:
                        a = Action.Up1;
                        break;
                    case Keys.NumPad9:
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
                    hs = phy.Next(hs.Value, a);
                    mapc.SetHelirin(hs);
                }
            }
        }

        private void downloadInputs_Click(object sender, EventArgs e)
        {
            HelirinState? hs = null;
            string[] inputs = com.DownloadInputs(out hs);
            this.hs = hs;
            mapc.SetHelirin(hs);
            ExecuteInputs(inputs);
        }
    }
}