using MvvmUnity.Unity;

namespace MvvmUnity.Samples.CharacterCreator
{
    public partial class CharacterCreatorView
    {
        [global::System.CodeDom.Compiler.GeneratedCode("MvvmUnity.BindingCodeGenerator", "2.0")]
        protected override void BindGenerated(
            global::MvvmUnity.Unity.BindingScope bindings,
            global::MvvmUnity.Samples.CharacterCreator.CharacterCreatorViewModel viewModel)
        {
            bindings.Input(_name, viewModel.Name);
            bindings.Active(_nameMissing, viewModel.NameMissing);
            bindings.Color(_classColor, viewModel.ClassColor);
            bindings.Text(_classInitial, viewModel.ClassInitial);
            bindings.Text(_classDescription, viewModel.ClassDescription);
            bindings.Text(_pointsLeft, viewModel.PointsLeft);
            bindings.Fill(_pointsSpent, viewModel.PointsSpent);
            bindings.Slider(_difficulty, viewModel.Difficulty);
            bindings.Text(_difficultyLabel, viewModel.DifficultyLabel);
            bindings.Toggle(_hardcore, viewModel.Hardcore);
            bindings.Text(_status, viewModel.Status);
            bindings.Text(_partySize, viewModel.PartySize);
            bindings.Click(_randomize, viewModel.Randomize);
            bindings.Interactable(_randomize, viewModel.CanRandomize);
            bindings.Click(_reset, viewModel.Reset);
            bindings.Command(_create, viewModel.Create);
            bindings.Visible(_confirmationPanel, viewModel.ConfirmationVisible);
            bindings.Text(_confirmation, viewModel.Confirmation);
            bindings.Click(_dismiss, viewModel.Dismiss);
            bindings.Observe(viewModel.State, Render);
        }
    }
}
