using System.IO;
using System.Linq;
using System.Reflection;
using MvvmUnity.Editor;
using MvvmUnity.Tests.Generation;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

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

        [Test]
        public void Generation_EmitsBindCustomHookForManualBindings()
        {
            BindingCodeGenerator.Rebuild();
            var path = AssetDatabase
                .FindAssets("IntentView t:MonoScript")
                .Select(AssetDatabase.GUIDToAssetPath)
                .First(candidate => candidate.EndsWith("IntentView.Bindings.g.cs"));
            var source = File.ReadAllText(path);

            StringAssert.Contains("BindCustom(bindings, viewModel);", source);
            StringAssert.Contains(
                "partial void BindCustom(global::MvvmUnity.Unity.BindingScope bindings, global::MvvmUnity.Tests.Generation.IntentViewModel viewModel);",
                source);
        }

        [Test]
        public void BindCustom_RunsAlongsideGeneratedConventionBindings()
        {
            var viewObject = new GameObject("IntentView", typeof(RectTransform));
            var submitObject = new GameObject("Submit", typeof(RectTransform), typeof(Button));
            var statusObject = new GameObject("Status", typeof(RectTransform), typeof(Text));
            try
            {
                var view = viewObject.AddComponent<IntentView>();
                typeof(IntentView)
                    .GetField("_submit", BindingFlags.Instance | BindingFlags.NonPublic)
                    .SetValue(view, submitObject.GetComponent<Button>());
                typeof(IntentView)
                    .GetField("_status", BindingFlags.Instance | BindingFlags.NonPublic)
                    .SetValue(view, statusObject.GetComponent<Text>());

                view.SetViewModel(new IntentViewModel());

                Assert.IsTrue(view.CustomBindCalled);
            }
            finally
            {
                Object.DestroyImmediate(viewObject);
                Object.DestroyImmediate(submitObject);
                Object.DestroyImmediate(statusObject);
            }
        }
    }
}
