using KuruBot.Properties;
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
    public partial class MapControl : Control
    {
        Form1 parent = null;
        bool showG = true;
        bool showP = false;

        public MapControl(Form1 p)
        {
            InitializeComponent();
            BackColor = Color.White;
            parent = p;
        }

        public void SetSettings(bool showG, bool showP)
        {
            this.showG = showG;
            this.showP = showP;
            Refresh();
        }

        int tile_size = 8;
        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
            Graphics g = CreateGraphics();
            g.Clear(BackColor);

            if (parent.map != null)
            {
                Map m = parent.map;
                int width = m.Width * tile_size;
                int height = m.Height * tile_size;
                int start_x = (Width - width) / 2;
                int start_y = (Height - height) / 2;

                Pen myPen = new Pen(Color.Red, 5);
                Brush myBrush = Brushes.Red;

                if (showG)
                {
                    for (int y = 0; y < m.Height; y++)
                    {
                        for (int x = 0; x < m.Width; x++)
                        {
                            Rectangle? sprite = m.GetTileSprite(m.TileAt(x, y));
                            if (sprite.HasValue)
                            {
                                Rectangle dest = new Rectangle(start_x + x * tile_size, start_y + y * tile_size, tile_size, tile_size);
                                g.DrawImage(Resources.sprites, dest, sprite.Value, GraphicsUnit.Pixel);
                            }
                        }
                    }
                }
                if (showP)
                {
                    for (int y = 0; y < m.HeightPx; y++)
                    {
                        for (int x = 0; x < m.WidthPx; x++)
                        {
                            if (m.IsPixelInCollision(x,y))
                            {
                                Rectangle dest = new Rectangle(start_x + x, start_y + y, 1, 1);
                                g.FillRectangle(myBrush, dest);
                            }
                        }
                    }
                }
            }
        }
    }
}
