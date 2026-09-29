using System;
using System.Linq;

namespace MvvmUnity.Editor
{
    internal static class TypeNames
    {
        public static string Display(Type type) => Compose(type, Display);

        public static string Qualified(Type type)
        {
            var owner = type.IsNested
                ? Qualified(type.DeclaringType) + "."
                : "global::" + (string.IsNullOrEmpty(type.Namespace) ? string.Empty : type.Namespace + ".");
            return owner + Compose(type, Qualified);
        }

        private static string Compose(Type type, Func<Type, string> argumentName)
        {
            var arity = type.Name.IndexOf('`');
            if (!type.IsGenericType || arity < 0)
                return type.Name;

            var arguments = type.GetGenericArguments().Select(argumentName);
            return type.Name.Substring(0, arity) + "<" + string.Join(", ", arguments) + ">";
        }
    }
}
