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
        bool drawMode = false;

        Map m = null;
        bool showC = false;
        bool showG = true;
        bool showP = false;
        Bitmap bmap = null;
        GraphicalHelirin? helirin = null;

        CostMap cost_map = null;
        int cost_map_real_invul = 0;
        Pixel? start_pixel = null;
        Pixel? end_pixel = null;
        Color highlight_color = Color.Red;
        bool[,] highlight_map = null;

        Pixel computed_start_pixel;
        int computed_bmp_start_x = 0;
        int computed_bmp_start_y = 0;
        float computed_scale = 0;

        public MapControl(Form1 p, Map m, bool showG, bool showP, bool showC)
        {
            InitializeComponent();
            BackColor = Color.White;
            SetSettings(m, showG, showP, showC);

            this.p = p;
            MouseClick += Control1_MouseClick;
        }

        public Bitmap GetBackground()
        {
            return bmap;
        }

        public void SetFixedWindow(Pixel? start, Pixel? end)
        {
            start_pixel = start;
            end_pixel = end;
            Redraw();
        }

        public void SetDrawMode(bool draw_mode, Color? color = null)
        {
            if (draw_mode != drawMode)
            {
                drawMode = draw_mode;
                if (draw_mode)
                {
                    MouseClick -= Control1_MouseClick;
                    MouseMove += Control1_MouseMove;
                    MouseDown += Control1_MouseDown;
                }
                else
                {
                    MouseMove -= Control1_MouseMove;
                    MouseDown -= Control1_MouseDown;
                    MouseClick += Control1_MouseClick;
                }
            }

            if (color.HasValue)
                SetHighlight(color.Value, highlight_map);
        }

        bool[,] CopyBidimArray(bool[,] arr)
        {
            if (arr == null)
                return null;
            bool[,] res = new bool[arr.GetLength(0), arr.GetLength(1)];
            for (int i = 0; i < arr.GetLength(0); i++)
                for (int j = 0; j < arr.GetLength(1); j++)
                    res[i, j] = arr[i, j];
            return res;
        }

        public bool[,] GetHighlightMap()
        {
            return CopyBidimArray(highlight_map);
        }

        private void Control1_MouseClick(Object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                GraphicalHelirin gh = new GraphicalHelirin((int)((e.X - computed_bmp_start_x) / computed_scale + computed_start_pixel.x),
                                                           (int)((e.Y - computed_bmp_start_y) / computed_scale + computed_start_pixel.y), 0);
                p.SetHelirinState(Physics.FromGraphicalHelirin(gh, true));
            }
            if (e.Button == MouseButtons.Right)
            {
                GraphicalHelirin gh = new GraphicalHelirin((int)((e.X - computed_bmp_start_x) / computed_scale + computed_start_pixel.x),
                                                           (int)((e.Y - computed_bmp_start_y) / computed_scale + computed_start_pixel.y), 0);
                p.SetHelirinState(Physics.FromGraphicalHelirin(gh, false));
            }
        }

        private int euclidian_dist_squared(int x1, int y1, int x2, int y2)
        {
            return (x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1);
        }

        public int brush_radius_squared = 64;
        private void Control1_MouseMove(Object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (highlight_map == null)
                    highlight_map = new bool[bmap.Height, bmap.Width];

                int x = (int)((e.X - computed_bmp_start_x) / computed_scale);
                int y = (int)((e.Y - computed_bmp_start_y) / computed_scale);
                bool[,] new_map = new bool[highlight_map.GetLength(0), highlight_map.GetLength(1)];
                for (int i = 0; i < new_map.GetLength(0); i++)
                {
                    for (int j = 0; j < new_map.GetLength(1); j++)
                    {
                        if (highlight_map[i, j] || euclidian_dist_squared(x, y, j, i) <= brush_radius_squared)
                            new_map[i, j] = true;
                    }
                }
                SetHighlight(highlight_color, new_map);
            }
        }
        private void Control1_MouseDown(Object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                ResetHighlight();
        }

        public void SetHighlight(Color c, bool[,] map)
        {
            if (map == null || map.GetLength(0) != bmap.Height || map.GetLength(1) != bmap.Width)
            {
                highlight_color = c;
                if (highlight_map != null)
                {
                    highlight_map = null;
                    Refresh();
                }
            }
            else if (!c.Equals(highlight_color) || highlight_map == null
                  || map.GetLength(0) != highlight_map.GetLength(0) || map.GetLength(1) != highlight_map.GetLength(1))
            {
                highlight_map = CopyBidimArray(map);
                highlight_color = c;
                Refresh();
            }
            else
            {
                Graphics g = CreateGraphics();
                Brush b = new SolidBrush(highlight_color);
                int size = (int)Math.Ceiling(computed_scale);
                for (int i = 0; i < map.GetLength(0); i++)
                {
                    for (int j = 0; j < map.GetLength(1); j++)
                    {
                        if (map[i, j] && !highlight_map[i, j])
                        {
                            highlight_map[i, j] = true;
                            Rectangle dest = new Rectangle((int)(computed_bmp_start_x + j*computed_scale), (int)(computed_bmp_start_y + i*computed_scale), size, size);
                            g.FillRectangle(b, dest);
                        }
                    }
                }
            }
        }
        public void ResetHighlight()
        {
            highlight_map = null;
            Refresh();
        }

        public void SetSettings(Map m, bool showG, bool showP, bool showC)
        {
            this.m = m;
            this.showG = showG;
            this.showP = showP;
            this.showC = showC;
            if (!start_pixel.HasValue || !end_pixel.HasValue)
                highlight_map = null;
            Redraw();
        }

        public void SetHelirin(HelirinState st)
        {
            if (st != null)
                helirin = Physics.ToGraphicalHelirin(st);
            else
                helirin = null;
            Refresh();
        }

        public void SetCostMap(CostMap cost_map, int cost_map_real_invul)
        {
            this.cost_map = cost_map;
            this.cost_map_real_invul = cost_map_real_invul;
            highlight_map = null;
            if (showC)
                Redraw();
        }

        protected void Redraw()
        {
            if (m != null)
            {
                Bitmap bitmap;
                int width = showP ? 3* m.WidthPx : m.WidthPx;
                int height = showP ? 3 * m.HeightPx : m.HeightPx;
                int start_x = showP ? -m.WidthPx : 0;
                int start_y = showP ? -m.HeightPx : 0;

                if (start_pixel.HasValue)
                {
                    start_x = start_pixel.Value.x;
                    start_y = start_pixel.Value.y;
                }
                if (start_pixel.HasValue && end_pixel.HasValue)
                {
                    width = end_pixel.Value.x - start_pixel.Value.x + 1;
                    height = end_pixel.Value.y - start_pixel.Value.y + 1;
                }
                if (showC && cost_map != null)
                {
                    width = Math.Min(width,cost_map.Width);
                    height = Math.Min(height,cost_map.Height);
                }
                bitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                computed_start_pixel = new Pixel((short)start_x, (short)start_y);

                Graphics g = Graphics.FromImage(bitmap);
                g.Clear(BackColor);

                Brush startingBrush = new SolidBrush(Color.FromArgb(0x77, 0xD0, 0x00, 0x00));
                Brush healingBrush = new SolidBrush(Color.FromArgb(0x77, 0x40, 0x40, 0xFF));
                Brush endingBrush = new SolidBrush(Color.FromArgb(0x77, 0xD0, 0xD0, 0x00));
                Brush springBrush = new SolidBrush(Color.FromArgb(0x77, 0x80, 0x80, 0x80));
                Brush collisionBrush = Brushes.SlateGray;
                Brush phyStartingBrush = new SolidBrush(Color.FromArgb(0x55, 0xD0, 0x00, 0x00));
                Brush phyHealingBrush = new SolidBrush(Color.FromArgb(0x55, 0x40, 0x40, 0xFF));
                Brush phyEndingBrush = new SolidBrush(Color.FromArgb(0x55, 0xD0, 0xD0, 0x00));
                Brush phySpringBrush = new SolidBrush(Color.FromArgb(0x55, 0x80, 0x80, 0x80));

                if (showC && cost_map != null)
                {
                    float max = cost_map.GetMaxWeightExceptInfinity(cost_map_real_invul);
                    if (max <= 0)
                        max = float.Epsilon;
                    for (int y = 0; y < bitmap.Height; y++)
                    {
                        for (int x = 0; x < bitmap.Width; x++)
                        {
                            Rectangle dest = new Rectangle(x, y, 1, 1);
                            float cost = cost_map.CostAtIndex(x, y, cost_map_real_invul);
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
                                short xp = (short)(x + start_x);
                                short yp = (short)(y + start_y);

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
                        for (int y = 0; y < bitmap.Height; y+=Map.tile_size)
                        {
                            for (int x = 0; x < bitmap.Width; x+=Map.tile_size)
                            {
                                int tile_x = x + start_x;
                                int tile_y = y + start_y;
                                if (tile_x < 0 || tile_y < 0 || tile_x >= m.WidthPx || tile_y >= m.HeightPx)
                                    continue;
                                ushort tile = m.TileAt(tile_x / Map.tile_size, tile_y / Map.tile_size);

                                int x_offset = tile_x % Map.tile_size;
                                int y_offset = tile_y % Map.tile_size;
                                Rectangle dest = new Rectangle(x-x_offset, y-y_offset, Map.tile_size, Map.tile_size);
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

                computed_bmp_start_x = start_x;
                computed_bmp_start_y = start_y;
            }

            computed_bmp_start_x = start_x;
            computed_bmp_start_y = start_y;
            computed_scale = scale;

            if (highlight_map != null)
            {
                Brush b = new SolidBrush(highlight_color);
                int size = (int)Math.Ceiling(computed_scale);
                for (int i = 0; i < highlight_map.GetLength(0); i++)
                {
                    for (int j = 0; j < highlight_map.GetLength(1); j++)
                    {
                        if (highlight_map[i, j])
                        {
                            Rectangle dest = new Rectangle((int)(computed_bmp_start_x + j * computed_scale), (int)(computed_bmp_start_y + i * computed_scale), size, size);
                            g.FillRectangle(b, dest);
                        }
                    }
                }
            }

            int real_start_x = start_x - (int)(computed_start_pixel.x * scale);
            int real_start_y = start_y - (int)(computed_start_pixel.y * scale);
            // TODO: only if bonus not already owned
            if (m != null) {
                Rectangle? bonus = m.GetBonusPxRect();
                if (bonus.HasValue)
                {
                    float x = bonus.Value.X;
                    float y = bonus.Value.Y;
                    float w = bonus.Value.Width;
                    float h = bonus.Value.Height;
                    Brush b = new SolidBrush(Color.MediumPurple);
                    g.FillRectangle(b, real_start_x + x * scale, real_start_y + y * scale, w * scale, h * scale);
                }
            }
                
            if (helirin != null)
            {
                GraphicalHelirin h = helirin.Value;
                Pen myPen = new Pen(Color.Black, 1);

                int offset_x = h.pixelX - Map.helirin_radius;
                int offset_y = h.pixelY - Map.helirin_radius;
                int size = Map.helirin_radius * 2;
                g.DrawEllipse(myPen, real_start_x + offset_x*scale, real_start_y + offset_y*scale, size*scale, size*scale);

                offset_x = h.pixelX - 2;
                offset_y = h.pixelY - 2;
                size = 4;
                g.DrawEllipse(myPen, real_start_x + offset_x*scale, real_start_y + offset_y*scale, size*scale, size*scale);

                Size o = new Size(real_start_x, real_start_y);
                Point p1 = new Point((int)(h.pixelX + Math.Sin(h.angle) * Map.helirin_radius), (int)(h.pixelY - Math.Cos(h.angle) * Map.helirin_radius));
                Point p2 = new Point((int)(h.pixelX - Math.Sin(h.angle) * Map.helirin_radius), (int)(h.pixelY + Math.Cos(h.angle) * Map.helirin_radius));
                p1 = new Point((int)(p1.X * scale), (int)(p1.Y * scale));
                p2 = new Point((int)(p2.X * scale), (int)(p2.Y * scale));
                g.DrawLine(myPen, Point.Add(p1,o), Point.Add(p2,o));
            }
        }
    }
}
