using System.IO;
using System.Linq;
using System.Reflection;
using MvvmUnity.Editor;
using MvvmUnity.Tests.Generation;
using NUnit.Framework;
using UnityEditor;

namespace MvvmUnity.Tests
{
    public sealed class GeneratedBindingTests
    {
        [Test]
        public void IntentBinding_IsGeneratedAsViewOverride()
        {
            var method = typeof(IntentView).GetMethod(
                "Bind",
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);

            Assert.That(method, Is.Not.Null);
        }

        [Test]
        public void Conventions_GenerateMatchingFieldAndInferredObserverSource()
        {
            BindingCodeGenerator.Rebuild();
            var path = AssetDatabase
                .FindAssets("IntentView t:MonoScript")
                .Select(AssetDatabase.GUIDToAssetPath)
                .First(candidate => candidate.EndsWith("IntentView.Bindings.g.cs"));
            var source = File.ReadAllText(path);

            StringAssert.Contains("bindings.Click(_submit, viewModel.Submit);", source);
            StringAssert.Contains("bindings.Observe(viewModel.Status, Render);", source);
            StringAssert.DoesNotContain("bindings.Text(_status", source);
        }
    }
}
