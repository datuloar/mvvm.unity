# MVVM Framework for Unity
This MVVM (Model-View-ViewModel) framework for Unity is designed to simplify the separation of UI and logic, allowing both designers and developers to work in parallel. It provides clear and intuitive bindings between the Unity UI components and the ViewModel logic, following modern MVVM patterns.

> **Note**: This framework is conceptual and is primarily intended for projects where UI-heavy elements are frequently updated, making it easier for designers to modify the UI without affecting the underlying logic.

## Features

* Separation of Concerns: Keeps Views (UI) completely separate from the ViewModel (business logic). Views handle UI updates, while ViewModels deal with application logic.

* Tag-Based Binding: Use custom tags on GameObjects to bind them to ViewModel properties and commands without writing additional glue code:
1. [BindableBtn] for buttons,
2. [BindableTxt] for text (TMP_Text),
3. [BindableImg] for images,
4. [BindableSld] for sliders.
4. [BindableGo] for gameObjects (for SetActive).

* Reactive Properties: Automatically update the UI when data in the ViewModel changes, without needing to manually update UI elements. (u can use UniRx if modify code)

* Command Binding: Connect buttons and sliders directly to ViewModel commands, eliminating the need for event handlers in the UI.

* Explicit and Implicit Keys: Use explicit keys in the [Bindable] attribute, or fall back to the default property or method name when no key is provided.

* Dynamic UI Creation: Supports the dynamic instantiation of Views at runtime, with automatic ViewModel binding.

# Installation

## As a unity module
Installation of the unity module in the video via a git link in the PackageManager is supported:
```
"com.mvvm.unity": "https://github.com/datuloar/mvvm.unity.git",
```
By default, the latest release version is used. If you need a “in development” version with the latest changes, you should switch to the `develop` branch:
```
"com.mvvm.unity": "https://github.com/datuloar/mvvm.unity.git#develop",
```

# How It Works
The framework works by tagging UI elements in Unity and binding them to the ViewModel properties and commands. Each UI element is identified by its tag, and these tags map directly to properties and commands in the ViewModel.

## Binding Example
To bind a user interface element to a property or command of ViewModel:

First of all, you create a GameObject parent for elements, add a `View` component to it, which automatically pulls up objects inside the parent with the necessary prefixes specified above in the documentation, that is, the designer who will lay out must specify the necessary objects, which will subsequently be bound to ViewModel with their names after the tag.

1. Properties: Add [BindableProperty] to a ViewModel property. For example:
```c#
[BindableProperty]
public string CoinCount => ValueFormatter.Format(_model.Coins);
```

2. Commands: Add [BindableCommand] to a ViewModel method:
```c#
[BindableCommand]
public void IncreaseCoinCount() => _model.AddCoins(1);
```

3. Explicit Keys: You can explicitly specify a key for properties or commands by passing it to the attribute:
```c#
[BindableProperty("CoinsDisplay")]
public string CoinCount => ValueFormatter.Format(_model.Coins);

[BindableCommand("AddCoinCommand")]
public void IncreaseCoinCount() => _model.AddCoins(1);
```

4. UI Element Naming: In Unity, name your GameObjects using the appropriate tag and key:

A button to increase the coin count: [BindableBtn] IncreaseCoinCount
A text field to display the coin count: [BindableTxt] CoinCount
When the button is clicked, the IncreaseCoinCount command is executed, and the text element is automatically updated when the CoinCount property changes.

------------------------------

### If you don't understand, and u want more features, you can look at the example of usage in the folder `Sample`