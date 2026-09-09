# Testing and performance

## ViewModel tests

Reference `mvvm.unity.core` from EditMode tests. Test initial state, each intent, command availability, and disposal of model subscriptions without loading a scene.

```csharp
[Test]
public void SubmitPublishesErrorWhenModelRejects()
{
    var viewModel = new LoginViewModel(new RejectingLogin());
    viewModel.Submit.Execute();
    Assert.AreEqual("Invalid", viewModel.Error.Value);
}
```

## Binding tests

For Unity adapters, create the smallest GameObject/widget, bind it, mutate both sides where relevant, dispose the `BindingScope`, and assert no later updates cross the boundary.

## Runtime cost

- Binding is direct delegate subscription.
- Publishing a value is an equality check plus event invocation.
- Generated code performs direct method calls.
- Editor generation indexes scripts once per rebuild; convention inference has no player cost.
- There is no runtime reflection, expression compilation, polling, `Update`, `Task`, or hidden scheduler.

Prefer event-driven refresh. Do not publish every frame unless the UI truly changes every frame. For large dynamic lists, a snapshot render is simple; profile before adding pooling or incremental collection machinery. Add those mechanisms only after measured churn justifies their extra lifetime/state complexity.

## Allocation guidance

`ObservableValue<T>` itself allocates no object per unchanged assignment. Subscribing creates one disposable owner. Snapshot DTOs and rebuilt dynamic rows do allocate, so use them at user-action/tick frequency, not in a hot `Update` loop.
