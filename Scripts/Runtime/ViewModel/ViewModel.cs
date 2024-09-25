using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace mvvm.unity.Core
{
    public class ViewModel : IViewModel
    {
        private readonly Dictionary<PropertyInfo, Property> _cachedPropertiesMap = new();
        private readonly Dictionary<MethodInfo, Command> _cachedCommandsMap = new();

        public IEnumerable<IProperty> GetProperties()
        {
            var type = GetType();

            foreach (var property in type.GetProperties())
            {
                if (_cachedPropertiesMap.TryGetValue(property, out Property cachedProperty))
                {
                    yield return cachedProperty;
                    continue;
                }

                var bindableProperty = property.GetCustomAttributes<BindablePropertyAttribute>().FirstOrDefault();

                if (bindableProperty != null)
                {
                    cachedProperty = new Property(this, property, bindableProperty.Key ?? property.Name);
                    _cachedPropertiesMap[property] = cachedProperty;
                    yield return cachedProperty;
                }
            }
        }

        public IEnumerable<ICommand> GetCommands()
        {
            var type = GetType();

            foreach (var method in type.GetMethods())
            {
                if (_cachedCommandsMap.TryGetValue(method, out var cachedCommand))
                {
                    yield return cachedCommand;
                    continue;
                }

                var bindableCommand = method.GetCustomAttributes<BindableCommandAttribute>().FirstOrDefault();

                if (bindableCommand != null)
                {
                    cachedCommand = new Command(this, method, bindableCommand.Key ?? method.Name);
                    _cachedCommandsMap[method] = cachedCommand;
                    yield return cachedCommand;
                }
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            foreach (var property in _cachedPropertiesMap.Values)
            {
                if (property.Name == propertyName)
                    property.OnChanged();
            }
        }
    }
}