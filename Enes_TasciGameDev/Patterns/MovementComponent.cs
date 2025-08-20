using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using Enes_TasciGameDev.Obs; // Zorg ervoor dat dit de juiste namespace is voor je Obstacle klasse.

public class MovementComponent
{
    public Vector2 Velocity { get; private set; }
    public float MaxSpeed { get; set; } = 3f;
    private float speedBoostTimer = 0f;
    private float originalMaxSpeed;
    private float friction = 0.9f;
    private int frameWidth, frameHeight;
    public int Row { get; private set; }
    public enum Direction { Left, Right }
    public Direction LastDirection { get; private set; } = Direction.Right;


    private bool useArrowKeys;

    public MovementComponent(int frameWidth, int frameHeight, bool useArrowKeys = false)
    {
        this.frameWidth = frameWidth;
        this.frameHeight = frameHeight;
        this.Row = 0;
        this.useArrowKeys = useArrowKeys;
    }

    public Vector2 Update(GameTime gameTime, Vector2 currentPosition, GraphicsDevice graphicsDevice, List<Obstacle> obstacles)
    {
        Vector2 movement = Vector2.Zero;
        KeyboardState keyboardState = Keyboard.GetState();

        if (useArrowKeys)
        {
            if (keyboardState.IsKeyDown(Keys.Up)) movement.Y -= 1;
            if (keyboardState.IsKeyDown(Keys.Down)) movement.Y += 1;
            if (keyboardState.IsKeyDown(Keys.Left)) movement.X -= 1;
            if (keyboardState.IsKeyDown(Keys.Right)) movement.X += 1;
        }
        else
        {
            if (keyboardState.IsKeyDown(Keys.Z)) movement.Y -= 1;
            if (keyboardState.IsKeyDown(Keys.S)) movement.Y += 1;
            if (keyboardState.IsKeyDown(Keys.Q)) movement.X -= 1;
            if (keyboardState.IsKeyDown(Keys.D)) movement.X += 1;
        }

        // normale movement logica
        if (movement != Vector2.Zero)
        {
            movement.Normalize();
            Velocity = movement * MaxSpeed;
        }
        else
        {
            Velocity *= friction;
        }
        if (movement.X < 0) LastDirection = Direction.Left;
        else if (movement.X > 0) LastDirection = Direction.Right;


        Vector2 newPosition = currentPosition + Velocity;

        // Collision check
        if (obstacles != null)
        {
            Rectangle newBounds = new Rectangle(
                (int)newPosition.X,
                (int)newPosition.Y,
                frameWidth,
                frameHeight
            );
            foreach (var obstacle in obstacles)
            {
                if (newBounds.Intersects(obstacle.Bounds))
                {
                    newPosition = currentPosition;
                    Velocity = Vector2.Zero;
                    break;
                }
            }
        }

        // Clamp binnen scherm
        newPosition.X = MathHelper.Clamp(newPosition.X, 0, graphicsDevice.Viewport.Width - frameWidth);
        newPosition.Y = MathHelper.Clamp(newPosition.Y, 0, graphicsDevice.Viewport.Height - frameHeight);

        // Update animation row based on movement
        if (Velocity.LengthSquared() > 0.1f)
        {
            SetAnimationRow(Velocity);
        }
        else
        {
            Row = 0; // Idle (aannemende dat rij 0 de down-idle is)
        }

        // Update speed boost
        if (speedBoostTimer > 0)
        {
            speedBoostTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (speedBoostTimer <= 0)
            {
                MaxSpeed = originalMaxSpeed;
            }
        }

        return newPosition;
    }

    private void SetAnimationRow(Vector2 movement)
    {
        if (Math.Abs(movement.X) > Math.Abs(movement.Y))
        {
            if (movement.X < 0) Row = 1; // Left
            else if (movement.X > 0) Row = 2; // Right
        }
        else
        {
            if (movement.Y < 0) Row = 3; // Up
            else if (movement.Y > 0) Row = 0; // Down
        }
    }

    public void ApplySpeedBoost(float multiplier, float duration)
    {
        originalMaxSpeed = MaxSpeed;
        MaxSpeed *= multiplier;
        speedBoostTimer = duration;
    }
}