using Enes_TasciGameDev;
using Enes_TasciGameDev.Entities;
using Enes_TasciGameDev.Items;
using Enes_TasciGameDev.Prop;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

public class Level2 : IGameState
{
    private Game1 game;
    private Player player;
    private Texture2D playerTexture;
    private Texture2D background;
    private List<Coin> coins;
    private Texture2D coinTexture;
    private Random random = new Random();
    private Texture2D scoreBackground;
    private SpriteFont scoreFont;
    private Coin currentCoin;
    private Texture2D finishTexture;
    private Finish finish;
    private bool levelPassed = false;
    private Texture2D overlay;
    private List<Skeleton> skeletons;
    private Texture2D skeletonTexture;
    private Texture2D thiefTexture;
    private List<Thief> thieves;
    private bool gameOver = false;
    private double damageCooldown = 1.0;
    private double damageTimer = 0;
    private List<PowerUp> powerUps;
    private Texture2D healthBoostTexture;
    private Texture2D speedBoostTexture;
    private HashSet<int> triggeredPowerups = new HashSet<int>();

    public Level2(Game1 game)
    {
        this.game = game;
    }

    public void LoadContent()
    {
        // Textures
        background = game.Content.Load<Texture2D>("bgLevel2");
        playerTexture = game.Content.Load<Texture2D>("player");
        coinTexture = game.Content.Load<Texture2D>("coin");
        finishTexture = game.Content.Load<Texture2D>("finish");
        skeletonTexture = game.Content.Load<Texture2D>("skeleton");
        thiefTexture = game.Content.Load<Texture2D>("thief");
        healthBoostTexture = game.Content.Load<Texture2D>("healthPowerUp");
        speedBoostTexture = game.Content.Load<Texture2D>("speedPowerUp");

        // Player
        player = new Player(new Vector2(100, 100), playerTexture, 4, 4);

        // Score UI
        scoreFont = game.Content.Load<SpriteFont>("scoreFont");
        scoreBackground = new Texture2D(game.GraphicsDevice, 1, 1);
        scoreBackground.SetData(new[] { Color.LightGray });
        overlay = new Texture2D(game.GraphicsDevice, 1, 1);
        overlay.SetData(new[] { Color.Black });

        // Coins
        coins = new List<Coin>();
        for (int i = 0; i < 7; i++)
        {
            Vector2 pos = new Vector2(
                random.Next(0, game.GraphicsDevice.Viewport.Width - coinTexture.Width),
                random.Next(0, game.GraphicsDevice.Viewport.Height - coinTexture.Height)
            );
            coins.Add(new Coin(coinTexture, pos));
        }
        SpawnNextCoin();

        // Thieves
        thieves = new List<Thief>
        {
            new Thief(thiefTexture, new Vector2(300, 150), 2f),
            new Thief(thiefTexture, new Vector2(500, 400), 2f)
        };

        // Skeletons
        // Skeletons
        var patrolRoute1 = new List<Vector2> { new Vector2(200, 200), new Vector2(400, 200), new Vector2(400, 400), new Vector2(200, 400) };
        var patrolRoute2 = new List<Vector2> { new Vector2(300, 100), new Vector2(500, 100), new Vector2(500, 300), new Vector2(300, 300) };
        var patrolRoute3 = new List<Vector2> { new Vector2(400, 150), new Vector2(600, 150), new Vector2(600, 350), new Vector2(400, 350) };
        var patrolRoute4 = new List<Vector2> { new Vector2(150, 300), new Vector2(350, 300), new Vector2(350, 500), new Vector2(150, 500) };
        var patrolRoute5 = new List<Vector2> { new Vector2(500, 250), new Vector2(700, 250), new Vector2(700, 450), new Vector2(500, 450) };

        skeletons = new List<Skeleton>
        {
            new Skeleton(skeletonTexture, new Vector2(200, 200), 1.3f, patrolRoute1),
            new Skeleton(skeletonTexture, new Vector2(300, 100), 1.3f, patrolRoute2),
            new Skeleton(skeletonTexture, new Vector2(400, 150), 1.3f, patrolRoute3),
            new Skeleton(skeletonTexture, new Vector2(150, 300), 1.3f, patrolRoute4),
            new Skeleton(skeletonTexture, new Vector2(500, 250), 1.3f, patrolRoute5)
        };

        // PowerUps list
        powerUps = new List<PowerUp>();

        // Finish not spawned yet
        finish = null;
    }

    private void SpawnNextCoin()
    {
        if (player.Coins < 7)
        {
            float scale = 0.1f;
            int coinWidth = (int)(coinTexture.Width * scale);
            int coinHeight = (int)(coinTexture.Height * scale);

            int x = random.Next(0, game.GraphicsDevice.Viewport.Width - coinWidth);
            int y = random.Next(0, game.GraphicsDevice.Viewport.Height - coinHeight);

            currentCoin = new Coin(coinTexture, new Vector2(x, y), scale);
        }
        else
        {
            currentCoin = null;
        }
    }

    private void CheckForFinishSpawn()
    {
        if (player.Coins >= 7 && finish == null)
        {
            float scale = 0.5f;
            Vector2 pos = new Vector2(
                game.GraphicsDevice.Viewport.Width - finishTexture.Width * scale - 20,
                (game.GraphicsDevice.Viewport.Height - finishTexture.Height * scale) / 2
            );
            finish = new Finish(finishTexture, pos, scale);
        }
    }

    private void SpawnPowerUp(PowerUpType type, Texture2D texture, float scale)
    {
        Vector2 pos = new Vector2(
            random.Next(0, game.GraphicsDevice.Viewport.Width - 50),
            random.Next(0, game.GraphicsDevice.Viewport.Height - 50)
        );

        powerUps.Add(new PowerUp(pos, type, texture, scale));
    }

    public void Update(GameTime gameTime)
    {
        if (gameOver || levelPassed)
        {
            var keyboard = Keyboard.GetState();
            if (keyboard.IsKeyDown(Keys.Enter) || keyboard.IsKeyDown(Keys.Tab))
                game.ChangeState(new StartScreen(game));

            return;
        }

        player.Update(gameTime, game.GraphicsDevice);
        Rectangle playerBounds = player.GetBounds();

        // Coin pickup
        if (currentCoin != null && playerBounds.Intersects(currentCoin.GetBounds()))
        {
            player.AddCoin();
            SpawnNextCoin();

            if (player.Coins == 1 && !triggeredPowerups.Contains(1))
            {
                SpawnPowerUp(PowerUpType.HealthBoost, healthBoostTexture, 0.03f);
                triggeredPowerups.Add(1);
            }

            if (player.Coins == 3 && !triggeredPowerups.Contains(3))
            {
                SpawnPowerUp(PowerUpType.SpeedBoost, speedBoostTexture, 0.08f);
                triggeredPowerups.Add(3);
            }

            CheckForFinishSpawn();
        }

        // Thieves
        foreach (var thief in thieves)
        {
            thief.Update(gameTime, player, game.GraphicsDevice.Viewport.Width, game.GraphicsDevice.Viewport.Height);

            if (thief.GetBounds().Intersects(playerBounds))
            {
                player.RemoveCoin();

                if (player.Coins < 1) triggeredPowerups.Remove(1);
                if (player.Coins < 3) triggeredPowerups.Remove(3);
            }
        }

        // Skeletons
        foreach (var skeleton in skeletons)
        {
            skeleton.Update(gameTime, skeletons);

            if (skeleton.GetBounds().Intersects(playerBounds))
            {
                damageTimer += gameTime.ElapsedGameTime.TotalSeconds;
                if (damageTimer >= damageCooldown)
                {
                    player.TakeDamage();
                    damageTimer = 0;
                    skeleton.Stun();

                    if (player.isDead)
                        gameOver = true;
                }
            }
        }

        // PowerUps
        foreach (var powerUp in powerUps)
            powerUp.Update(player);

        // Finish line
        if (finish != null && playerBounds.Intersects(finish.GetBounds()))
            levelPassed = true;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Begin();

        // Background
        spriteBatch.Draw(background, new Rectangle(0, 0,
            game.GraphicsDevice.Viewport.Width,
            game.GraphicsDevice.Viewport.Height), Color.White);

        // Score
        string scoreText = $"Coins: {player.Coins}";
        Vector2 textSize = scoreFont.MeasureString(scoreText);
        Rectangle scoreRect = new Rectangle(
            game.GraphicsDevice.Viewport.Width - (int)textSize.X - 20,
            10,
            (int)textSize.X + 20,
            (int)textSize.Y + 10
        );
        spriteBatch.Draw(scoreBackground, scoreRect, Color.LightGray);
        spriteBatch.DrawString(scoreFont, scoreText, new Vector2(scoreRect.X + 10, scoreRect.Y + 5), Color.Black);

        // Health
        spriteBatch.DrawString(scoreFont, $"Health: {player.Health}", new Vector2(10, 10), Color.Red);

        // Coins
        currentCoin?.Draw(spriteBatch);

        // Player
        player.Draw(spriteBatch);

        // Enemies
        foreach (var skeleton in skeletons) skeleton.Draw(spriteBatch);
        foreach (var thief in thieves) thief.Draw(spriteBatch);

        // Finish
        finish?.Draw(spriteBatch);

        // PowerUps
        foreach (var powerUp in powerUps) powerUp.Draw(spriteBatch);

        // Overlay messages
        if (levelPassed || gameOver)
        {
            spriteBatch.Draw(overlay, new Rectangle(0, 0,
                game.GraphicsDevice.Viewport.Width,
                game.GraphicsDevice.Viewport.Height), Color.Black * 0.6f);

            string message = levelPassed ? "Level 2 Complete!\nPress Enter or Tab for Menu"
                                         : "Game Over!\nPress Enter to Retry\nPress Tab for Menu";

            Vector2 size = scoreFont.MeasureString(message);
            Vector2 position = new Vector2(
                (game.GraphicsDevice.Viewport.Width - size.X) / 2,
                (game.GraphicsDevice.Viewport.Height - size.Y) / 2
            );

            spriteBatch.DrawString(scoreFont, message, position, levelPassed ? Color.Yellow : Color.Red);
        }

        spriteBatch.End();
    }
}
