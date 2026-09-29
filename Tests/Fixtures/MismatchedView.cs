using MvvmUnity.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace MvvmUnity.Tests.Fixtures
{
    public sealed class MismatchedView : MvvmView<IntentViewModel>
    {
        [SerializeField] private Text _submit;
    }
}
