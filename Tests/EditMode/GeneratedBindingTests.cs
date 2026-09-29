using System.Collections.Generic;
using System.IO;
using System.Linq;

using MvvmUnity.Editor;
using MvvmUnity.Tests.Fixtures;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace MvvmUnity.Tests
{
    public sealed class GeneratedBindingTests
    {
        private GameObject _root;
        private IntentView _view;
        private Button _submit;

        [SetUp]
        public void SetUp()
        {
            _root = new GameObject("IntentView", typeof(RectTransform));
            _submit = new GameObject("Submit", typeof(RectTransform), typeof(Button)).GetComponent<Button>();
            _submit.transform.SetParent(_root.transform);
            _view = _root.AddComponent<IntentView>();
            var serialized = new SerializedObject(_view);
            serialized.FindProperty("_submit").objectReferenceValue = _submit;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        [TearDown]
        public void TearDown() => Object.DestroyImmediate(_root);

        [Test]
        public void CommittedSourceMatchesGeneratorOutput()
        {
            Assert.That(ViewBindingAnalyzer.TryAnalyze(typeof(IntentView), true, new List<string>(), out var plan), Is.True);

            var path = AssetDatabase
                .FindAssets("IntentView t:MonoScript")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Single(candidate => candidate.EndsWith("IntentView.Bindings.g.cs"));

            Assert.That(File.ReadAllText(path), Is.EqualTo(BindingSourceWriter.Write(plan)));
        }

        [Test]
        public void GeneratedBindingsWireIntentsObserversAndCustomHook()
        {
            var viewModel = new IntentViewModel();

            _view.SetViewModel(viewModel);
            _submit.onClick.Invoke();

            Assert.That(_view.RenderedStatus, Is.EqualTo("Submitted 1"));
            Assert.That(_view.RenderedAttempts, Is.EqualTo(1));
            Assert.That(_view.CustomBindings, Is.EqualTo(1));
        }

        [Test]
        public void ScopedObserverReceivesFreshScopePerRender()
        {
            _view.SetViewModel(new IntentViewModel());
            var first = _view.AttemptsScope;

            _submit.onClick.Invoke();

            Assert.That(_view.AttemptsScope, Is.Not.SameAs(first));
        }
    }
}
