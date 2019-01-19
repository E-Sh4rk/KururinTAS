﻿using KuruBot.Properties;
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

        Map m = null;
        bool showC = false;
        bool showG = true;
        bool showP = false;
        Bitmap bmap = null;
        GraphicalHelirin? helirin = null;
        float[,] cost_map = null;

        public MapControl(Map m, bool showG, bool showP, bool showC)
        {
            InitializeComponent();
            BackColor = Color.White;
            SetSettings(m, showG, showP, showC);
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

        public void SetCostMap(float[,] cost_map)
        {
            this.cost_map = cost_map;
            if (showC)
                Redraw();
        }

        protected void Redraw()
        {
            if (m != null)
            {
                int width = m.Width * Map.tile_size;
                int height = m.Height * Map.tile_size;

                int start_x = 0;
                int start_y = 0;
                Bitmap bitmap = null;
                if (showP)
                {
                    start_x = width;
                    start_y = height;
                    bitmap = new Bitmap(3*width, 3*height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
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
                if (showP)
                {
                    start_x += (int)(bmap.Width * scale / 3);
                    start_y += (int)(bmap.Height * scale / 3);
                }
            }
                
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
