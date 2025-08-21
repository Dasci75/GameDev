using Enes_TasciGameDev.Prop;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enes_TasciGameDev.Manager
{
    public class FinishManager
    {
        private Finish finish;
        private Texture2D finishTexture;
        private int coinsRequired;
        private GraphicsDevice graphics;

        public FinishManager(Texture2D finishTexture, int coinsRequired, GraphicsDevice graphics)
        {
            this.finishTexture = finishTexture;
            this.coinsRequired = coinsRequired;
            this.graphics = graphics;
        }

        public Finish GetFinish() => finish;

        public void Update(Player player)
        {
            if (player.Coins >= coinsRequired && finish == null)
            {
                float scale = 0.5f;
                int finishWidth = (int)(finishTexture.Width * scale);
                int finishHeight = (int)(finishTexture.Height * scale);

                Vector2 pos = new Vector2(
                    graphics.Viewport.Width - finishWidth - 20,
                    (graphics.Viewport.Height - finishHeight) / 2
                );

                finish = new Finish(finishTexture, pos, scale);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            finish?.Draw(spriteBatch);
        }

        public bool PlayerReached(Player player)
        {
            return finish != null && player.GetBounds().Intersects(finish.GetBounds());
        }
    }
}
