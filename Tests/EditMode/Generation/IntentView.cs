using MvvmUnity.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace MvvmUnity.Tests.Generation
{
    [GenerateBindings]
    public sealed partial class IntentView : MvvmView<IntentViewModel>
    {
        [SerializeField] private Button _submit;
        [IgnoreBinding]
        [SerializeField] private Text _status;

        public string LastStatus { get; private set; } = string.Empty;

        [Observe]
        private void Render(string status)
        {
            LastStatus = status;
        }
    }
}
