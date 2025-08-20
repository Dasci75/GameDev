using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Enes_TasciGameDev.Obs
{
    public class Obstacle
    {
        public Vector2 Position;
        public Texture2D Texture;
        public float Scale;
        public Rectangle Bounds => new Rectangle(
            (int)Position.X,
            (int)Position.Y,
            (int)(Texture.Width * Scale),
            (int)(Texture.Height * Scale)
        );

        public Obstacle(Vector2 position, Texture2D texture, float scale = 1f)
        {
            Position = position;
            Texture = texture;
            Scale = scale;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, null, Color.White, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0f);
        }
    }
}