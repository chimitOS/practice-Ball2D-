using Ball2D;
using System.Collections.Generic;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Shapes;

namespace billiards_wpf_.Physics
{
    public class BilliardTable
    {
        //игровое поле
        public float Left = 60;
        public float Top = 80;
        public float Width = 680;
        public float Height = 340;
        public float Right => Left + Width;
        public float Bottom => Top + Height;

        //лузы
        public float PocketRadius = 22;
        public float CenterX => Left + Width / 2;

        public List<CustomVector2> Pockets = new List<CustomVector2>();

        public BilliardTable()
        {
            Pockets.Add(new CustomVector2(Left, Top));
            Pockets.Add(new CustomVector2(CenterX, Top));
            Pockets.Add(new CustomVector2(Right, Top));
            Pockets.Add(new CustomVector2(Left, Bottom));
            Pockets.Add(new CustomVector2(CenterX, Bottom));
            Pockets.Add(new CustomVector2(Right, Bottom));
        }

        /// <summary>
        /// Проверка, попал ли шар в лузу
        /// </summary>
        public bool IsBallPocketed(Ball ball)
        {
            foreach (var pocket in Pockets)
            {
                float dx = ball.Position.X - pocket.X;
                float dy = ball.Position.Y - pocket.Y;

                float disntace = dx * dx + dy * dy;

                if (disntace <=  PocketRadius * PocketRadius)
                {
                    return true;
                }
            }
            return false;
        }

    }
}
