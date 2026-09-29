using System;

namespace MvvmUnity.Samples.CharacterCreator
{
    public sealed class HeroDraft
    {
        public const int PointBudget = 9;
        public const int MaxPointsPerStat = 5;

        private static readonly int StatCount = Enum.GetValues(typeof(HeroStat)).Length;

        private readonly int[] _points = new int[StatCount];

        public event Action Changed;

        public int SpentPoints
        {
            get
            {
                var spent = 0;
                foreach (var points in _points)
                    spent += points;

                return spent;
            }
        }

        public int RemainingPoints => PointBudget - SpentPoints;

        public int Points(HeroStat stat) => _points[(int)stat];

        public bool CanIncrease(HeroStat stat) => RemainingPoints > 0 && Points(stat) < MaxPointsPerStat;

        public bool CanDecrease(HeroStat stat) => Points(stat) > 0;

        public void Increase(HeroStat stat)
        {
            if (!CanIncrease(stat))
                return;

            _points[(int)stat]++;
            Changed?.Invoke();
        }

        public void Decrease(HeroStat stat)
        {
            if (!CanDecrease(stat))
                return;

            _points[(int)stat]--;
            Changed?.Invoke();
        }

        public void Distribute(Random random)
        {
            Array.Clear(_points, 0, _points.Length);
            while (RemainingPoints > 0)
            {
                var stat = random.Next(_points.Length);
                if (_points[stat] < MaxPointsPerStat)
                    _points[stat]++;
            }

            Changed?.Invoke();
        }

        public void Clear()
        {
            Array.Clear(_points, 0, _points.Length);
            Changed?.Invoke();
        }
    }
}
