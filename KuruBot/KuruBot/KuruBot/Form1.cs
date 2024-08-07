﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace KuruBot
{
    public partial class Form1 : Form
    {
        Com com = null;
        Map map = null;
        MapControl mapc = null;
        Physics phy = null;
        HelirinState hs = null;
        Bot b = null;
        Thread thread = null;

        const string config_default_file = "config.ini";
        public Form1()
        {
            InitializeComponent();
            
            if (File.Exists(config_default_file))
                Settings.LoadConfig(config_default_file);
            else
                Settings.SaveConfig(config_default_file);

            mapc = new MapControl(this, null, showGMap.Checked, showPMap.Checked, showObjects.Checked, showCostMap.Checked);
            mapc.Dock = DockStyle.Fill;
            main_panel.Controls.Add(mapc);

            updateSCMbutton();
            initLastActionDropdown();
        }

        bool controls_enabled = true;
        private void DisableControls()
        {
            controls_enabled = false;
            foreach (Control c in Controls)
                c.Enabled = false;
            abort.Enabled = true;

            showCostMap.Enabled = true;
            showPMap.Enabled = true;
            showGMap.Enabled = true;
            showObjects.Enabled = true;
            bonusCostMap.Enabled = true;
            switchCostMap.Enabled = true;
            invulCostMap.Enabled = true;
        }
        private void EnableControls()
        {
            controls_enabled = true;
            foreach (Control c in Controls)
                c.Enabled = true;
            abort.Enabled = false;
        }

        private void TaskStarted()
        {
            this.UIThread(() => { DisableControls(); mapc.ResetHighlight(); progressBar.Value = 0; });
        }
        private void TaskEnded()
        {
            this.UIThread(() => { EnableControls(); mapc.ResetHighlight(); progressBar.Value = 0; thread = null; });
        }
        private void SolverEnd(Action[] res)
        {
            this.UIThread(() => {
                if (res == null)
                    MessageBox.Show(this, "No solution found!", "No solution");
                else
                {
                    int nbInputChanges = 0;
                    Action? lastAction = null;
                    foreach (Action a in res)
                    {
                        if (a != lastAction)
                        {
                            lastAction = a;
                            nbInputChanges++;
                        }
                    }
                    MessageBox.Show(this, "Solution found : " + res.Length + " frames, " + nbInputChanges + " input changes.", "Solution");
                    ExecuteInputs(res);
                }
            });
        }

        public void SetHelirinState(HelirinState st)
        {
            this.UIThread(() => { hs = st; mapc.SetHelirin(hs); updateLastActionDropdown(); } );   
        }
        public void UpdateProgressBarAndHighlight(float value, bool[,] highlight)
        {
            this.UIThread(() => {
                if (value < 0)
                    progressBar.Value = 0;
                else if (value > 100)
                    progressBar.Value = 100;
                else
                    progressBar.Value = (int)value;
                if (mapc != null)
                    mapc.SetHighlight(Color.Red, highlight);
            });
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
                mapc.SetSettings(map, showGMap.Checked, showPMap.Checked, showObjects.Checked, showCostMap.Checked);
            }
        }

        private void showPosition_Click(object sender, EventArgs e)
        {
            if (com != null)
            {
                hs = com.GetHelirin();
                mapc.SetHelirin(hs);
                updateLastActionDropdown();
            }
        }

        private void showGPMap_CheckedChanged(object sender, EventArgs e)
        {
            mapc.SetSettings(map, showGMap.Checked, showPMap.Checked, showObjects.Checked, showCostMap.Checked);
        }

        Action[] last_inputs = null;
        HelirinState[] last_positions = null;
        HelirinState[] last_positions_emu = null;
        private void ExecuteInputs(Action[] inputs)
        {
            if (inputs == null || hs == null || phy == null)
                return;

            List<Action> last_inputs = new List<Action>();
            List<HelirinState> last_positions = new List<HelirinState>();
            last_positions.Add(hs);
            foreach (Action input in inputs)
            {
                hs = phy.Next(hs, input);
                last_inputs.Add(input);
                last_positions.Add(hs);
            }

            this.last_inputs = last_inputs.ToArray();
            this.last_positions = last_positions.ToArray();
            last_positions_emu = null;
            mapc.SetHelirin(hs);
            updateLastActionDropdown();
        }
        private void ExecuteInputsStr(string[] inputs)
        {
            if (inputs == null || hs == null || phy == null)
                return;

            List<Action> a_inputs = new List<Action>();
            foreach (string input in inputs)
                a_inputs.Add(Controller.effect_to_action(Controller.string_to_effect(input)));

            ExecuteInputs(a_inputs.ToArray());
        }

        private void playMove_KeyDown(object sender, KeyEventArgs e)
        {
            if (!controls_enabled)
                return;
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

                if (hs != null && phy != null)
                {
                    hs = phy.Next(hs, a);
                    mapc.SetHelirin(hs);
                    updateLastActionDropdown();
                }
            }
        }

        private void downloadInputs_Click(object sender, EventArgs e)
        {
            if (com == null)
                return;
            HelirinState hs = null;
            string[] inputs = com.DownloadInputs(out hs);
            this.hs = hs;
            mapc.SetHelirin(hs);
            updateLastActionDropdown();
            ExecuteInputsStr(inputs);
        }

        HelirinState hs_bkp = null;
        private void bkpPos_Click(object sender, EventArgs e)
        {
            hs_bkp = hs;
        }

        private void restorePos_Click(object sender, EventArgs e)
        {
            hs = hs_bkp;
            mapc.SetHelirin(hs);
            updateLastActionDropdown();
        }

        private void convertInputsFromBk2_Click(object sender, EventArgs e)
        {
            try
            {
                if (loadFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filename = loadFileDialog.FileName;
                    string new_filename = Path.Combine(Path.GetDirectoryName(filename), Path.GetFileNameWithoutExtension(filename));
                    new_filename += "_conv.txt";
                    File.WriteAllLines(new_filename, Controller.from_bz2_format(File.ReadAllLines(filename)));
                }
            }
            catch { }
        }

        private void convertInputsToBk2_Click(object sender, EventArgs e)
        {
            try
            {
                if (loadFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filename = loadFileDialog.FileName;
                    string new_filename = Path.Combine(Path.GetDirectoryName(filename), Path.GetFileNameWithoutExtension(filename));
                    new_filename += "_conv.txt";
                    File.WriteAllText(new_filename, Controller.to_bz2_format(File.ReadAllLines(filename), true));
                }
            }
            catch { }
        }

        private void sendLastInputs_Click(object sender, EventArgs e)
        {
            if (com != null && last_inputs != null && last_positions != null)
                last_positions_emu = com.Play(last_positions[0], last_inputs);
        }

        private void logLastMoves_Click(object sender, EventArgs e)
        {
            if (saveLogFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string res = "";
                    res += Com.hs_to_string(last_positions[0]);
                    if (last_positions_emu != null)
                        res += Com.hs_to_string(last_positions_emu[0]);
                    res += "----------\n";
                    for (int i = 0; i < last_inputs.Length; i++)
                    {
                        res += Controller.effect_to_string(Controller.action_to_effect(last_inputs[i])) + "\n";
                        res += Com.hs_to_string(last_positions[i+1]);
                        if (last_positions_emu != null)
                            res += Com.hs_to_string(last_positions_emu[i+1]);
                        res += "----------\n";
                    }
                    File.WriteAllText(saveLogFileDialog.FileName, res);
                }
                catch { }
            }
        }

        private void saveInputs_Click(object sender, EventArgs e)
        {
            if (last_inputs == null)
                return;
            if (saveLogFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string res = "";
                    foreach (Action input in last_inputs)
                        res += Controller.effect_to_string(Controller.action_to_effect(input)) + "\n";
                    File.WriteAllText(saveLogFileDialog.FileName, res);
                }
                catch { }
            }
        }

        private void enterInputString_Click(object sender, EventArgs e)
        {
            try
            {
                if (loadFileDialog.ShowDialog() == DialogResult.OK)
                    ExecuteInputsStr(File.ReadAllLines(loadFileDialog.FileName));
            }
            catch { }
        }

        private void savePos_Click(object sender, EventArgs e)
        {
            if (hs == null)
                return;
            if (saveLogFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string res = Com.hs_to_string(hs);
                    File.WriteAllText(saveLogFileDialog.FileName, res);
                }
                catch { }
            }
        }

        private void loadPos_Click(object sender, EventArgs e)
        {
            try
            {
                if (loadFileDialog.ShowDialog() == DialogResult.OK)
                {
                    hs = Com.parse_hs(File.ReadAllLines(loadFileDialog.FileName)[0]);
                    mapc.SetHelirin(hs);
                    updateLastActionDropdown();
                }
            }
            catch { }
        }

        private void saveMap_Click(object sender, EventArgs e)
        {
            if (map == null)
                return;
            if (saveLogFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string res = Com.map_to_string(map);
                    File.WriteAllText(saveLogFileDialog.FileName, res);
                }
                catch { }
            }
        }

        private void loadMap_Click(object sender, EventArgs e)
        {
            try
            {
                if (loadFileDialog.ShowDialog() == DialogResult.OK)
                {
                    map = Com.parse_map(File.ReadAllLines(loadFileDialog.FileName));
                    if (map != null)
                        phy = new Physics(map);
                    mapc.SetSettings(map, showGMap.Checked, showPMap.Checked, showObjects.Checked, showCostMap.Checked);
                }

            }
            catch { }
        }

        private void updateLastActionDropdown()
        {
            if (hs == null)
            {
                lastMove.SelectedIndex = 0;
                return;
            }
            if (hs.lastAction.HasValue) lastMove.SelectedIndex = ((int)hs.lastAction.Value) + 1;
            else lastMove.SelectedIndex = 0;
        }
        private void initLastActionDropdown()
        {
            lastMove.Items.Add("-");
            foreach (Action a in Enum.GetValues(typeof(Action)))
            {
                lastMove.Items.Add(Controller.effect_to_string(Controller.action_to_effect(a)));
            }
            updateLastActionDropdown();
        }

        private void updateSCMbutton()
        {
            switchCostMap.Text = "H:" + cm_preview_l.ToString();
            bonusCostMap.Text = cm_preview_bonus ? "B" : "T";
        }
        private void updateCostMapDisplay()
        {
            mapc.SetCostMap(b.GetCostMap((byte)cm_preview_l, (sbyte)invulCostMap.Value, !cm_preview_bonus),
                  CostMap.GetRealInvul((byte)cm_preview_l, (sbyte)invulCostMap.Value));
        }

        int cm_preview_l = Settings.full_life;
        private void switchCostMap_Click(object sender, EventArgs e)
        {
            if (b != null)
            {
                cm_preview_l--;
                if (cm_preview_l < 1)
                    cm_preview_l = Settings.full_life;
                updateCostMapDisplay();
                updateSCMbutton();
            }
        }
        private void invulCostMap_ValueChanged(object sender, EventArgs e)
        {
            if (b != null)
                updateCostMapDisplay();
        }
        bool cm_preview_bonus = false;
        private void bonusCostMap_Click(object sender, EventArgs e)
        {
            if (b != null)
            {
                cm_preview_bonus = !cm_preview_bonus;
                updateCostMapDisplay();
                updateSCMbutton();
            }
        }

        private void computeCM()
        {
            if (b != null)
            {
                thread = new Thread(delegate () {
                    TaskStarted();
                    b.ComputeNewCostMaps();
                    this.UIThread(() => updateCostMapDisplay());
                    TaskEnded();
                });
                thread.Start();
            }
        }

        private void computeCostMap_Click(object sender, EventArgs e)
        {
            computeCM();
        }

        private void makeSolver(Pixel start, Pixel end)
        {
            if (map != null && phy != null)
            {
                thread = new Thread(delegate () {
                    TaskStarted();
                    b = new Bot(this, map, phy, start, end);
                    this.UIThread(() => mapc.SetFixedWindow(start, end));
                    TaskEnded();
                });
                thread.Start();
            }
        }

        private void buildSolver_Click(object sender, EventArgs e)
        {
            if (map == null) return;
            Pixel start = new Pixel(0, 0);
            Pixel end = new Pixel((short)(map.WidthPx - 1), (short)(map.HeightPx - 1));
            makeSolver(start, end);
        }

        private void buildSolverAny_Click(object sender, EventArgs e)
        {
            if (map == null) return;
            Pixel start = new Pixel((short)(-map.WidthPx), 0);
            Pixel end = new Pixel((short)(map.WidthPx - 1), (short)(2 * map.HeightPx - 1));
            makeSolver(start, end);
        }

        private void solve_Click(object sender, EventArgs e)
        {
            if (b != null && hs != null)
            {
                thread = new Thread(delegate () {
                    TaskStarted();
                    SolverEnd(b.Solve(hs, (int)minInvulScore.Value));
                    TaskEnded();
                    GC.Collect();
                });
                thread.Start();
            }
        }

        private void AbortTask()
        {
            if (thread != null)
            {
                try
                {
                    thread.Abort();
                    TaskEnded();
                    GC.Collect();
                }
                catch { }
            }
        }

        private void abort_Click(object sender, EventArgs e)
        {
            AbortTask();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            AbortTask();
        }

        private void drawOnMap_CheckedChanged(object sender, EventArgs e)
        {
            if (mapc == null || mapc.GetBackground() == null)
            {
                drawOnMap.Checked = false;
                return;
            }
                
            if (drawOnMap.Checked)
            {
                mapc.ResetHighlight();
                mapc.SetDrawMode(true, Color.Blue);
            }
            else
            {
                mapc.SetDrawMode(false);
                mapc.ResetHighlight();
            }
        }

        private void setConstraints_Click(object sender, EventArgs e)
        {
            if (mapc == null || b == null)
                return;
            b.SetConstraints(mapc.GetHighlightMap());
        }

        private void setTarget_Click(object sender, EventArgs e)
        {
            if (mapc == null || b == null)
                return;
            b.SetTarget(mapc.GetHighlightMap());
        }

        private void destroySolver_Click(object sender, EventArgs e)
        {
            b = null;
            if (mapc != null)
            {
                mapc.SetFixedWindow(null, null);
                mapc.SetCostMap(null, 0);
            }
        }

        private void brushRadius_ValueChanged(object sender, EventArgs e)
        {
            if (mapc != null)
                mapc.brush_radius_squared = (int)(brushRadius.Value * brushRadius.Value);
        }

        private void loadConfig_Click(object sender, EventArgs e)
        {
            if (loadIniDialog.ShowDialog() == DialogResult.OK)
            {
                Settings.LoadConfig(loadIniDialog.FileName);
                mapc.Refresh();
            }
        }

        private void saveConfig_Click(object sender, EventArgs e)
        {
            if (saveIniDialog.ShowDialog() == DialogResult.OK)
                Settings.SaveConfig(saveIniDialog.FileName);
        }

        private void copyInputs_Click(object sender, EventArgs e)
        {
            if (last_inputs == null)
                return;
            try
            {
                string[] lines = new string[last_inputs.Length];
                for (int i = 0; i < lines.Length; i++)
                    lines[i] += Controller.effect_to_string(Controller.action_to_effect(last_inputs[i])) + "\n";
                string content = Controller.to_bz2_format(lines, false);
                Clipboard.SetText(content);
            }
            catch { }
        }

        private void pasteInputs_Click(object sender, EventArgs e)
        {
            try
            {
                string[] content = Clipboard.GetText().Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
                ExecuteInputsStr(Controller.from_bz2_format(content));
            }
            catch { }
        }

        private void lastMove_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (hs == null)
            {
                lastMove.SelectedIndex = 0;
                return;
            }
            int i = lastMove.SelectedIndex;
            if (i == 0) hs.lastAction = null;
            else hs.lastAction = (Action)(i-1);
        }
    }
}
