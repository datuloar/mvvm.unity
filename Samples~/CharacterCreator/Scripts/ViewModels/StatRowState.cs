namespace MvvmUnity.Samples.CharacterCreator
{
    public sealed class StatRowState
    {
        public StatRowState(HeroStat stat, string label, string points, float meter)
        {
            Stat = stat;
            Label = label;
            Points = points;
            Meter = meter;
        }

        public HeroStat Stat { get; }

        public string Label { get; }

        public string Points { get; }

        public float Meter { get; }
    }
}
