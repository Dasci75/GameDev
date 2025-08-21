using Enes_TasciGameDev.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enes_TasciGameDev.Manager
{
    public class PowerUpManager
    {
        private List<PowerUp> powerUps = new List<PowerUp>();
        private HashSet<int> triggeredCoinCounts = new HashSet<int>(); // voorkomt dubbele spawns
        private Player player;
        private Game1 game;
        private Random random = new Random();

        public PowerUpManager(Player player, Game1 game)
        {
            this.player = player;
            this.game = game;
        }

        public void SpawnPowerUp(PowerUpType type, Texture2D texture, float scale)
        {
            Vector2 pos = new Vector2(
                random.Next(0, game.GraphicsDevice.Viewport.Width - 50),
                random.Next(0, game.GraphicsDevice.Viewport.Height - 50)
            );

            PowerUp pu = new PowerUp(pos, type, texture, scale);
            powerUps.Add(pu);
        }

        public void CheckCoinTriggers(Texture2D healthTex, Texture2D speedTex)
        {
            if (player.Coins == 1 && !triggeredCoinCounts.Contains(1))
            {
                SpawnPowerUp(PowerUpType.HealthBoost, healthTex, 0.02f);
                triggeredCoinCounts.Add(1);
            }

            if (player.Coins == 3 && !triggeredCoinCounts.Contains(3))
            {
                SpawnPowerUp(PowerUpType.SpeedBoost, speedTex, 0.1f);
                triggeredCoinCounts.Add(3);
            }
        }

        public void Update()
        {
            foreach (var pu in powerUps)
            {
                if (!pu.IsCollected && pu.GetBounds().Intersects(player.GetBounds()))
                {
                    new PowerUpCommand(player, pu).Execute();
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var pu in powerUps)
                pu.Draw(spriteBatch);
        }
    }
}
