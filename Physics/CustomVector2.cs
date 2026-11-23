namespace Ball2D
{
    public struct CustomVector2
    {
        public float X;
        public float Y;

        public CustomVector2(float x, float y)
        {
            X = x;
            Y = y;
        }

        public float Length() => (float)Math.Sqrt(X * X + Y * Y);
        public float LengthSquared() => X * X + Y * Y;

        public static CustomVector2 operator +(CustomVector2 a, CustomVector2 b) => new CustomVector2(a.X + b.X, a.Y + b.Y);
        public static CustomVector2 operator -(CustomVector2 a, CustomVector2 b) => new CustomVector2(a.X - b.X, a.Y - b.Y);
        public static CustomVector2 operator *(CustomVector2 a, float scalar) => new CustomVector2(a.X * scalar, a.Y * scalar);
        public static CustomVector2 operator /(CustomVector2 a, float scalar) => new CustomVector2(a.X / scalar, a.Y / scalar);

        public static float Dot(CustomVector2 a, CustomVector2 b) => (a.X * b.X) + (a.Y * b.Y);

        public static CustomVector2 Zero => new CustomVector2(0f, 0f);

        public Microsoft.Xna.Framework.Vector2 ToXna() => new Microsoft.Xna.Framework.Vector2(X, Y);
    }
}
