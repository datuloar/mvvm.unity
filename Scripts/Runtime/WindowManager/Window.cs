using System.Threading.Tasks;

namespace mvvm.unity.Core
{
    public class Window : View, IWindow
    {
        public async Task ShowAsync()
        {
            gameObject.SetActive(true);
            await OnShowAsync();
        }

        public async Task HideAsync()
        {
            await OnHideAsync();
            gameObject.SetActive(false);
        }

        protected virtual async Task OnShowAsync() { }
        protected virtual async Task OnHideAsync() { }
    }
}