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

        public struct GraphicalHelirin
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

        Form1 p;

        Map m = null;
        bool showC = false;
        bool showG = true;
        bool showP = false;
        Bitmap bmap = null;
        GraphicalHelirin? helirin = null;
        float[,] cost_map = null;
        Flooding.Pixel? cost_map_start_pixel = null;

        public MapControl(Form1 p, Map m, bool showG, bool showP, bool showC)
        {
            InitializeComponent();
            BackColor = Color.White;
            SetSettings(m, showG, showP, showC);

            this.p = p;
            MouseClick += Control1_MouseClick;
        }

        private void Control1_MouseClick(Object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                GraphicalHelirin gh = new GraphicalHelirin((int)((e.X - last_start_x) / last_scale), (int)((e.Y - last_start_y) / last_scale), 0);
                p.SetHelirinState(Physics.FromGraphicalHelirin(gh, true));
            }
            if (e.Button == MouseButtons.Right)
            {
                GraphicalHelirin gh = new GraphicalHelirin((int)((e.X - last_start_x) / last_scale), (int)((e.Y - last_start_y) / last_scale), 0);
                p.SetHelirinState(Physics.FromGraphicalHelirin(gh, false));
            }
        }

            public void SetSettings(Map m, bool showG, bool showP, bool showC)
        {
            this.m = m;
            this.showG = showG;
            this.showP = showP;
            this.showC = showC;
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

        public void SetCostMap(float[,] cost_map, Flooding.Pixel start_px)
        {
            this.cost_map = cost_map;
            this.cost_map_start_pixel = start_px;
            if (showC)
                Redraw();
        }

        protected void Redraw()
        {
            if (m != null)
            {
                int width = m.WidthPx;
                int height = m.HeightPx;

                int start_x = 0;
                int start_y = 0;
                Bitmap bitmap = null;

                if (showC && cost_map != null)
                    bitmap = new Bitmap(cost_map.GetLength(1), cost_map.GetLength(0), System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                else if (showP)
                {
                    start_x = width;
                    start_y = height;
                    bitmap = new Bitmap(3 * width, 3 * height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                }
                else
                    bitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                Graphics g = Graphics.FromImage(bitmap);
                g.Clear(BackColor);

                Brush startingBrush = new SolidBrush(Color.FromArgb(0x77, 0xD0, 0x00, 0x00));
                Brush healingBrush = new SolidBrush(Color.FromArgb(0x77, 0x40, 0x40, 0xFF));
                Brush endingBrush = new SolidBrush(Color.FromArgb(0x77, 0xD0, 0xD0, 0x00));
                Brush springBrush = new SolidBrush(Color.FromArgb(0x77, 0x80, 0x80, 0x80));
                Brush collisionBrush = Brushes.Red;
                Brush phyStartingBrush = new SolidBrush(Color.FromArgb(0x55, 0xD0, 0x00, 0x00));
                Brush phyHealingBrush = new SolidBrush(Color.FromArgb(0x55, 0x40, 0x40, 0xFF));
                Brush phyEndingBrush = new SolidBrush(Color.FromArgb(0x55, 0xD0, 0xD0, 0x00));
                Brush phySpringBrush = new SolidBrush(Color.FromArgb(0x55, 0x80, 0x80, 0x80));

                if (showC && cost_map != null)
                {
                    float max = Flooding.GetMaxWeightExceptInfinity(cost_map);
                    for (int y = 0; y < bitmap.Height; y++)
                    {
                        for (int x = 0; x < bitmap.Width; x++)
                        {
                            Rectangle dest = new Rectangle(x, y, 1, 1);
                            float cost = cost_map[y, x];
                            int color = cost < float.PositiveInfinity ? (int)((cost/max)*255) : 255;
                            color = 255 - color;
                            Brush brush = new SolidBrush(Color.FromArgb(255,color,color,color));
                            g.FillRectangle(brush, dest);
                        }
                    }
                }
                else
                {
                    if (showP)
                    {

                        for (int y = 0; y < bitmap.Height; y++)
                        {
                            for (int x = 0; x < bitmap.Width; x++)
                            {
                                short xp = (short)(x - start_x);
                                short yp = (short)(y - start_y);

                                Rectangle dest = new Rectangle(x, y, 1, 1);
                                if (m.IsPixelInCollision(xp, yp))
                                    g.FillRectangle(collisionBrush, dest);

                                Map.Zone zone = m.IsPixelInZone(xp, yp);
                                if (zone == Map.Zone.Starting)
                                    g.FillRectangle(phyStartingBrush, dest);
                                else if (zone == Map.Zone.Healing)
                                    g.FillRectangle(phyHealingBrush, dest);
                                else if (zone == Map.Zone.Ending)
                                    g.FillRectangle(phyEndingBrush, dest);

                                Map.Spring[] springs = m.IsPixelInSpring(xp, yp);
                                foreach (Map.Spring s in springs)
                                    g.FillRectangle(phySpringBrush, dest);
                            }
                        }
                    }

                    if (showG)
                    {
                        for (int y = 0; y < m.Height; y++)
                        {
                            for (int x = 0; x < m.Width; x++)
                            {
                                ushort tile = m.TileAt(x, y);

                                Rectangle dest = new Rectangle(start_x + x * Map.tile_size, start_y + y * Map.tile_size, Map.tile_size, Map.tile_size);
                                Rectangle? sprite = m.GetTileSprite(tile);
                                if (sprite.HasValue)
                                    g.DrawImage(Resources.sprites, dest, sprite.Value, GraphicsUnit.Pixel);

                                Map.Zone zone = m.IsTileAZone(tile);
                                if (zone == Map.Zone.Starting)
                                    g.FillRectangle(startingBrush, dest);
                                else if (zone == Map.Zone.Healing)
                                    g.FillRectangle(healingBrush, dest);
                                else if (zone == Map.Zone.Ending)
                                    g.FillRectangle(endingBrush, dest);

                                if (m.IsTileASpring(tile).HasValue)
                                {
                                    dest = new Rectangle(dest.X, dest.Y, Map.spring_size, Map.spring_size);
                                    g.FillRectangle(springBrush, dest);
                                }
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

        int last_start_x = 0;
        int last_start_y = 0;
        float last_scale = 0;
        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);

            Graphics g = CreateGraphics();
            g.Clear(BackColor);

            int start_x = 0;
            int start_y = 0;
            float scale = 1;

            if (bmap != null)
            {
                scale = Math.Min((float)Width / bmap.Width, (float)Height / bmap.Height);
                start_x = (Width - (int)(bmap.Width*scale)) / 2;
                start_y = (Height - (int)(bmap.Height*scale)) / 2;
                g.DrawImage(bmap, start_x, start_y, bmap.Width*scale, bmap.Height*scale);

                if (showC && cost_map_start_pixel.HasValue)
                {
                    start_x += -cost_map_start_pixel.Value.x;
                    start_y += -cost_map_start_pixel.Value.y;
                }
                else if (showP)
                {
                    start_x += (int)(bmap.Width * scale / 3);
                    start_y += (int)(bmap.Height * scale / 3);
                }
            }

            last_start_x = start_x;
            last_start_y = start_y;
            last_scale = scale;
                
            if (helirin != null)
            {
                GraphicalHelirin h = helirin.Value;
                Pen myPen = new Pen(Color.Black, 1);

                int offset_x = h.pixelX - Map.helirin_radius;
                int offset_y = h.pixelY - Map.helirin_radius;
                int size = Map.helirin_radius * 2;
                g.DrawEllipse(myPen, start_x + offset_x*scale, start_y + offset_y*scale, size*scale, size*scale);

                offset_x = h.pixelX - 2;
                offset_y = h.pixelY - 2;
                size = 4;
                g.DrawEllipse(myPen, start_x + offset_x * scale, start_y + offset_y * scale, size * scale, size * scale);

                Size o = new Size(start_x, start_y);
                Point p1 = new Point((int)(h.pixelX + Math.Sin(h.angle) * Map.helirin_radius), (int)(h.pixelY - Math.Cos(h.angle) * Map.helirin_radius));
                Point p2 = new Point((int)(h.pixelX - Math.Sin(h.angle) * Map.helirin_radius), (int)(h.pixelY + Math.Cos(h.angle) * Map.helirin_radius));
                p1 = new Point((int)(p1.X * scale), (int)(p1.Y * scale));
                p2 = new Point((int)(p2.X * scale), (int)(p2.Y * scale));
                g.DrawLine(myPen, Point.Add(p1,o), Point.Add(p2,o));
            }
        }
    }
}
