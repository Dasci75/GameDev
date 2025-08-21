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
        private Player player;

        public PowerUpManager(Player player) => this.player = player;

        public void SpawnPowerUp(PowerUpType type, Texture2D texture, Vector2 position, float scale)
        {
            PowerUp pu = new PowerUp(position, type, texture, scale);
            powerUps.Add(pu);
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
