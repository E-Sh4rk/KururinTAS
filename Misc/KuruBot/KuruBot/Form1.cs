using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace KuruBot
{
    public partial class Form1 : Form
    {
        internal Com com = null;
        internal Map map = null;
        MapControl mapc = null;

        public Form1()
        {
            InitializeComponent();
            mapc = new MapControl(this);
            mapc.Dock = DockStyle.Fill;
            main_panel.Controls.Add(mapc);
            mapc.SetSettings(showGMap.Checked, showPMap.Checked);
        }

        private void connect_Click(object sender, EventArgs e)
        {
            if (kuruComFolderDialog.ShowDialog() == DialogResult.OK)
                com = new Com(kuruComFolderDialog.SelectedPath);
        }

        private void dlMap_Click(object sender, EventArgs e)
        {
            if (com != null)
            {
                map = com.DownloadMap();
                mapc.Refresh();
            }
        }

        private void showGPMap_CheckedChanged(object sender, EventArgs e)
        {
            mapc.SetSettings(showGMap.Checked, showPMap.Checked);
        }
    }
}
