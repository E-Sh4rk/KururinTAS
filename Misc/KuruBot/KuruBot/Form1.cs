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
        internal Com com = null;
        internal Map map = null;
        MapControl mapc = null;

        public Form1()
        {
            InitializeComponent();
            mapc = new MapControl(this, showGMap.Checked, showPMap.Checked);
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
                mapc.Redraw();
            }
        }

        private void showPosition_Click(object sender, EventArgs e)
        {
            if (com != null)
            {
                HelirinState? h = com.GetHelirin();
                if (h.HasValue)
                    mapc.SetHelirin(Physics.ToGraphicalHelirin(h.Value));
                else
                    mapc.SetHelirin(null);
            }
        }

        private void showGPMap_CheckedChanged(object sender, EventArgs e)
        {
            mapc.SetSettings(showGMap.Checked, showPMap.Checked);
        }

    }
}
