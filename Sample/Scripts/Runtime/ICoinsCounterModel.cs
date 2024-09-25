using System;

namespace mvvm.unity.Samples
{
    public interface ICoinsCounterModel
    {
        int Coins { get; }

        event Action<int> Changed;

        void AddCoins(int amount);
    }
}