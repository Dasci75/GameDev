using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public class Player
{
    public Vector2 Position;
    private Texture2D texture;
    private float speed = 150f;

    public Player(Texture2D tex, Vector2 startPos)
    {
        texture = tex;
        Position = startPos;
    }

    public void Update(GameTime gameTime)
    {
        KeyboardState ks = Keyboard.GetState();
        float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (ks.IsKeyDown(Keys.Up)) Position.Y -= speed * delta;
        if (ks.IsKeyDown(Keys.Down)) Position.Y += speed * delta;
        if (ks.IsKeyDown(Keys.Left)) Position.X -= speed * delta;
        if (ks.IsKeyDown(Keys.Right)) Position.X += speed * delta;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(texture, Position, Color.White);
    }

}
