using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enes_TasciGameDev.Entities
{
    public class Goblin
    {
        private Texture2D texture;
        private Vector2 position;
        private float speed;
        private bool stunned = false;
        private double stunTimer = 0;
        private double stunDuration = 2.0; // 2 seconden


        // Animatie
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
            this.position = position;
            this.speed = speed;

            frameWidth = texture.Width / columns;   // 144/3 = 48
            frameHeight = texture.Height / rows;    // 192/4 = 48
        }

        public void Update(GameTime gameTime, Vector2 playerPosition, List<Goblin> allGoblins)
        {
            // 1. Handle stunned state
            if (stunned)
            {
                stunTimer += gameTime.ElapsedGameTime.TotalSeconds;
                if (stunTimer >= stunDuration)
                {
                    stunned = false;
                    stunTimer = 0;
                }
                return; // stop met bewegen zolang stunned
            }

            // 2. Move towards player
            Vector2 direction = playerPosition - position;
            if (direction != Vector2.Zero)
            {
                direction.Normalize();
                position += direction * speed;
            }

            // 3. Separation to avoid stacking
            foreach (var other in allGoblins)
            {
                if (other == this) continue;

                Vector2 diff = position - other.position;
                float distance = diff.Length();

                if (distance < frameWidth) // if too close
                {
                    diff.Normalize();
                    position += diff * (frameWidth - distance) * 0.5f; // push away
                }
            }

            // 4. Update animation
            timer += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (timer > interval)
            {
                currentFrame++;
                if (currentFrame >= columns) currentFrame = 0;
                timer = 0;
            }

            // 5. Determine row for animation
            if (direction.X > 0) row = 2;       // right
            else if (direction.X < 0) row = 1;  // left
            else if (direction.Y > 0) row = 0;  // down
            else if (direction.Y < 0) row = 3;  // up
        }

        // Nieuwe methode om de goblin te stunnen
        public void Stun()
        {
            stunned = true;
            stunTimer = 0;
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            Rectangle sourceRect = new Rectangle(currentFrame * frameWidth, row * frameHeight, frameWidth, frameHeight);
            spriteBatch.Draw(texture, position, sourceRect, Color.White);
        }

        public Rectangle GetBounds()
        {
            return new Rectangle((int)position.X, (int)position.Y, frameWidth, frameHeight);
        }
    }
}
