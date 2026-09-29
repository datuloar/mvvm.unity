namespace MvvmUnity.Samples.CharacterCreator
{
    public interface IHeroRoster
    {
        int Count { get; }

        void Add(HeroProfile hero);
    }
}
