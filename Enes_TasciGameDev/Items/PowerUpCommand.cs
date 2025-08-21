using Enes_TasciGameDev.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enes_TasciGameDev.Items
{
    public class PowerUpCommand : ICommand
    {
        private Player player;
        private PowerUp powerUp; // hele power-up, niet alleen het type

        public PowerUpCommand(Player player, PowerUp powerUp)
        {
            this.player = player;
            this.powerUp = powerUp;
        }

        public void Execute()
        {
            if (powerUp.IsCollected) return;

            switch (powerUp.Type)
            {
                case PowerUpType.HealthBoost:
                    player.Health++;
                    break;
                case PowerUpType.SpeedBoost:
                    player.ApplySpeedBoost(2f, 10f);
                    break;
            }

            powerUp.IsCollected = true;
        }
    }
}