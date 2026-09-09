using System;

namespace MvvmUnity.Samples
{
    public sealed class CoinsCounterModel : ICoinsCounterModel
    {
        public int Coins { get; private set; }

        public event Action<int> Changed = delegate { };

        public void AddCoins(int amount)
        {
            Coins += amount;
            Changed.Invoke(Coins);
        }
    }
}
