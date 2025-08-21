using Enes_TasciGameDev.Interfaces;
using Enes_TasciGameDev.Manager;
using Enes_TasciGameDev.Prop;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

public class Player
{
    private Vector2 position;
    private Texture2D texture;
    private int rows, columns;
    private int currentFrame;
    private double timer;
    private double interval = 100;
    private int frameWidth, frameHeight;

    // Voeg de MovementComponent toe
    private MovementComponent movementComponent;

    public int Health { get; set; } = 5;
    public int Coins { get; set; } = 0;
    public bool isDead = false;
    private int deathRow = 4;
    private int deathFramesCount = 4;

    private List<IPlayerObserver> observers = new List<IPlayerObserver>();

    public Player(Vector2 position, Texture2D texture, int rows, int columns)
    {
        this.position = position;
        this.texture = texture;
        this.rows = rows;
        this.columns = columns;
        frameWidth = texture.Width / columns;
        frameHeight = texture.Height / rows;

        // Initialiseer de MovementComponent
        movementComponent = new MovementComponent(frameWidth, frameHeight);
    }
    public void RegisterObserver(IPlayerObserver observer) => observers.Add(observer);
    public void UnregisterObserver(IPlayerObserver observer) => observers.Remove(observer);

    public void ApplySpeedBoost(float multiplier, float duration)
    {
        // Roep de methode op de component aan
        movementComponent.ApplySpeedBoost(multiplier, duration);
    }

    public void AddCoin()
    {
        Coins++;
        GameManager.Instance.AddCoins(1); // Update singleton
        foreach (var obs in observers) obs.OnCoinCollected(Coins);
    }

    public void TakeDamage()
    {
        if (Health > 0)
        {
            Health--;
            foreach (var obs in observers) obs.OnHealthChanged(Health);
            if (Health <= 0)
                Die();
        }
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

        // Roep de update methode van de MovementComponent aan
        position = movementComponent.Update(gameTime, position, graphicsDevice, obstacles);

        // Update animatie, gebaseerd op de snelheid van de component
        if (movementComponent.Velocity.LengthSquared() > 0.1f)
        {
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

    public void Draw(SpriteBatch spriteBatch)
    {
        int currentRow = isDead ? deathRow : movementComponent.Row;
        Rectangle sourceRect = new Rectangle(currentFrame * frameWidth, currentRow * frameHeight, frameWidth, frameHeight);
        spriteBatch.Draw(texture, position, sourceRect, Color.White);
    }
}