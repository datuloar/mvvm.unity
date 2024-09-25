using System.Threading.Tasks;

namespace mvvm.unity.Core
{
    public interface IWindow
    {
        Task HideAsync();
        Task ShowAsync();
    }
}