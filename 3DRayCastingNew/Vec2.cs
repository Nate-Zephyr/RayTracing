namespace _3DRayCastingNew
{
    public class Vec2
    {
        public double x, y;

        public Vec2() { x = 0; y = 0; }
        public Vec2(double value) { x = value; y = value; }
        public Vec2(double x, double y) { this.x = x; this.y = y; }
        public Vec2(Vec2 vec) { x = vec.x; y = vec.y; }

        public static Vec2 operator +(Vec2 first, Vec2 second) { return new Vec2(first.x + second.x, first.y + second.y); }
        public static Vec2 operator -(Vec2 first, Vec2 second) { return new Vec2(first.x - second.x, first.y - second.y); }
        public static Vec2 operator *(Vec2 first, Vec2 second) { return new Vec2(first.x * second.x, first.y * second.y); }
        public static Vec2 operator /(Vec2 first, Vec2 second) { return new Vec2(first.x / second.x, first.y / second.y); }
        public static Vec2 operator +(Vec2 v, double value) { return new Vec2(v.x + value, v.y + value); }
        public static Vec2 operator -(Vec2 v, double value) { return new Vec2(v.x - value, v.y - value); }
        public static Vec2 operator *(Vec2 v, double value) { return new Vec2(v.x * value, v.y * value); }
        public static Vec2 operator /(Vec2 v, double value) { return new Vec2(v.x / value, v.y / value); }
        public static Vec2 operator -(Vec2 first) { return new Vec2(-first.x, -first.y); }

        public Vec2 xy { get { return new Vec2(x, y); } set { x = value.x; y = value.y; } }
        public Vec2 yx { get { return new Vec2(y, x); } set { y = value.x; x = value.y; } }
    };
}
