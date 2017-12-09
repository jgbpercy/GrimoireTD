using NUnit.Framework;
using NSubstitute;
using GrimoireTD.UI;
using GrimoireTD.Dependencies;
using GrimoireTD.Creeps;

namespace GrimoireTD.Tests.GameModeManagerTests
{
    public class GameModeManagerTests
    {
        private IModelInterfaceController interfaceController;

        private IReadOnlyGameModel gameModel;

        private IReadOnlyCreepManager creepManager;

        [SetUp]
        public void EachTestSetUp()
        {
            interfaceController = Substitute.For<IModelInterfaceController>();
            DepsProv.TheInterfaceController = () => interfaceController;

            gameModel = Substitute.For<IReadOnlyGameModel>();
            creepManager = Substitute.For<IReadOnlyCreepManager>();

            gameModel.CreepManager.Returns(creepManager);

            DepsProv.SetTheGameModel(gameModel);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            typeof(DepsProv).TypeInitializer.Invoke(null, null);
        }

        private CGameModeManager ConstructAndSetUpSubject()
        {
            var subject = ConstructSubject();

            SetUpSubject(subject);

            return subject;
        }

        private CGameModeManager ConstructSubject()
        {
            return new CGameModeManager();
        }

        private void SetUpSubject(CGameModeManager subject)
        {
            subject.SetUp();
        }

        [Test]
        public void SetUp_Always_PutsTheGameInBuildMode()
        {
            var subject = ConstructAndSetUpSubject();

            Assert.AreEqual(GameMode.BUILD, subject.CurrentGameMode);
        }

        [Test]
        public void OnEnterDefendModePlayerAction_Always_ChangesGameToDefendMode()
        {
            var subject = ConstructAndSetUpSubject();

            interfaceController.OnEnterDefendModePlayerAction += Raise.EventWith(new EAOnEnterDefendModePlayerAction());

            Assert.AreEqual(GameMode.DEFEND, subject.CurrentGameMode);
        }

        [Test]
        public void OnEnterDefendModePlayerAction_Always_FiresOnEnterDefendModeEvent()
        {
            var subject = ConstructAndSetUpSubject();

            var eventTester = new EventTester<EAOnEnterDefendMode>();
            subject.OnEnterDefendMode += eventTester.Handler;

            interfaceController.OnEnterDefendModePlayerAction += Raise.EventWith(new EAOnEnterDefendModePlayerAction());

            eventTester.AssertFired(1);
        }

        [Test]
        public void OnWaveOver_Always_ChangesGameToBuildMode()
        {
            var subject = ConstructAndSetUpSubject();

            interfaceController.OnEnterDefendModePlayerAction += Raise.EventWith(new EAOnEnterDefendModePlayerAction());

            creepManager.OnWaveOver += Raise.EventWith(new EAOnWaveOver());

            Assert.AreEqual(GameMode.BUILD, subject.CurrentGameMode);
        }

        [Test]
        public void OnWaveOver_Always_FiresOnEnterBuildModeEvent()
        {
            var subject = ConstructAndSetUpSubject();

            var eventTester = new EventTester<EAOnEnterBuildMode>();
            subject.OnEnterBuildMode += eventTester.Handler;

            interfaceController.OnEnterDefendModePlayerAction += Raise.EventWith(new EAOnEnterDefendModePlayerAction());

            creepManager.OnWaveOver += Raise.EventWith(new EAOnWaveOver());

            eventTester.AssertFired(1);
        }
    }
}