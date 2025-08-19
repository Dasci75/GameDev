using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enes_TasciGameDev.Prop
{
    public class Finish
    {
        private Texture2D texture;
        public Vector2 Position;
        private float scale;

        public Finish(Texture2D texture, Vector2 position, float scale = 1f)
        {
            this.texture = texture;
            this.Position = position;
            this.scale = scale;
        }

        public Rectangle GetBounds()
        {
            return new Rectangle(
                (int)Position.X,
                (int)Position.Y,
                (int)(texture.Width * scale),
                (int)(texture.Height * scale)
            );
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Position, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }
    }

}
