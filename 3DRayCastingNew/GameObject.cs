using System.Collections.Generic;

namespace _3DRayCastingNew
{
    public class GameObject
    {
        public int ID;
        public int[] MergedID;
        public string Shape;
        //public Vec3 Rotation;
        public Vec3 Vert1;
        public Vec3 Vert2;
        public Vec3 Vert3;
        public Vec3 Position;
        public Vec3 Size;
        public double Radius;
        public Vec3 Color;
        public double Brightness;
        public double Diffusion;
        public double Reflection;
        public double Refraction;
        public double RefractionRate;

        public double Intersection = 0;
        public double CurrentDistance { get { return Intersection; } set { value = Intersection; } }
        public GameObject(int ID, int[] MergedID, string Shape, Vec3 Vert1, Vec3 Vert2, Vec3 Vert3, Vec3 Color, double Brightness, double Diffusion, double Reflection, double Refraction, double RefractionRate) // Треугольник
        {
            this.ID = ID;
            this.MergedID = MergedID;
            this.Shape = Shape;
            this.Vert1 = Vert1;
            this.Vert2 = Vert2;
            this.Vert3 = Vert3;
            this.Color = Color;
            this.Brightness = Brightness;
            this.Diffusion = Diffusion;
            this.Reflection = Reflection;
            this.Refraction = Refraction;
            this.RefractionRate = RefractionRate;
        }
        public GameObject(int ID, string Shape, Vec3 Position, Vec3 Size, Vec3 Color, double Brightness, double Diffusion, double Reflection, double Refraction, double RefractionRate) // Блок
        {
            this.ID = ID;
            this.Shape = Shape;
            this.Position = Position;
            this.Size = Size;
            this.Color = Color;
            this.Brightness = Brightness;
            this.Diffusion = Diffusion;
            this.Reflection = Reflection;
            this.Refraction = Refraction;
            this.RefractionRate = RefractionRate;
        }
        public GameObject(int ID, string Shape, Vec3 Position, double Radius, Vec3 Color, double Brightness, double Diffusion, double Reflection, double Refraction, double RefractionRate) // Сфера
        {
            this.ID = ID;
            this.Shape = Shape;
            this.Position = Position;
            this.Radius = Radius;
            this.Color = Color;
            this.Brightness = Brightness;
            this.Diffusion = Diffusion;
            this.Reflection = Reflection;
            this.Refraction = Refraction;
            this.RefractionRate = RefractionRate;
        }
    }
}
