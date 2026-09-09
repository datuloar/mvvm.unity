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
                throw new System.ArgumentNullException(nameof(viewModel));
            Unbind();
            if (_ownsViewModel && _viewModel != null)
                _viewModel.Dispose();
            _viewModel = viewModel;
            _ownsViewModel = ownsViewModel;
            if (isActiveAndEnabled)
                Bind();
        }

        protected virtual void Bind(BindingScope bindings, TViewModel viewModel)
        {
        }

        protected virtual void OnEnable()
        {
            if (_viewModel != null)
                Bind();
        }

        protected virtual void OnDisable()
        {
            Unbind();
        }

        protected virtual void OnDestroy()
        {
            Unbind();
            if (_ownsViewModel && _viewModel != null)
                _viewModel.Dispose();
            _viewModel = null;
            _ownsViewModel = false;
        }

        private void Bind()
        {
            Unbind();
            _bindings = new BindingScope();
            Bind(_bindings, _viewModel);
        }

        private void Unbind()
        {
            if (_bindings == null)
                return;
            _bindings.Dispose();
            _bindings = null;
        }
    }
}
