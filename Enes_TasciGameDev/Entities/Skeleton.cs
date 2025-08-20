using Enes_TasciGameDev.Obs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Enes_TasciGameDev.Entities
{
    public class Skeleton : Enemy
    {
        private Texture2D texture;
        private float speed;
        private bool stunned = false;
        private double stunTimer = 0;
        private double stunDuration = 1.0;

        // Animation
        private int rows = 4;
        private int columns = 3;
        private int currentFrame = 0;
        private int row = 0;
        private double timer = 0;
        private double interval = 200; // milliseconds
        private int frameWidth;
        private int frameHeight;

        // Patrol
        private List<Vector2> patrolPoints;
        private int currentTarget = 0;
        private double waitTimer = 0;
        private double waitDuration = 0.5;

        // Avoid overlapping
        private float separationDistance = 20f;

        public Skeleton(Texture2D texture, Vector2 position, float speed, List<Vector2> patrolPoints)
        {
            this.texture = texture;
            this.Position = position;
            this.speed = speed;
            this.patrolPoints = patrolPoints;

            frameWidth = texture.Width / columns;
            frameHeight = (texture.Height / rows) + 1;
        }

        public override Rectangle GetBounds()
        {
            return new Rectangle((int)Position.X, (int)Position.Y, frameWidth, frameHeight);
        }

        public override void Update(GameTime gameTime, Vector2 playerPosition, List<Enemy> enemies, List<Obstacle> obstacles, int screenWidth, int screenHeight)
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

            if (patrolPoints.Count == 0) return;

            Vector2 target = patrolPoints[currentTarget];
            Vector2 direction = target - Position;
            float distance = direction.Length();

            if (distance < 1f)
            {
                waitTimer += gameTime.ElapsedGameTime.TotalSeconds;
                if (waitTimer >= waitDuration)
                {
                    currentTarget = (currentTarget + 1) % patrolPoints.Count;
                    waitTimer = 0;
                }
            }
            else
            {
                direction.Normalize();
                Vector2 newPosition = Position + direction * speed;

                // Clamp position to stay within screen boundaries
                newPosition.X = MathHelper.Clamp(newPosition.X, 0, screenWidth - frameWidth);
                newPosition.Y = MathHelper.Clamp(newPosition.Y, 0, screenHeight - frameHeight);
                Position = newPosition;

                // Avoid other enemies
                foreach (var other in enemies)
                {
                    if (other == this) continue;
                    Vector2 diff = Position - other.Position;
                    float diffLength = diff.Length();
                    if (diffLength < separationDistance && diffLength > 0)
                    {
                        diff.Normalize();
                        Position += diff * (separationDistance - diffLength) * 0.5f;
                    }
                }

                // Re-clamp after separation to ensure staying on screen
                Position = new Vector2(
                    MathHelper.Clamp(Position.X, 0, screenWidth - frameWidth),
                    MathHelper.Clamp(Position.Y, 0, screenHeight - frameHeight)
                );
            }

            // Animation update
            timer += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (timer > interval)
            {
                currentFrame++;
                if (currentFrame >= columns) currentFrame = 0;
                timer = 0;
            }

            // Animation row based on movement direction
            if (direction.X > 0) row = 1;      // right
            else if (direction.X < 0) row = 3; // left
            else if (direction.Y > 0) row = 2; // down
            else if (direction.Y < 0) row = 0; // up
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Rectangle sourceRect = new Rectangle(currentFrame * frameWidth, row * frameHeight, frameWidth, frameHeight);
            spriteBatch.Draw(texture, Position, sourceRect, stunned ? Color.LightGray : Color.White);
        }

        public override void Stun()
        {
            stunned = true;
            stunTimer = 0;
        }
    }
}