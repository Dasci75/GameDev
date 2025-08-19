using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Enes_TasciGameDev.Entities
{
    public class Thief
    {
        private Texture2D texture;
        public Vector2 Position;
        private float speed;

        // Animatie
        private int rows = 4;      // up, right, down, left
        private int columns = 3;   // frames per row
        private int currentFrame = 0;
        private int row = 2;        // start facing down
        private double timer = 0;
        private double interval = 200; // ms per frame
        private int frameWidth;
        private int frameHeight;

        // Coin target
        private Coin targetCoin;
        private double stealDelay = 0.5; // seconden na stelen
        private double delayTimer = 0;
        private bool stealing = false;

        public Thief(Texture2D texture, Vector2 position, float speed)
        {
            this.texture = texture;
            this.Position = position;
            this.speed = speed;

            frameWidth = texture.Width / columns;
            frameHeight = texture.Height / rows;
        }

        public void Update(GameTime gameTime, List<Coin> coins)
        {
            if (coins.Count == 0) return; // niks om te stelen

            // Kies dichtstbijzijnde coin
            if (targetCoin == null || !coins.Contains(targetCoin))
            {
                float minDist = float.MaxValue;
                foreach (var coin in coins)
                {
                    float dist = Vector2.Distance(Position, coin.Position);
                    if (dist < minDist)
                    {
                        minDist = dist;
                        targetCoin = coin;
                    }
                }
            }

            if (targetCoin != null)
            {
                Vector2 direction = targetCoin.Position - Position;

                if (direction.Length() > speed)
                {
                    direction.Normalize();
                    Position += direction * speed;
                }
                else
                {
                    // Coin bereikt
                    coins.Remove(targetCoin);
                    targetCoin = null;
                }

                // Animatie row
                if (Math.Abs(direction.X) > Math.Abs(direction.Y))
                    row = direction.X > 0 ? 1 : 3;
                else
                    row = direction.Y > 0 ? 2 : 0;
            }

            // Animatie frame update
            timer += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (timer > interval)
            {
                currentFrame = (currentFrame + 1) % columns;
                timer = 0;
            }
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            Rectangle sourceRect = new Rectangle(currentFrame * frameWidth, row * frameHeight, frameWidth, frameHeight);
            spriteBatch.Draw(texture, Position, sourceRect, Color.White);
        }

        public Rectangle GetBounds()
        {
            return new Rectangle((int)Position.X, (int)Position.Y, frameWidth, frameHeight);
        }
    }
}
