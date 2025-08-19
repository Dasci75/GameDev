using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class Coin
{
    public Vector2 Position { get; private set; }
    private Texture2D texture;
    private int width, height;
    private float scale; // schaalfactor

    public Coin(Texture2D texture, Vector2 position, float scale = 0.1f)
    {
        this.texture = texture;
        Position = position;
        this.scale = scale;

        width = (int)(texture.Width * scale);
        height = (int)(texture.Height * scale);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(texture, Position, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
    }

    public Rectangle GetBounds()
    {
        return new Rectangle((int)Position.X, (int)Position.Y, width, height);
    }
}
