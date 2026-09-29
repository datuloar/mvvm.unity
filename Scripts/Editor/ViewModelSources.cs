using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MvvmUnity.Editor
{
    internal static class ViewModelSources
    {
        private const BindingFlags PublicInstance = BindingFlags.Instance | BindingFlags.Public;

        public static Type SourceType(Type viewModelType, string name)
        {
            var property = viewModelType
                .GetProperties(PublicInstance)
                .FirstOrDefault(candidate => candidate.Name == name && candidate.GetMethod != null);
            if (property != null)
                return property.PropertyType;

            var field = viewModelType.GetField(name, PublicInstance);
            if (field != null)
                return field.FieldType;

            return IsIntent(viewModelType, name) ? typeof(Action) : null;
        }

        public static IReadOnlyList<string> Publishing(Type viewModelType, Type valueType)
        {
            var properties = viewModelType
                .GetProperties(PublicInstance)
                .Where(property => property.GetMethod != null && ObservableContracts.Publishes(property.PropertyType, valueType))
                .Select(property => property.Name);
            var fields = viewModelType
                .GetFields(PublicInstance)
                .Where(field => ObservableContracts.Publishes(field.FieldType, valueType))
                .Select(field => field.Name);
            return properties.Concat(fields).Distinct().ToArray();
        }

        private static bool IsIntent(Type viewModelType, string name)
        {
            var methods = viewModelType
                .GetMethods(PublicInstance)
                .Where(method => method.Name == name)
                .ToArray();
            return methods.Length == 1 &&
                   methods[0].ReturnType == typeof(void) &&
                   !methods[0].IsGenericMethod &&
                   methods[0].GetParameters().Length == 0;
        }
    }
}
