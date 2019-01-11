using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KuruBot
{
    class KuruMath
    {
        int nb_bits_angle;
        int nb_bits_after_point;
        int[] cos_table = null;
        int[] sin_table = null;

        public KuruMath(int nb_bits_angle = 8, int nb_bits_after_point = 8)
        {
            this.nb_bits_after_point = nb_bits_after_point;
            this.nb_bits_angle = nb_bits_angle;
            // Init tables
            int nb_angles = 1 << nb_bits_angle;
            int factor = 1 << nb_bits_after_point;
            cos_table = new int[nb_angles];
            sin_table = new int[nb_angles];
            for (uint i = 0; i < nb_angles; i++)
            {
                cos_table[i] = (int)(Math.Cos(2 * Math.PI * i / nb_angles) * factor);
                sin_table[i] = (int)(Math.Sin(2 * Math.PI * i / nb_angles) * factor);
            }
        }

        public int factor_by_cos(int radius, short rot)
        {
            int angle = (ushort)rot >> (16 - nb_bits_angle);
            return (radius >> nb_bits_after_point) * cos_table[angle];
        }
        public int factor_by_sin(int radius, short rot)
        {
            int angle = (ushort)rot >> (16 - nb_bits_angle);
            return (radius >> nb_bits_after_point) * sin_table[angle];
        }
    }
}
