using System;
using System.ComponentModel;

using MvvmUnity.Core;
using UnityEngine;

namespace MvvmUnity.Unity
{
    public abstract class MvvmView<TViewModel> : MonoBehaviour
        where TViewModel : class, IViewModel
    {
        private TViewModel _viewModel;
        private BindingScope _bindings;
        private bool _ownsViewModel;

        protected TViewModel ViewModel => _viewModel;

        public void SetViewModel(TViewModel viewModel, bool ownsViewModel = false)
        {
            if (viewModel == null)
                throw new ArgumentNullException(nameof(viewModel));

            Unbind();
            if (!ReferenceEquals(_viewModel, viewModel))
                ReleaseViewModel();

            _viewModel = viewModel;
            _ownsViewModel = ownsViewModel;
            if (isActiveAndEnabled)
                Rebind();
        }

        protected virtual void Bind(BindingScope bindings, TViewModel viewModel)
        {
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        protected virtual void BindGenerated(BindingScope bindings, TViewModel viewModel)
        {
        }

        protected virtual void OnEnable()
        {
            if (_viewModel != null)
                Rebind();
        }

        protected virtual void OnDisable() => Unbind();

        protected virtual void OnDestroy()
        {
            Unbind();
            ReleaseViewModel();
            _viewModel = null;
        }

        private void Rebind()
        {
            Unbind();
            _bindings = new BindingScope();
            BindGenerated(_bindings, _viewModel);
            Bind(_bindings, _viewModel);
        }

        private void Unbind()
        {
            _bindings?.Dispose();
            _bindings = null;
        }

        private void ReleaseViewModel()
        {
            if (_ownsViewModel)
                _viewModel?.Dispose();

            _ownsViewModel = false;
        }
    }
}
