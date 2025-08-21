using Enes_TasciGameDev;
using Enes_TasciGameDev.Entities;
using Enes_TasciGameDev.Factory;
using Enes_TasciGameDev.Items;
using Enes_TasciGameDev.Prop;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Enes_TasciGameDev
{
    public class Level1 : IGameState
    {
        private Game1 game;
        private Player player;
        private Texture2D playerTexture;
        private Texture2D background;
        private KeyboardState previousKeyboardState;
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
        private List<Entities.Enemy> enemies; // Changed from List<Goblin> to List<Enemy>
        private Texture2D goblinTexture;
        private bool gameOver = false;
        private double damageCooldown = 1.0; // 1 second between hits
        private double damageTimer = 0;
        private List<PowerUp> powerUps;
        private List<Obstacle> obstaclesList;
        private Texture2D obstacleTexture;
        private Dog dog;
        private Texture2D dogTexture;


        public Level1(Game1 game)
        {
            this.game = game;
            powerUps = new List<PowerUp>();
            enemies = new List<Entities.Enemy>(); // Initialize enemies list
        }

        public void LoadContent()
        {
            //create dog
            dogTexture = game.Content.Load<Texture2D>("dog");
            dog = new Dog(new Vector2(200, 200), dogTexture, rows: 2, columns: 3);

            // Obstacles (tinier scales)
            obstacleTexture = game.Content.Load<Texture2D>("obstacle"); // Simple square/block texture
            obstaclesList = new List<Obstacle>
            {
                new Obstacle(new Vector2(300, 300), obstacleTexture, 0.1f),
                new Obstacle(new Vector2(500, 150), obstacleTexture, 0.15f),
                new Obstacle(new Vector2(100, 400), obstacleTexture, 0.2f)
            };

            // PowerUps
            Texture2D healthTexture = game.Content.Load<Texture2D>("healthPowerUp");
            Texture2D speedTexture = game.Content.Load<Texture2D>("speedPowerUp");
            SpawnPowerUps(healthTexture, speedTexture);

            // Goblin enemies using EnemyFactory
            goblinTexture = game.Content.Load<Texture2D>("goblin");
            for (int i = 0; i < 6; i++)
            {
                Vector2 pos;
                bool validPosition;
                do
                {
                    pos = new Vector2(
                        random.Next(0, game.GraphicsDevice.Viewport.Width - (goblinTexture.Width / 3)), // Use frameWidth approximation
                        random.Next(0, game.GraphicsDevice.Viewport.Height - ((goblinTexture.Height / 4) + 1))
                    );
                    validPosition = true;
                    Goblin tempGoblin = (Goblin)EnemyFactory.CreateEnemy(EnemyType.Goblin, goblinTexture, pos, speed: 1f); // Temporary goblin for bounds
                    Rectangle goblinBounds = tempGoblin.GetBounds();
                    foreach (var obstacle in obstaclesList)
                    {
                        if (goblinBounds.Intersects(obstacle.Bounds))
                        {
                            validPosition = false;
                            break;
                        }
                    }
                } while (!validPosition);
                Enemy goblin = (Enemy)EnemyFactory.CreateEnemy(EnemyType.Goblin, goblinTexture, pos, speed: 1f);
                enemies.Add(goblin);
            }

            // Finish line
            finishTexture = game.Content.Load<Texture2D>("finish");
            finish = null; // Not visible yet

            // Background
            background = game.Content.Load<Texture2D>("bgLevel1");
            scoreFont = game.Content.Load<SpriteFont>("scoreFont");

            // Score background
            scoreBackground = new Texture2D(game.GraphicsDevice, 1, 1);
            scoreBackground.SetData(new[] { Color.LightGray });

            // Player
            playerTexture = game.Content.Load<Texture2D>("player");
            player = new Player(new Vector2(400, 240), playerTexture, 4, 4);

            // Coins
            coinTexture = game.Content.Load<Texture2D>("coin");
            coins = new List<Coin>();
            for (int i = 0; i < 5; i++)
            {
                Vector2 pos = new Vector2(
                    random.Next(0, game.GraphicsDevice.Viewport.Width - coinTexture.Width),
                    random.Next(0, game.GraphicsDevice.Viewport.Height - coinTexture.Height)
                );
                coins.Add(new Coin(coinTexture, pos));
            }
            SpawnNextCoin();

            // Overlay
            overlay = new Texture2D(game.GraphicsDevice, 1, 1);
            overlay.SetData(new[] { Color.Black });
        }

        private void SpawnPowerUps(Texture2D healthTexture, Texture2D speedTexture)
        {
            powerUps.Add(new PowerUp(new Vector2(300, 200), PowerUpType.HealthBoost, healthTexture, 0.02f));
            powerUps.Add(new PowerUp(new Vector2(200, 300), PowerUpType.SpeedBoost, speedTexture, 0.1f));
        }

        private void SpawnNextCoin()
        {
            if (player.Coins < 5)
            {
                float scale = 0.1f;
                int coinWidth = (int)(coinTexture.Width * scale);
                int coinHeight = (int)(coinTexture.Height * scale);

                int x = random.Next(0, game.GraphicsDevice.Viewport.Width - coinWidth);
                int y = random.Next(0, game.GraphicsDevice.Viewport.Height - coinHeight);

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
            if (player.Coins >= 5 && finish == null)
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
            dog.Update(gameTime, game.GraphicsDevice, obstaclesList);

            if (gameOver || levelPassed)
            {
                var keyboard = Keyboard.GetState();
                if (keyboard.IsKeyDown(Keys.Enter))
                {
                    if (levelPassed)
                        game.ChangeState(new Level2(game));
                    else
                        game.ChangeState(new StartScreen(game));
                }
                if (keyboard.IsKeyDown(Keys.Tab))
                {
                    game.ChangeState(new StartScreen(game));
                }
                return;
            }

            player.Update(gameTime, game.GraphicsDevice, obstaclesList);
            Rectangle playerBounds = player.GetBounds();

            if (currentCoin != null && playerBounds.Intersects(currentCoin.GetBounds()))
            {
                player.AddCoin();
                SpawnNextCoin();
            }

            foreach (var enemy in enemies)
            {
                enemy.Update(gameTime, player.Position, enemies, obstaclesList, game.GraphicsDevice.Viewport.Width, game.GraphicsDevice.Viewport.Height);
                if (enemy.GetBounds().Intersects(player.GetBounds()))
                {
                    damageTimer += gameTime.ElapsedGameTime.TotalSeconds;
                    if (damageTimer >= damageCooldown)
                    {
                        player.TakeDamage();
                        damageTimer = 0;
                        enemy.Stun();
                        if (player.isDead)
                        {
                            gameOver = true;
                            Console.WriteLine("Game Over!");
                        }
                    }
                }
            }

            foreach (var powerUp in powerUps)
            {
                if (!powerUp.IsCollected && powerUp.GetBounds().Intersects(player.GetBounds()))
                {
                    powerUp.Apply(player);
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

            // Draw background
            spriteBatch.Draw(background, new Rectangle(0, 0,
                game.GraphicsDevice.Viewport.Width,
                game.GraphicsDevice.Viewport.Height), Color.White);

            // Draw obstacles
            foreach (var obstacle in obstaclesList)
            {
                obstacle.Draw(spriteBatch);
            }

            // Draw score background and text
            string scoreText = $"Coins: {player.Coins}";
            Vector2 textSize = scoreFont.MeasureString(scoreText);
            int paddingX = 20;
            int paddingY = 10;
            Rectangle backgroundRect = new Rectangle(
                game.GraphicsDevice.Viewport.Width - (int)textSize.X - paddingX,
                10,
                (int)textSize.X + paddingX,
                (int)textSize.Y + paddingY
            );
            spriteBatch.Draw(scoreBackground, backgroundRect, Color.LightGray);
            spriteBatch.DrawString(scoreFont, scoreText,
                new Vector2(backgroundRect.X + paddingX / 2, backgroundRect.Y + paddingY / 2),
                Color.Black);

            // Draw health
            string healthText = $"Health: {player.Health}";
            spriteBatch.DrawString(scoreFont, healthText, new Vector2(10, 10), Color.Red);

            // Draw coin
            if (currentCoin != null)
            {
                currentCoin.Draw(spriteBatch);
            }

            // Draw player
            player.Draw(spriteBatch);

            // Draw enemies
            foreach (var enemy in enemies)
            {
                enemy.Draw(spriteBatch);
            }

            // Draw powerups
            foreach (var powerUp in powerUps)
            {
                powerUp.Update(player);
                powerUp.Draw(spriteBatch);
            }
            //draw dog
            dog.Draw(spriteBatch);

            // Draw finish line
            if (finish != null)
            {
                finish.Draw(spriteBatch);
            }

            // Draw level passed or game over overlay
            if (levelPassed)
            {
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
            else if (gameOver)
            {
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
}