using Enes_TasciGameDev.Entities;
using Enes_TasciGameDev.Items;
using Enes_TasciGameDev.Manager;
using Enes_TasciGameDev.Prop;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enes_TasciGameDev.Initializer
{
    public class LevelInitializer
    {
        public struct LevelConfig
        {
            public string BackgroundTextureName;
            public int MaxCoins;
            public int CoinsRequiredForFinish;
            public Vector2 PlayerStartPosition;
            public List<Obstacle> Obstacles;
            public List<Enemy> Enemies;
        }

        public static void Initialize(
            Game1 game,
            LevelConfig config,
            out Player player,
            out Dog dog,
            out Texture2D background,
            out Texture2D coinTexture,
            out Texture2D finishTexture,
            out Texture2D healthBoostTexture,
            out Texture2D speedBoostTexture,
            out CoinManager coinManager,
            out EnemyManager enemyManager,
            out FinishManager finishManager,
            out PowerUpManager powerUpManager,
            out UIManager uiManager,
            out Texture2D dogTexture,
            out SpriteFont scoreFont,
            out Texture2D scoreBackground,
            out Texture2D overlay)
        {
            // Load textures
            background = game.Content.Load<Texture2D>(config.BackgroundTextureName);
            dogTexture = game.Content.Load<Texture2D>("dog");
            Texture2D playerTexture = game.Content.Load<Texture2D>("player");
            coinTexture = game.Content.Load<Texture2D>("coin");
            finishTexture = game.Content.Load<Texture2D>("finish");
            healthBoostTexture = game.Content.Load<Texture2D>("healthPowerUp");
            speedBoostTexture = game.Content.Load<Texture2D>("speedPowerUp");

            // Initialize player
            player = new Player(config.PlayerStartPosition, playerTexture, 4, 4);

            // Initialize dog
            dog = new Dog(new Vector2(200, 200), dogTexture, rows: 2, columns: 3);

            // Initialize UI textures and font
            scoreFont = game.Content.Load<SpriteFont>("scoreFont");
            scoreBackground = new Texture2D(game.GraphicsDevice, 1, 1);
            scoreBackground.SetData(new[] { Color.LightGray });
            overlay = new Texture2D(game.GraphicsDevice, 1, 1);
            overlay.SetData(new[] { Color.Black });

            // Initialize managers
            coinManager = new CoinManager(player, coinTexture, game.GraphicsDevice, config.MaxCoins);
            coinManager.SpawnNextCoin();
            enemyManager = new EnemyManager(player);
            foreach (var enemy in config.Enemies)
            {
                enemyManager.AddEnemy(enemy);
            }
            finishManager = new FinishManager(finishTexture, config.CoinsRequiredForFinish, game.GraphicsDevice);
            powerUpManager = new PowerUpManager(player, game);
            uiManager = new UIManager(player, scoreFont, scoreBackground, overlay);

            // Spawn initial power-ups
            powerUpManager.SpawnPowerUp(PowerUpType.HealthBoost, healthBoostTexture, 0.02f);
            powerUpManager.SpawnPowerUp(PowerUpType.SpeedBoost, speedBoostTexture, 0.1f);
        }
    }
}