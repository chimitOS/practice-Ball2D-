using System.Numerics;

namespace Ball2D
{
    public static class Physics
    {
        public static void ResolveScreenBounds(Ball ball, int screenWidth, int screenHeight)
        {

            //левая стена
            if (ball.Position.X - ball.Radius < 0)
            {
                ball.Position.X = ball.Radius;
                ball.Velocity.X = -ball.Velocity.X;
            }

            //правая стена
            else if (ball.Position.X + ball.Radius > screenWidth)
            {
                ball.Position.X = screenWidth - ball.Radius;
                ball.Velocity.X = -ball.Velocity.X;
            }

            //верхняя стена
            if (ball.Position.Y - ball.Radius < 0)
            {
                ball.Position.Y = ball.Radius;
                ball.Velocity.Y = -ball.Velocity.Y;
            }

            //нижняя стена
            else if (ball.Position.Y + ball.Radius > screenHeight)
            {
                ball.Position.Y = screenHeight - ball.Radius;
                ball.Velocity.Y = -ball.Velocity.Y;
            }
        }
        public static void ResolveBallCollision(Ball b1, Ball b2)
        {
            CustomVector2 delta = b2.Position - b1.Position; //вектор смещения между центрами
            float distance = delta.Length();                 //длина вектора в пикселях
            float minDistance = b1.Radius + b2.Radius;

            if (distance <= minDistance)
            {
                //направление удара (от центра первого шара к центру второго)
                CustomVector2 normal = delta / distance;

                //разрешение проникновения
                float penetration = minDistance - distance;

                CustomVector2 separation = normal * (penetration / 2.0f);
                b1.Position -= separation;
                b2.Position += separation;

                //расчет импульса
                CustomVector2 relativeVelocity = b2.Velocity - b1.Velocity;

                //скорость сближения
                float velAlongNormal = CustomVector2.Dot(relativeVelocity, normal);

                //коэффициент упругости
                float restitution = 1.0f;

                float m1 = b1.Radius;
                float m2 = b2.Radius;

                float impulseMagnitude = -(1 + restitution) * velAlongNormal / (1 / m1 + 1 / m2);

                b1.Velocity -= normal * (impulseMagnitude / m1);
                b2.Velocity += normal * (impulseMagnitude / m2);
            }
        }
    }
}