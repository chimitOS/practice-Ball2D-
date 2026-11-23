using System.Numerics;
using System.Runtime.Intrinsics.X86;

namespace Ball2D
{
    public static class Physics
    {

        public static float Restitution = 1.0f;
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
                float restitution = Restitution;

                float m1 = b1.Radius;
                float m2 = b2.Radius;

                float impulseMagnitude = -(1 + restitution) * velAlongNormal / (1 / m1 + 1 / m2);

                b1.Velocity -= normal * (impulseMagnitude / m1);
                b2.Velocity += normal * (impulseMagnitude / m2);
            }
        }

        public static void ResolveStickCollision(Ball ball, Line line)
        {
            CustomVector2 ab = line.End - line.Start;       //вектор палки
            CustomVector2 ap = ball.Position - line.Start;  //вектор от центра шара до начала палки

            float abLenSq = ab.LengthSquared();
            if (abLenSq == 0) return;


            float t = CustomVector2.Dot(ap, ab) / abLenSq;
            if (t < 0.0f) t = 0.0f;
            if (t > 1.0f) t = 1.0f;

            CustomVector2 closesPoint = line.Start + ab * t; //точные координаты проекции шара

            CustomVector2 delta = ball.Position - closesPoint;
            float distance = delta.Length();

            if (distance < ball.Radius)
            {
                CustomVector2 normal = delta / distance; //направление отскока от палки

                float penetration = ball.Radius - distance;
                ball.Position += normal * penetration;

                float velAlongNormal = CustomVector2.Dot(ball.Velocity, normal);

                if (velAlongNormal < 0)
                {
                    float restitution = Restitution;

                    ball.Velocity -= normal * ((1 + restitution) * velAlongNormal);
                }

            }
        }
    }
}