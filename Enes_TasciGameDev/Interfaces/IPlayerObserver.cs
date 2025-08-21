using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enes_TasciGameDev.Interfaces
{
    public interface IPlayerObserver
    {
        void OnCoinCollected(int newCoinCount);
        void OnHealthChanged(int newHealth);
    }
}
