using System;

namespace WaddleRush.Simulation
{
    public struct TrailSample
    {
        public Vec2 Position;
        public Vec2 Direction;
        public float Distance;
        public TrailSample(Vec2 position, Vec2 direction, float distance)
        { Position = position; Direction = direction; Distance = distance; }
    }

    public sealed class TrailBuffer
    {
        readonly TrailSample[] samples;
        int start, count;
        float totalDistance;
        public int Count => count;
        public float TotalDistance => totalDistance;

        public TrailBuffer(int capacity)
        {
            if (capacity < 2) throw new ArgumentOutOfRangeException(nameof(capacity));
            samples = new TrailSample[capacity];
        }

        public bool Add(Vec2 position, Vec2 direction, float minimumSpacing = .08f)
        {
            if (count > 0 && Vec2.Distance(Get(count - 1).Position, position) < minimumSpacing) return false;
            if (count > 0) totalDistance += Vec2.Distance(Get(count - 1).Position, position);
            var sample = new TrailSample(position, direction.Normalized, totalDistance);
            if (count < samples.Length) samples[(start + count++) % samples.Length] = sample;
            else
            {
                start = (start + 1) % samples.Length;
                samples[(start + count - 1) % samples.Length] = sample;
            }
            return true;
        }

        public TrailSample Get(int logicalIndex)
        {
            if (logicalIndex < 0 || logicalIndex >= count) throw new ArgumentOutOfRangeException(nameof(logicalIndex));
            return samples[(start + logicalIndex) % samples.Length];
        }

        public TrailSample SampleBehind(float distanceBehind)
        {
            if (count == 0) return default;
            var target = totalDistance - Math.Max(0, distanceBehind);
            if (target <= Get(0).Distance) return Get(0);
            for (var i = count - 1; i > 0; --i)
            {
                var newer = Get(i); var older = Get(i - 1);
                if (older.Distance > target) continue;
                var span = Math.Max(.0001f, newer.Distance - older.Distance);
                var t = (target - older.Distance) / span;
                return new TrailSample(older.Position + (newer.Position - older.Position) * t,
                    (older.Direction + (newer.Direction - older.Direction) * t).Normalized, target);
            }
            return Get(0);
        }
    }
}
