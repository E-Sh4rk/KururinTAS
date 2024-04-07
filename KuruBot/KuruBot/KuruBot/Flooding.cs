using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Priority_Queue;

namespace KuruBot
{
    public class Flooding
    {
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

        public bool IsHealZone(short x, short y)
        {
            Map.Zone zone = m.IsPixelInZone(x, y);
            return zone == Map.Zone.Healing || zone == Map.Zone.Starting;
        }

        public bool IsWall(short x, short y)
        {
            return m.IsPixelInCollision(x, y);
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

        CostMap ComputeCostMap(bool no_wall_clip, bool targetBonus)
        {
            // Simple Dijkstra algorithm with some extra parameters.
            // Ground wall bonus is not applied during the Dijkstra algorithm because it would generate negative weights.
            // Instead, it will be applied after, when asking a specific cost, to each pixel in collision (proportionally if current weight is smaller than gwb min dist).

            int width = PixelEnd.x - PixelStart.x + 1;
            int height = PixelEnd.y - PixelStart.y + 1;
            float[,] res = new float[height,width];
            SimplePriorityQueue<Pixel> q = new SimplePriorityQueue<Pixel>();

            // Init target
            bool[,] target;
            if (targetBonus)
            {
                target = new bool[height, width];
                for (short y = PixelStart.y; y <= PixelEnd.y; y++)
                    for (short x = PixelStart.x; x <= PixelEnd.x; x++)
                        target[y - PixelStart.y, x - PixelStart.x] = m.IsPixelInBonus(x, y) != Map.BonusType.None;
            }
            else
            {
                target = this.target;
                if (target == null)
                {
                    if (Settings.target_is_oob)
                    {
                        target = new bool[height, width];
                        for (short y = 0; y < legal_zones.GetLength(0); y++)
                            for (short x = 0; x < legal_zones.GetLength(1); x++)
                                target[y, x] = !legal_zones[y, x] && dist_to_wall[y,x] >= Settings.target_oob_dist;
                    }
                    else
                    {
                        target = new bool[height, width];
                        for (short y = PixelStart.y; y <= PixelEnd.y; y++)
                            for (short x = PixelStart.x; x <= PixelEnd.x; x++)
                                target[y - PixelStart.y, x - PixelStart.x] = m.IsPixelInZone(x, y) == Map.Zone.Ending;
                    }
                }
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
                bool from_wall = IsWall(p.x, p.y);
                bool from_healzone = IsHealZone(p.x,p.y);

                PixelDist[] neighbors = Neighbors(p);
                foreach (PixelDist npd in neighbors)
                {
                    int npy = npd.px.y - PixelStart.y;
                    int npx = npd.px.x - PixelStart.x;

                    if (constraints != null && constraints[npy, npx])
                        continue;

                    bool to_wall = IsWall(npd.px.x, npd.px.y);
                    bool to_healzone = IsHealZone(npd.px.x, npd.px.y);

                    if (no_wall_clip && from_wall && !to_wall)
                        continue;

                    float nw = weight;
                    if (to_wall)
                        nw += npd.dist / Settings.wall_speed;
                    else
                        nw += npd.dist / Settings.ground_speed;

                    if (from_wall && !to_wall)
                        nw += Settings.enter_wall_cost;

                    if (from_healzone && !to_healzone)
                        nw += Settings.enter_healzone_cost;

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

            return new CostMap(res, dist_to_wall, PixelStart, !no_wall_clip);
        }

        public ExtendedCostMap ComputeExtendedCostMap(bool no_wall_clip)
        {
            CostMap targetCM = ComputeCostMap(no_wall_clip, false);
            CostMap bonusCM = null;
            if (m.HasBonus != Map.BonusType.None && Settings.bonus_required)
            {
                bonusCM = ComputeCostMap(no_wall_clip, true);
                float max = 0;
                Rectangle r = m.GetBonusPxRect().Value;
                for (int y = r.Top; y < r.Bottom; y++)
                    for (int x = r.Left; x < r.Right; x++)
                        max = Math.Max(max, targetCM.CostAtPx((short)x, (short)y, 0));
                bonusCM.GlobalMalus += max + Settings.back_before_bonus_malus;
            }
            return new ExtendedCostMap(targetCM, bonusCM);
        }
    }
}
