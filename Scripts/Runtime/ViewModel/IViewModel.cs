using System.Collections.Generic;

namespace mvvm.unity.Core
{
    public interface IViewModel
    {
        IEnumerable<ICommand> GetCommands();
        IEnumerable<IProperty> GetProperties();
    }
}