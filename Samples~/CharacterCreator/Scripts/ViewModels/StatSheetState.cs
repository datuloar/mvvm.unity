using System.Collections.Generic;

namespace MvvmUnity.Samples.CharacterCreator
{
    public sealed class StatSheetState
    {
        private static readonly HeroStat[] Stats = { HeroStat.Strength, HeroStat.Agility, HeroStat.Intellect };

        private StatSheetState(IReadOnlyList<StatRowState> rows)
        {
            Rows = rows;
        }

        public IReadOnlyList<StatRowState> Rows { get; }

        public static StatSheetState From(HeroDraft draft)
        {
            var rows = new StatRowState[Stats.Length];
            for (var index = 0; index < Stats.Length; index++)
            {
                var stat = Stats[index];
                var points = draft.Points(stat);
                rows[index] = new StatRowState(
                    stat,
                    stat.ToString().ToUpperInvariant(),
                    points.ToString(),
                    (float)points / HeroDraft.MaxPointsPerStat);
            }

            return new StatSheetState(rows);
        }
    }
}
