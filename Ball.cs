namespace Ball2D
{
    public class Ball
    {
        public CustomVector2 Position;
        public CustomVector2 Velocity;
        public float Radius;
        public float mass = 1.0f;

        public Ball(CustomVector2 position, CustomVector2 velocity, float radius)
        {
            Position = position;
            Velocity = velocity;
            Radius = radius;
        }

        public void Update(float deltaTime)
        {
            Position += Velocity * deltaTime;
        }
    }
}