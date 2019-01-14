using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KuruBot
{
    class KuruMath
    {
        int shift_angle;
        int factor;
        int[] cos_table = null;
        int[] sin_table = null;

        public KuruMath(int nb_bits_angle_precision = 8, int sin_cos_factor = 0x100)
        {
            shift_angle = 16 - nb_bits_angle_precision;
            factor = sin_cos_factor;
            // Init tables
            int nb_angles = 1 << nb_bits_angle_precision;
            cos_table = new int[nb_angles];
            sin_table = new int[nb_angles];
            for (uint i = 0; i < nb_angles; i++)
            {
                cos_table[i] = (int)(Math.Cos(2 * Math.PI * i / nb_angles) * sin_cos_factor);
                sin_table[i] = (int)(Math.Sin(2 * Math.PI * i / nb_angles) * sin_cos_factor);
            }
        }

        // /!\ Overflow: radius * factor should be smaller than 2^31
        public int factor_by_cos(int radius, short rot)
        {
            int angle = (ushort)rot >> shift_angle;
            return (radius * cos_table[angle]) / factor;
        }
        // /!\ Overflow: radius * factor should be smaller than 2^31
        public int factor_by_sin(int radius, short rot)
        {
            int angle = (ushort)rot >> shift_angle;
            return (radius * sin_table[angle]) / factor;
        }
    }
}
