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

        public CoinManager(Player player, Texture2D coinTexture, GraphicsDevice graphics)
        {
            this.player = player;
            this.coinTexture = coinTexture;
            this.graphics = graphics;
            coins = new List<Coin>();
        }

        public void SpawnNextCoin()
        {
            int width = coinTexture.Width;
            int height = coinTexture.Height;
            Vector2 pos = new Vector2(random.Next(0, graphics.Viewport.Width - width),
                                      random.Next(0, graphics.Viewport.Height - height));
            currentCoin = new Coin(coinTexture, pos);
        }

        public void Update()
        {
            if (currentCoin != null && currentCoin.GetBounds().Intersects(player.GetBounds()))
            {
                player.AddCoin();
                SpawnNextCoin();
            }
        }

        public void Draw(SpriteBatch spriteBatch) => currentCoin?.Draw(spriteBatch);
    }

}
