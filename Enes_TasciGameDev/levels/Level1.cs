using Enes_TasciGameDev;
using Enes_TasciGameDev.Entities;
using Enes_TasciGameDev.Prop;
using Microsoft.VisualBasic.Devices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;

public class Level1 : IGameState
{
    private Game1 game;
    private Player player;  // Your Player class
    private Texture2D playerTexture;
    private Texture2D background;
    private KeyboardState previousKeyboardState;
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
    private List<Goblin> goblins;
    private Texture2D goblinTexture;
    private bool gameOver = false;
    private double damageCooldown = 1.0; // 1 seconde tussen hits
    private double damageTimer = 0;






    public Level1(Game1 game)
    {
        this.game = game;
    }

    public void LoadContent()
    {
        //goblin enemy spawnen
        goblinTexture = game.Content.Load<Texture2D>("goblin"); // voeg enemy.png toe aan Content
        goblins = new List<Goblin>();

        // spawn bv 3 vijanden random
        for (int i = 0; i < 3; i++)
        {
            Vector2 pos = new Vector2(
                random.Next(0, game.GraphicsDevice.Viewport.Width - goblinTexture.Width),
                random.Next(0, game.GraphicsDevice.Viewport.Height - goblinTexture.Height)
            );
            goblins.Add(new Goblin(goblinTexture, pos, speed: 1f));
        }
        //finish line
        finishTexture = game.Content.Load<Texture2D>("finish"); // Voeg house.png toe aan Content
        finish = null; // nog niet zichtbaar

        // Background
        background = game.Content.Load<Texture2D>("bgLevel1");
        scoreFont = game.Content.Load<SpriteFont>("scoreFont"); // grote font, bv 48px

        // Maak een kleine texture van 1x1 pixel en kleur hem later lichtgrijs
        scoreBackground = new Texture2D(game.GraphicsDevice, 1, 1);
        scoreBackground.SetData(new[] { Color.LightGray });
        
        //player
        playerTexture = game.Content.Load<Texture2D>("player");
        player = new Player(new Vector2(400, 240), playerTexture, 4, 4);

        // Coins
        coinTexture = game.Content.Load<Texture2D>("coin"); // voeg coin.png toe aan Content
        coins = new List<Coin>();

        //overlay
        overlay = new Texture2D(game.GraphicsDevice, 1, 1);
        overlay.SetData(new[] { Color.Black });

        for (int i = 0; i < 5; i++) // spawn 5 coins random
        {
            Vector2 pos = new Vector2(
                random.Next(0, game.GraphicsDevice.Viewport.Width - coinTexture.Width),
                random.Next(0, game.GraphicsDevice.Viewport.Height - coinTexture.Height)
            );
            coins.Add(new Coin(coinTexture, pos));
        }
        SpawnNextCoin();
    }
    private void SpawnNextCoin()
    {
        if (score < 5)
        {
            float scale = 0.1f; //zelfde als in Coin
            int coinWidth = (int)(coinTexture.Width * scale);
            int coinHeight = (int)(coinTexture.Height * scale);

            int screenWidth = game.GraphicsDevice.Viewport.Width;
            int screenHeight = game.GraphicsDevice.Viewport.Height;

            int x = random.Next(0, screenWidth - coinWidth);
            int y = random.Next(0, screenHeight - coinHeight);

            currentCoin = new Coin(coinTexture, new Vector2(x, y), scale);

            Console.WriteLine($"Spawn coin at: {x}, {y}");
        }
        else
        {
            currentCoin = null;
        }
    }





    private void CheckForFinishSpawn()
    {
        if (score >= 5 && finish == null)
        {
            // Spawn huisje rechts-midden, iets kleiner
            float scale = 0.5f; // halve grootte van de originele texture
            int finishWidth = (int)(finishTexture.Width * scale);
            int finishHeight = (int)(finishTexture.Height * scale);

            Vector2 pos = new Vector2(
                game.GraphicsDevice.Viewport.Width - finishWidth - 20, // dichter bij rechts
                (game.GraphicsDevice.Viewport.Height - finishHeight) / 2 // precies verticaal gecentreerd
            );

            finish = new Finish(finishTexture, pos, scale); // Finish class moet scale ondersteunen
        }
    }
    


    public void Update(GameTime gameTime)
    {
        MouseState mouseState = Microsoft.Xna.Framework.Input.Mouse.GetState();


        if (gameOver || levelPassed)
        {
            var keyboard = Microsoft.Xna.Framework.Input.Keyboard.GetState();

            if (keyboard.IsKeyDown(Keys.Enter))
            {
                // Go to Level 2 if the player passed the level
                if (levelPassed)
                    game.ChangeState(new Level2(game));
                else // If game over, restart Level 1 or go to menu (you choose)
                    game.ChangeState(new StartScreen(game));
            }

            if (keyboard.IsKeyDown(Keys.Tab))
            {
                // Always go back to Main Menu
                game.ChangeState(new StartScreen(game));
            }

            return; // Freeze game
        }


        player.Update(gameTime, game.GraphicsDevice);

        Rectangle playerBounds = player.GetBounds();

        if (currentCoin != null && playerBounds.Intersects(currentCoin.GetBounds()))
        {
            score++;
            SpawnNextCoin();
        }

        foreach (var goblin in goblins)
        {
            goblin.Update(gameTime, player.Position, goblins);

            if (goblin.GetBounds().Intersects(player.GetBounds()))
            {
                // Player krijgt schade
                damageTimer += gameTime.ElapsedGameTime.TotalSeconds;
                if (damageTimer >= damageCooldown)
                {
                    player.TakeDamage();
                    damageTimer = 0;

                    // Stun de goblin 2 sec
                    goblin.Stun(); // Je moet stun functie toevoegen in Goblin

                    if (player.isDead)
                    {
                        gameOver = true;
                        Console.WriteLine("Game Over!");
                    }
                }
            }
        }

        CheckForFinishSpawn();

        if (finish != null && playerBounds.Intersects(finish.GetBounds()))
        {
            levelPassed = true;
        }
    }




    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Begin();

        // Teken de achtergrond
        spriteBatch.Draw(background, new Rectangle(0, 0,
            game.GraphicsDevice.Viewport.Width,
            game.GraphicsDevice.Viewport.Height), Color.White);

        // Score tekst en achtergrond (rechtsboven)
        string scoreText = $"Coins: {score}";
        Vector2 textSize = scoreFont.MeasureString(scoreText);

        // Achtergrond rechthoek met padding
        int paddingX = 20;
        int paddingY = 10;
        Rectangle backgroundRect = new Rectangle(
            game.GraphicsDevice.Viewport.Width - (int)textSize.X - paddingX,
            10,
            (int)textSize.X + paddingX,
            (int)textSize.Y + paddingY
        );

        // Lichtgrijze achtergrond
        spriteBatch.Draw(scoreBackground, backgroundRect, Color.LightGray);

        // Score tekst
        spriteBatch.DrawString(scoreFont, scoreText,
            new Vector2(backgroundRect.X + paddingX / 2, backgroundRect.Y + paddingY / 2),
            Color.Black);

        // Teken de coins en hp
        if (currentCoin != null)
        {
            currentCoin.Draw(spriteBatch);
        }
        string healthText = $"Health: {player.Health}";
        spriteBatch.DrawString(scoreFont, healthText, new Vector2(10, 10), Color.Red);

        // Teken de speler
        player.Draw(spriteBatch);

        //teken enemies
        foreach (var enemy in goblins)
        {
            enemy.Draw(spriteBatch);
        }

        //teken finish line
        if (finish != null)
        {
            finish.Draw(spriteBatch);
        }

        // Level passed melding
        if (levelPassed)
        {
            // Teken semi-transparante overlay
            spriteBatch.Draw(overlay, new Rectangle(0, 0,
                game.GraphicsDevice.Viewport.Width,
                game.GraphicsDevice.Viewport.Height), Color.Black * 0.6f);

            string message = "\nLevel Complete!\n\nPress Enter for Level 2\n\nPress Tab for Menu\n";
            Vector2 size = scoreFont.MeasureString(message);
            Vector2 position = new Vector2(
                (game.GraphicsDevice.Viewport.Width - size.X) / 2,
                (game.GraphicsDevice.Viewport.Height - size.Y) / 2
            );

            spriteBatch.DrawString(scoreFont, message, position, Color.Yellow);
        }
        //gameover scherm
        if (gameOver)
        {
            // semi-transparante overlay
            spriteBatch.Draw(overlay, new Rectangle(0, 0,
                game.GraphicsDevice.Viewport.Width,
                game.GraphicsDevice.Viewport.Height), Color.Black * 0.6f);

            string message = "Game Over!\n\nPress Enter for Main Menu";
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
