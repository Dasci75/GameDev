using Enes_TasciGameDev.Obs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

public class Player
{
    private Vector2 position;
    private Texture2D texture;
    private int rows, columns;
    private int currentFrame;
    private int row;
    private double timer;
    private double interval = 100;
    private int frameWidth, frameHeight;
    public float Speed { get; set; } = 2f;
    private float speedBoostTimer = 0f;
    private float originalSpeed;
    private Vector2 velocity;
    private float acceleration = 0.5f;
    private float friction = 0.9f;
    public float MaxSpeed { get; private set; } = 3f;
    public int Health { get; set; } = 5;
    public int Coins { get; set; } = 0;
    public bool isDead = false;
    private int deathRow = 4;
    private int deathFramesCount = 4;

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

    public void Update(GameTime gameTime, GraphicsDevice graphicsDevice, List<Obstacle> obstacles)
    {
        if (isDead)
        {
            timer += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (timer > interval)
            {
                currentFrame++;
                if (currentFrame >= deathFramesCount)
                    currentFrame = deathFramesCount - 1;
                timer = 0;
            }
            return;
        }

        Vector2 movement = Vector2.Zero;
        KeyboardState keyboardState = Keyboard.GetState();

        if (keyboardState.IsKeyDown(Keys.Z)) movement.Y -= 1;
        if (keyboardState.IsKeyDown(Keys.S)) movement.Y += 1;
        if (keyboardState.IsKeyDown(Keys.Q)) movement.X -= 1;
        if (keyboardState.IsKeyDown(Keys.D)) movement.X += 1;

        // Calculate proposed new position
        Vector2 newPosition = position + velocity;

        // Check collisions with obstacles
        Rectangle playerBounds = GetBounds();
        bool collisionDetected = false;

        if (obstacles != null)
        {
            foreach (var obstacle in obstacles)
            {
                Rectangle newPlayerBounds = new Rectangle(
                    (int)newPosition.X + 8,
                    (int)newPosition.Y + 8,
                    frameWidth - 16,
                    frameHeight - 16
                );

                if (newPlayerBounds.Intersects(obstacle.Bounds))
                {
                    collisionDetected = true;
                    newPosition = position; // Revert to previous position
                    velocity = Vector2.Zero; // Stop all movement
                    break;
                }
            }
        }

        // Update position if no collision
        if (!collisionDetected || obstacles == null)
        {
            position = newPosition;
        }

        // Apply acceleration
        if (movement != Vector2.Zero)
        {
            movement.Normalize();
            Vector2 targetVelocity = movement * MaxSpeed;
            velocity = Vector2.Lerp(velocity, targetVelocity, 0.04f);
        }
        else
        {
            velocity *= friction;
        }

        // Limit speed
        if (velocity.Length() > MaxSpeed)
        {
            velocity.Normalize();
            velocity *= MaxSpeed;
        }

        // Clamp position to screen bounds
        position.X = MathHelper.Clamp(position.X, 0, graphicsDevice.Viewport.Width - frameWidth);
        position.Y = MathHelper.Clamp(position.Y, 0, graphicsDevice.Viewport.Height - frameHeight);

        // Update animation
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
            currentFrame = 0; // Idle
        }

        // Update speed boost
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
        int padding = 8;
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
            if (movement.X < 0) row = 1; // Left
            else if (movement.X > 0) row = 2; // Right
        }
        else
        {
            if (movement.Y < 0) row = 3; // Up
            else if (movement.Y > 0) row = 0; // Down
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        Rectangle sourceRect = new Rectangle(currentFrame * frameWidth, row * frameHeight, frameWidth, frameHeight);
        spriteBatch.Draw(texture, position, sourceRect, Color.White);
    }
}