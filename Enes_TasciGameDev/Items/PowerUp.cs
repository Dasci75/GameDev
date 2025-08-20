using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace Enes_TasciGameDev.Items
{
    public class PowerUp
    {
        public Vector2 Position { get; private set; }
        public Texture2D texture { get; private set; }
        public bool IsCollected { get; private set; } = false;
        public PowerUpType Type { get; private set; }

        private int width, height;
        private float scale;

        public PowerUp(Vector2 position, PowerUpType type, Texture2D texture, float scale = 0.1f)
        {
            Position = position;
            Type = type;
            this.texture = texture;
            this.scale = scale;

            width = (int)(texture.Width * scale);
            height = (int)(texture.Height * scale);
        }

        public void Apply(Player player)
        {
            if (IsCollected) return;

            if (Type == PowerUpType.HealthBoost)
                player.Health++;
            else if (Type == PowerUpType.SpeedBoost)
                player.ApplySpeedBoost(2f, 10f);

            IsCollected = true;
        }
        public void Update(Player player)
        {
            if (!IsCollected && GetBounds().Intersects(player.GetBounds()))
            {
                Apply(player); // Pas toepassen bij collision
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!IsCollected)
                spriteBatch.Draw(texture, Position, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }

        public Rectangle GetBounds()
        {
            return new Rectangle((int)Position.X, (int)Position.Y, width, height);
        }
    }
    public enum PowerUpType
    {
        HealthBoost,
        SpeedBoost
    }
}
