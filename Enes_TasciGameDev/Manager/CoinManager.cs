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
    public class CoinManager
    {
        private List<Coin> coins;
        private Coin currentCoin;
        private Random random = new Random();
        private Player player;
        private Texture2D coinTexture;
        private GraphicsDevice graphics;

        private int maxCoins;   // ✅ nieuw
        private bool stopSpawning = false; // ✅ nieuw

        public CoinManager(Player player, Texture2D coinTexture, GraphicsDevice graphics, int maxCoins)
        {
            this.player = player;
            this.coinTexture = coinTexture;
            this.graphics = graphics;
            this.maxCoins = maxCoins;
            coins = new List<Coin>();
        }

        public void SpawnNextCoin()
        {
            if (stopSpawning) return; // ✅ geen nieuwe coins meer

            int width = coinTexture.Width;
            int height = coinTexture.Height;
            Vector2 pos = new Vector2(
                random.Next(0, graphics.Viewport.Width - width),
                random.Next(0, graphics.Viewport.Height - height));

            currentCoin = new Coin(coinTexture, pos);
        }

        public void Update()
        {
            if (currentCoin != null && currentCoin.GetBounds().Intersects(player.GetBounds()))
            {
                player.AddCoin();

                if (player.Coins >= maxCoins)
                {
                    stopSpawning = true;
                    currentCoin = null; // laatste coin verwijderen
                }
                else
                {
                    SpawnNextCoin();
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch) => currentCoin?.Draw(spriteBatch);
    }
}

