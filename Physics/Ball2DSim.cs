using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Ball2D
{
    public class Ball2DSim : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch? _spriteBatch;
        private Texture2D? _pixel;

        private Simulation _simulation;

        public Ball2DSim()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _graphics.PreferredBackBufferWidth = 800;
            _graphics.PreferredBackBufferHeight = 500;

            _simulation = new Simulation();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _pixel = new Texture2D(GraphicsDevice, 1, 1);
            _pixel.SetData(new[] { Color.White });
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            _simulation.Update(deltaTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(245, 245, 245));

            _spriteBatch!.Begin();

            foreach (var line in _simulation.Lines)
            {
                DrawLine(_spriteBatch!, line.Start, line.End, Color.Black, 4f);
            }

            foreach (var ball in _simulation.Balls)
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