using Enes_TasciGameDev.Entities;
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
    public class EnemyManager
    {
        private List<Enemy> enemies = new List<Enemy>();
        private Player player;

        public EnemyManager(Player player) => this.player = player;

        public void AddEnemy(Enemy enemy) => enemies.Add(enemy);

        public void Update(GameTime gameTime, List<Obstacle> obstacles, double damageCooldown, ref double damageTimer, ref bool gameOver)
        {
            Rectangle playerBounds = player.GetBounds();
            foreach (var enemy in enemies)
            {
                enemy.Update(gameTime, player.Position, enemies, obstacles, 800, 600); // example
                if (enemy.GetBounds().Intersects(playerBounds))
                {
                    damageTimer += gameTime.ElapsedGameTime.TotalSeconds;
                    if (damageTimer >= damageCooldown)
                    {
                        player.TakeDamage();
                        damageTimer = 0;
                        enemy.Stun();
                        if (player.isDead) gameOver = true;
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var enemy in enemies) enemy.Draw(spriteBatch);
        }
    }

}
