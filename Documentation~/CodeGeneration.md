# Code generation

Binding code is produced in the Unity Editor as ordinary C#. Players contain direct method calls and no reflection.

## Workflow

1. Make the View `partial`, inherit `MvvmView<TViewModel>` and add `[GenerateBindings]`.
2. Name serialized widgets after ViewModel members. Add `[Bind]` only for aliases, extra bindings or explicit targets. Mark render methods with `[Observe]`.
3. Save. The generator runs after every script reload; `Tools > MVVM > Rebuild Generated Bindings` runs it on demand.
4. Commit `Generated/<Namespace>.<View>.Bindings.g.cs` together with the View.

The generated partial overrides `BindGenerated`. `Bind` stays free for hand-written bindings, so custom code never waits for generation and never collides with it.

```csharp
public partial class LoginView
{
    [global::System.CodeDom.Compiler.GeneratedCode("MvvmUnity.BindingCodeGenerator", "2.0")]
    protected override void BindGenerated(
        global::MvvmUnity.Unity.BindingScope bindings,
        global::Game.LoginViewModel viewModel)
    {
        bindings.Input(_name, viewModel.Name);
        bindings.Command(_submit, viewModel.Submit);
    }
}
```

Output is deterministic and uses LF line endings. Files are rewritten only when their content changes, followed by a single asset refresh.

## Validation

Generation stops for a View and reports every problem at once when it finds:

- a nested or generic View, or one that does not inherit `MvvmView<T>`;
- a `[Bind]` source that is not a public ViewModel member;
- a widget that cannot bind its source, including convention matches;
- an explicit `BindingTarget` incompatible with the widget or source;
- an `[Observe]` method with an unsupported signature, a missing source, or several candidate sources;
- a View with nothing to generate;
- `[Bind]`, `[IgnoreBinding]` or `[Observe]` on a type without `[GenerateBindings]`.

Errors name the View and member. The previous generated file is kept until the View is valid again, so a broken edit never removes code that other scripts compile against.

## Ownership and cleanup

The generator only deletes files that carry its `GeneratedCode` marker and belong to no View. `Tools > MVVM > Clean Generated Bindings` deletes all of them; custom `Bind` overrides keep compiling because they never depend on generated members.

## Continuous integration

`MvvmUnity.Editor.BindingCodeGenerator.Rebuild` is public for `-executeMethod`. Run it in batch mode and fail the build on a non-empty `git diff` to guarantee that committed bindings match their Views.
