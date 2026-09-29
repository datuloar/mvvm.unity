using System.Collections.Generic;
using System.Linq;

using MvvmUnity.Core;
using MvvmUnity.Unity;
using TMPro;

namespace MvvmUnity.Samples.CharacterCreator
{
    public static class ChoiceBindingExtensions
    {
        public static void Choice(
            this BindingScope bindings,
            TMP_Dropdown target,
            IReadOnlyList<string> options,
            IObservableValue<int> selection)
        {
            target.ClearOptions();
            target.AddOptions(options.ToList());
            bindings.Dropdown(target, selection);
        }
    }
}
