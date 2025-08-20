using Enes_TasciGameDev.Obs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

public class Dog
{
    private Vector2 position;
    private Texture2D texture;
    private int rows, columns;
    private int currentFrame;
    private double timer;
    private double interval = 150; // iets trager zodat animatie duidelijker is
    private int frameWidth, frameHeight;

    private float scale;

    private int currentRow; // 0 = rechts, 1 = links
    private Vector2 velocity;
    private float speed = 2.5f;

    public Dog(Vector2 position, Texture2D texture, int rows, int columns, float scale = 0.2f)
    {
        this.position = position;
        this.texture = texture;
        this.rows = rows;
        this.columns = columns;
        this.scale = scale;

        frameWidth = (texture.Width / columns) - 12;
        frameHeight = texture.Height / rows;
    }

    public void Update(GameTime gameTime, GraphicsDevice graphicsDevice, List<Obstacle> obstacles)
    {
        velocity = Vector2.Zero;
        KeyboardState keyboardState = Keyboard.GetState();

        // pijltjestoetsen besturing
        if (keyboardState.IsKeyDown(Keys.Left))
        {
            velocity.X -= 1;
            currentRow = 1; // links animatie
        }
        if (keyboardState.IsKeyDown(Keys.Right))
        {
            velocity.X += 1;
            currentRow = 0; // rechts animatie
        }
        if (keyboardState.IsKeyDown(Keys.Up))
        {
            velocity.Y -= 1;
            currentRow = 1; // ook links animatie
        }
        if (keyboardState.IsKeyDown(Keys.Down))
        {
            velocity.Y += 1;
            currentRow = 0; // ook rechts animatie
        }

        if (velocity != Vector2.Zero)
        {
            velocity.Normalize();
            position += velocity * speed;

            // animatie frame update
            timer += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (timer > interval)
            {
                currentFrame = (currentFrame + 1) % columns;
                timer = 0;
            }
        }
        else
        {
            currentFrame = 0; // idle frame
        }

        // Clamp binnen scherm
        position.X = MathHelper.Clamp(position.X, 0, graphicsDevice.Viewport.Width - frameWidth);
        position.Y = MathHelper.Clamp(position.Y, 0, graphicsDevice.Viewport.Height - frameHeight);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        Rectangle sourceRect = new Rectangle(
            currentFrame * frameWidth,
            currentRow * frameHeight,
            frameWidth,
            frameHeight
        );

        spriteBatch.Draw(
            texture,
            position,
            sourceRect,
            Color.White,
            0f,
            Vector2.Zero,
            scale,
            SpriteEffects.None,
            0f
        );
    }

    public Rectangle GetBounds()
    {
        int padding = 8;
        return new Rectangle(
            (int)position.X + padding,
            (int)position.Y + padding,
            (int)((frameWidth - 2 * padding) * scale),
            (int)((frameHeight - 2 * padding) * scale)
        );
    }
}
