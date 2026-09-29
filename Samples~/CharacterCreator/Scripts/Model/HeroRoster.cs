using System;
using System.Collections.Generic;

namespace MvvmUnity.Samples.CharacterCreator
{
    public sealed class HeroRoster : IHeroRoster
    {
        private readonly List<HeroProfile> _heroes = new List<HeroProfile>();

        public int Count => _heroes.Count;

        public void Add(HeroProfile hero) => _heroes.Add(hero ?? throw new ArgumentNullException(nameof(hero)));
    }
}
