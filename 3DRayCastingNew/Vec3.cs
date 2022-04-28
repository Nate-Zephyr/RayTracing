namespace _3DRayCastingNew
{
	public class Vec3
	{
		public double x, y, z;

		public Vec3() { x = 0; y = 0; z = 0; }
		public Vec3(double value) { x = value; y = value; z = value; }
		public Vec3(double x, double y, double z) { this.x = x; this.y = y; this.z = z; }
		public Vec3(Vec3 vec) { x = vec.x; y = vec.y; z = vec.z; }
		public Vec3(double value, Vec2 vec) { x = value; y = vec.x; z = vec.y; }
		public Vec3(Vec2 vec, double value) { x = vec.x; y = vec.y; z = value; }

		public static Vec3 operator +(Vec3 first, Vec3 second) { return new Vec3(first.x + second.x, first.y + second.y, first.z + second.z); }
		public static Vec3 operator -(Vec3 first, Vec3 second) { return new Vec3(first.x - second.x, first.y - second.y, first.z - second.z); }
		public static Vec3 operator *(Vec3 first, Vec3 second) { return new Vec3(first.x * second.x, first.y * second.y, first.z * second.z); }
		public static Vec3 operator /(Vec3 first, Vec3 second) { return new Vec3(first.x / second.x, first.y / second.y, first.z / second.z); }
		public static Vec3 operator +(Vec3 v, double value) { return new Vec3(v.x + value, v.y + value, v.z + value); }
		public static Vec3 operator -(Vec3 v, double value) { return new Vec3(v.x - value, v.y - value, v.z - value); }
		public static Vec3 operator *(Vec3 v, double value) { return new Vec3(v.x * value, v.y * value, v.z * value); }
		public static Vec3 operator /(Vec3 v, double value) { return new Vec3(v.x / value, v.y / value, v.z / value); }
		public static Vec3 operator +(double value, Vec3 v) { return new Vec3(value + v.x, value + v.y, value + v.z); }
		public static Vec3 operator -(double value, Vec3 v) { return new Vec3(value - v.x, value - v.y, value - v.z); }
		public static Vec3 operator *(double value, Vec3 v) { return new Vec3(value * v.x, value * v.y, value * v.z); }
		public static Vec3 operator /(double value, Vec3 v) { return new Vec3(value / v.x, value / v.y, value / v.z); }
		public static bool operator ==(Vec3 first, Vec3 second) { if (first.x == second.x && first.y == second.y && first.z == second.z) return true; else return false; }
		public static bool operator !=(Vec3 first, Vec3 second) { if (first.x != second.x || first.y != second.y || first.z != second.z) return true; else return false; }

		public static Vec3 operator -(Vec3 first) { return new Vec3(-first.x, -first.y, -first.z); }

		public static Vec3 operator *(Vec3 v, Mat3 m)
		{
			return new Vec3(
				m.a1 * v.x + m.a2 * v.y + m.a3 * v.z,
				m.b1 * v.x + m.b2 * v.y + m.b3 * v.z,
				m.c1 * v.x + m.c2 * v.y + m.c3 * v.z);
		}

		public Vec3 xyz { get { return new Vec3(x, y, z); } set { x = value.x; y = value.y; z = value.z; } }
		public Vec3 xzy { get { return new Vec3(x, z, y); } set { x = value.x; z = value.y; y = value.z; } }
		public Vec3 yxz { get { return new Vec3(y, x, z); } set { y = value.x; x = value.y; z = value.z; } }
		public Vec3 yzx { get { return new Vec3(y, z, x); } set { y = value.x; z = value.y; x = value.z; } }
		public Vec3 zxy { get { return new Vec3(z, x, y); } set { z = value.x; x = value.y; y = value.z; } }
		public Vec3 zyx { get { return new Vec3(z, y, x); } set { z = value.x; y = value.y; x = value.z; } }
		public Vec2 xy { get { return new Vec2(x, y); } set { x = value.x; y = value.y; } }
		public Vec2 yx { get { return new Vec2(y, x); } set { y = value.x; x = value.y; } }
		public Vec2 zx { get { return new Vec2(z, x); } set { z = value.x; x = value.y; } }
		public Vec2 xz { get { return new Vec2(x, z); } set { x = value.x; z = value.y; } }
		public Vec2 zy { get { return new Vec2(z, y); } set { z = value.x; y = value.y; } }
		public Vec2 yz { get { return new Vec2(y, z); } set { y = value.x; z = value.y; } }
	}
}