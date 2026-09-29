using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using MvvmUnity.Unity;
using UnityEditor;
using UnityEngine;

namespace MvvmUnity.Editor
{
    internal static class ViewBindingAnalyzer
    {
        private const BindingFlags DeclaredMembers =
            BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

        public static bool TryAnalyze(
            Type viewType,
            bool conventions,
            ICollection<string> errors,
            out ViewBindingPlan plan)
        {
            plan = null;
            var knownErrors = errors.Count;
            var viewModelType = ViewModelTypeOf(viewType);
            if (viewType.IsNested || viewType.IsGenericType)
                errors.Add(ViewError(viewType, "a generated View must be a top-level non-generic class"));
            if (viewModelType == null)
            {
                errors.Add(ViewError(viewType, "a generated View must inherit MvvmView<TViewModel>"));
                return false;
            }

            var fields = FieldBindings(viewType, viewModelType, conventions, errors);
            var observers = ObserverBindings(viewType, viewModelType, errors);
            if (errors.Count == knownErrors && fields.Count == 0 && observers.Count == 0)
                errors.Add(ViewError(viewType, "nothing to generate; remove [GenerateBindings] and override Bind manually"));
            if (errors.Count != knownErrors)
                return false;

            plan = new ViewBindingPlan(viewType, viewModelType, fields, observers);
            return true;
        }

        public static void CollectDetachedAttributes(ICollection<string> errors)
        {
            var members = TypeCache.GetFieldsWithAttribute<BindAttribute>().Cast<MemberInfo>()
                .Concat(TypeCache.GetFieldsWithAttribute<IgnoreBindingAttribute>())
                .Concat(TypeCache.GetMethodsWithAttribute<ObserveAttribute>());
            foreach (var member in members)
            {
                if (!member.DeclaringType.IsDefined(typeof(GenerateBindingsAttribute), false))
                    errors.Add(MemberError(member, "binding attributes require [GenerateBindings] on the declaring View"));
            }
        }

        private static Type ViewModelTypeOf(Type viewType)
        {
            for (var current = viewType; current != null; current = current.BaseType)
            {
                if (current.IsGenericType && current.GetGenericTypeDefinition() == typeof(MvvmView<>))
                    return current.GetGenericArguments()[0];
            }

            return null;
        }

        private static List<FieldBinding> FieldBindings(
            Type viewType,
            Type viewModelType,
            bool conventions,
            ICollection<string> errors)
        {
            var bindings = new List<FieldBinding>();
            foreach (var field in viewType.GetFields(DeclaredMembers).OrderBy(field => field.MetadataToken))
            {
                var declared = field.GetCustomAttributes<BindAttribute>().ToArray();
                if (field.IsStatic && declared.Length > 0)
                    errors.Add(MemberError(field, "[Bind] requires an instance field"));
                else if (declared.Length > 0)
                    ResolveDeclared(field, viewModelType, declared, bindings, errors);
                else if (conventions && IsConventionCandidate(field, viewModelType, out var source))
                    Resolve(field, viewModelType, source, BindingTarget.Auto, bindings, errors);
            }

            return bindings;
        }

        private static void ResolveDeclared(
            FieldInfo field,
            Type viewModelType,
            IEnumerable<BindAttribute> declared,
            ICollection<FieldBinding> bindings,
            ICollection<string> errors)
        {
            foreach (var bind in declared)
                Resolve(field, viewModelType, bind.Source, bind.Target, bindings, errors);
        }

        private static bool IsConventionCandidate(FieldInfo field, Type viewModelType, out string source)
        {
            source = ConventionName(field.Name);
            return !field.IsStatic &&
                   field.IsDefined(typeof(SerializeField), false) &&
                   !field.IsDefined(typeof(IgnoreBindingAttribute), false) &&
                   source.Length > 0 &&
                   ViewModelSources.SourceType(viewModelType, source) != null;
        }

        private static string ConventionName(string fieldName)
        {
            var name = fieldName.TrimStart('_');
            return name.Length == 0 ? string.Empty : char.ToUpperInvariant(name[0]) + name.Substring(1);
        }

        private static void Resolve(
            FieldInfo field,
            Type viewModelType,
            string source,
            BindingTarget target,
            ICollection<FieldBinding> bindings,
            ICollection<string> errors)
        {
            var sourceType = ViewModelSources.SourceType(viewModelType, source);
            if (sourceType == null)
            {
                errors.Add(MemberError(field, $"public source '{source}' was not found on {TypeNames.Display(viewModelType)}"));
                return;
            }

            if (target == BindingTarget.Auto && !BindingRules.TryInfer(field.FieldType, sourceType, out target))
            {
                errors.Add(MemberError(field, $"cannot bind {TypeNames.Display(field.FieldType)} to '{source}' ({TypeNames.Display(sourceType)}); use [IgnoreBinding] or a matching source"));
                return;
            }

            if (!BindingRules.Supports(target, field.FieldType, sourceType))
            {
                errors.Add(MemberError(field, $"{TypeNames.Display(field.FieldType)} cannot bind '{source}' ({TypeNames.Display(sourceType)}) as {target}"));
                return;
            }

            bindings.Add(new FieldBinding(field.Name, source, target));
        }

        private static List<ObserverBinding> ObserverBindings(Type viewType, Type viewModelType, ICollection<string> errors)
        {
            var bindings = new List<ObserverBinding>();
            foreach (var method in viewType.GetMethods(DeclaredMembers).OrderBy(method => method.MetadataToken))
            {
                foreach (var observe in method.GetCustomAttributes<ObserveAttribute>())
                {
                    if (!TryObservedType(method, out var observedType))
                        errors.Add(MemberError(method, "an observer must be an instance void method taking (TState) or (TState, BindingScope)"));
                    else if (TryObservedSource(viewModelType, observe.Source, observedType, out var source, out var error))
                        bindings.Add(new ObserverBinding(method.Name, source));
                    else
                        errors.Add(MemberError(method, error));
                }
            }

            return bindings;
        }

        private static bool TryObservedType(MethodInfo method, out Type observedType)
        {
            var parameters = method.GetParameters();
            observedType = parameters.Length > 0 ? parameters[0].ParameterType : null;
            return !method.IsStatic &&
                   !method.IsGenericMethod &&
                   method.ReturnType == typeof(void) &&
                   (parameters.Length == 1 || (parameters.Length == 2 && parameters[1].ParameterType == typeof(BindingScope)));
        }

        private static bool TryObservedSource(
            Type viewModelType,
            string requested,
            Type observedType,
            out string source,
            out string error)
        {
            source = requested;
            error = null;
            if (requested.Length > 0)
            {
                var sourceType = ViewModelSources.SourceType(viewModelType, requested);
                if (sourceType == null)
                    error = $"public source '{requested}' was not found on {TypeNames.Display(viewModelType)}";
                else if (!ObservableContracts.Publishes(sourceType, observedType))
                    error = $"'{requested}' does not publish {TypeNames.Display(observedType)}";
                return error == null;
            }

            var candidates = ViewModelSources.Publishing(viewModelType, observedType);
            if (candidates.Count == 1)
            {
                source = candidates[0];
                return true;
            }

            error = candidates.Count == 0
                ? $"no public observable publishes {TypeNames.Display(observedType)}"
                : $"several observables publish {TypeNames.Display(observedType)} ({string.Join(", ", candidates)}); specify [Observe(nameof(...))]";
            return false;
        }

        private static string ViewError(Type type, string message) => $"MVVM {type.FullName}: {message}.";

        private static string MemberError(MemberInfo member, string message) =>
            $"MVVM {member.DeclaringType.FullName}.{member.Name}: {message}.";
    }
}
