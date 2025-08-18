using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
public class Player
{
    public Vector2 Position;
    private Texture2D texture;

    // Sprite sheet info
    private int columns;
    private int rows;
    private int currentFrame = 0;
    private int frameWidth;
    private int frameHeight;
    private double animationTimer = 0;
    private double timePerFrame = 0.1; // seconds per frame

    private float speed = 150f;

    public Player(Texture2D tex, Vector2 startPos, int columns, int rows)
    {
        texture = tex;
        Position = startPos;
        this.columns = columns;
        this.rows = rows;

        frameWidth = texture.Width / columns;
        frameHeight = texture.Height / rows;
    }

    public void Update(GameTime gameTime)
    {
        // Movement
        var ks = Keyboard.GetState();
        float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (ks.IsKeyDown(Keys.Up)) Position.Y -= speed * delta;
        if (ks.IsKeyDown(Keys.Down)) Position.Y += speed * delta;
        if (ks.IsKeyDown(Keys.Left)) Position.X -= speed * delta;
        if (ks.IsKeyDown(Keys.Right)) Position.X += speed * delta;

        // Animation
        animationTimer += gameTime.ElapsedGameTime.TotalSeconds;
        if (animationTimer >= timePerFrame)
        {
            currentFrame++;
            if (currentFrame >= columns * rows)
                currentFrame = 0;
            animationTimer = 0;
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        int col = currentFrame % columns;
        int row = currentFrame / columns;

        Rectangle sourceRectangle = new Rectangle(
            col * frameWidth,
            row * frameHeight,
            frameWidth,
            frameHeight
        );

        spriteBatch.Begin();
        spriteBatch.Draw(texture, Position, sourceRectangle, Color.White);
        spriteBatch.End();
    }
}

