using System;
using UnityEngine;

namespace MvvmUnity.Unity
{
    public sealed class BindingScopeHost : MonoBehaviour
    {
        private IDisposable _binding;

        public void Hold(IDisposable binding)
        {
            if (binding == null)
                throw new ArgumentNullException(nameof(binding));
            if (_binding != null)
                _binding.Dispose();
            _binding = binding;
        }

        private void OnDestroy()
        {
            if (_binding != null)
                _binding.Dispose();
            _binding = null;
        }
    }
}
