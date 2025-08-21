using Enes_TasciGameDev.Entities;
using Enes_TasciGameDev.Factory;
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
        private Texture2D playerTexture;
        private Texture2D background;

        // Managers
        private CoinManager coinManager;
        private EnemyManager enemyManager;
        private PowerUpManager powerUpManager;
        private UIManager uiManager;
        private FinishManager finishManager;

        // Extra entities
        private List<Obstacle> obstaclesList;
        private Texture2D obstacleTexture;
        private Dog dog;
        private Texture2D dogTexture;

        // Level flow
        private Texture2D finishTexture;
        private Finish finish;
        private bool levelPassed = false;
        private bool gameOver = false;

        // Damage timing
        private double damageCooldown = 1.0;
        private double damageTimer = 0;

        public Level1(Game1 game)
        {
            this.game = game;
        }

        public void LoadContent()
        {
            // Background
            background = game.Content.Load<Texture2D>("bgLevel1");

            // Player
            playerTexture = game.Content.Load<Texture2D>("player");
            player = new Player(new Vector2(400, 240), playerTexture, 4, 4);

            // Dog
            dogTexture = game.Content.Load<Texture2D>("dog");
            dog = new Dog(new Vector2(200, 200), dogTexture, rows: 2, columns: 3);

            // Obstacles
            obstacleTexture = game.Content.Load<Texture2D>("obstacle");
            obstaclesList = new List<Obstacle>
            {
                new Obstacle(new Vector2(300, 300), obstacleTexture, 0.1f),
                new Obstacle(new Vector2(500, 150), obstacleTexture, 0.15f),
                new Obstacle(new Vector2(100, 400), obstacleTexture, 0.2f)
            };

            // Finish line
            finishTexture = game.Content.Load<Texture2D>("finish");
            finishManager = new FinishManager(finishTexture, 5, game.GraphicsDevice); // 5 coins nodig

            // Managers initialiseren
            Texture2D coinTexture = game.Content.Load<Texture2D>("coin");
            coinManager = new CoinManager(player, coinTexture, game.GraphicsDevice, 5);
            coinManager.SpawnNextCoin();

            enemyManager = new EnemyManager(player);
            Texture2D goblinTexture = game.Content.Load<Texture2D>("goblin");
            Random random = new Random();
            for (int i = 0; i < 6; i++)
            {
                Vector2 pos = new Vector2(
                    random.Next(0, game.GraphicsDevice.Viewport.Width - (goblinTexture.Width / 3)),
                    random.Next(0, game.GraphicsDevice.Viewport.Height - (goblinTexture.Height / 4))
                );
                var goblin = (Goblin)EnemyFactory.CreateEnemy(EnemyType.Goblin, goblinTexture, pos, speed: 1f);
                enemyManager.AddEnemy(goblin);
            }

            powerUpManager = new PowerUpManager(player, game);
            Texture2D healthTexture = game.Content.Load<Texture2D>("healthPowerUp");
            Texture2D speedTexture = game.Content.Load<Texture2D>("speedPowerUp");
            powerUpManager.SpawnPowerUp(PowerUpType.HealthBoost, healthTexture, 0.02f);
            powerUpManager.SpawnPowerUp(PowerUpType.SpeedBoost, speedTexture, 0.1f);

            // UI Manager
            SpriteFont scoreFont = game.Content.Load<SpriteFont>("scoreFont");
            Texture2D scoreBg = new Texture2D(game.GraphicsDevice, 1, 1);
            scoreBg.SetData(new[] { Color.LightGray });

            Texture2D overlay = new Texture2D(game.GraphicsDevice, 1, 1);
            overlay.SetData(new[] { Color.Black });

            uiManager = new UIManager(player, scoreFont, scoreBg, overlay);
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
            enemyManager.Update(gameTime, obstaclesList, damageCooldown, ref damageTimer, ref gameOver);
            powerUpManager.Update();

            // Check finish
            finishManager.Update(player);

            if (finishManager.PlayerReached(player))
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
