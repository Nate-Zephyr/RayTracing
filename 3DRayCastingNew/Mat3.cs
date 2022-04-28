namespace _3DRayCastingNew
{
    public class Mat3
    {
		public double a1, a2, a3, b1, b2, b3, c1, c2, c3;

		public Mat3(double a1, double a2, double a3, double b1, double b2, double b3, double c1, double c2, double c3)
		{
			this.a1 = a1; this.a2 = a2; this.a3 = a3;
			this.b1 = b1; this.b2 = b2; this.b3 = b3;
			this.c1 = c1; this.c2 = c2; this.c3 = c3;
		}

		public Mat3(Mat3 mat)
		{
			a1 = mat.a1; a2 = mat.a2; a3 = mat.a3;
			b1 = mat.b1; b2 = mat.b2; b3 = mat.b3;
			c1 = mat.c1; c2 = mat.c2; c3 = mat.c3;
		}

		public static Mat3 operator -(Mat3 m) { return new Mat3(-m.a1, -m.a2, -m.a3, -m.b1, -m.b2, -m.b3, -m.c1, -m.c2, -m.c3); }
    }
}