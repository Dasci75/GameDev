using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Data;

public class Player
{
    private Vector2 position;
    private Texture2D texture;
    private int rows, columns;
    private int currentFrame;
    private int row; // current animation row
    private double timer;
    private double interval = 100; // milliseconds per frame
    private int frameWidth, frameHeight;

    public int Health { get; private set; } = 5;
    public int Coins { get; set; } = 0;

    public bool isDead = false;
    private int deathRow = 4; // row in sprite sheet for death animation
    private int deathFramesCount = 4; // number of frames in death animation

    public Player(Vector2 position, Texture2D texture, int rows, int columns)
    {
        this.position = position;
        this.texture = texture;
        this.rows = rows;
        this.columns = columns;

        frameWidth = texture.Width / columns;
        frameHeight = texture.Height / rows;
        row = 0;
    }

    public void TakeDamage()
    {
        if (Health > 0)
        {
            Health--;
            if (Health <= 0)
                Die();
        }
    }

    private void Die()
    {
        isDead = true;
        currentFrame = 0;
        row = deathRow;
    }

    public void Update(GameTime gameTime, GraphicsDevice graphicsDevice)
    {
        if (isDead)
        {
            // update death animation
            timer += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (timer > interval)
            {
                currentFrame++;
                if (currentFrame >= deathFramesCount)
                    currentFrame = deathFramesCount - 1; // stop at last frame
                timer = 0;
            }
            return; // skip movement updates
        }

        Vector2 movement = Vector2.Zero;
        KeyboardState keyboardState = Keyboard.GetState();

        if (keyboardState.IsKeyDown(Keys.Z)) movement.Y -= 1;
        if (keyboardState.IsKeyDown(Keys.S)) movement.Y += 1;
        if (keyboardState.IsKeyDown(Keys.Q)) movement.X -= 1;
        if (keyboardState.IsKeyDown(Keys.D)) movement.X += 1;

        if (movement != Vector2.Zero)
        {
            position += movement * 2f;

            position.X = MathHelper.Clamp(position.X, 0, graphicsDevice.Viewport.Width - frameWidth);
            position.Y = MathHelper.Clamp(position.Y, 0, graphicsDevice.Viewport.Height - frameHeight);

            SetAnimationRow(movement);

            timer += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (timer > interval)
            {
                currentFrame++;
                if (currentFrame >= columns) currentFrame = 0;
                timer = 0;
            }
        }
        else
        {
            currentFrame = 0; // idle
        }
    }
    public Vector2 Position
    {
        get { return position; }
    }
    public Rectangle GetBounds()
    {
        int padding = 8; // kleiner dan het frame
        return new Rectangle(
            (int)position.X + padding,
            (int)position.Y + padding,
            frameWidth - 2 * padding,
            frameHeight - 2 * padding
        );
    }

    private void SetAnimationRow(Vector2 movement)
    {
        if (movement.Y < 0) row = 3; // up
        else if (movement.Y > 0) row = 0; // down
        else if (movement.X < 0) row = 1; // left
        else if (movement.X > 0) row = 2; // right
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        Rectangle sourceRect = new Rectangle(currentFrame * frameWidth, row * frameHeight, frameWidth, frameHeight);
        spriteBatch.Draw(texture, position, sourceRect, Color.White);
    }
}
