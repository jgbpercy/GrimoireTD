using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using UnityEngine;
using GrimoireTD.Abilities.DefendMode;
using GrimoireTD.Abilities;
using GrimoireTD.DefendingEntities;
using GrimoireTD.Technical;
using GrimoireTD.ChannelDebug;

namespace GrimoireTD.Tests.DefendModeAbilityManagerTests
{
    public class DefendModeAbilityManagerTests
    {
        private float defaultDeltaTime = 0.2f;

        private IAbilities abilities = Substitute.For<IAbilities>();

        private IDefendModeAbility abilityOne = Substitute.For<IDefendModeAbility>();
        private IDefendModeAbility abilityTwo = Substitute.For<IDefendModeAbility>();
        private IDefendModeAbility abilityThree = Substitute.For<IDefendModeAbility>();

        private IDefendingEntity attachedToDefendingEntity = Substitute.For<IDefendingEntity>();

        private IReadOnlyGameModel gameModel = Substitute.For<IReadOnlyGameModel>();

        private IReadOnlyGameStateManager gameStateManager = Substitute.For<IReadOnlyGameStateManager>();

        private CDefendModeAbilityManager subject;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            CDebug.InitialiseDebugChannels();

            gameModel.GameStateManager.Returns(gameStateManager);

            GameModels.Models.Add(gameModel);

            GameObject modelObjectFrameUpdaterGo = new GameObject();

            modelObjectFrameUpdaterGo.AddComponent<ModelObjectFrameUpdater>();
        }

        [SetUp]
        public void EachTestSetUp()
        {
            gameStateManager.CurrentGameMode.Returns(GameMode.DEFEND);

            abilities.DefendModeAbilities().Returns(new List<IDefendModeAbility>
            {
                abilityOne,
                abilityTwo,
                abilityThree
            });

            abilityOne.IsOffCooldown.Returns(true);
            abilityTwo.IsOffCooldown.Returns(true);
            abilityThree.IsOffCooldown.Returns(true);

            abilityOne.ClearReceivedCalls();
            abilityTwo.ClearReceivedCalls();
            abilityThree.ClearReceivedCalls();

            abilityOne.ExecuteAbility(Arg.Any<IDefendingEntity>()).Returns(true);
            abilityTwo.ExecuteAbility(Arg.Any<IDefendingEntity>()).Returns(true);
            abilityThree.ExecuteAbility(Arg.Any<IDefendingEntity>()).Returns(true);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            typeof(GameModels).TypeInitializer.Invoke(null, null);
        }

        private CDefendModeAbilityManager ConstructSubject()
        {
            return new CDefendModeAbilityManager(
                abilities, 
                attachedToDefendingEntity
            );
        }

        [Test]
        public void Manager_WhenInBuildMode_DoesNotExecuteOffCooldownAbilities()
        {
            gameStateManager.CurrentGameMode.Returns(GameMode.BUILD);

            var subject = ConstructSubject();

            subject.ModelObjectFrameUpdate(defaultDeltaTime);

            abilityOne.DidNotReceive().ExecuteAbility(Arg.Any<IDefendingEntity>());
            abilityTwo.DidNotReceive().ExecuteAbility(Arg.Any<IDefendingEntity>());
            abilityThree.DidNotReceive().ExecuteAbility(Arg.Any<IDefendingEntity>());
        }

        [Test]
        public void Manager_WhenInDefendMode_ExecutesTheFirstAbilityInTheListOnce()
        {
            var subject = ConstructSubject();

            subject.ModelObjectFrameUpdate(defaultDeltaTime);

            abilityOne.Received(1).ExecuteAbility(attachedToDefendingEntity);
        }

        //Intended behaviour until upswing/backswing times/animations
        [Test]
        public void Manager_WhenMultipleAbilitiesAreOffCooldown_ExecutesOneAbilityPerUpdate()
        {
            var subject = ConstructSubject();

            subject.ModelObjectFrameUpdate(defaultDeltaTime);

            abilityOne.Received(1).ExecuteAbility(attachedToDefendingEntity);
            abilityTwo.DidNotReceive().ExecuteAbility(Arg.Any<IDefendingEntity>());

            abilityOne.OnAbilityExecuted += Raise.EventWith(new EAOnAbilityExecuted(abilityOne));

            subject.ModelObjectFrameUpdate(defaultDeltaTime);

            abilityTwo.Received(1).ExecuteAbility(attachedToDefendingEntity);
            abilityThree.DidNotReceive().ExecuteAbility(Arg.Any<IDefendingEntity>());
        }

        [Test]
        public void Manager_WhenMultipleAbilitiesAreOffCooldownAndTheFirstCannotExecute_ExecutesTheNextAbility()
        {
            var subject = ConstructSubject();

            abilityOne.ExecuteAbility(Arg.Any<IDefendingEntity>()).Returns(false);

            subject.ModelObjectFrameUpdate(defaultDeltaTime);

            abilityTwo.Received(1).ExecuteAbility(attachedToDefendingEntity);
            abilityThree.DidNotReceive().ExecuteAbility(Arg.Any<IDefendingEntity>());

            abilityTwo.OnAbilityExecuted += Raise.EventWith(new EAOnAbilityExecuted(abilityTwo));

            subject.ModelObjectFrameUpdate(defaultDeltaTime);

            abilityThree.Received(1).ExecuteAbility(attachedToDefendingEntity);
        }

        [Test]
        public void Manager_IfAllAbilitiesAreOnCooldown_DoesNotExecuteAny()
        {
            abilityOne.IsOffCooldown.Returns(false);
            abilityTwo.IsOffCooldown.Returns(false);
            abilityThree.IsOffCooldown.Returns(false);

            var subject = ConstructSubject();

            subject.ModelObjectFrameUpdate(defaultDeltaTime);

            abilityOne.DidNotReceive().ExecuteAbility(Arg.Any<IDefendingEntity>());
            abilityTwo.DidNotReceive().ExecuteAbility(Arg.Any<IDefendingEntity>());
            abilityThree.DidNotReceive().ExecuteAbility(Arg.Any<IDefendingEntity>());
        }

        [Test]
        public void Manager_WhenANewAbillityIsAddedWhichIsOffCooldown_ExecutesThatAbility()
        {
            abilities.DefendModeAbilities().Returns(new List<IDefendModeAbility>());

            var subject = ConstructSubject();

            abilities.OnDefendModeAbilityAdded += Raise.EventWith(new EAOnDefendModeAbilityAdded(abilityOne));

            subject.ModelObjectFrameUpdate(defaultDeltaTime);

            abilityOne.Received(1).ExecuteAbility(attachedToDefendingEntity);
        }

        [Test]
        public void Manager_WhenAnAbilityThatWasOnCooldownComesOffCooldown_ExecutesThatAbility()
        {
            abilityOne.IsOffCooldown.Returns(false);
            abilityTwo.IsOffCooldown.Returns(false);
            abilityThree.IsOffCooldown.Returns(false);

            var subject = ConstructSubject();

            subject.ModelObjectFrameUpdate(defaultDeltaTime);

            abilityTwo.OnAbilityOffCooldown += Raise.EventWith(new EAOnAbilityOffCooldown(abilityTwo));

            subject.ModelObjectFrameUpdate(defaultDeltaTime);

            abilityTwo.Received(1).ExecuteAbility(attachedToDefendingEntity);
        }

        [Test]
        public void Manager_WhenAnAbilityIsAddedOnCooldownWhichLaterComesOffCooldown_ExecutesThatAbility()
        {
            abilities.DefendModeAbilities().Returns(new List<IDefendModeAbility>());

            abilityThree.IsOffCooldown.Returns(false);

            var subject = ConstructSubject();

            subject.ModelObjectFrameUpdate(defaultDeltaTime);

            abilities.OnDefendModeAbilityAdded += Raise.EventWith(new EAOnDefendModeAbilityAdded(abilityThree));

            subject.ModelObjectFrameUpdate(defaultDeltaTime);

            abilityThree.OnAbilityOffCooldown += Raise.EventWith(new EAOnAbilityOffCooldown(abilityThree));

            subject.ModelObjectFrameUpdate(defaultDeltaTime);

            abilityThree.Received(1).ExecuteAbility(attachedToDefendingEntity);
        }

        [Test]
        public void Manager_WhenAbilityIsRemoved_DoesNotExecuteItWhenItComesOffCooldown()
        {
            abilityOne.IsOffCooldown.Returns(false);

            abilities.DefendModeAbilities().Returns(new List<IDefendModeAbility> { abilityOne });

            var subject = ConstructSubject();

            subject.ModelObjectFrameUpdate(defaultDeltaTime);

            abilities.OnDefendModeAbilityRemoved += Raise.EventWith(new EAOnDefendModeAbilityRemoved(abilityOne));

            subject.ModelObjectFrameUpdate(defaultDeltaTime);

            abilityOne.OnAbilityOffCooldown += Raise.EventWith(new EAOnAbilityOffCooldown(abilityOne));

            subject.ModelObjectFrameUpdate(defaultDeltaTime);

            abilityOne.DidNotReceive().ExecuteAbility(Arg.Any<IDefendingEntity>());
        }

        [Test]
        public void Manager_WhenAllAbilitiesComeOffColldown_EmitsEventAtCorrectTime()
        {
            abilityOne.IsOffCooldown.Returns(false);
            abilityTwo.IsOffCooldown.Returns(false);
            abilityThree.IsOffCooldown.Returns(false);

            var subject = ConstructSubject();

            var eventTester = new EventTester<EAOnAllDefendModeAbilitiesOffCooldown>();
            subject.OnAllDefendModeAbilitiesOffCooldown += eventTester.Handler;

            subject.ModelObjectFrameUpdate(defaultDeltaTime);
            abilityOne.OnAbilityOffCooldown += Raise.EventWith(new EAOnAbilityOffCooldown(abilityOne));
            eventTester.AssertFired(false);

            subject.ModelObjectFrameUpdate(defaultDeltaTime);
            abilityTwo.OnAbilityOffCooldown += Raise.EventWith(new EAOnAbilityOffCooldown(abilityTwo));
            eventTester.AssertFired(false);

            subject.ModelObjectFrameUpdate(defaultDeltaTime);
            abilityThree.OnAbilityOffCooldown += Raise.EventWith(new EAOnAbilityOffCooldown(abilityThree));
            eventTester.AssertFired(1);
        }

        [Test]
        public void Manager_WhenAnAbilityWasAddedAndAllAbilitiesComeOffCooldown_EmitsEventAtCorrectTime()
        {
            abilityOne.IsOffCooldown.Returns(false);
            abilityTwo.IsOffCooldown.Returns(false);
            abilityThree.IsOffCooldown.Returns(false);

            abilities.DefendModeAbilities().Returns(new List<IDefendModeAbility> { abilityOne, abilityTwo });

            var subject = ConstructSubject();

            var eventTester = new EventTester<EAOnAllDefendModeAbilitiesOffCooldown>();
            subject.OnAllDefendModeAbilitiesOffCooldown += eventTester.Handler;

            subject.ModelObjectFrameUpdate(defaultDeltaTime);
            abilityOne.OnAbilityOffCooldown += Raise.EventWith(new EAOnAbilityOffCooldown(abilityOne));
            eventTester.AssertFired(false);

            subject.ModelObjectFrameUpdate(defaultDeltaTime);
            abilities.OnDefendModeAbilityAdded += Raise.EventWith(new EAOnDefendModeAbilityAdded(abilityThree));

            subject.ModelObjectFrameUpdate(defaultDeltaTime);
            abilityThree.OnAbilityOffCooldown += Raise.EventWith(new EAOnAbilityOffCooldown(abilityThree));
            eventTester.AssertFired(false);

            subject.ModelObjectFrameUpdate(defaultDeltaTime);
            abilityTwo.OnAbilityOffCooldown += Raise.EventWith(new EAOnAbilityOffCooldown(abilityTwo));
            eventTester.AssertFired(1);
        }

        [Test]
        public void Manager_WhenAnAbilityWasRemovedAndAllOtherAbilitiesComeOffCooldown_EmitsEventAtCorrectTime()
        {
            abilityOne.IsOffCooldown.Returns(false);
            abilityTwo.IsOffCooldown.Returns(false);
            abilityThree.IsOffCooldown.Returns(false);

            var subject = ConstructSubject();

            var eventTester = new EventTester<EAOnAllDefendModeAbilitiesOffCooldown>();
            subject.OnAllDefendModeAbilitiesOffCooldown += eventTester.Handler;

            subject.ModelObjectFrameUpdate(defaultDeltaTime);
            abilityThree.OnAbilityOffCooldown += Raise.EventWith(new EAOnAbilityOffCooldown(abilityThree));
            eventTester.AssertFired(false);

            subject.ModelObjectFrameUpdate(defaultDeltaTime);
            abilities.OnDefendModeAbilityRemoved += Raise.EventWith(new EAOnDefendModeAbilityRemoved(abilityTwo));

            subject.ModelObjectFrameUpdate(defaultDeltaTime);
            abilityOne.OnAbilityOffCooldown += Raise.EventWith(new EAOnAbilityOffCooldown(abilityOne));
            eventTester.AssertFired(1);
        }

        [Test]
        public void Manager_WhenAnAbilityComesOffCooldownAndThenIsExecutedAndThenAllAbilitiesComeOffCooldown_EmitsEventAtCorrectTime()
        {
            abilityOne.IsOffCooldown.Returns(false);
            abilityTwo.IsOffCooldown.Returns(false);
            abilityThree.IsOffCooldown.Returns(false);

            var subject = ConstructSubject();

            var eventTester = new EventTester<EAOnAllDefendModeAbilitiesOffCooldown>();
            subject.OnAllDefendModeAbilitiesOffCooldown += eventTester.Handler;

            subject.ModelObjectFrameUpdate(defaultDeltaTime);
            abilityThree.OnAbilityOffCooldown += Raise.EventWith(new EAOnAbilityOffCooldown(abilityThree));
            eventTester.AssertFired(false);

            subject.ModelObjectFrameUpdate(defaultDeltaTime);
            abilityTwo.OnAbilityOffCooldown += Raise.EventWith(new EAOnAbilityOffCooldown(abilityTwo));
            eventTester.AssertFired(false);

            subject.ModelObjectFrameUpdate(defaultDeltaTime);
            abilityThree.OnAbilityExecuted += Raise.EventWith(new EAOnAbilityExecuted(abilityThree));
            eventTester.AssertFired(false);

            subject.ModelObjectFrameUpdate(defaultDeltaTime);
            abilityOne.OnAbilityOffCooldown += Raise.EventWith(new EAOnAbilityOffCooldown(abilityOne));
            eventTester.AssertFired(false);

            subject.ModelObjectFrameUpdate(defaultDeltaTime);
            abilityThree.OnAbilityOffCooldown += Raise.EventWith(new EAOnAbilityOffCooldown(abilityThree));
            eventTester.AssertFired(1);
        }
    }
}