# Architecture

The package owns binding mechanics, not application architecture. Dependencies point one way:

```text
View (MonoBehaviour) -> ViewModel (plain C#) -> models and services
        ^                     |
        +-- generated bindings and BindingScope
```

## Responsibilities

| Layer | Owns | Never does |
| --- | --- | --- |
| Model | Authoritative state and rules | Formatting, localization, UI state |
| ViewModel | Presentation state, formatting, validation, user intents | Touch `GameObject`, widgets or scenes |
| View | Widget references, rendering, custom input adapters | Load data, compute rules, format values |
| Composition root | Creating the graph and deciding lifetimes | Contain presentation logic |

## Lifetime

- `BindingScope` owns every listener created for one binding pass. Disposing it removes all of them.
- `MvvmView<T>` creates a scope on enable and disposes it on disable or destroy.
- `ViewModel.Own` ties subscriptions and commands to the ViewModel; `Dispose` releases them after `OnDispose`.
- A View disposes its ViewModel only when it was given `ownsViewModel: true`. Reassigning the same instance keeps it alive.

## Fine-grained and snapshot state

Use one observable per value for ordinary forms and two-way controls. Use `StateViewModel<TState>` when a region renders as one coherent snapshot, such as a list or a map, and observe it with `[Observe]`. Do not mirror the same authoritative state in both forms.

## Scenes and dependency injection

Use one composition root per independently loaded scene. With VContainer, Zenject or a plain entry point, construct the ViewModel there and call `SetViewModel` after the View exists. DI-owned ViewModels are disposed by their container, so pass `ownsViewModel: false`.
