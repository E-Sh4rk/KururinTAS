using KuruBot.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace KuruBot
{
    public class Map
    {
        public const int tile_size = 8;
        public const int helirin_radius = 32; // Not really... This value is only used for rendering.

        // Graphical atrributes
        ushort[,] map = null;
        SortedSet<ushort> void_sprites = null;
        const ushort max_wall_type = 130;
        
        // Physical attributes
        BitArray physical_map = null;

        public Map(ushort[,] map)
        {
            // Initialize graphical map
            this.map = map;
            string[] void_sprites_str = Resources.void_sprites.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            void_sprites = new SortedSet<ushort>();
            foreach (string e in void_sprites_str)
                void_sprites.Add(Convert.ToUInt16(e));

            // Build physical map from graphical map
            Bitmap bmp = Resources.sprites;
            physical_map = new BitArray(HeightPx * WidthPx, false);
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    Rectangle? sprite = GetTileSprite(TileAt(x,y));
                    if (sprite.HasValue)
                    {
                        Rectangle s = sprite.Value;
                        for (int y2 = 0; y2 < tile_size; y2++)
                        {
                            for (int x2 = 0; x2 < tile_size; x2++)
                            {
                                Color c = bmp.GetPixel(s.X+x2, s.Y+y2);
                                int x_phy = x * tile_size + x2;
                                int y_phy = y * tile_size + y2;
                                if (c.A != 255 || c.B != 255 || c.G != 255 || c.R != 255)
                                    physical_map[x_phy+y_phy * WidthPx] = true;
                            }
                        }
                    }
                }
            }
        }

        public int Height
        {
            get { return map.GetLength(0); }
        }
        public int HeightPx
        {
            get { return map.GetLength(0) * tile_size; }
        }
        public int Width
        {
            get { return map.GetLength(1); }
        }
        public int WidthPx
        {
            get { return map.GetLength(1) * tile_size; }
        }

        public bool IsTileAWall(ushort tile)
        {
            ushort type = (ushort)((uint)tile & 0x3FF);
            if (type <= max_wall_type && !void_sprites.Contains(type))
                return true;
            return false;
        }
        public enum Zone
        {
            None = 0,
            Healing,
            Ending
        }
        public Zone IsTileAZone(ushort tile)
        {
            // TODO
            return Zone.None;
        }
        public Rectangle? GetTileSprite(ushort tile)
        {
            if (IsTileAWall(tile))
            {
                int type = tile & 0x3FF;
                int orientation = (tile & 0xC00) >> 10;
                return new Rectangle(type*tile_size, orientation*tile_size, tile_size, tile_size);
            }
            return null;
        }
        // For graphical purpose only (do not use for testing collisions). No OOB.
        public ushort TileAt(int x, int y)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
                return 0x0;
            return map[y, x];  
        }
        // For physical purpose
        public bool IsPixelInCollision(short x, short y)
        {
            // Note: these calculus simulate those of the actual Kururin implementation
            uint xm = (ushort)x % (uint)WidthPx;
            uint ym = (ushort)y % (uint)HeightPx;
            uint addr = xm + ym * (uint)WidthPx;
            return physical_map[(int)addr];
        }
        public Zone IsPixelInZone(short x, short y)
        {
            // Note: these calculus simulate those of the actual Kururin implementation
            // C# integer division semantics matches the C one
            // C# modulo semantics matches the C one
            int x_tile = x / tile_size;
            int y_tile = y / tile_size;
            int xm = x_tile % Width;
            int ym = y_tile % Height;
            ushort tile = TileAt(xm, ym);
            return IsTileAZone(tile);
        }
    }
}
