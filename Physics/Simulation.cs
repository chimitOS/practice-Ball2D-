using System;
using System.Collections.Generic;
using System.Text;

namespace Ball2D
{
    public class Simulation
    {
        private List<Ball> _balls = new List<Ball>();
        private List<Line> _lines = new List<Line>();
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

            Random rand = new Random();
            for (int i = 0; i < 20; i++)
            {
                _balls.Add(new Ball(
                    position: new CustomVector2(rand.Next(100, screenWidth - 100), rand.Next(100, screenHeight - 100)),
                    velocity: new CustomVector2(rand.Next(-250, 250), rand.Next(-250, 250)),
                    radius: rand.Next(15, 30)
                ));
            }

            _lines.Add(new Line(new CustomVector2(200, 400), new CustomVector2(600, 250)));
            _lines.Add(new Line(new CustomVector2(150, 100), new CustomVector2(150, 300)));
        }
        public void Update(float deltaTime)
        {

            foreach (var ball in _balls)
            {
                ball.Update(deltaTime);
                Physics.ResolveScreenBounds(ball, WorldWidth, WorldHeight);

                foreach (var line in _lines)
                {
                    Physics.ResolveStickCollision(ball, line);
                }
            }

            for (int i = 0; i < _balls.Count; i++)
            {
                for (int j = i + 1; j < _balls.Count; j++)
                {
                    Physics.ResolveBallCollision(_balls[i], _balls[j]);
                }
            }
        }
        public IReadOnlyList<Ball> Balls => _balls;
        public IReadOnlyList<Line> Lines => _lines;
    }
}
