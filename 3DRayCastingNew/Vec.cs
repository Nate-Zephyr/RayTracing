using System;

namespace _3DRayCastingNew
{
    public static class Vec
    {
        public static double Sign(double a) { return Convert.ToDouble(0 < a) - Convert.ToDouble(a < 0); }
        public static double Step(double edge, double a) { return Convert.ToDouble(a > edge); }
        public static double Length(Vec2 v) { return Math.Sqrt(v.x * v.x + v.y * v.y); }
        public static double Length(Vec3 v) { return Math.Sqrt(v.x * v.x + v.y * v.y + v.z * v.z); }
        public static double Dot(Vec2 a, Vec2 b) { return a.x * b.x + a.y * b.y; }
        public static double Dot(Vec3 a, Vec3 b) { return a.x * b.x + a.y * b.y + a.z * b.z; }
        public static Vec2 Norm(Vec2 v) { return v / new Vec2(Length(v)); }
        public static Vec3 Norm(Vec3 v) { return v / new Vec3(Length(v)); }
        public static Vec2 Abs(Vec2 v) { return new Vec2(Math.Abs(v.x), Math.Abs(v.y)); }
        public static Vec3 Abs(Vec3 v) { return new Vec3(Math.Abs(v.x), Math.Abs(v.y), Math.Abs(v.z)); }
        public static Vec2 Sign(Vec2 v) { return new Vec2(Sign(v.x), Sign(v.y)); }
        public static Vec3 Sign(Vec3 v) { return new Vec3(Sign(v.x), Sign(v.y), Sign(v.z)); }
        public static Vec2 Step(Vec2 edge, Vec2 v) { return new Vec2(Step(edge.x, v.x), Step(edge.y, v.y)); }
        public static Vec3 Step(Vec3 edge, Vec3 v) { return new Vec3(Step(edge.x, v.x), Step(edge.y, v.y), Step(edge.z, v.z)); }
        public static Vec2 Max(Vec2 first, Vec2 second) { return new Vec2(Math.Max(first.x, second.x), Math.Max(first.y, second.y)); }
        public static Vec3 Max(Vec3 first, Vec3 second) { return new Vec3(Math.Max(first.x, second.x), Math.Max(first.y, second.y), Math.Max(first.z, second.z)); }
        public static Vec3 Cross(Vec3 first, Vec3 second) { return new Vec3(first.y * second.z - first.z * second.y, first.z * second.x - first.x * second.z, first.x * second.y - first.y * second.x); }

        public static Mat3 Rotate3(Vec2 angle)
        {
            double sH = Math.Sin(angle.x);
            double cH = Math.Cos(angle.x);
            double sV = Math.Sin(angle.y);
            double cV = Math.Cos(angle.y);

            return new Mat3(cH, sH * cV, -sH * sV,
                           -sH, cH * cV, -cH * sV,
                           0, sV, cV);
        }

        public static Vec3 RotateX(Vec3 a, double angle)
        {
            Vec3 b = a;
            b.z = a.z * Math.Cos(angle) - a.y * Math.Sin(angle);
            b.y = a.z * Math.Sin(angle) + a.y * Math.Cos(angle);
            return b;
        }

        public static Vec3 RotateY(Vec3 a, double angle)
        {
            Vec3 b = a;
            b.x = a.x * Math.Cos(angle) - a.z * Math.Sin(angle);
            b.z = a.x * Math.Sin(angle) + a.z * Math.Cos(angle);
            return b;
        }

        public static Vec3 RotateZ(Vec3 a, double angle)
        {
            Vec3 b = a;
            b.x = a.x * Math.Cos(angle) - a.y * Math.Sin(angle);
            b.y = a.x * Math.Sin(angle) + a.y * Math.Cos(angle);
            return b;
        }
        public static double triIntersection(Vec3 ro, Vec3 rd, Vec3 v1, Vec3 v2, Vec3 v3, ref Vec3 outNormal)
        {
            Vec3 v2v1 = v2 - v1;
            Vec3 v3v1 = v3 - v1;
            Vec3 rov1 = ro - v1;
            Vec3 n = Cross(v2v1, v3v1);
            Vec3 q = Cross(rov1, rd);
            double d = 1.0 / Dot(rd, n);
            double u = d * Dot(-q, v3v1);
            double v = d * Dot(q, v2v1);
            double t = d * Dot(-n, rov1);
            if (u < 0.0 || u > 1.0 || v < 0.0 || (u + v) > 1.0) return -1; // Нет пересечения

            outNormal = Norm(n);
            return t;
        }

        public static Vec2 sphIntersection(Vec3 ro, Vec3 rd, Vec3 pos, double radius, ref Vec3 outNormal)
        {
            ro -= pos;

            double b = Dot(ro, rd);
            double c = Dot(ro, ro) - radius * radius;
            double h = b * b - c;
            if (h < 0) return new Vec2(-1); // Нет пересечения

            h = Math.Sqrt(h);

            outNormal = Norm(ro + rd * (-b - h));
            //return -b - h;
            return new Vec2(-b - h, -b + h);
        }

        public static Vec2 boxIntersection(Vec3 ro, Vec3 rd, Vec3 pos, Vec3 size, ref Vec3 outNormal)
        {
            ro -= pos;

            Vec3 m = new Vec3(1) / rd;
            Vec3 n = m * ro;
            Vec3 k = Abs(m) * size;
            Vec3 t1 = -n - k;
            Vec3 t2 = -n + k;
            double tN = Math.Max(Math.Max(t1.x, t1.y), t1.z);
            double tF = Math.Min(Math.Min(t2.x, t2.y), t2.z);
            if (tN > tF || tF < 0) return new Vec2(-1); // Нет пересечения

            outNormal = -Sign(rd) * Step(t1.yzx, t1) * Step(t1.zxy, t1);
            return new Vec2(tN, tF);
        }
    }
}
