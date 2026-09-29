using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using UnityEditor;

namespace MvvmUnity.Editor
{
    internal sealed class ScriptIndex
    {
        private const string GeneratedFolder = "Generated";
        private const string GeneratedSuffix = ".Bindings.g.cs";

        private readonly Dictionary<Type, string> _sources;
        private readonly List<string> _generated;

        private ScriptIndex(Dictionary<Type, string> sources, List<string> generated)
        {
            _sources = sources;
            _generated = generated;
        }

        public static ScriptIndex Load()
        {
            var sources = new Dictionary<Type, string>();
            var generated = new List<string>();
            foreach (var guid in AssetDatabase.FindAssets("t:MonoScript"))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                if (path.EndsWith(GeneratedSuffix, StringComparison.Ordinal))
                {
                    generated.Add(path);
                    continue;
                }

                var script = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
                var type = script == null ? null : script.GetClass();
                if (type != null && !sources.ContainsKey(type))
                    sources.Add(type, path);
            }

            return new ScriptIndex(sources, generated);
        }

        public static bool WriteIfChanged(string path, string source)
        {
            if (File.Exists(path) && File.ReadAllText(path) == source)
                return false;

            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllText(path, source, new UTF8Encoding(false));
            return true;
        }

        public bool TryGetGeneratedPath(Type viewType, out string path)
        {
            path = null;
            if (!_sources.TryGetValue(viewType, out var sourcePath))
                return false;

            var folder = Path.Combine(Path.GetDirectoryName(sourcePath), GeneratedFolder);
            path = Path.Combine(folder, viewType.FullName + GeneratedSuffix).Replace('\\', '/');
            return true;
        }

        public bool DeleteGenerated(ICollection<string> keep)
        {
            var deleted = false;
            foreach (var path in _generated)
            {
                if (!keep.Contains(path) && IsOwned(path))
                    deleted |= AssetDatabase.DeleteAsset(path);
            }

            return deleted;
        }

        private static bool IsOwned(string path) =>
            File.ReadAllText(path).Contains(BindingSourceWriter.OwnershipMarker);
    }
}
