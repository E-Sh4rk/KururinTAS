using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Priority_Queue;

namespace KuruBot
{
    public class Flooding
    {
        // All these values must be non-negative
        const float ground_speed = 3;
        const float wall_speed = ground_speed; // Should be equal to ground speed (we can't benefit from wall speed for ever, so a constant bonus is more adapted).
        const float ground_wall_bonus = 7 - 2 - ground_speed; // Bonus applied to each pixel in a wall (in a post procedure) in order to encourage wall exploration. Unit: weight/frame.
        const float ground_wall_bonus_min_dist = 7 - 2; // Min weight required for a wall to benefit from full bonus. Must be greater than ground_wall_bonus. Unit: dist/frame.
        const float wall_ground_malus = ground_speed * 20; // Malus applied everytime we leave a wall clip, in order to capture the fact that doing the other way could be expensive.
        const float wall_clip_end_dist = 4; // Distance from the wall at which the helirin has no control anymore.

        const float sqrt2 = 1.41421356237F;

        // Ground wall bonus is not applied during the Dijkstra algorithm because it would generate negative weights.
        // Instead, it will be applied after to each pixel in collision (proportionally if current weight is smaller than gwb min dist).

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

        float[,] ComputeCostMap(float gwb_multiplier, float wgm_multiplier, bool no_wall_clip)
        {
            float gwb = ground_wall_bonus * gwb_multiplier;
            float gwb_md = ground_wall_bonus_min_dist * gwb_multiplier;
            float wgm = wall_ground_malus * wgm_multiplier;

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
                bool from_near_wall = dist_to_wall[p.y - PixelStart.y, p.x - PixelStart.x] <= wall_clip_end_dist;

                PixelDist[] neighbors = Neighbors(p);
                foreach (PixelDist npd in neighbors)
                {
                    int npy = npd.px.y - PixelStart.y;
                    int npx = npd.px.x - PixelStart.x;
                    if (constraints == null || !constraints[npy,npx])
                    {
                        bool to_wall = m.IsPixelInCollision(npd.px.x, npd.px.y);
                        bool to_near_wall = dist_to_wall[npy, npx] <= wall_clip_end_dist;

                        if (no_wall_clip && to_wall)
                            continue;

                        float nw = weight;
                        if (from_near_wall && !to_near_wall)
                            nw += npd.dist / ground_speed + wgm;
                        else if (from_wall && to_wall)
                            nw += npd.dist / wall_speed;
                        else
                            nw += npd.dist / ground_speed;

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

        public float[,] ComputeCostMap(float gwb_multiplier, float wgm_multiplier, WallClipSetting wcs)
        {
            if (wcs == WallClipSetting.NoWallClip)
                return ComputeCostMap(0, 0, true);
            else if (wcs == WallClipSetting.Allow)
                return ComputeCostMap(gwb_multiplier, wgm_multiplier, false);
            else
                return ComputeCostMap(gwb_multiplier, float.PositiveInfinity, false);
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
