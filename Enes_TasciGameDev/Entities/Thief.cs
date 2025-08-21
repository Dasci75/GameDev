using Enes_TasciGameDev.Prop;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Enes_TasciGameDev.Entities
{
    public class Thief : Enemy
    {
        private Texture2D texture;
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

        private double stealCooldown = 1.0;
        private double stealTimer = 0;

        public Thief(Texture2D texture, Vector2 position, float speed)
        {
            this.texture = texture;
            Position = position;
            this.speed = speed;

            frameWidth = texture.Width / columns;
            frameHeight = texture.Height / rows;

            ChooseRandomDirection();
            changeDirInterval = rnd.NextDouble() * 2 + 1; // 1–3 seconds
        }

        private void ChooseRandomDirection()
        {
            float angle = (float)(rnd.NextDouble() * Math.PI * 2);
            direction = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        }

        public override Rectangle GetBounds()
        {
            int padding = 4;
            return new Rectangle((int)Position.X + padding, (int)Position.Y + padding,
                                frameWidth - 2 * padding, frameHeight - 2 * padding);
        }

        public override void Update(GameTime gameTime, Vector2 playerPosition, List<Enemy> enemies, List<Obstacle> obstacles, int screenWidth, int screenHeight)
        {
            // Random direction change timer
            changeDirTimer += gameTime.ElapsedGameTime.TotalSeconds;
            if (changeDirTimer > changeDirInterval)
            {
                ChooseRandomDirection();
                changeDirTimer = 0;
                changeDirInterval = rnd.NextDouble() * 2 + 1;
            }

            // Movement (delta time → framerate onafhankelijk)
            Vector2 newPosition = Position + direction * speed;

            // Clamp position
            if (newPosition.X <= 0 || newPosition.X >= screenWidth - frameWidth)
            {
                direction.X *= -1;
                newPosition.X = MathHelper.Clamp(newPosition.X, 0, screenWidth - frameWidth);
            }

            if (newPosition.Y <= 0 || newPosition.Y >= screenHeight - frameHeight)
            {
                direction.Y *= -1;
                newPosition.Y = MathHelper.Clamp(newPosition.Y, 0, screenHeight - frameHeight);
            }

            Position = newPosition;

            // Animation row
            if (Math.Abs(direction.X) > Math.Abs(direction.Y))
                row = direction.X > 0 ? 1 : 3;
            else
                row = direction.Y > 0 ? 2 : 0;

            // Update animation
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

            // Coin stealing
            stealTimer += gameTime.ElapsedGameTime.TotalSeconds;
            if (GetBounds().Intersects(new Rectangle((int)playerPosition.X, (int)playerPosition.Y, 32, 32)) && stealTimer >= stealCooldown)
            {
                stealTimer = 0;
            }
        }


        public override void Draw(SpriteBatch spriteBatch)
        {
            Rectangle sourceRect = new Rectangle(currentFrame * frameWidth, row * frameHeight, frameWidth, frameHeight);
            Rectangle destRect = new Rectangle((int)Position.X, (int)Position.Y, frameWidth, frameHeight);

            spriteBatch.Draw(texture, destRect, sourceRect, Color.White);
        }


        public override void Stun()
        {
            // Thief doesn't support stunning
        }
    }
}