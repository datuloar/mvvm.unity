# Getting started

## Assemblies

- `mvvm.unity.core`: plain C# observables, commands, ViewModel lifetime, and snapshot ViewModels.
- `mvvm.unity`: Unity Views and binding adapters for uGUI/TextMeshPro.
- `mvvm.unity.editor`: validation and editor-time C# generation.

Reference `mvvm.unity.core` from model/application assemblies. Reference `mvvm.unity` only from presentation assemblies. The package does not require a DI container.

## ViewModel

Use `ObservableValue<T>` for mutable state. Expose `IReadOnlyObservableValue<T>` unless the View must write through a two-way binding.

```csharp
public sealed class LoginViewModel : ViewModel
{
    public LoginViewModel(ILogin login)
    {
        Name = new ObservableValue<string>("");
        Submit = new RelayCommand(() => login.Enter(Name.Value));
    }

    public IObservableValue<string> Name { get; }
    public ICommand Submit { get; }
}
```

ViewModels do not reference `GameObject`, uGUI, scenes, or `MonoBehaviour`. Inject models/services through constructors.

## View

```csharp
[GenerateBindings]
public sealed partial class LoginView : MvvmView<LoginViewModel>
{
    [SerializeField] private TMP_InputField _name;

    [SerializeField] private Button _submit;
}
```

The `_name` and `_submit` conventions generate direct bindings to `Name` and `Submit`. Use `[Bind(nameof(...))]` only when the names intentionally differ.

Run `Tools > MVVM > Rebuild Generated Bindings`. The generated partial is placed in `Generated/` beside the View and must be committed with the source.

## Composition

Create ViewModels in the scene composition root and call `SetViewModel` only after the View's serialized/runtime widgets are ready. Pass `ownsViewModel: true` only when the View itself owns that instance; DI-owned ViewModels are normally disposed by their scope.
