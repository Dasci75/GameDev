using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Enes_TasciGameDev.Obs;
using Enes_TasciGameDev.Entities;

namespace Enes_TasciGameDev.Entities
{
    public class Goblin : Enemy
    {
        private Texture2D texture;
        private float speed;
        private bool stunned = false;
        private double stunTimer = 0;
        private double stunDuration = 2.0; // 2 seconds
        private int rows = 4;
        private int columns = 3;
        private int currentFrame = 0;
        private int row = 0;
        private double timer = 0;
        private double interval = 200; // ms per frame
        private int frameWidth;
        private int frameHeight;

        public Goblin(Texture2D texture, Vector2 position, float speed)
        {
            this.texture = texture;
            Position = position; // Use the Position property from Enemy
            this.speed = speed;
            frameWidth = texture.Width / columns;
            frameHeight = (texture.Height / rows) + 1;
        }

        public override Vector2 Position { get; set; } // This should now work

        public override Rectangle GetBounds()
        {
            return new Rectangle((int)Position.X, (int)Position.Y, frameWidth, frameHeight);
        }

        public override void Update(GameTime gameTime, Vector2 playerPosition, List<Enemy> enemies, List<Obstacle> obstacles, int screenWidth, int screenHeight)
        {
            // Handle stunned state
            if (stunned)
            {
                stunTimer += gameTime.ElapsedGameTime.TotalSeconds;
                if (stunTimer >= stunDuration)
                {
                    stunned = false;
                    stunTimer = 0;
                }
                return; // Stop moving while stunned
            }

            // Calculate proposed new position
            Vector2 direction = playerPosition - Position;
            Vector2 newPosition = Position;
            if (direction != Vector2.Zero)
            {
                direction.Normalize();
                newPosition = Position + direction * speed;
            }

            // Check collisions with obstacles
            bool collisionDetected = false;
            if (obstacles != null)
            {
                Rectangle newGoblinBounds = new Rectangle(
                    (int)newPosition.X,
                    (int)newPosition.Y,
                    frameWidth,
                    frameHeight
                );

                foreach (var obstacle in obstacles)
                {
                    if (newGoblinBounds.Intersects(obstacle.Bounds))
                    {
                        collisionDetected = true;
                        newPosition = Position; // Revert to previous position
                        break;
                    }
                }
            }

            // Update position if no collision
            if (!collisionDetected)
            {
                Position = newPosition;
            }

            // Separation to avoid stacking with other enemies
            foreach (var other in enemies)
            {
                if (other == this) continue;

                Vector2 diff = Position - other.Position;
                float distance = diff.Length();

                if (distance < frameWidth) // If too close
                {
                    diff.Normalize();
                    Position += diff * (frameWidth - distance) * 0.5f; // Push away
                }
            }

            // Update animation
            timer += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (timer > interval)
            {
                currentFrame++;
                if (currentFrame >= columns) currentFrame = 0;
                timer = 0;
            }

            // Determine row for animation
            if (direction.X > 0) row = 2;       // Right
            else if (direction.X < 0) row = 1;  // Left
            else if (direction.Y > 0) row = 0;  // Down
            else if (direction.Y < 0) row = 3;  // Up
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Rectangle sourceRect = new Rectangle(currentFrame * frameWidth, row * frameHeight, frameWidth, frameHeight);
            spriteBatch.Draw(texture, Position, sourceRect, Color.White);
        }

        public override void Stun()
        {
            stunned = true;
            stunTimer = 0;
        }
    }
}