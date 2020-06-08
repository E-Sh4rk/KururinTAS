using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace KuruBot
{
    class KuruMath
    {
        public static KuruMath instance = new KuruMath();

        int shift_angle;
        int factor;
        int[] cos_table = null;
        int[] sin_table = null;
        int atan2_dist_bits;
        int[,] atan2_table = null;

        public KuruMath(int nb_bits_angle_precision = 8, int sin_cos_factor = 0x100, int atan2_nb_bits_dist_precision = 6)
        {
            shift_angle = 16 - nb_bits_angle_precision;
            factor = sin_cos_factor;
            atan2_dist_bits = atan2_nb_bits_dist_precision;
            // Init sin/cos tables
            int nb_angles = 1 << nb_bits_angle_precision;
            cos_table = new int[nb_angles];
            sin_table = new int[nb_angles];
            for (uint i = 0; i < nb_angles; i++)
            {
                cos_table[i] = (int)(Math.Cos(2 * Math.PI * i / nb_angles) * sin_cos_factor);
                sin_table[i] = (int)(Math.Sin(2 * Math.PI * i / nb_angles) * sin_cos_factor);
            }
            // Testing: dump sin/cos tables (should match 0x1CF34)
            /*FileStream sinfs = File.OpenWrite("sin.bin");
            FileStream cosfs = File.OpenWrite("cos.bin");
            for (uint i = 0; i < nb_angles; i++)
            {
                sinfs.Write(BitConverter.GetBytes(sin_table[i]), 0, 2);
                cosfs.Write(BitConverter.GetBytes(cos_table[i]), 0, 2);
            }
            sinfs.Close();
            cosfs.Close();*/
            // Init atan2 table (should match 0x1D334)
            int nb_dists = 1 << atan2_nb_bits_dist_precision;
            atan2_table = new int[nb_dists, nb_dists];
            for (uint j = 0; j < nb_dists; j++)
            {
                for (uint i = 0; i < nb_dists; i++)
                {
                    atan2_table[i, j] = (int)Math.Round(2 * Math.Atan2(i, j) * 0x40 / Math.PI);
                }
            }
            // Testing: dump atan2 table
            /*FileStream atfs = File.OpenWrite("atan2.bin");
            for (uint j = 0; j < nb_dists; j++)
            {
                for (uint i = 0; i < nb_dists; i++)
                {
                    atfs.WriteByte((byte)atan2_table[i, j]);
                }
            }
            atfs.Close();*/
        }

        // /!\ Overflow: radius * factor should be smaller than 2^31
        public int cos(int radius, short rot)
        {
            int angle = (ushort)rot >> shift_angle;
            return (radius * cos_table[angle]) / factor;
        }
        // /!\ Overflow: radius * factor should be smaller than 2^31
        public int sin(int radius, short rot)
        {
            int angle = (ushort)rot >> shift_angle;
            return (radius * sin_table[angle]) / factor;
        }

        int most_significant_bit(int v)
        {
            int res = -1;
            while (v > 0)
            {
                res++;
                v >>= 1;
            }
            return res;
        }
        public short atan2(int dx, int dy)
        {
            int atanx = Math.Abs(dx);
            int atany = Math.Abs(dy);
            int sz = Math.Max(most_significant_bit(atanx), most_significant_bit(atany)) + 1;
            if (sz > atan2_dist_bits)
            {
                atanx >>= sz - atan2_dist_bits;
                atany >>= sz - atan2_dist_bits;
            }
            int res = atan2_table[atanx, atany];
            if (dx < 0)
                res = -res;
            if (dy >= 0)
                res = (sbyte)(0x80 - res);
            return (short)(res << 8);
        }
    }
}
