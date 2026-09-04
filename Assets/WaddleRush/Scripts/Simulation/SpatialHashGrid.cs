using System;
using System.Collections.Generic;

namespace WaddleRush.Simulation
{
    public struct TrailSegment
    {
        public int OwnerId, UnitIndex; public Vec2 A, B; public float Radius;
        public TrailSegment(int owner, int index, Vec2 a, Vec2 b, float radius)
        { OwnerId = owner; UnitIndex = index; A = a; B = b; Radius = radius; }
    }

    public sealed class SpatialHashGrid
    {
        readonly float cellSize;
        readonly Dictionary<long, List<TrailSegment>> cells = new Dictionary<long, List<TrailSegment>>();
        public SpatialHashGrid(float cellSize = 4f) { this.cellSize = Math.Max(.1f, cellSize); }
        static long Key(int x, int y) => ((long)x << 32) ^ (uint)y;
        int Cell(float value) => (int)Math.Floor(value / cellSize);
        public void Clear() => cells.Clear();
        public void Insert(TrailSegment segment)
        {
            var minX = Cell(Math.Min(segment.A.X, segment.B.X) - segment.Radius);
            var maxX = Cell(Math.Max(segment.A.X, segment.B.X) + segment.Radius);
            var minY = Cell(Math.Min(segment.A.Y, segment.B.Y) - segment.Radius);
            var maxY = Cell(Math.Max(segment.A.Y, segment.B.Y) + segment.Radius);
            for (var x = minX; x <= maxX; x++) for (var y = minY; y <= maxY; y++)
            { var k = Key(x, y); if (!cells.TryGetValue(k, out var list)) cells[k] = list = new List<TrailSegment>(); list.Add(segment); }
        }
        public void Query(Vec2 point, float radius, List<TrailSegment> results)
        {
            results.Clear();
            for (var x = Cell(point.X - radius); x <= Cell(point.X + radius); x++)
                for (var y = Cell(point.Y - radius); y <= Cell(point.Y + radius); y++)
                    if (cells.TryGetValue(Key(x, y), out var list)) results.AddRange(list);
        }
    }
}
