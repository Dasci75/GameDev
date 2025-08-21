using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enes_TasciGameDev.Manager
{
    public class UIManager
    {
        private Player player;
        private SpriteFont font;
        private Texture2D scoreBg, overlay;

        public UIManager(Player player, SpriteFont font, Texture2D scoreBg, Texture2D overlay)
        {
            this.player = player;
            this.font = font;
            this.scoreBg = scoreBg;
            this.overlay = overlay;
        }

        public void DrawScore(SpriteBatch spriteBatch, GraphicsDevice graphics)
        {
            string scoreText = $"Coins: {player.Coins}";
            Vector2 size = font.MeasureString(scoreText);
            spriteBatch.Draw(scoreBg, new Rectangle(graphics.Viewport.Width - (int)size.X - 20, 10, (int)size.X + 20, (int)size.Y + 10), Color.LightGray);
            spriteBatch.DrawString(font, scoreText, new Vector2(graphics.Viewport.Width - (int)size.X - 10, 10), Color.Black);
            spriteBatch.DrawString(font, $"Health: {player.Health}", new Vector2(10, 10), Color.Red);
        }

        public void DrawOverlay(SpriteBatch spriteBatch, GraphicsDevice graphics, bool gameOver, bool levelPassed)
        {
            if (!gameOver && !levelPassed) return;

            spriteBatch.Draw(overlay, new Rectangle(0, 0, graphics.Viewport.Width, graphics.Viewport.Height), Color.Black * 0.6f);
            string message = levelPassed ? "Level Complete!\n\nPress Enter for Next Level\n\nPress Tab for Main Menu" : "Game Over!\n\nPress Enter for Main Menu"; Vector2 size = font.MeasureString(message);
            Vector2 position = new Vector2((graphics.Viewport.Width - size.X) / 2, (graphics.Viewport.Height - size.Y) / 2);
            spriteBatch.DrawString(font, message, position, levelPassed ? Color.Yellow : Color.Red);
        }
    }

}
