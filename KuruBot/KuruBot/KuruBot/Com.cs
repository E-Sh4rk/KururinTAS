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
        const string in_filename = "in@.txt";
        const string out_filename = "out@.txt";

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

        public static Map parse_map(string[] lines)
        {
            string[] headers = lines[0].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            ushort xl = Convert.ToUInt16(headers[0]);
            ushort yl = Convert.ToUInt16(headers[1]);
            ushort[,] map = new ushort[yl, xl];
            for (ushort i = 0; i < yl; i++)
            {
                string[] line = lines[i + 1].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                for (ushort j = 0; j < xl; j++)
                    map[i, j] = Convert.ToUInt16(line[j]);
            }
            return new Map(map);
        }

        public static string map_to_string(Map m)
        {
            if (m == null)
                return null;
            string res = m.Width.ToString() + " " + m.Height.ToString() + "\n";
            for (int y = 0; y < m.Height; y++)
            {
                for (int x = 0; x < m.Width; x++)
                    res += m.TileAt(x,y).ToString() + " ";
                res += "\n";
            }
            return res;
        }

        public Map DownloadMap()
        {
            string in_path = this.in_path.Replace("@", "0");
            string out_path = this.out_path.Replace("@", "0");
            try
            {
                if (File.Exists(in_path))
                    return null;
                if (File.Exists(out_path))
                    File.Delete(out_path);
                File.WriteAllText(in_path, "DUMPMAP");
                while (!File.Exists(out_path))
                    Thread.Sleep(250);
                return parse_map(File.ReadAllLines(out_path));
            }
            catch { }
            return null;
        }

        public static HelirinState parse_hs(string content)
        {
            string[] headers = content.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            int xpos = Convert.ToInt32(headers[0]);
            int ypos = Convert.ToInt32(headers[1]);
            int xb = Convert.ToInt32(headers[2]);
            int yb = Convert.ToInt32(headers[3]);
            short rot = Convert.ToInt16(headers[4]);
            short rot_rate = Convert.ToInt16(headers[5]);
            short rot_srate = Convert.ToInt16(headers[6]);
            byte life = Convert.ToByte(headers[7]);
            sbyte invul = Convert.ToSByte(headers[8]);
            bool hasBonus = headers.Length > 9 ? (Convert.ToInt32(headers[9]) != 0) : false;
            return new HelirinState(xpos, ypos, xb, yb, rot, rot_rate, rot_srate, life, invul, hasBonus);
        }

        public static string hs_to_string(HelirinState hs)
        {
            string xpos_str = hs.xpos.ToString();
            string ypos_str = hs.ypos.ToString();
            string xb_str = hs.xb.ToString();
            string yb_str = hs.yb.ToString();
            string rot_str = hs.rot.ToString();
            string rot_rate_str = hs.rot_rate.ToString();
            string rot_srate_str = hs.rot_srate.ToString();
            string life_str = hs.life.ToString();
            string invul_str = hs.invul.ToString();
            return xpos_str + " " + ypos_str + " " + xb_str + " " + yb_str + " " + rot_str + " " + rot_rate_str + " " + rot_srate_str
                    + " " + life_str + " " + invul_str + " " + (hs.HasBonus() ? "1" : "0") + "\n";
        }

        public HelirinState GetHelirin()
        {
            string in_path = this.in_path.Replace("@", "1");
            string out_path = this.out_path.Replace("@", "1");
            try
            {
                if (File.Exists(in_path))
                    return null;
                if (File.Exists(out_path))
                    File.Delete(out_path);
                File.WriteAllText(in_path, "GETPOS");
                while (!File.Exists(out_path))
                    Thread.Sleep(250);
                return parse_hs(File.ReadAllLines(out_path)[0]);
            }
            catch { }
            return null;
        }

        public HelirinState[] Play(HelirinState hs, Action[] actions)
        {
            string in_path = this.in_path.Replace("@", "2");
            string out_path = this.out_path.Replace("@", "2");
            try
            {
                if (File.Exists(in_path))
                    return null;
                if (File.Exists(out_path))
                    File.Delete(out_path);
                string txt = "PLAY\n";
                txt += hs_to_string(hs);
                foreach (Action a in actions)
                {
                    string str = Controller.effect_to_string(Controller.action_to_effect(a));
                    if (String.IsNullOrEmpty(str))
                        str = "_"; // No empty line because they are ignored by the LUA script
                    txt += str + "\n";
                }
                File.WriteAllText(in_path, txt);
                while (!File.Exists(out_path))
                    Thread.Sleep(250);
                // Parsing the file
                string[] res = File.ReadAllLines(out_path);
                List<HelirinState> hs_res = new List<HelirinState>();
                try
                {
                    foreach (string line in res)
                    {
                        if (!String.IsNullOrEmpty(line))
                            hs_res.Add(parse_hs(line));
                    }
                }
                catch { }
                return hs_res.ToArray();
            }
            catch { }
            return null;
        }

        public string[] DownloadInputs(out HelirinState hs)
        {
            hs = null;
            string in_path = this.in_path.Replace("@", "2");
            string out_path = this.out_path.Replace("@", "2");
            try
            {
                if (File.Exists(in_path))
                    return null;
                if (File.Exists(out_path))
                    File.Delete(out_path);
                File.WriteAllText(in_path, "STARTRECORD");
                while (!File.Exists(out_path))
                    Thread.Sleep(250);
                string[] res = File.ReadAllLines(out_path);
                hs = parse_hs(res[0]);
                string[] res2 = new string[res.Length-1];
                Array.Copy(res, 1, res2, 0, res2.Length);
                return res2;
            }
            catch { }
            return null;
        }

    }
}
