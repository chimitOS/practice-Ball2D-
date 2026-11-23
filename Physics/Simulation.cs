using billiards_wpf_.Physics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Documents;

namespace Ball2D
{
    public class Simulation
    {
        private List<Ball> _balls = new List<Ball>();
        private List<Line> _lines = new List<Line>();
        public SimulationSettings Settings = new SimulationSettings();
        public BilliardTable billiardTable = new BilliardTable();

        public Ball CueBall { get; private set; }

        int WorldWidth = 800;
        int WorldHeight = 500;

        public Simulation()
        {
            Initialize();
        }
        private void Initialize()
        {
            int screenWidth = WorldWidth;
            int screenHeight = WorldHeight;

            var table = billiardTable;
            float gap = table.PocketRadius;

            float cueX = billiardTable.Left + billiardTable.Width / 4;
            float cueY = billiardTable.Top + billiardTable.Height / 2;

            CueBall = new Ball(
                position: new CustomVector2(cueX, cueY),
                velocity: new CustomVector2(0, 0),
                radius: 15
            );

            _balls.Add(CueBall);

            float radius = 15f;
            float spacing = radius * 2 + 0.5f;
            float startX = billiardTable.Left + billiardTable.Width * 0.65f;
            float centerY = billiardTable.Top + billiardTable.Height / 2;

            for (int row = 0; row < 5; row++)
            {
                for (int column = 0; column <= row; column++)
                {
                    _balls.Add(new Ball(
                        position: new CustomVector2(startX + row * spacing * (float)Math.Sqrt(3.0) / 2f, 
                                                    centerY + (column - row/2.0f) * spacing),
                        velocity: new CustomVector2(0, 0),
                        radius: 15
                    ));
                }
            }

            //левый верхний борт
            _lines.Add(new Line(
                new CustomVector2(table.Left + gap, table.Top),
                new CustomVector2(table.CenterX - gap, table.Top))
            );

            //левый нижний борт
            _lines.Add(new Line(
                new CustomVector2(table.Left + gap, table.Bottom),
                new CustomVector2(table.CenterX - gap, table.Bottom))
            );

            //правый нижний борт
            _lines.Add(new Line(
                new CustomVector2(table.CenterX + gap, table.Bottom),
                new CustomVector2(table.Right - gap, table.Bottom))
            );

            //правый верхний борт
            _lines.Add(new Line(
                new CustomVector2(table.CenterX + gap, table.Top),
                new CustomVector2(table.Right - gap, table.Top))
            );

            //левый борт
            _lines.Add(new Line(
                new CustomVector2(table.Left, table.Top + gap),
                new CustomVector2(table.Left, table.Bottom - gap))
            );

            //правый борт
            _lines.Add(new Line(
                new CustomVector2(table.Right, table.Bottom - gap),
                new CustomVector2(table.Right, table.Top + gap))
            );
        }

        public bool AreAllBallsStopped()
        {
            foreach (var ball in _balls)
            {
                if (ball.Velocity.LengthSquared() >= 0.0001f)
                {
                    return false;
                }
            }
            return true;
        }

        public void Update(float deltaTime)
        {
            List<Ball> ballsInPocket = new List<Ball>();

            foreach (var ball in _balls)
            {
                ball.Update(deltaTime, Settings.Friction);
                Physics.ResolveScreenBounds(ball, WorldWidth, WorldHeight);

                if (billiardTable.IsBallPocketed(ball))
                {
                    ballsInPocket.Add(ball);
                    continue;
                }

                foreach (var line in _lines)
                {
                    Physics.ResolveStickCollision(ball, line, Settings.Restitution);
                }
            }

            foreach (var ball in ballsInPocket)
            {
                _balls.Remove(ball);
            }

            for (int i = 0; i < _balls.Count; i++)
            {
                for (int j = i + 1; j < _balls.Count; j++)
                {
                    Physics.ResolveBallCollision(_balls[i], _balls[j], Settings.Restitution);
                }
            }
        }
        public IReadOnlyList<Ball> Balls => _balls;
        public IReadOnlyList<Line> Lines => _lines;
    }
}
