using System;

namespace mvvm.unity.Samples
{
    public class CoinsCounterModel : ICoinsCounterModel
    {
        public int Coins { get; private set; }

        public event Action<int> Changed;

        public void AddCoins(int amount)
        {
            Coins += amount;
            Changed.Invoke(Coins);
        }
    }
}