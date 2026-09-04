using System;

namespace WaddleRush.Simulation
{
    [Serializable]
    public struct Vec2
    {
        public float X, Y;
        public Vec2(float x, float y) { X = x; Y = y; }
        public static Vec2 operator +(Vec2 a, Vec2 b) => new Vec2(a.X + b.X, a.Y + b.Y);
        public static Vec2 operator -(Vec2 a, Vec2 b) => new Vec2(a.X - b.X, a.Y - b.Y);
        public static Vec2 operator *(Vec2 a, float b) => new Vec2(a.X * b, a.Y * b);
        public float SqrMagnitude => X * X + Y * Y;
        public float Magnitude => (float)Math.Sqrt(SqrMagnitude);
        public Vec2 Normalized => Magnitude > .00001f ? this * (1f / Magnitude) : new Vec2(0, 1);
        public static float Distance(Vec2 a, Vec2 b) => (a - b).Magnitude;
        public static float Dot(Vec2 a, Vec2 b) => a.X * b.X + a.Y * b.Y;
    }

    public static class Geometry2D
    {
        public static float DistancePointSegment(Vec2 p, Vec2 a, Vec2 b)
        {
            var ab = b - a;
            var d = ab.SqrMagnitude;
            if (d < .000001f) return Vec2.Distance(p, a);
            var t = Math.Max(0f, Math.Min(1f, Vec2.Dot(p - a, ab) / d));
            return Vec2.Distance(p, a + ab * t);
        }
    }
}
