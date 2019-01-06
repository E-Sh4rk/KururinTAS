using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace KuruBot
{
    class Com
    {
        const string subfolder = "tasks";
        const string in_filename = "in.txt";
        const string out_filename = "out.txt";

        string path = null;
        string in_path = null;
        string out_path = null;

        public Com(string path)
        {
            this.path = Path.Combine(path, subfolder);
            try
            {
                Directory.CreateDirectory(this.path);
            }
            catch { }
            in_path = Path.Combine(this.path, in_filename);
            out_path = Path.Combine(this.path, out_filename);
        }

        public Map DownloadMap()
        {
            try
            {
                if (File.Exists(in_path))
                    return null;
                if (File.Exists(out_path))
                    File.Delete(out_path);
                File.WriteAllText(in_path, "DUMPMAP");
                while (!File.Exists(out_path))
                    Thread.Sleep(250);
                // Parsing the file
                string[] res = File.ReadAllLines(out_path);
                string[] headers = res[0].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                ushort xl = Convert.ToUInt16(headers[0]);
                ushort yl = Convert.ToUInt16(headers[1]);
                ushort[,] map = new ushort[yl,xl];
                for (ushort i = 0; i < yl; i++)
                {
                    string[] line = res[i+1].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    for (ushort j = 0; j < xl; j++)
                    {
                        map[i, j] = Convert.ToUInt16(line[j]);
                    }
                }
                return new Map(map);
            }
            catch { }
            return null;
        }

        public HelirinState? GetHelirin()
        {
            try
            {
                if (File.Exists(in_path))
                    return null;
                if (File.Exists(out_path))
                    File.Delete(out_path);
                File.WriteAllText(in_path, "GETPOS");
                while (!File.Exists(out_path))
                    Thread.Sleep(250);
                // Parsing the file
                string[] res = File.ReadAllLines(out_path);
                string[] headers = res[0].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                uint xpos = Convert.ToUInt32(headers[0]);
                uint ypos = Convert.ToUInt32(headers[1]);
                ushort rot = Convert.ToUInt16(headers[2]);
                short rot_srate = Convert.ToInt16(headers[3]);
                return new HelirinState(xpos, ypos, rot, rot_srate);
            }
            catch { }
            return null;
        }

    }
}
