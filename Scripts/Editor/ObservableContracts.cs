using System;

using MvvmUnity.Core;

namespace MvvmUnity.Editor
{
    internal static class ObservableContracts
    {
        public static bool Publishes(Type source, Type value) =>
            Implements(source, typeof(IReadOnlyObservableValue<>), value);

        public static bool Accepts(Type source, Type value) =>
            Implements(source, typeof(IObservableValue<>), value);

        private static bool Implements(Type source, Type contract, Type value)
        {
            if (IsClosedContract(source, contract, value))
                return true;

            foreach (var candidate in source.GetInterfaces())
            {
                if (IsClosedContract(candidate, contract, value))
                    return true;
            }

            return false;
        }

        private static bool IsClosedContract(Type candidate, Type contract, Type value) =>
            candidate.IsGenericType &&
            candidate.GetGenericTypeDefinition() == contract &&
            candidate.GetGenericArguments()[0] == value;
    }
}
