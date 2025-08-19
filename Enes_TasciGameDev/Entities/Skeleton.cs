using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Enes_TasciGameDev.Entities
{
    public class Skeleton
    {
        private Texture2D texture;
        private Vector2 position;
        private float speed;
        private bool stunned = false;
        private double stunTimer = 0;
        private double stunDuration = 1.0;

        private int rows = 4;    // 4 directions: up, right, down, left
        private int columns = 3; // 4 frames per direction
        private int currentFrame = 0;
        private int row = 0;
        private double timer = 0;
        private double interval = 200;
        private int frameWidth;
        private int frameHeight;

        public Skeleton(Texture2D texture, Vector2 position, float speed)
        {
            this.texture = texture;
            this.position = position;
            this.speed = speed;

            frameWidth = texture.Width / columns;   // 144 / 4 = 36
            frameHeight = texture.Height / rows + 1;    // 256 / 4 = 64
        }

        public void Update(GameTime gameTime, Vector2 playerPosition, List<Skeleton> allSkeletons)
        {
            if (stunned)
            {
                stunTimer += gameTime.ElapsedGameTime.TotalSeconds;
                if (stunTimer >= stunDuration)
                {
                    stunned = false;
                    stunTimer = 0;
                }
                return;
            }

            Vector2 direction = playerPosition - position;
            if (direction != Vector2.Zero)
            {
                direction.Normalize();
                position += direction * speed;
            }

            foreach (var other in allSkeletons)
            {
                if (other == this) continue;

                Vector2 diff = position - other.position;
                float distance = diff.Length();

                if (distance < frameWidth)
                {
                    diff.Normalize();
                    position += diff * (frameWidth - distance) * 0.5f;
                }
            }

            timer += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (timer > interval)
            {
                currentFrame++;
                if (currentFrame >= columns) currentFrame = 0;
                timer = 0;
            }

            // Determine row based on movement
            if (direction.X > 0) row = 1;       // right
            else if (direction.X < 0) row = 3;  // left
            else if (direction.Y > 0) row = 2;  // down
            else if (direction.Y < 0) row = 0;  // up
        }

        public void Stun()
        {
            stunned = true;
            stunTimer = 0;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Rectangle sourceRect = new Rectangle(currentFrame * frameWidth, row * frameHeight, frameWidth, frameHeight);
            spriteBatch.Draw(texture, position, sourceRect, stunned ? Color.LightGray : Color.White);
        }

        public Rectangle GetBounds()
        {
            return new Rectangle((int)position.X, (int)position.Y, frameWidth, frameHeight);
        }
    }
}
