﻿using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using GrimoireTD.Abilities.DefendMode;
using GrimoireTD.Abilities;
using GrimoireTD.Defenders;
using GrimoireTD.Dependencies;

namespace GrimoireTD.Tests.DefendModeAbilityManagerTests
{
    public class DefendModeAbilityManagerTests
    {
        //Primitives and Basic Objects
        private float defaultDeltaTime = 0.2f;

        //Model and Frame Updater
        private FrameUpdaterStub frameUpdater;

        private IReadOnlyGameModel gameModel = Substitute.For<IReadOnlyGameModel>();

        private IReadOnlyGameModeManager gameModeManager = Substitute.For<IReadOnlyGameModeManager>();

        //Other Deps Passed To Ctor
        private IAbilities abilities = Substitute.For<IAbilities>();

        private IDefendModeAbility ability1 = Substitute.For<IDefendModeAbility>();
        private IDefendModeAbility ability2 = Substitute.For<IDefendModeAbility>();
        private IDefendModeAbility ability3 = Substitute.For<IDefendModeAbility>();

        private IDefender attachedToDefender = Substitute.For<IDefender>();

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            //Model and Frame Updater
            DepsProv.TheModelObjectFrameUpdater = () =>
            {
                return frameUpdater;
            };

            gameModel.GameModeManager.Returns(gameModeManager);

            DepsProv.SetTheGameModel(gameModel);
        }

        [SetUp]
        public void EachTestSetUp()
        {
            //Model and Frame Updater
            frameUpdater = new FrameUpdaterStub();

            gameModeManager.CurrentGameMode.Returns(GameMode.DEFEND);

            //Other Deps Passed To Ctor
            abilities.DefendModeAbilities().Returns(new List<IDefendModeAbility>
            {
                ability1,
                ability2,
                ability3
            });

            ability1.IsOffCooldown.Returns(true);
            ability2.IsOffCooldown.Returns(true);
            ability3.IsOffCooldown.Returns(true);

            ability1.ClearReceivedCalls();
            ability2.ClearReceivedCalls();
            ability3.ClearReceivedCalls();

            ability1.ExecuteAbility(Arg.Any<IDefender>()).Returns(true);
            ability2.ExecuteAbility(Arg.Any<IDefender>()).Returns(true);
            ability3.ExecuteAbility(Arg.Any<IDefender>()).Returns(true);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            typeof(DepsProv).TypeInitializer.Invoke(null, null);
        }

        private CDefendModeAbilityManager ConstructSubject()
        {
            return new CDefendModeAbilityManager(
                abilities, 
                attachedToDefender
            );
        }

        [Test]
        public void Manager_WhenInBuildMode_DoesNotExecuteOffCooldownAbilities()
        {
            gameModeManager.CurrentGameMode.Returns(GameMode.BUILD);

            ConstructSubject();

            frameUpdater.RunUpdate(defaultDeltaTime);

            ability1.DidNotReceive().ExecuteAbility(Arg.Any<IDefender>());
            ability2.DidNotReceive().ExecuteAbility(Arg.Any<IDefender>());
            ability3.DidNotReceive().ExecuteAbility(Arg.Any<IDefender>());
        }

        [Test]
        public void Manager_WhenInDefendMode_ExecutesTheFirstAbilityInTheListOnce()
        {
            ConstructSubject();

            frameUpdater.RunUpdate(defaultDeltaTime);

            ability1.Received(1).ExecuteAbility(attachedToDefender);
        }

        //Intended behaviour until upswing/backswing times/animations
        [Test]
        public void Manager_WhenMultipleAbilitiesAreOffCooldown_ExecutesOneAbilityPerUpdate()
        {
            ConstructSubject();

            frameUpdater.RunUpdate(defaultDeltaTime);

            ability1.Received(1).ExecuteAbility(attachedToDefender);
            ability2.DidNotReceive().ExecuteAbility(Arg.Any<IDefender>());

            ability1.OnAbilityExecuted += Raise.EventWith(new EAOnAbilityExecuted(ability1));

            frameUpdater.RunUpdate(defaultDeltaTime);

            ability2.Received(1).ExecuteAbility(attachedToDefender);
            ability3.DidNotReceive().ExecuteAbility(Arg.Any<IDefender>());
        }

        [Test]
        public void Manager_WhenMultipleAbilitiesAreOffCooldownAndTheFirstCannotExecute_ExecutesTheNextAbility()
        {
            ConstructSubject();

            ability1.ExecuteAbility(Arg.Any<IDefender>()).Returns(false);

            frameUpdater.RunUpdate(defaultDeltaTime);

            ability2.Received(1).ExecuteAbility(attachedToDefender);
            ability3.DidNotReceive().ExecuteAbility(Arg.Any<IDefender>());

            ability2.OnAbilityExecuted += Raise.EventWith(new EAOnAbilityExecuted(ability2));

            frameUpdater.RunUpdate(defaultDeltaTime);

            ability3.Received(1).ExecuteAbility(attachedToDefender);
        }

        [Test]
        public void Manager_IfAllAbilitiesAreOnCooldown_DoesNotExecuteAny()
        {
            ability1.IsOffCooldown.Returns(false);
            ability2.IsOffCooldown.Returns(false);
            ability3.IsOffCooldown.Returns(false);

            ConstructSubject();

            frameUpdater.RunUpdate(defaultDeltaTime);

            ability1.DidNotReceive().ExecuteAbility(Arg.Any<IDefender>());
            ability2.DidNotReceive().ExecuteAbility(Arg.Any<IDefender>());
            ability3.DidNotReceive().ExecuteAbility(Arg.Any<IDefender>());
        }

        [Test]
        public void Manager_WhenANewAbillityIsAddedWhichIsOffCooldown_ExecutesThatAbility()
        {
            abilities.DefendModeAbilities().Returns(new List<IDefendModeAbility>());

            ConstructSubject();

            abilities.OnDefendModeAbilityAdded += Raise.EventWith(new EAOnDefendModeAbilityAdded(ability1));

            frameUpdater.RunUpdate(defaultDeltaTime);

            ability1.Received(1).ExecuteAbility(attachedToDefender);
        }

        [Test]
        public void Manager_WhenAnAbilityThatWasOnCooldownComesOffCooldown_ExecutesThatAbility()
        {
            ability1.IsOffCooldown.Returns(false);
            ability2.IsOffCooldown.Returns(false);
            ability3.IsOffCooldown.Returns(false);

            ConstructSubject();

            frameUpdater.RunUpdate(defaultDeltaTime);

            ability2.OnAbilityOffCooldown += Raise.EventWith(new EAOnAbilityOffCooldown(ability2));

            frameUpdater.RunUpdate(defaultDeltaTime);

            ability2.Received(1).ExecuteAbility(attachedToDefender);
        }

        [Test]
        public void Manager_WhenAnAbilityIsAddedOnCooldownWhichLaterComesOffCooldown_ExecutesThatAbility()
        {
            abilities.DefendModeAbilities().Returns(new List<IDefendModeAbility>());

            ability3.IsOffCooldown.Returns(false);

            ConstructSubject();

            frameUpdater.RunUpdate(defaultDeltaTime);

            abilities.OnDefendModeAbilityAdded += Raise.EventWith(new EAOnDefendModeAbilityAdded(ability3));

            frameUpdater.RunUpdate(defaultDeltaTime);

            ability3.OnAbilityOffCooldown += Raise.EventWith(new EAOnAbilityOffCooldown(ability3));

            frameUpdater.RunUpdate(defaultDeltaTime);

            ability3.Received(1).ExecuteAbility(attachedToDefender);
        }

        [Test]
        public void Manager_WhenAbilityIsRemoved_DoesNotExecuteItWhenItComesOffCooldown()
        {
            ability1.IsOffCooldown.Returns(false);

            abilities.DefendModeAbilities().Returns(new List<IDefendModeAbility> { ability1 });

            ConstructSubject();

            frameUpdater.RunUpdate(defaultDeltaTime);

            abilities.OnDefendModeAbilityRemoved += Raise.EventWith(new EAOnDefendModeAbilityRemoved(ability1));

            frameUpdater.RunUpdate(defaultDeltaTime);

            ability1.OnAbilityOffCooldown += Raise.EventWith(new EAOnAbilityOffCooldown(ability1));

            frameUpdater.RunUpdate(defaultDeltaTime);

            ability1.DidNotReceive().ExecuteAbility(Arg.Any<IDefender>());
        }

        [Test]
        public void Manager_WhenAllAbilitiesComeOffColldown_EmitsEventAtCorrectTime()
        {
            ability1.IsOffCooldown.Returns(false);
            ability2.IsOffCooldown.Returns(false);
            ability3.IsOffCooldown.Returns(false);

            var subject = ConstructSubject();

            var eventTester = new EventTester<EAOnAllDefendModeAbilitiesOffCooldown>();
            subject.OnAllDefendModeAbilitiesOffCooldown += eventTester.Handler;

            frameUpdater.RunUpdate(defaultDeltaTime);
            ability1.OnAbilityOffCooldown += Raise.EventWith(new EAOnAbilityOffCooldown(ability1));
            eventTester.AssertFired(false);

            frameUpdater.RunUpdate(defaultDeltaTime);
            ability2.OnAbilityOffCooldown += Raise.EventWith(new EAOnAbilityOffCooldown(ability2));
            eventTester.AssertFired(false);

            frameUpdater.RunUpdate(defaultDeltaTime);
            ability3.OnAbilityOffCooldown += Raise.EventWith(new EAOnAbilityOffCooldown(ability3));
            eventTester.AssertFired(1);
        }

        [Test]
        public void Manager_WhenAnAbilityWasAddedAndAllAbilitiesComeOffCooldown_EmitsEventAtCorrectTime()
        {
            ability1.IsOffCooldown.Returns(false);
            ability2.IsOffCooldown.Returns(false);
            ability3.IsOffCooldown.Returns(false);

            abilities.DefendModeAbilities().Returns(new List<IDefendModeAbility> { ability1, ability2 });

            var subject = ConstructSubject();

            var eventTester = new EventTester<EAOnAllDefendModeAbilitiesOffCooldown>();
            subject.OnAllDefendModeAbilitiesOffCooldown += eventTester.Handler;

            frameUpdater.RunUpdate(defaultDeltaTime);
            ability1.OnAbilityOffCooldown += Raise.EventWith(new EAOnAbilityOffCooldown(ability1));
            eventTester.AssertFired(false);

            frameUpdater.RunUpdate(defaultDeltaTime);
            abilities.OnDefendModeAbilityAdded += Raise.EventWith(new EAOnDefendModeAbilityAdded(ability3));

            frameUpdater.RunUpdate(defaultDeltaTime);
            ability3.OnAbilityOffCooldown += Raise.EventWith(new EAOnAbilityOffCooldown(ability3));
            eventTester.AssertFired(false);

            frameUpdater.RunUpdate(defaultDeltaTime);
            ability2.OnAbilityOffCooldown += Raise.EventWith(new EAOnAbilityOffCooldown(ability2));
            eventTester.AssertFired(1);
        }

        [Test]
        public void Manager_WhenAnAbilityWasRemovedAndAllOtherAbilitiesComeOffCooldown_EmitsEventAtCorrectTime()
        {
            ability1.IsOffCooldown.Returns(false);
            ability2.IsOffCooldown.Returns(false);
            ability3.IsOffCooldown.Returns(false);

            var subject = ConstructSubject();

            var eventTester = new EventTester<EAOnAllDefendModeAbilitiesOffCooldown>();
            subject.OnAllDefendModeAbilitiesOffCooldown += eventTester.Handler;

            frameUpdater.RunUpdate(defaultDeltaTime);
            ability3.OnAbilityOffCooldown += Raise.EventWith(new EAOnAbilityOffCooldown(ability3));
            eventTester.AssertFired(false);

            frameUpdater.RunUpdate(defaultDeltaTime);
            abilities.OnDefendModeAbilityRemoved += Raise.EventWith(new EAOnDefendModeAbilityRemoved(ability2));

            frameUpdater.RunUpdate(defaultDeltaTime);
            ability1.OnAbilityOffCooldown += Raise.EventWith(new EAOnAbilityOffCooldown(ability1));
            eventTester.AssertFired(1);
        }

        [Test]
        public void Manager_WhenAnAbilityComesOffCooldownAndThenIsExecutedAndThenAllAbilitiesComeOffCooldown_EmitsEventAtCorrectTime()
        {
            ability1.IsOffCooldown.Returns(false);
            ability2.IsOffCooldown.Returns(false);
            ability3.IsOffCooldown.Returns(false);

            var subject = ConstructSubject();

            var eventTester = new EventTester<EAOnAllDefendModeAbilitiesOffCooldown>();
            subject.OnAllDefendModeAbilitiesOffCooldown += eventTester.Handler;

            frameUpdater.RunUpdate(defaultDeltaTime);
            ability3.OnAbilityOffCooldown += Raise.EventWith(new EAOnAbilityOffCooldown(ability3));
            eventTester.AssertFired(false);

            frameUpdater.RunUpdate(defaultDeltaTime);
            ability2.OnAbilityOffCooldown += Raise.EventWith(new EAOnAbilityOffCooldown(ability2));
            eventTester.AssertFired(false);

            frameUpdater.RunUpdate(defaultDeltaTime);
            ability3.OnAbilityExecuted += Raise.EventWith(new EAOnAbilityExecuted(ability3));
            eventTester.AssertFired(false);

            frameUpdater.RunUpdate(defaultDeltaTime);
            ability1.OnAbilityOffCooldown += Raise.EventWith(new EAOnAbilityOffCooldown(ability1));
            eventTester.AssertFired(false);

            frameUpdater.RunUpdate(defaultDeltaTime);
            ability3.OnAbilityOffCooldown += Raise.EventWith(new EAOnAbilityOffCooldown(ability3));
            eventTester.AssertFired(1);
        }
    }
}