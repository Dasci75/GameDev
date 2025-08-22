using Enes_TasciGameDev.Entities;
using Enes_TasciGameDev.Factory;
using Enes_TasciGameDev.Initializer;
using Enes_TasciGameDev.Interfaces;
using Enes_TasciGameDev.Items;
using Enes_TasciGameDev.Manager;
using Enes_TasciGameDev.Prop;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Enes_TasciGameDev.Levels
{
    public class Level1 : IGameState
    {
        private Game1 game;
        private Player player;
        private Texture2D background;
        private Texture2D coinTexture;
        private Texture2D finishTexture;
        private Texture2D healthBoostTexture;
        private Texture2D speedBoostTexture;
        private Texture2D scoreBackground;
        private SpriteFont scoreFont;
        private Texture2D overlay;
        private bool gameOver = false;
        private bool levelPassed = false;
        private double damageCooldown = 1.0;
        private double damageTimer = 0;
        private List<Obstacle> obstaclesList;
        private Dog dog;
        private Texture2D dogTexture;

        // Managers
        private CoinManager coinManager;
        private EnemyManager enemyManager;
        private PowerUpManager powerUpManager;
        private UIManager uiManager;
        private FinishManager finishManager;

        public Level1(Game1 game)
        {
            this.game = game;
        }

        public void LoadContent()
        {
            // Initialize enemies
            Texture2D goblinTexture = game.Content.Load<Texture2D>("goblin");
            Random random = new Random();
            List<Enemy> enemies = new List<Enemy>();
            for (int i = 0; i < 6; i++)
            {
                Vector2 pos = new Vector2(
                    random.Next(0, game.GraphicsDevice.Viewport.Width - (goblinTexture.Width / 3)),
                    random.Next(0, game.GraphicsDevice.Viewport.Height - (goblinTexture.Height / 4))
                );
                var goblin = (Goblin)EnemyFactory.CreateEnemy(EnemyType.Goblin, goblinTexture, pos, speed: 1f);
                enemies.Add(goblin);
            }

            // Initialize obstacles
            Texture2D obstacleTexture = game.Content.Load<Texture2D>("obstacle");
            obstaclesList = new List<Obstacle>
        {
            new Obstacle(new Vector2(300, 300), obstacleTexture, 0.1f),
            new Obstacle(new Vector2(500, 150), obstacleTexture, 0.15f),
            new Obstacle(new Vector2(100, 400), obstacleTexture, 0.2f)
        };

            // Configure level
            var config = new LevelInitializer.LevelConfig
            {
                BackgroundTextureName = "bgLevel1",
                MaxCoins = 5,
                CoinsRequiredForFinish = 5,
                PlayerStartPosition = new Vector2(400, 240),
                Obstacles = obstaclesList,
                Enemies = enemies
            };

            // Initialize using LevelInitializer
            LevelInitializer.Initialize(
                game,
                config,
                out player,
                out dog,
                out background,
                out coinTexture,
                out finishTexture,
                out healthBoostTexture,
                out speedBoostTexture,
                out coinManager,
                out enemyManager,
                out finishManager,
                out powerUpManager,
                out uiManager,
                out dogTexture,
                out scoreFont,
                out scoreBackground,
                out overlay);
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

            // Update player
            player.Update(gameTime, game.GraphicsDevice, obstaclesList);

            // Managers updaten
            coinManager.Update();
            int screenWidth = game.GraphicsDevice.Viewport.Width;
            int screenHeight = game.GraphicsDevice.Viewport.Height;
            enemyManager.Update(gameTime, obstaclesList, damageCooldown, ref damageTimer, ref gameOver, screenWidth, screenHeight);
            powerUpManager.Update();

            // Check finish
            finishManager.Update(player);

            // Speler én hond moeten bij de finish zijn
            if (finishManager.PlayerReached(player) && finishManager.DogReached(dog))
            {
                levelPassed = true;
            }

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            // Background
            spriteBatch.Draw(background, new Rectangle(0, 0,
                game.GraphicsDevice.Viewport.Width,
                game.GraphicsDevice.Viewport.Height), Color.White);

            // Obstacles
            foreach (var obstacle in obstaclesList)
                obstacle.Draw(spriteBatch);

            // Managers tekenen
            coinManager.Draw(spriteBatch);
            player.Draw(spriteBatch);
            enemyManager.Draw(spriteBatch);
            powerUpManager.Draw(spriteBatch);

            // Dog
            dog.Draw(spriteBatch);

            // Finish
            finishManager.Draw(spriteBatch);

            // UI
            uiManager.DrawScore(spriteBatch, game.GraphicsDevice);
            uiManager.DrawOverlay(spriteBatch, game.GraphicsDevice, gameOver, levelPassed);

            spriteBatch.End();
        }
    }
}
