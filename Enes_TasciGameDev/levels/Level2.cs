using Enes_TasciGameDev;
using Enes_TasciGameDev.Entities;
using Enes_TasciGameDev.Factory;
using Enes_TasciGameDev.Initializer;
using Enes_TasciGameDev.Items;
using Enes_TasciGameDev.Manager;
using Enes_TasciGameDev.Prop;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Enes_TasciGameDev
{
    public class Level2 : IGameState
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
        private List<Obstacle> obstacles;
        private Dog dog;
        private Texture2D dogTexture;

        // Managers
        private CoinManager coinManager;
        private EnemyManager enemyManager;
        private FinishManager finishManager;
        private PowerUpManager powerUpManager;
        private UIManager uiManager;
        private GameManager gameManager;

        public Level2(Game1 game)
        {
            this.game = game;
            obstacles = new List<Obstacle>();
            gameManager = GameManager.Instance;
        }

        public void LoadContent()
        {
            // Initialize enemies
            Texture2D skeletonTexture = game.Content.Load<Texture2D>("skeleton");
            Texture2D thiefTexture = game.Content.Load<Texture2D>("thief");
            List<Enemy> enemies = new List<Enemy>
            {
                (Enemy)EnemyFactory.CreateEnemy(EnemyType.Thief, thiefTexture, new Vector2(300, 150), 2f),
                (Enemy)EnemyFactory.CreateEnemy(EnemyType.Thief, thiefTexture, new Vector2(500, 400), 2f),
                (Enemy)EnemyFactory.CreateEnemy(EnemyType.Skeleton, skeletonTexture, new Vector2(200, 200), 1.3f,
                    new List<Vector2> { new Vector2(200, 200), new Vector2(400, 200), new Vector2(400, 400), new Vector2(200, 400) }),
                (Enemy)EnemyFactory.CreateEnemy(EnemyType.Skeleton, skeletonTexture, new Vector2(300, 100), 1.3f,
                    new List<Vector2> { new Vector2(300, 100), new Vector2(200, 100), new Vector2(200, 300), new Vector2(300, 300) }),
                (Enemy)EnemyFactory.CreateEnemy(EnemyType.Skeleton, skeletonTexture, new Vector2(400, 150), 1.3f,
                    new List<Vector2> { new Vector2(400, 150), new Vector2(200, 150), new Vector2(200, 350), new Vector2(400, 350) }),
                (Enemy)EnemyFactory.CreateEnemy(EnemyType.Skeleton, skeletonTexture, new Vector2(150, 300), 1.3f,
                    new List<Vector2> { new Vector2(100, 300), new Vector2(350, 300), new Vector2(350, 200), new Vector2(150, 200) }),
                (Enemy)EnemyFactory.CreateEnemy(EnemyType.Skeleton, skeletonTexture, new Vector2(200, 250), 1.3f,
                    new List<Vector2> { new Vector2(150, 250), new Vector2(300, 250), new Vector2(300, 200), new Vector2(500, 200) })
            };

            // Configure level
            var config = new LevelInitializer.LevelConfig
            {
                BackgroundTextureName = "bgLevel2",
                MaxCoins = 7,
                CoinsRequiredForFinish = 7,
                PlayerStartPosition = new Vector2(100, 100),
                Obstacles = obstacles,
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
            dog.Update(gameTime, game.GraphicsDevice, null);

            if (gameOver || levelPassed)
            {
                var keyboard = Keyboard.GetState();
                if (keyboard.IsKeyDown(Keys.Enter) || keyboard.IsKeyDown(Keys.Tab))
                    game.ChangeState(new StartScreen(game));
                return;
            }

            player.Update(gameTime, game.GraphicsDevice, null);
            coinManager.Update();
            int screenWidth = game.GraphicsDevice.Viewport.Width;
            int screenHeight = game.GraphicsDevice.Viewport.Height;
            enemyManager.Update(gameTime, obstacles, damageCooldown, ref damageTimer, ref gameOver, screenWidth, screenHeight);
            finishManager.Update(player);
            powerUpManager.CheckCoinTriggers(healthBoostTexture, speedBoostTexture);
            powerUpManager.Update();

            // Handle thief coin stealing
            foreach (var enemy in enemyManager.enemies)
            {
                if (enemy is Thief && enemy.GetBounds().Intersects(player.GetBounds()) && player.Coins > 0)
                {
                    player.RemoveCoin();
                    coinManager.SpawnNextCoin(); // Ensure a new coin spawns if stolen
                }
            }

            // Check for level completion
            if (finishManager.PlayerReached(player))
                levelPassed = true;

            // Update game over state
            if (player.isDead)
                gameOver = true;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            // Draw background
            spriteBatch.Draw(background, new Rectangle(0, 0,
                game.GraphicsDevice.Viewport.Width,
                game.GraphicsDevice.Viewport.Height), Color.White);

            // Draw game elements
            coinManager.Draw(spriteBatch);
            player.Draw(spriteBatch);
            dog.Draw(spriteBatch);
            enemyManager.Draw(spriteBatch);
            finishManager.Draw(spriteBatch);
            powerUpManager.Draw(spriteBatch);

            // Draw UI
            uiManager.DrawScore(spriteBatch, game.GraphicsDevice);
            uiManager.DrawOverlay(spriteBatch, game.GraphicsDevice, gameOver, levelPassed);

            spriteBatch.End();
        }
    }
}