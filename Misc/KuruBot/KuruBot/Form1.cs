using System;
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
        HelirinState? hs = null;
        Bot b = null;

        public Form1()
        {
            InitializeComponent();
            mapc = new MapControl(null, showGMap.Checked, showPMap.Checked, showCostMap.Checked);
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
                mapc.SetSettings(map, showGMap.Checked, showPMap.Checked, showCostMap.Checked);
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
            mapc.SetSettings(map, showGMap.Checked, showPMap.Checked, showCostMap.Checked);
        }

        Action[] last_inputs = null;
        HelirinState[] last_positions = null;
        HelirinState[] last_positions_emu = null;
        private void ExecuteInputs(string[] inputs)
        {
            if (inputs == null || !hs.HasValue || phy == null)
                return;

            List<Action> last_inputs = new List<Action>();
            List<HelirinState> last_positions = new List<HelirinState>();
            last_positions.Add(hs.Value);
            foreach (string input in inputs)
            {
                Action a = Controller.effect_to_action(Controller.string_to_effect(input));
                hs = phy.Next(hs.Value, a);
                last_inputs.Add(a);
                last_positions.Add(hs.Value);
            }

            this.last_inputs = last_inputs.ToArray();
            this.last_positions = last_positions.ToArray();
            last_positions_emu = null;
            mapc.SetHelirin(hs);
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

        HelirinState? hs_bkp = null;
        private void bkpPos_Click(object sender, EventArgs e)
        {
            hs_bkp = hs;
        }

        private void restorePos_Click(object sender, EventArgs e)
        {
            hs = hs_bkp;
            mapc.SetHelirin(hs);
        }

        private void convertInputs_Click(object sender, EventArgs e)
        {
            try
            {
                if (inputsFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filename = inputsFileDialog.FileName;
                    string new_filename = Path.Combine(Path.GetDirectoryName(filename), Path.GetFileNameWithoutExtension(filename));
                    new_filename += "_conv.txt";
                    File.WriteAllLines(new_filename, Controller.from_bz2_format(File.ReadAllLines(filename)));
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
                if (inputsFileDialog.ShowDialog() == DialogResult.OK)
                    ExecuteInputs(File.ReadAllLines(inputsFileDialog.FileName));
            }
            catch { }
        }

        private void savePos_Click(object sender, EventArgs e)
        {
            if (!hs.HasValue)
                return;
            if (saveLogFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string res = Com.hs_to_string(hs.Value);
                    File.WriteAllText(saveLogFileDialog.FileName, res);
                }
                catch { }
            }
        }

        private void loadPos_Click(object sender, EventArgs e)
        {
            try
            {
                if (inputsFileDialog.ShowDialog() == DialogResult.OK)
                {
                    hs = Com.parse_hs(File.ReadAllLines(inputsFileDialog.FileName)[0]);
                    mapc.SetHelirin(hs);
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
                if (inputsFileDialog.ShowDialog() == DialogResult.OK)
                {
                    map = Com.parse_map(File.ReadAllLines(inputsFileDialog.FileName));
                    if (map != null)
                        phy = new Physics(map);
                    mapc.SetSettings(map, showGMap.Checked, showPMap.Checked, showCostMap.Checked);
                }

            }
            catch { }
        }

        private void computeCostMap_Click(object sender, EventArgs e)
        {
            if (b != null)
            {
                b.ComputeNewCostMaps(40, 40, Flooding.WallClipSetting.Allow);
                mapc.SetCostMap(b.GetCurrentCostMap());
            }
        }

        private void buildSolver_Click(object sender, EventArgs e)
        {
            if (map != null)
                b = new Bot(map, new Flooding.Pixel(0,0), new Flooding.Pixel(map.WidthPx, map.HeightPx));
        }
    }
}
