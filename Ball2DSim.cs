using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ball2D
{
    public class Ball2DSim : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch? _spriteBatch;
        private Texture2D? _pixel;
        private List<Ball> _balls = new List<Ball>();
        private List<Line> _lines = new List<Line>();

        public Ball2DSim()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _graphics.PreferredBackBufferWidth = 800;
            _graphics.PreferredBackBufferHeight = 500;
        }

        protected override void Initialize()
        {
            int screenWidth = _graphics.PreferredBackBufferWidth;
            int screenHeight = _graphics.PreferredBackBufferHeight;

            Random rand = new Random();
            for (int i = 0; i < 20; i++)
            {
                _balls.Add(new Ball(
                    position: new CustomVector2(rand.Next(100, screenWidth - 100), rand.Next(100, screenHeight - 100)),
                    velocity: new CustomVector2(rand.Next(-250, 250), rand.Next(-250, 250)),
                    radius: rand.Next(15, 30)
                ));

                _lines.Add(new Line(new CustomVector2(200, 400), new CustomVector2(600, 250)));
                _lines.Add(new Line(new CustomVector2(150, 100), new CustomVector2(150, 300)));

            }

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _pixel = new Texture2D(GraphicsDevice, 1, 1);
            _pixel.SetData(new[] { Color.White });
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            foreach (var ball in _balls)
            {
                ball.Update(dt);
                Physics.ResolveScreenBounds(ball, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);

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

            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(245, 245, 245));

            _spriteBatch!.Begin();

            foreach (var line in _lines)
            {
                DrawLine(_spriteBatch!, line.Start, line.End, Color.Black, 4f);
            }

            foreach (var ball in _balls)
            {
                DrawCircle(_spriteBatch!, ball.Position, ball.Radius, Color.Black, 3f);
            }

            _spriteBatch!.End();

            base.Draw(gameTime);
        }

        private void DrawLine(SpriteBatch sb, CustomVector2 start, CustomVector2 end, Color color, float thickness = 1f)
        {
            CustomVector2 edge = end - start;
            float angle = (float)Math.Atan2(edge.Y, edge.X);

            sb.Draw(_pixel,
                start.ToXna(),
                null,
                color,
                angle,
                CustomVector2.Zero.ToXna(),
                new CustomVector2(edge.Length(), thickness).ToXna(),
                SpriteEffects.None,
                0);
        }

        private void DrawCircle(SpriteBatch sb, CustomVector2 center, float radius, Color color, float thickness = 1f, int segments = 32)
        {
            float step = MathHelper.TwoPi / segments;
            for (float theta = 0; theta < MathHelper.TwoPi; theta += step)
            {
                CustomVector2 p1 = center + new CustomVector2((float)Math.Cos(theta), (float)Math.Sin(theta)) * radius;
                CustomVector2 p2 = center + new CustomVector2((float)Math.Cos(theta + step), (float)Math.Sin(theta + step)) * radius;
                DrawLine(sb, p1, p2, color, thickness);
            }
        }
    }
}