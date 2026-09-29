# Testing and performance

## ViewModel tests

ViewModels live in plain C# and run in EditMode without a scene. Test the initial state, every intent, command availability and disposal:

```csharp
[Test]
public void CreateRequiresANameAndTheWholeBudget()
{
    _viewModel.Name.Value = "Aria";
    Assert.That(_viewModel.Create.CanExecute, Is.False);

    SpendBudget();

    Assert.That(_viewModel.Create.CanExecute, Is.True);
}
```

The Character Creator sample contains a complete ViewModel test suite.

## Binding tests

For a custom `BindingScope` extension, create the smallest widget, bind it, change both sides, dispose the scope and assert that nothing crosses the boundary afterwards. `BindingExtensionsTests` shows the pattern for every built-in target.

## Package validation

- `mvvm.unity.tests` covers observables, commands, ViewModel lifetime, scopes, every built-in binding, View ownership, the inference rules, the analyzer and generated output.
- `CommittedSourceMatchesGeneratorOutput` fails when a committed generated file differs from what the generator produces.
- The suite is run on Unity 2022.3 LTS and Unity 6.3 together with the sample tests.

## Runtime cost

- A binding is a direct delegate subscription created once per enable.
- Publishing a value is an equality check plus one event invocation; an unchanged assignment allocates nothing.
- Generated code performs direct typed calls. There is no reflection, expression compilation, polling, `Update` loop, `Task` or hidden scheduler at runtime.

Prefer event-driven updates and publish snapshots at user-action frequency. Snapshot DTOs and rebuilt rows allocate, so keep them out of per-frame loops and profile before introducing pooling.
