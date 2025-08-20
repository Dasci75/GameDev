using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
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

    public float Speed { get; set; } = 2f;
    private float speedBoostTimer = 0f;
    private float originalSpeed;

    private Vector2 velocity;         // huidige snelheid
    private float acceleration = 0.5f; // hoe snel speler versnelt
    private float friction = 0.9f;     // weerstand (hoe snel hij vertraagt)
    public float MaxSpeed { get; private set; } = 3f; // maximale snelheid



    public int Health { get; set; } = 5;
    public int Coins { get; set; } = 0;

    public bool isDead = false;
    private int deathRow = 4; // row in sprite sheet for d)eath animation
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
            {
                isDead = true;
            }
        }
    }
    public void ApplySpeedBoost(float multiplier, float duration)
    {
        originalSpeed = MaxSpeed;
        MaxSpeed *= multiplier;
        speedBoostTimer = duration;
    }

    public void AddCoin()
    {
        Coins++;
    }

    public void RemoveCoin()
    {
        if (Coins > 0)
            Coins--;
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

        // Versnelling toepassen
        if (movement != Vector2.Zero)
        {
            movement.Normalize(); // zodat diagonaal niet sneller is
            Vector2 targetVelocity = movement * MaxSpeed;

            velocity = Vector2.Lerp(velocity, targetVelocity, 0.04f);

        }
        else
        {
            // Geen input → wrijving toepassen
            velocity *= friction;
        }

        // Snelheid limiteren
        if (velocity.Length() > MaxSpeed)
        {
            velocity.Normalize();
            velocity *= MaxSpeed;
        }

        // Positie updaten
        position += velocity;

        // Grenzen scherm
        position.X = MathHelper.Clamp(position.X, 0, graphicsDevice.Viewport.Width - frameWidth);
        position.Y = MathHelper.Clamp(position.Y, 0, graphicsDevice.Viewport.Height - frameHeight);

        // Animatie alleen bij beweging
        if (velocity.LengthSquared() > 0.1f)
        {
            SetAnimationRow(velocity);

            timer += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (timer > interval)
            {
                currentFrame = (currentFrame + 1) % columns;
                timer = 0;
            }
        }
        else
        {
            currentFrame = 0; // idle
        }


        if (speedBoostTimer > 0)
        {
            speedBoostTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (speedBoostTimer <= 0)
                MaxSpeed = originalSpeed;
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
        if (Math.Abs(movement.X) > Math.Abs(movement.Y))
        {
            if (movement.X < 0) row = 1; // left
            else if (movement.X > 0) row = 2; // right
        }
        else
        {
            if (movement.Y < 0) row = 3; // up
            else if (movement.Y > 0) row = 0; // down
        }
    }


    public void Draw(SpriteBatch spriteBatch)
    {
        Rectangle sourceRect = new Rectangle(currentFrame * frameWidth, row * frameHeight, frameWidth, frameHeight);
        spriteBatch.Draw(texture, position, sourceRect, Color.White);
    }
}
