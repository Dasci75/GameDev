using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Enes_TasciGameDev.Entities
{
    public class Thief
    {
        private Texture2D texture;
        public Vector2 Position;
        private float speed;
        private int rows = 4;
        private int columns = 3;
        private int currentFrame = 0;
        private int row = 2;
        private double timer = 0;
        private double interval = 200; // ms per frame
        private int frameWidth;
        private int frameHeight;

        private Vector2 direction;
        private double changeDirTimer = 0;
        private double changeDirInterval;
        private static Random rnd = new Random();

        public Thief(Texture2D texture, Vector2 position, float speed)
        {
            this.texture = texture;
            this.Position = position;
            this.speed = speed;

            frameWidth = texture.Width / columns;
            frameHeight = texture.Height / rows;

            ChooseRandomDirection();
            changeDirInterval = rnd.NextDouble() * 2 + 1; // 1–3 seconden
        }

        private void ChooseRandomDirection()
        {
            float angle = (float)(rnd.NextDouble() * Math.PI * 2);
            direction = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        }

        public void Update(GameTime gameTime, Player player, int screenWidth, int screenHeight)
        {
            // Willekeurig richting veranderen
            changeDirTimer += gameTime.ElapsedGameTime.TotalSeconds;
            if (changeDirTimer > changeDirInterval)
            {
                ChooseRandomDirection();
                changeDirTimer = 0;
                changeDirInterval = rnd.NextDouble() * 2 + 1; // nieuwe interval
            }

            // Beweging
            Position += direction * speed;

            // Binnen scherm houden
            Position.X = MathHelper.Clamp(Position.X, 0, screenWidth - frameWidth);
            Position.Y = MathHelper.Clamp(Position.Y, 0, screenHeight - frameHeight);

            // Animatie row
            if (Math.Abs(direction.X) > Math.Abs(direction.Y))
                row = direction.X > 0 ? 2 : 1;
            else
                row = direction.Y > 0 ? 0 : 3;

            // Alleen animatie updaten als thief beweegt
            if (direction != Vector2.Zero)
            {
                timer += gameTime.ElapsedGameTime.TotalMilliseconds;
                if (timer > interval)
                {
                    currentFrame = (currentFrame + 1) % columns;
                    timer = 0;
                }
            }
            else
            {
                currentFrame = 0;
            }

            // Coin stelen
            if (GetBounds().Intersects(player.GetBounds()) && player.Coins > 0)
            {
                player.Coins--;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Rectangle sourceRect = new Rectangle(currentFrame * frameWidth, row * frameHeight, frameWidth, frameHeight);
            spriteBatch.Draw(texture, Position, sourceRect, Color.White);
        }

        public Rectangle GetBounds()
        {
            // Kleine padding zodat collision makkelijker werkt
            int padding = 4;
            return new Rectangle((int)Position.X + padding, (int)Position.Y + padding,
                                 frameWidth - 2 * padding, frameHeight - 2 * padding);
        }
    }
}
