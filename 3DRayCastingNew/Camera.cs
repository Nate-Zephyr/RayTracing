namespace _3DRayCastingNew
{
    public class Camera
    {
        public Vec3 Position;
        public Vec2 Rotation;
        public double FOV;
        public Camera(Vec3 Position, Vec2 Rotation, double FOV)
        {
            this.Position = Position;
            this.Rotation = Rotation;
            this.FOV = FOV;
        }
    }
}
