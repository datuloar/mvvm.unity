# Bindings

## Convention field bindings

On a `[GenerateBindings]` View, a private `[SerializeField]` widget binds to the public ViewModel member whose name equals the field name without leading underscores and with the first letter capitalized:

```csharp
[SerializeField] private TMP_Text _title;
[SerializeField] private Button _submit;
```

`_title` binds to `Title`, `_submit` binds to `Submit`. A field without a matching public member stays an ordinary View concern. A matching name with an incompatible widget is a generation error, so a typo never silently produces a dead widget. Mark a matching field `[IgnoreBinding]` when the View owns it manually, or use `[GenerateBindings(Conventions = false)]` to opt a whole View out of conventions.

## Supported targets

The generator infers the target from the widget and source types. Rules are evaluated in this order, and the first match wins:

| View field | ViewModel source | Target |
| --- | --- | --- |
| `Button` | `ICommand` | `Command` |
| `Button` | public parameterless `void` method | `Click` |
| `Text` / `TMP_Text` | `IReadOnlyObservableValue<string>` | `Text` |
| `Image` | `IReadOnlyObservableValue<Sprite>` | `Sprite` |
| `Image` | `IReadOnlyObservableValue<float>` | `Fill` |
| any `Graphic` | `IReadOnlyObservableValue<Color>` | `Color` |
| `GameObject` | `IReadOnlyObservableValue<bool>` | `Active` |
| `CanvasGroup` | `IReadOnlyObservableValue<bool>` | `Visible` |
| `Slider` | `IObservableValue<float>` | `Slider` |
| `Scrollbar` | `IObservableValue<float>` | `Scrollbar` |
| `Toggle` | `IObservableValue<bool>` | `Toggle` |
| `InputField` / `TMP_InputField` | `IObservableValue<string>` | `Input` |
| `Dropdown` / `TMP_Dropdown` | `IObservableValue<int>` | `Dropdown` |
| any `Selectable` | `IReadOnlyObservableValue<bool>` | `Interactable` |

Two-way targets write into the widget through `Set*WithoutNotify`, so a ViewModel update never echoes back as a user edit.

## Aliases and multiple bindings

`[Bind(nameof(...))]` binds a field whose name differs from the source. Repeat the attribute to bind one widget to several sources, and pass a `BindingTarget` when an explicit target reads better than inference:

```csharp
[Bind(nameof(CharacterCreatorViewModel.Randomize))]
[Bind(nameof(CharacterCreatorViewModel.CanRandomize))]
[SerializeField] private Button _randomize;

[Bind(nameof(CharacterCreatorViewModel.ConfirmationVisible))]
[SerializeField] private CanvasGroup _confirmationPanel;
```

An explicit `[Bind]` replaces the convention for that field. Binding attributes on a type without `[GenerateBindings]` are reported as errors instead of being ignored.

## Commands

Bind a public parameterless ViewModel method when the action is always available. Use `ICommand` when the button must reflect availability. `RefreshOn` re-evaluates `CanExecute` whenever an observable changes:

```csharp
Create = Own(new RelayCommand(CreateHero, CanCreate).RefreshOn(Name));
```

`ICommand<T>` carries a stable argument for repeated controls such as list rows:

```csharp
bindings.Command(_increase, viewModel.Increase, row.Stat);
```

The binding scope owns both the click listener and the `CanExecuteChanged` subscription.

## Snapshot observation

Annotate a private instance `void` method with `[Observe]`. The generator subscribes it to the only public `IReadOnlyObservableValue<T>` publishing the method's first parameter type:

```csharp
[Observe]
private void Render(MapScreenState state)
{
    _nodes.Paint(state.Nodes);
}
```

When several sources publish that type, name one with `[Observe(nameof(MapViewModel.State))]`; generation lists every candidate instead of choosing silently.

## Dynamic rows

Add a `BindingScope` parameter when a render creates bindings of its own. Each emission receives a fresh scope and the previous one is disposed first, so rebuilt rows never leak listeners:

```csharp
[Observe]
private void Render(StatSheetState sheet, BindingScope bindings)
{
    for (var index = 0; index < sheet.Rows.Count; index++)
        RowAt(index).Render(sheet.Rows[index], ViewModel.Increase, ViewModel.Decrease, bindings);
}
```

The same behavior is available in hand-written code through `bindings.Observe(source, (state, scope) => ...)`.

## Custom bindings

Override `Bind` for anything the conventions do not cover. It runs after the generated bindings in the same scope, on Views with or without `[GenerateBindings]`:

```csharp
protected override void Bind(BindingScope bindings, CharacterCreatorViewModel viewModel) =>
    bindings.Choice(_classIndex, viewModel.ClassNames, viewModel.ClassIndex);
```

Reusable custom controls belong in a small `BindingScope` extension that registers the inverse of every subscription:

```csharp
public static void Dial(this BindingScope scope, Dial target, IObservableValue<float> source)
{
    UnityAction<float> fromView = value => source.Value = value;
    Action<float> fromViewModel = target.SetValueWithoutNotify;
    target.Changed.AddListener(fromView);
    source.Changed += fromViewModel;
    fromViewModel(source.Value);
    scope.Add(new ActionDisposable(() =>
    {
        target.Changed.RemoveListener(fromView);
        source.Changed -= fromViewModel;
    }));
}
```
