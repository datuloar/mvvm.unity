using NUnit.Framework;

using Random = System.Random;

namespace MvvmUnity.Samples.CharacterCreator.Tests
{
    public sealed class HeroDraftTests
    {
        [Test]
        public void IncreaseRespectsPerStatCap()
        {
            var draft = new HeroDraft();

            for (var index = 0; index < HeroDraft.MaxPointsPerStat + 2; index++)
                draft.Increase(HeroStat.Strength);

            Assert.That(draft.Points(HeroStat.Strength), Is.EqualTo(HeroDraft.MaxPointsPerStat));
            Assert.That(draft.CanIncrease(HeroStat.Strength), Is.False);
        }

        [Test]
        public void IncreaseRespectsBudget()
        {
            var draft = new HeroDraft();

            for (var index = 0; index < HeroDraft.PointBudget; index++)
                draft.Increase((HeroStat)(index % 3));

            Assert.That(draft.RemainingPoints, Is.Zero);
            Assert.That(draft.CanIncrease(HeroStat.Intellect), Is.False);
        }

        [Test]
        public void DistributeSpendsTheWholeBudgetWithinCaps()
        {
            var draft = new HeroDraft();

            draft.Distribute(new Random(42));

            Assert.That(draft.RemainingPoints, Is.Zero);
            Assert.That(draft.Points(HeroStat.Strength), Is.InRange(0, HeroDraft.MaxPointsPerStat));
            Assert.That(draft.Points(HeroStat.Agility), Is.InRange(0, HeroDraft.MaxPointsPerStat));
            Assert.That(draft.Points(HeroStat.Intellect), Is.InRange(0, HeroDraft.MaxPointsPerStat));
        }

        [Test]
        public void ChangesAreAnnounced()
        {
            var draft = new HeroDraft();
            var notifications = 0;
            draft.Changed += () => notifications++;

            draft.Increase(HeroStat.Agility);
            draft.Decrease(HeroStat.Agility);
            draft.Decrease(HeroStat.Agility);

            Assert.That(notifications, Is.EqualTo(2));
        }
    }
}
