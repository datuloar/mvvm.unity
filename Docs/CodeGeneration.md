# Code generation

Generation runs in the Unity Editor and writes ordinary C#. Runtime assemblies contain no reflection.

## Workflow

1. Make the View `partial` and inherit `MvvmView<TViewModel>`.
2. Add `[GenerateBindings]`.
3. Name serialized widget fields after ViewModel members. Add `[Bind(nameof(...))]` only for aliases. Mark snapshot render methods with `[Observe]`; add `nameof` only when multiple observables publish the same type.
4. Run `Tools > MVVM > Rebuild Generated Bindings`.
5. Inspect and commit the generated file beside the View.

Generation also runs after an editor domain reload. Files are rewritten only when content changed, then Unity receives one asset refresh. Orphan discovery queries script assets rather than recursively reading every asset file.

The generator indexes imported `MonoScript` assets once per rebuild, including Editor-only/test assemblies. Generation time grows with the number of scripts plus the number of Views instead of rescanning the AssetDatabase for every View.

## Validation

The generator rejects:

- nested or generic View types;
- a ViewModel attribute that disagrees with `MvvmView<T>`;
- missing/non-public ViewModel sources;
- overloaded, parameterized, generic or non-void methods used as click intents;
- incompatible widget/source types;
- ambiguous automatic targets;
- ambiguous source inference for `[Observe]`;
- invalid observer signatures;
- Views with no bindings.

Errors identify the View member and are emitted before generated code changes.

## Refactoring safely

`nameof` makes source renames compiler-visible. Before removing or renaming View fields referenced by an old generated partial, run `Tools > MVVM > Clean Generated Bindings`. Rebuild after the source compiles. Only files carrying the package ownership header are deleted.

For unusual controls, keep the generated common bindings and add an explicit `Bind` override only when the whole binding plan is genuinely custom. A small `BindingScope` extension is usually easier to reuse.
