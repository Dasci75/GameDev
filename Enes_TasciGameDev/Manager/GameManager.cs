using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enes_TasciGameDev.Manager
{
    public class GameManager
    {
        private static GameManager instance;
        public static GameManager Instance => instance ??= new GameManager();

        public int TotalCoins { get; private set; }
        public bool IsGameOver { get; set; } = false;

        private GameManager() { }

        public void AddCoins(int amount)
        {
            TotalCoins += amount;
            Console.WriteLine($"Total Coins: {TotalCoins}");
        }
    }
}
