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

        public class GraphicalHelirin
        {
            public GraphicalHelirin(int px, int py, float a)
            {
                pixelX = px;
                pixelY = py;
                angle = a;
            }

            public int pixelX;
            public int pixelY;
            public float angle;
        }

        Map m = null;
        bool showG = true;
        bool showP = false;
        Bitmap bmap = null;
        GraphicalHelirin helirin = null;

        public MapControl(Map m, bool showG, bool showP)
        {
            InitializeComponent();
            BackColor = Color.White;
            SetSettings(m, showG, showP);
        }

        public void SetSettings(Map m, bool showG, bool showP)
        {
            this.m = m;
            this.showG = showG;
            this.showP = showP;
            Redraw();
        }

        public void SetHelirin(HelirinState? st)
        {
            if (st.HasValue)
                helirin = Physics.ToGraphicalHelirin(st.Value);
            else
                helirin = null;
            Refresh();
        }

        protected void Redraw()
        {
            if (m != null)
            {
                int width = m.Width * Map.tile_size;
                int height = m.Height * Map.tile_size;

                Bitmap bitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                Graphics g = Graphics.FromImage(bitmap);
                g.Clear(BackColor);

                if (showP)
                {
                    Brush myBrush = Brushes.Red;
                    for (int y = 0; y < m.HeightPx; y++)
                    {
                        for (int x = 0; x < m.WidthPx; x++)
                        {
                            if (m.IsPixelInCollision(x, y))
                            {
                                Rectangle dest = new Rectangle(x, y, 1, 1);
                                g.FillRectangle(myBrush, dest);
                            }
                        }
                    }
                }

                if (showG)
                {
                    for (int y = 0; y < m.Height; y++)
                    {
                        for (int x = 0; x < m.Width; x++)
                        {
                            Rectangle? sprite = m.GetTileSprite(m.TileAt(x, y));
                            if (sprite.HasValue)
                            {
                                Rectangle dest = new Rectangle(x * Map.tile_size, y * Map.tile_size, Map.tile_size, Map.tile_size);
                                g.DrawImage(Resources.sprites, dest, sprite.Value, GraphicsUnit.Pixel);
                            }
                        }
                    }
                }

                bmap = bitmap;
            }
            else
                bmap = null;

            Refresh();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);

            Graphics g = CreateGraphics();
            g.Clear(BackColor);

            int start_x = 0;
            int start_y = 0;

            if (bmap != null)
            {
                start_x = (Width - bmap.Width) / 2;
                start_y = (Height - bmap.Height) / 2;
                g.DrawImage(bmap, start_x, start_y);
            }
                
            if (helirin != null)
            {
                Pen myPen = new Pen(Color.Black, 1);
                g.DrawEllipse(myPen, start_x + helirin.pixelX - Map.helirin_radius, start_y + helirin.pixelY - Map.helirin_radius, Map.helirin_radius * 2, Map.helirin_radius * 2);
                g.DrawEllipse(myPen, start_x + helirin.pixelX - 2, start_y + helirin.pixelY - 2, 4, 4);
                Size o = new Size(start_x, start_y);
                Point p1 = new Point((int)(helirin.pixelX+KuruMath.Sin(helirin.angle)*Map.helirin_radius), (int)(helirin.pixelY - KuruMath.Cos(helirin.angle) * Map.helirin_radius));
                Point p2 = new Point((int)(helirin.pixelX - KuruMath.Sin(helirin.angle) * Map.helirin_radius), (int)(helirin.pixelY + KuruMath.Cos(helirin.angle) * Map.helirin_radius));
                g.DrawLine(myPen, Point.Add(p1,o), Point.Add(p2,o));
            }
        }
    }
}
