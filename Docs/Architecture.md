# Architecture and scene composition

The package owns binding mechanics, not application architecture. Keep the dependency direction simple:

```text
Unity View -> ViewModel -> model/service contracts
              |
              +-> observable presentation state and commands
```

## Responsibilities

- Model: authoritative game/data state and rules.
- ViewModel: localized presentation state and user intents; plain C#.
- View: widget references, rendering, input adapters, animation hooks.
- Composition root: constructs the graph and defines lifetime.

A View must not load saves, query global state, choose localization keys, or calculate gameplay rules. A ViewModel must not find scene objects or reference Unity UI.

## Unity scenes

Use one composition root per independently loadable scene. With VContainer this is normally one `LifetimeScope`. Register exactly one scene entry object (`IStartable`) that receives the composed View and ViewModel and calls the View's `Enter`/`SetViewModel` method.

Do not add a second `EntryPoint` MonoBehaviour beside the scope. The roles are distinct:

- `LifetimeScope`: composition root.
- scene `Flow`/entry object: starts the already-composed graph.

Project-wide services belong to a parent scope. Scene state and ViewModels use scoped lifetime. Runtime-created overlays may use a small factory when more than one instance is genuinely created.

## Snapshot versus fine-grained state

Use fine-grained observable properties for ordinary fixed forms and two-way controls. Use `StateViewModel<TState>` when a screen is naturally refreshed as one coherent snapshot or owns dynamic collections. Do not mirror the same authoritative state in both forms without a concrete reason.
