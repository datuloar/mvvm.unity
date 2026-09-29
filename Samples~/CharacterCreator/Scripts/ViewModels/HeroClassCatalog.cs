using System.Collections.Generic;

using UnityEngine;

namespace MvvmUnity.Samples.CharacterCreator
{
    public static class HeroClassCatalog
    {
        public static readonly IReadOnlyList<string> Names = new[] { "Warrior", "Ranger", "Mage" };

        private static readonly string[] Descriptions =
        {
            "Holds the front line. Strength turns every blow into momentum.",
            "Strikes from the shadows. Agility decides who shoots first.",
            "Bends the arcane. Intellect fuels spells that change the battle."
        };

        private static readonly Color[] Colors =
        {
            new Color(0.91f, 0.36f, 0.31f),
            new Color(0.33f, 0.78f, 0.47f),
            new Color(0.42f, 0.53f, 0.98f)
        };

        public static string Name(HeroClass heroClass) => Names[(int)heroClass];

        public static string Description(HeroClass heroClass) => Descriptions[(int)heroClass];

        public static Color Tint(HeroClass heroClass) => Colors[(int)heroClass];
    }
}
