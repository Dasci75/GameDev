using Enes_TasciGameDev;
using Enes_TasciGameDev.Entities;
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
    private int score = 0;
    private Texture2D scoreBackground;
    private SpriteFont scoreFont;
    private Coin currentCoin;
    private Texture2D finishTexture;
    private Finish finish;
    private bool levelPassed = false;
    private Texture2D overlay;
    List<Skeleton> skeletons;
    Texture2D skeletonTexture;
    Texture2D thiefTexture;
    List<Thief> thieves;
    private bool gameOver = false;
    private double damageCooldown = 1.0;
    private double damageTimer = 0;

    public Level2(Game1 game)
    {
        this.game = game;
    }

    public void LoadContent()
    {
        // Different background for Level2
        background = game.Content.Load<Texture2D>("bgLevel2");

        // Player
        playerTexture = game.Content.Load<Texture2D>("player");
        player = new Player(new Vector2(100, 100), playerTexture, 4, 4);

        // Score + overlay
        scoreFont = game.Content.Load<SpriteFont>("scoreFont");
        scoreBackground = new Texture2D(game.GraphicsDevice, 1, 1);
        scoreBackground.SetData(new[] { Color.LightGray });
        overlay = new Texture2D(game.GraphicsDevice, 1, 1);
        overlay.SetData(new[] { Color.Black });

        // Coins
        coinTexture = game.Content.Load<Texture2D>("coin");
        coins = new List<Coin>();
        for (int i = 0; i < 7; i++) // maybe require more coins
        {
            Vector2 pos = new Vector2(
                random.Next(0, game.GraphicsDevice.Viewport.Width - coinTexture.Width),
                random.Next(0, game.GraphicsDevice.Viewport.Height - coinTexture.Height)
            );
            coins.Add(new Coin(coinTexture, pos));
        }
        SpawnNextCoin();
        //thieves
        thiefTexture = game.Content.Load<Texture2D>("thief");

        thieves = new List<Thief>
        {
            new Thief(thiefTexture, new Vector2(300, 150), 2f),
            new Thief(thiefTexture, new Vector2(500, 400), 2f)
        };

        // skeletons
        skeletonTexture = game.Content.Load<Texture2D>("skeleton");

        skeletons = new List<Skeleton>
        {
            new Skeleton(skeletonTexture, new Vector2(200, 200), 1.3f),
            new Skeleton(skeletonTexture, new Vector2(200, 200), 1.3f),
            new Skeleton(skeletonTexture, new Vector2(200, 200), 1.3f)

        };

        // Finish line
        finishTexture = game.Content.Load<Texture2D>("finish");
        finish = null;
    }

    private void SpawnNextCoin()
    {
        if (score < 7)
        {
            float scale = 0.1f;
            int coinWidth = (int)(coinTexture.Width * scale);
            int coinHeight = (int)(coinTexture.Height * scale);

            int screenWidth = game.GraphicsDevice.Viewport.Width;
            int screenHeight = game.GraphicsDevice.Viewport.Height;

            int x = random.Next(0, screenWidth - coinWidth);
            int y = random.Next(0, screenHeight - coinHeight);

            currentCoin = new Coin(coinTexture, new Vector2(x, y), scale);
        }
        else
        {
            currentCoin = null;
        }
    }

    private void CheckForFinishSpawn()
    {
        if (score >= 7 && finish == null)
        {
            float scale = 0.5f;
            int finishWidth = (int)(finishTexture.Width * scale);
            int finishHeight = (int)(finishTexture.Height * scale);

            Vector2 pos = new Vector2(
                game.GraphicsDevice.Viewport.Width - finishWidth - 20,
                (game.GraphicsDevice.Viewport.Height - finishHeight) / 2
            );

            finish = new Finish(finishTexture, pos, scale);
        }
    }

    public void Update(GameTime gameTime)
    {
        if (gameOver || levelPassed)
        {
            var keyboard = Keyboard.GetState();

            if (keyboard.IsKeyDown(Keys.Enter))
            {
                if (levelPassed)
                    game.ChangeState(new StartScreen(game)); // After Level2, go back to menu (or make Level3 later)
                else
                    game.ChangeState(new StartScreen(game));
            }

            if (keyboard.IsKeyDown(Keys.Tab))
            {
                game.ChangeState(new StartScreen(game));
            }

            return;
        }

        player.Update(gameTime, game.GraphicsDevice);

        Rectangle playerBounds = player.GetBounds();

        if (currentCoin != null && playerBounds.Intersects(currentCoin.GetBounds()))
        {
            score++;
            SpawnNextCoin();
        }

        foreach (var skeleton in skeletons)
        {
            skeleton.Update(gameTime, player.Position, skeletons);

            if (skeleton.GetBounds().Intersects(player.GetBounds()))
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

        foreach (var thief in thieves)
        {
            thief.Update(gameTime, player, game.GraphicsDevice.Viewport.Width, game.GraphicsDevice.Viewport.Height);
        }

        CheckForFinishSpawn();

        if (finish != null && playerBounds.Intersects(finish.GetBounds()))
            levelPassed = true;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Begin();

        spriteBatch.Draw(background, new Rectangle(0, 0,
            game.GraphicsDevice.Viewport.Width,
            game.GraphicsDevice.Viewport.Height), Color.White);

        string scoreText = $"Coins: {score}";
        Vector2 textSize = scoreFont.MeasureString(scoreText);

        Rectangle backgroundRect = new Rectangle(
            game.GraphicsDevice.Viewport.Width - (int)textSize.X - 20,
            10,
            (int)textSize.X + 20,
            (int)textSize.Y + 10
        );

        spriteBatch.Draw(scoreBackground, backgroundRect, Color.LightGray);
        spriteBatch.DrawString(scoreFont, scoreText, new Vector2(backgroundRect.X + 10, backgroundRect.Y + 5), Color.Black);

        if (currentCoin != null)
            currentCoin.Draw(spriteBatch);

        string healthText = $"Health: {player.Health}";
        spriteBatch.DrawString(scoreFont, healthText, new Vector2(10, 10), Color.Red);

        player.Draw(spriteBatch);

        foreach (var skeleton in skeletons)
            skeleton.Draw(spriteBatch);

        foreach (var thief in thieves)
        {
            thief.Draw(spriteBatch);
        }

        if (finish != null)
            finish.Draw(spriteBatch);

        if (levelPassed)
        {
            spriteBatch.Draw(overlay, new Rectangle(0, 0,
                game.GraphicsDevice.Viewport.Width,
                game.GraphicsDevice.Viewport.Height), Color.Black * 0.6f);

            string message = "Level 2 Complete!\nPress Enter to go to Menu\nPress Tab for Menu";
            Vector2 size = scoreFont.MeasureString(message);
            Vector2 position = new Vector2(
                (game.GraphicsDevice.Viewport.Width - size.X) / 2,
                (game.GraphicsDevice.Viewport.Height - size.Y) / 2
            );

            spriteBatch.DrawString(scoreFont, message, position, Color.Yellow);
        }

        if (gameOver)
        {
            spriteBatch.Draw(overlay, new Rectangle(0, 0,
                game.GraphicsDevice.Viewport.Width,
                game.GraphicsDevice.Viewport.Height), Color.Black * 0.6f);

            string message = "Game Over!\nPress Enter to Retry\nPress Tab for Menu";
            Vector2 size = scoreFont.MeasureString(message);
            Vector2 position = new Vector2(
                (game.GraphicsDevice.Viewport.Width - size.X) / 2,
                (game.GraphicsDevice.Viewport.Height - size.Y) / 2
            );

            spriteBatch.DrawString(scoreFont, message, position, Color.Red);
        }

        spriteBatch.End();
    }
}
