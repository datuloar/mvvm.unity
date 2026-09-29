namespace MvvmUnity.Samples.CharacterCreator
{
    public sealed class HeroProfile
    {
        public HeroProfile(string name, HeroClass heroClass, int strength, int agility, int intellect, bool hardcore)
        {
            Name = name;
            HeroClass = heroClass;
            Strength = strength;
            Agility = agility;
            Intellect = intellect;
            Hardcore = hardcore;
        }

        public string Name { get; }

        public HeroClass HeroClass { get; }

        public int Strength { get; }

        public int Agility { get; }

        public int Intellect { get; }

        public bool Hardcore { get; }
    }
}
