# Bindings

## Convention field bindings

On a `[GenerateBindings]` View, private `[SerializeField]` widget names map to public ViewModel members by removing leading underscores and capitalizing the first letter:

```csharp
[SerializeField] private TMP_Text _title; // ViewModel.Title
[SerializeField] private Button _submit;  // ViewModel.Submit
```

Only exact public matches are generated. An unmatched serialized field remains an ordinary View concern; an exact name with incompatible types is a generation error. Explicit `[Bind]` declarations take precedence and are never duplicated by convention. Mark a matching field `[IgnoreBinding]` when the View owns it manually. Use `[GenerateBindings(Conventions = false)]` when a legacy or unusual View should use explicit bindings only.

## Explicit field bindings

`[Bind(nameof(ViewModel.Member))]` infers these unambiguous pairs:

| View field | ViewModel source | Generated binding |
|---|---|---|
| `Text` / `TMP_Text` | read-only observable `string` | `Text` |
| `Image` | read-only observable `Sprite` | `Sprite` |
| `Image` | read-only observable `float` | `Fill` |
| `Graphic` (`Image`, `RawImage`, `Text`, `TMP_Text`, ...) | read-only observable `Color` | `Color` |
| `GameObject` | read-only observable `bool` | `Active` |
| `CanvasGroup` | read-only observable `bool` | `Visible` |
| `Selectable` | read-only observable `bool` | `Interactable` |
| `Button` | `ICommand` | `Command` |
| `Button` | public parameterless `void` intent method | `Click` |
| `Slider` | writable observable `float` | `Slider` |
| `Scrollbar` | writable observable `float` | `Scrollbar` |
| `Toggle` | writable observable `bool` | `Toggle` |
| `InputField` / `TMP_InputField` | writable observable `string` | `Input` |
| `Dropdown` / `TMP_Dropdown` | writable observable `int` | `Dropdown` |

Use `[Bind(source, BindingTarget.X)]` when an explicit target communicates intent better.

For an always-enabled action, bind a public parameterless ViewModel intent directly. The generated listener is still owned by the View scope:

```csharp
public void Close()
{
    _navigation.CloseSettings();
}

[Bind(nameof(SettingsViewModel.Close))]
[SerializeField] private Button _close;
```

Use `ICommand` instead when the View must reflect `CanExecute`.

## Snapshot observation

Annotate a private instance method with `[Observe]`. It must return `void` and accept exactly one argument. The generator selects the only public `IReadOnlyObservableValue<T>` that publishes that argument type.

```csharp
    [Observe]
    private void Render(MapScreenState state)
{
    _nodes.Paint(state.Nodes);
}
```

If two sources publish the same type, disambiguate with `[Observe(nameof(MapViewModel.State))]`. Generation reports all matching source names instead of choosing silently.

This is intended for dynamic lists, maps, composite widgets, and screen DTOs. The View still only paints; state calculation remains in the ViewModel.

## Explicit and custom bindings

Generated and explicit bindings can coexist. For a custom control, create a small extension that registers cleanup:

```csharp
public static void Dial(
    this BindingScope scope,
    Dial target,
    IObservableValue<float> source)
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

Never add runtime reflection or omit unsubscribe ownership.

## Mixing generated and custom bindings on the same View

`[GenerateBindings]` emits a `partial void BindCustom(BindingScope bindings, TViewModel viewModel);` call at the end of the generated `Bind` override. Implement it in a hand-written partial to add a one-off custom binding without giving up conventions/`[Bind]`/`[Observe]` for the rest of the View:

```csharp
public partial class SettingsPanelView
{
    partial void BindCustom(BindingScope bindings, SettingsViewModel viewModel)
    {
        bindings.Dial(_dial, viewModel.Volume);
    }
}
```

The hook is a no-op (and compiles away) when unimplemented, so it costs nothing on Views that only use conventions.

## Dynamic rows and command arguments

Use `ICommand<T>` when a repeated control sends a stable row/node ID. `BindingScope` owns both the listener and the `CanExecuteChanged` subscription:

```csharp
public ICommand<string> SelectResident { get; }

bindings.Command(button, viewModel.SelectResident, resident.Id);
```

Call `RelayCommand<T>.Refresh()` after the conditions used by `CanExecute(T)` change. Keep one child `BindingScope` per rebuilt dynamic collection and dispose the old scope before replacing its controls.
