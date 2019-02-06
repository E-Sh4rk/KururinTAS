using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Priority_Queue;

namespace KuruBot
{
    public class Flooding
    {
        public struct Pixel
        {
            public Pixel(short x, short y)
            {
                this.x = x;
                this.y = y;
            }
            public Pixel Add(Pixel p)
            {
                Pixel res = this;
                res.x += p.x;
                res.y += p.y;
                return res;
            }
            public short x;
            public short y;
        }

        public Flooding(Map m, Pixel start, Pixel end)
        {
            this.m = m;
            PixelStart = start;
            PixelEnd = end;
            legal_zones = ComputeLegalZones();
            dist_to_wall = ComputeDistanceToAWall();
        }

        Map m;
        bool[,] legal_zones;
        float[,] dist_to_wall;

        public void SetTarget(bool[,] target)
        {
            if (target != null)
            {
                if (target.GetLength(0) != PixelEnd.y - PixelStart.y + 1 || target.GetLength(1) != PixelEnd.x - PixelStart.x + 1)
                    target = null;
            }
            this.target = target;
        }
        public void SetConstraints(bool[,] constraints)
        {
            if (constraints != null)
            {
                if (constraints.GetLength(0) != PixelEnd.y - PixelStart.y + 1 || constraints.GetLength(1) != PixelEnd.x - PixelStart.x + 1)
                    constraints = null;
            }
            this.constraints = constraints;
        }

        bool[,] target = null; bool[,] constraints = null;

        public Pixel PixelStart { get; }
        public Pixel PixelEnd { get; }

        public float DistToWall(short x, short y)
        {
            return dist_to_wall[y - PixelStart.y, x - PixelStart.x];
        }

        public bool IsLegalZone(short x, short y)
        {
            return legal_zones[y - PixelStart.y, x - PixelStart.x];
        }

        public float Cost(float[,] cost, short x, short y)
        {
            return cost[y - PixelStart.y, x - PixelStart.x];
        }

        Pixel[] DiagNeighbors(Pixel p)
        {
            List<Pixel> res = new List<Pixel>();

            if (p.y > PixelStart.y && p.x > PixelStart.x)
                res.Add(p.Add(new Pixel(-1, -1)));
            if (p.y < PixelEnd.y && p.x < PixelEnd.x)
                res.Add(p.Add(new Pixel(1, 1)));
            if (p.y < PixelEnd.y && p.x > PixelStart.x)
                res.Add(p.Add(new Pixel(-1, 1)));
            if (p.y > PixelStart.y && p.x < PixelEnd.x)
                res.Add(p.Add(new Pixel(1, -1)));

            return res.ToArray();
        }

        Pixel[] StraightNeighbors(Pixel p)
        {
            List<Pixel> res = new List<Pixel>();

            if (p.y > PixelStart.y)
                res.Add(p.Add(new Pixel(0,-1)));
            if (p.y < PixelEnd.y)
                res.Add(p.Add(new Pixel(0,1)));
            if (p.x > PixelStart.x)
                res.Add(p.Add(new Pixel(-1,0)));
            if (p.x < PixelEnd.x)
                res.Add(p.Add(new Pixel(1,0)));

            return res.ToArray();
        }

        const float sqrt2 = 1.41421356237F;
        PixelDist[] Neighbors(Pixel p)
        {
            List<PixelDist> res = new List<PixelDist>();
            foreach (Pixel pix in StraightNeighbors(p))
                res.Add(new PixelDist(pix,1));
            foreach (Pixel pix in DiagNeighbors(p))
                res.Add(new PixelDist(pix,sqrt2));
            return res.ToArray();
        }

        struct PixelDist
        {
            public PixelDist(Pixel px, float dist)
            {
                this.px = px;
                this.dist = dist;
            }
            public Pixel px;
            public float dist;
        }

        bool[,] ComputeLegalZones()
        {
            int width = PixelEnd.x - PixelStart.x + 1;
            int height = PixelEnd.y - PixelStart.y + 1;
            bool[,] res = new bool[height, width];
            Queue<Pixel> q = new Queue<Pixel>();

            // Init
            for (short y = PixelStart.y; y <= PixelEnd.y; y++)
            {
                for (short x = PixelStart.x; x <= PixelEnd.x; x++)
                {
                    res[y - PixelStart.y, x - PixelStart.x] = false;
                    if (x >= 0 && x < m.WidthPx && y >= 0 && y < m.HeightPx) // We only consider the ending zones in the "legal map"
                        if (m.IsPixelInZone(x, y) == Map.Zone.Ending)
                            q.Enqueue(new Pixel(x, y));
                }
            }

            // Graph walk (breadth first search)
            while (q.Count > 0)
            {
                Pixel p = q.Dequeue();
                if (!res[p.y - PixelStart.y, p.x - PixelStart.x] && !m.IsPixelInCollision(p.x, p.y))
                {
                    res[p.y - PixelStart.y, p.x - PixelStart.x] = true;
                    Pixel[] neighbors = StraightNeighbors(p);
                    foreach (Pixel np in neighbors)
                        q.Enqueue(np);
                }
            }

            return res;
        }

        float[,] ComputeDistanceToAWall()
        {
            int width = PixelEnd.x - PixelStart.x + 1;
            int height = PixelEnd.y - PixelStart.y + 1;
            float[,] res = new float[height, width];
            SimplePriorityQueue<Pixel> q = new SimplePriorityQueue<Pixel>();

            // Init
            for (short y = PixelStart.y; y <= PixelEnd.y; y++)
            {
                for (short x = PixelStart.x; x <= PixelEnd.x; x++)
                {
                    if (m.IsPixelInCollision(x,y))
                    {
                        q.Enqueue(new Pixel(x, y), 0);
                        res[y - PixelStart.y, x - PixelStart.x] = 0;
                    }
                    else
                        res[y - PixelStart.y, x - PixelStart.x] = float.PositiveInfinity;
                }
            }

            // Dijkstra
            while (q.Count > 0)
            {
                Pixel p = q.Dequeue();
                float weight = res[p.y - PixelStart.y, p.x - PixelStart.x];

                PixelDist[] neighbors = Neighbors(p);
                foreach (PixelDist npd in neighbors)
                {
                    float nw = weight + npd.dist;
                    float ow = res[npd.px.y - PixelStart.y, npd.px.x - PixelStart.x];
                    if (nw < ow)
                    {
                        res[npd.px.y - PixelStart.y, npd.px.x - PixelStart.x] = nw;
                        if (ow < float.PositiveInfinity)
                            q.UpdatePriority(npd.px, nw);
                        else
                            q.Enqueue(npd.px, nw);
                    }
                }
            }

            return res;
        }

        float[,] ComputeCostMap(float gwb_multiplier, float allow_wall_ground_dist, bool no_wall_clip)
        {
            // Simple Dijkstra algorithm with some extra parameters.
            // Ground wall bonus is not applied during the Dijkstra algorithm because it would generate negative weights.
            // Instead, it will be applied after to each pixel in collision (proportionally if current weight is smaller than gwb min dist).

            float gwb = Settings.ground_wall_bonus * gwb_multiplier;
            float gwb_md = Settings.ground_wall_bonus_min_dist * gwb_multiplier;
            float wgm_per_px = Settings.wall_ground_malus / Settings.wall_ground_malus_dist;

            int width = PixelEnd.x - PixelStart.x + 1;
            int height = PixelEnd.y - PixelStart.y + 1;
            float[,] res = new float[height,width];
            SimplePriorityQueue<Pixel> q = new SimplePriorityQueue<Pixel>();

            // Init target
            bool[,] target = this.target;
            if (target == null)
            {
                target = new bool[height,width];
                for (short y = PixelStart.y; y <= PixelEnd.y; y++)
                    for (short x = PixelStart.x; x <= PixelEnd.x; x++)
                        target[y - PixelStart.y, x - PixelStart.x] = m.IsPixelInZone(x, y) == Map.Zone.Ending;
            }

            // Init
            for (short y = PixelStart.y; y <= PixelEnd.y; y++)
            {
                for (short x = PixelStart.x; x <= PixelEnd.x; x++)
                {
                    if (target[y - PixelStart.y, x - PixelStart.x])
                    {
                        q.Enqueue(new Pixel(x, y), 0);
                        res[y - PixelStart.y, x - PixelStart.x] = 0;
                    }
                    else
                        res[y - PixelStart.y, x - PixelStart.x] = float.PositiveInfinity;
                }
            }

            // Dijkstra
            while (q.Count > 0)
            {
                Pixel p = q.Dequeue();
                float weight = res[p.y - PixelStart.y, p.x - PixelStart.x];
                bool from_wall = m.IsPixelInCollision(p.x, p.y);
                float from_wall_dist = dist_to_wall[p.y - PixelStart.y, p.x - PixelStart.x];
                bool from_wc_allowed_zone = from_wall_dist <= allow_wall_ground_dist;

                PixelDist[] neighbors = Neighbors(p);
                foreach (PixelDist npd in neighbors)
                {
                    int npy = npd.px.y - PixelStart.y;
                    int npx = npd.px.x - PixelStart.x;

                    if (constraints != null && constraints[npy, npx])
                        continue;

                    bool to_wall = m.IsPixelInCollision(npd.px.x, npd.px.y);
                    float to_wall_dist = dist_to_wall[npy, npx];
                    bool to_wc_allowed_zone = to_wall_dist <= allow_wall_ground_dist;
                    bool to_legal_zone = legal_zones[npy, npx];

                    float wgm_coef = Math.Min(to_wall_dist, Settings.wall_ground_malus_dist) - from_wall_dist;
                    if (wgm_coef < 0)
                        wgm_coef = 0;

                    if (no_wall_clip && to_wall)
                        continue;

                    float nw = weight;
                    if (from_wc_allowed_zone && !to_wc_allowed_zone && !to_legal_zone)
                        nw = float.PositiveInfinity;
                    else if (allow_wall_ground_dist < float.PositiveInfinity && from_wall && to_legal_zone) // For legal zones, we use allow_wall_ground_dist=0
                        nw = float.PositiveInfinity;
                    else if (from_wall && to_wall)
                        nw += npd.dist / Settings.wall_speed;
                    else
                        nw += npd.dist / Settings.ground_speed + wgm_coef*wgm_per_px;

                    float ow = res[npy, npx];
                    if (nw < ow)
                    {
                        res[npy, npx] = nw;
                        if (ow < float.PositiveInfinity)
                            q.UpdatePriority(npd.px, nw);
                        else
                            q.Enqueue(npd.px, nw);
                    }
                }
            }

            // Post-procedure (bonus for walls + ensure that all costs are non-zero if not on an ending)
            for (short y = PixelStart.y; y <= PixelEnd.y; y++)
            {
                for (short x = PixelStart.x; x <= PixelEnd.x; x++)
                {
                    float w = res[y - PixelStart.y, x - PixelStart.x];
                    if (m.IsPixelInCollision(x, y))
                    {
                        if (w >= gwb_md)
                            w -= gwb;
                        else
                            w -= gwb * w / gwb_md;
                    }
                    if (w <= 0 && !target[y - PixelStart.y, x - PixelStart.x])
                        w = float.Epsilon;
                    res[y - PixelStart.y, x - PixelStart.x] = w;
                }
            }

            return res;
        }

        public enum WallClipSetting
        {
            NoWallClip = 0,     // No wall clip
            NoCompleteWallClip, // No complete wall clip (the helirin can't perform a new wall clip to reach the ending zone)
            Allow
        }

        public float[,] ComputeCostMap(int total_invul_frames, WallClipSetting wcs)
        {
            if (wcs == WallClipSetting.NoWallClip)
                return ComputeCostMap(0, 0, true);
            else if (wcs == WallClipSetting.Allow)
                return ComputeCostMap(total_invul_frames, float.PositiveInfinity, false);
            else
                return ComputeCostMap(total_invul_frames, total_invul_frames >= Settings.complete_wall_clip_duration
                    ? Settings.complete_wall_clip_max_dist
                    : Settings.complete_wall_clip_max_dist * total_invul_frames / Settings.complete_wall_clip_duration, false);
        }

        public static float GetMaxWeightExceptInfinity(float[,] map)
        {
            float res = 0;
            for (int y = 0; y < map.GetLength(0); y++)
                for (int x = 0; x < map.GetLength(1); x++)
                    if (map[y, x] < float.PositiveInfinity && map[y, x] > res)
                        res = map[y, x];
            return res;
        }

    }
}
