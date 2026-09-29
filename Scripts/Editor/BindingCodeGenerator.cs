using System;
using System.Collections.Generic;
using System.Reflection;

using MvvmUnity.Unity;
using UnityEditor;
using UnityEngine;

namespace MvvmUnity.Editor
{
    [InitializeOnLoad]
    public static class BindingCodeGenerator
    {
        static BindingCodeGenerator()
        {
            EditorApplication.delayCall += Rebuild;
        }

        [MenuItem("Tools/MVVM/Rebuild Generated Bindings")]
        public static void Rebuild()
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                EditorApplication.delayCall += Rebuild;
                return;
            }

            var scripts = ScriptIndex.Load();
            var errors = new List<string>();
            var expected = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var changed = false;

            ViewBindingAnalyzer.CollectDetachedAttributes(errors);
            foreach (var viewType in TypeCache.GetTypesWithAttribute<GenerateBindingsAttribute>())
            {
                if (!scripts.TryGetGeneratedPath(viewType, out var path))
                {
                    errors.Add($"MVVM source script was not found for {viewType.FullName}.");
                    continue;
                }

                expected.Add(path);
                var conventions = viewType.GetCustomAttribute<GenerateBindingsAttribute>().Conventions;
                if (ViewBindingAnalyzer.TryAnalyze(viewType, conventions, errors, out var plan))
                    changed |= ScriptIndex.WriteIfChanged(path, BindingSourceWriter.Write(plan));
            }

            changed |= scripts.DeleteGenerated(expected);
            foreach (var error in errors)
                Debug.LogError(error);

            if (changed)
                AssetDatabase.Refresh();
        }

        [MenuItem("Tools/MVVM/Clean Generated Bindings")]
        public static void Clean()
        {
            if (ScriptIndex.Load().DeleteGenerated(Array.Empty<string>()))
                AssetDatabase.Refresh();
        }
    }
}
