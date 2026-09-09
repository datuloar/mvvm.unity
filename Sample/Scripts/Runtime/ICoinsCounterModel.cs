using System;

namespace MvvmUnity.Samples
{
    public interface ICoinsCounterModel
    {
        int Coins { get; }

        event Action<int> Changed;

        void AddCoins(int amount);
    }
}
