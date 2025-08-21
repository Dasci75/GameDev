using Enes_TasciGameDev.Prop;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Enes_TasciGameDev.Entities
{
    public abstract class Enemy
    {
        public virtual Vector2 Position { get; set; } // Changed to virtual
        public abstract Rectangle GetBounds();
        public abstract void Update(GameTime gameTime, Vector2 playerPosition, List<Enemy> enemies, List<Obstacle> obstacles, int screenWidth, int screenHeight);
        public abstract void Draw(SpriteBatch spriteBatch);
        public abstract void Stun();
    }
}