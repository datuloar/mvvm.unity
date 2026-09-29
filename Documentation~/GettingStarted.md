# Getting started

## Install

Add the Git package to `Packages/manifest.json`:

```json
{
  "dependencies": {
    "com.mvvm.unity": "https://github.com/datuloar/mvvm.unity.git"
  }
}
```

The package needs uGUI and TextMeshPro. Unity 6 ships both in `com.unity.ugui`; Unity 2022.3 templates include `com.unity.textmeshpro`. Import TMP Essential Resources once if the project has none.

## Assemblies

| Assembly | Contents | Reference it from |
| --- | --- | --- |
| `mvvm.unity.core` | Observables, commands, `ViewModel`, `StateViewModel<T>`; no engine references | Models, services, ViewModels |
| `mvvm.unity` | `MvvmView<T>`, `BindingScope`, uGUI/TextMeshPro bindings, attributes | Views |
| `mvvm.unity.editor` | Binding generator and validation | Nothing; Editor only |

Projects without assembly definitions can use the runtime assemblies directly.

## ViewModel

Keep mutable state private and expose read-only observables. Expose a writable observable only for real two-way input:

```csharp
using MvvmUnity.Core;

public sealed class LoginViewModel : ViewModel
{
    private readonly ObservableValue<string> _name = new ObservableValue<string>(string.Empty);

    public LoginViewModel(ILogin login)
    {
        Submit = Own(new RelayCommand(() => login.Enter(_name.Value), () => _name.Value.Length > 0).RefreshOn(_name));
    }

    public IObservableValue<string> Name => _name;

    public ICommand Submit { get; }
}
```

`Own` ties a subscription or command to the ViewModel lifetime, so `Dispose` releases it without an `OnDispose` override. Override `OnDispose` only for plain C# events of external models.

ViewModels never reference `GameObject`, uGUI widgets, scenes or `MonoBehaviour`, which keeps them testable without Play Mode.

## View

```csharp
using MvvmUnity.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[GenerateBindings]
public sealed partial class LoginView : MvvmView<LoginViewModel>
{
    [SerializeField] private TMP_InputField _name;
    [SerializeField] private Button _submit;
}
```

Bindings are generated automatically after every script reload into `Generated/` beside the View. `Tools > MVVM > Rebuild Generated Bindings` regenerates on demand. Commit the generated file with the View.

## Composition

Create the ViewModel in the scene's composition root and pass it to the View:

```csharp
view.SetViewModel(new LoginViewModel(login), ownsViewModel: true);
```

The View binds when it is enabled and releases every binding when it is disabled or destroyed. With `ownsViewModel: true` it also disposes the ViewModel when it is replaced or the View is destroyed; leave it `false` when a DI container owns the instance.

Continue with [Bindings](Bindings.md) and the Character Creator sample.
