using UnityEngine;

using Random = System.Random;

namespace MvvmUnity.Samples.CharacterCreator
{
    public sealed class CharacterCreatorEntryPoint : MonoBehaviour
    {
        [SerializeField] private CharacterCreatorView _view;

        private void Start()
        {
            var viewModel = new CharacterCreatorViewModel(new HeroDraft(), new HeroRoster(), new Random());
            _view.SetViewModel(viewModel, true);
        }
    }
}
