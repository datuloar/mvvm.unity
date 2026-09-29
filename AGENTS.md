# MVVM Unity Agent Contract

## Mission

MVVM Unity is a small typed MVVM package for uGUI and TextMeshPro. Keep authoring obvious, generated code readable and runtime machinery direct: no reflection, no hidden scheduler and no third-party dependency.

## Read First

| Need | Read |
| --- | --- |
| Observables, commands, ViewModel lifetime | `Scripts/Runtime/Core` |
| Views, scopes, built-in bindings | `Scripts/Runtime/Binding` |
| Generation and diagnostics | `Scripts/Editor`, `Documentation~/CodeGeneration.md` |
| Binding rules | `Scripts/Editor/BindingRules.cs`, `Documentation~/Bindings.md` |
| Usage example | `Samples~/CharacterCreator` |
| Tests | `Tests/EditMode`, `Tests/Fixtures` |

## Architecture

- `mvvm.unity.core` has no engine references. Models and ViewModels depend on it only.
- `mvvm.unity` contains `MvvmView<T>`, `BindingScope`, `BindingExtensions` and the binding attributes.
- `mvvm.unity.editor` analyzes `[GenerateBindings]` Views and writes `Generated/<Namespace>.<View>.Bindings.g.cs`.
- Generated code overrides `BindGenerated`. `Bind` is reserved for hand-written bindings and runs afterwards.
- `BindingRules` is the single ordered table used for both inference and explicit-target validation.

## Invariants

- Every listener is registered in a `BindingScope` together with its inverse operation.
- A View disposes its ViewModel only when it owns it; reassigning the same instance keeps it alive.
- `ViewModel.Dispose` runs `OnDispose` once, then releases everything passed to `Own`.
- Two-way bindings write to widgets through `Set*WithoutNotify`.
- A scoped `[Observe]` render disposes the previous render scope before the next render.
- Generation never deletes a file without the `GeneratedCode` ownership marker and keeps the previous file while a View is invalid.
- Generated output is deterministic and uses LF line endings.

## Code Standard

`.editorconfig` is authoritative.

- Block-scoped namespaces with the `MvvmUnity` prefix; explicit accessibility.
- Types, methods, properties, events and constants use PascalCase; private fields use `_camelCase`.
- Editor helpers are `internal`; public API exists only where game code must call, implement or inherit it.
- Seal concrete classes unless inheritance is a deliberate extension point.
- One main type per file, files below 400 lines, methods short and direct.
- Source comments, XML documentation comments and TODO markers are forbidden. Durable explanation belongs in `Documentation~`.
- No runtime reflection, LINQ in binding hot paths, `Update` polling or async machinery.

## Asset Safety

- Keep every `.meta` file; move assets together with their metas.
- New assets need unique GUIDs.
- Regenerate the sample scene only with an understood Unity workflow; do not hand-edit its YAML.

## Validation

```powershell
powershell -File Tools~/validate.ps1
```

Then run the `mvvm.unity.tests` EditMode suite and the imported sample tests on Unity 2022.3 LTS and Unity 6. After changing the generator, check that `CommittedSourceMatchesGeneratorOutput` passes and that sample bindings are regenerated without diff.

## Definition of Done

- Lifetime and ownership stay correct on bind, rebind, disable and destroy.
- Public API changes are intentional and recorded in `CHANGELOG.md` and documentation.
- Tests cover the change; the validator and both Unity versions pass without warnings.
