using NUnit.Framework;

using Random = System.Random;

namespace MvvmUnity.Samples.CharacterCreator.Tests
{
    public sealed class CharacterCreatorViewModelTests
    {
        private HeroDraft _draft;
        private HeroRoster _roster;
        private CharacterCreatorViewModel _viewModel;

        [SetUp]
        public void SetUp()
        {
            _draft = new HeroDraft();
            _roster = new HeroRoster();
            _viewModel = new CharacterCreatorViewModel(_draft, _roster, new Random(7));
        }

        [TearDown]
        public void TearDown() => _viewModel.Dispose();

        [Test]
        public void StartsIncompleteWithGuidance()
        {
            Assert.That(_viewModel.NameMissing.Value, Is.True);
            Assert.That(_viewModel.Create.CanExecute, Is.False);
            Assert.That(_viewModel.Status.Value, Is.EqualTo("Name your hero to continue"));
            Assert.That(_viewModel.PointsLeft.Value, Is.EqualTo("9 / 9 points left"));
        }

        [Test]
        public void CreateRequiresANameAndTheWholeBudget()
        {
            _viewModel.Name.Value = "Aria";
            Assert.That(_viewModel.Create.CanExecute, Is.False);
            Assert.That(_viewModel.Status.Value, Is.EqualTo("Spend 9 more points"));

            SpendBudget();

            Assert.That(_viewModel.Create.CanExecute, Is.True);
            Assert.That(_viewModel.Status.Value, Is.EqualTo("Ready to join the party"));
        }

        [Test]
        public void CreateRegistersTheHeroAndShowsConfirmation()
        {
            _viewModel.Name.Value = "  Aria ";
            _viewModel.ClassIndex.Value = (int)HeroClass.Mage;
            SpendBudget();

            _viewModel.Create.Execute();

            Assert.That(_roster.Count, Is.EqualTo(1));
            Assert.That(_viewModel.Confirmation.Value, Is.EqualTo("Aria the Mage joined the party"));
            Assert.That(_viewModel.ConfirmationVisible.Value, Is.True);
            Assert.That(_viewModel.PartySize.Value, Is.EqualTo("Party: 1 hero"));

            _viewModel.Dismiss();
            Assert.That(_viewModel.ConfirmationVisible.Value, Is.False);
        }

        [Test]
        public void ClassSelectionUpdatesPresentation()
        {
            _viewModel.ClassIndex.Value = (int)HeroClass.Ranger;

            Assert.That(_viewModel.ClassInitial.Value, Is.EqualTo("R"));
            Assert.That(_viewModel.ClassColor.Value, Is.EqualTo(HeroClassCatalog.Tint(HeroClass.Ranger)));
            Assert.That(_viewModel.ClassDescription.Value, Is.EqualTo(HeroClassCatalog.Description(HeroClass.Ranger)));
        }

        [Test]
        public void StatCommandsFollowTheDraft()
        {
            Assert.That(_viewModel.Decrease.CanExecute(HeroStat.Agility), Is.False);

            _viewModel.Increase.Execute(HeroStat.Agility);

            Assert.That(_viewModel.Decrease.CanExecute(HeroStat.Agility), Is.True);
            Assert.That(_viewModel.State.Value.Rows[(int)HeroStat.Agility].Points, Is.EqualTo("1"));
            Assert.That(_viewModel.PointsSpent.Value, Is.EqualTo(1f / HeroDraft.PointBudget));
        }

        [Test]
        public void HardcoreDisablesRandomize()
        {
            _viewModel.Hardcore.Value = true;
            _viewModel.Randomize();

            Assert.That(_viewModel.CanRandomize.Value, Is.False);
            Assert.That(_viewModel.Name.Value, Is.Empty);
        }

        [Test]
        public void RandomizeProducesACompleteHero()
        {
            _viewModel.Randomize();

            Assert.That(_viewModel.NameMissing.Value, Is.False);
            Assert.That(_draft.RemainingPoints, Is.Zero);
            Assert.That(_viewModel.Create.CanExecute, Is.True);
        }

        [Test]
        public void ResetRestoresDefaults()
        {
            _viewModel.Randomize();
            _viewModel.Difficulty.Value = 0.9f;

            _viewModel.Reset();

            Assert.That(_viewModel.Name.Value, Is.Empty);
            Assert.That(_draft.SpentPoints, Is.Zero);
            Assert.That(_viewModel.DifficultyLabel.Value, Is.EqualTo("Veteran · 50%"));
        }

        [Test]
        public void DisposeStopsFollowingTheDraft()
        {
            _viewModel.Dispose();

            _draft.Increase(HeroStat.Strength);

            Assert.That(_viewModel.PointsLeft.Value, Is.EqualTo("9 / 9 points left"));
            Assert.That(_viewModel.PointsSpent.Value, Is.Zero);
        }

        private void SpendBudget()
        {
            for (var index = 0; index < HeroDraft.PointBudget; index++)
                _viewModel.Increase.Execute((HeroStat)(index % 3));
        }
    }
}
