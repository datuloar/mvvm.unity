# Changelog

## 2.0.0

- Added the Character Creator sample with a scene, screenshots and EditMode tests, replacing the coin counter sample.
- Added scoped observation: `[Observe]` methods and `BindingScope.Observe` accept a `BindingScope` that is renewed for every render.
- Added `ViewModel.Own` to tie subscriptions and commands to the ViewModel lifetime.
- Added `RelayCommand.RefreshOn` and `RelayCommand<T>.RefreshOn`; both commands implement `IDisposable`.
- Added `Color`, `Scrollbar` and `Dropdown`/`TMP_Dropdown` bindings.
- Added a parameterless `ObservableValue<T>` constructor.
- Changed generated code to override `BindGenerated`, leaving `Bind` for hand-written bindings on any View.
- Changed generated files to carry a `GeneratedCode` ownership marker instead of header comments and to use LF line endings.
- Changed the generator to report every problem in a View at once and to reject binding attributes outside `[GenerateBindings]` types.
- Changed binding inference and validation to share one ordered rule table.
- Fixed `SetViewModel` disposing an owned ViewModel when the same instance was assigned again.
- Removed `GenerateBindingsAttribute(Type)`; the ViewModel type is always taken from `MvvmView<T>`.
- Removed `BindingScopeHost`.
- Renamed `ObservableSubscriptions` to `ObservableValueExtensions`.
- Moved `BindingTarget.Auto` to the default enum value.
- Split the editor generator into focused internal types and made test fixtures a runtime test assembly.
- Raised the minimum Unity version to 2022.3 and validated the package on Unity 2022.3 LTS and Unity 6.3.
- Added `.editorconfig`, `.gitattributes`, `AGENTS.md`, `Documentation~` and the `Tools~/validate.ps1` repository gate.

## 1.3.0

- Added zero-attribute field binding for matching private serialized widget names.
- Added per-field `[IgnoreBinding]` and per-View convention opt-out.
- Added source inference for parameterless `[Observe]` declarations with explicit ambiguity diagnostics.
- Changed generator source discovery to one `MonoScript` index per rebuild.

## 1.2.0

- Added parameterized `ICommand<T>` and button binding.
- Added generated button binding to parameterless ViewModel intent methods.
- Preserved View accessibility in generated partial declarations.

## 1.1.0

- Replaced runtime reflection binding with editor-time typed C# generation.
- Added `[GenerateBindings]`, `[Bind]` and `[Observe]`.
- Added `StateViewModel<TState>`.
- Added automatic binding cleanup and package documentation.

## 1.0.0

- Initial package prototype.
