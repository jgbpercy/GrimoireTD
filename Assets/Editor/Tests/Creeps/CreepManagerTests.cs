using NUnit.Framework;
using NSubstitute;
using GrimoireTD.Dependencies;
using GrimoireTD.Creeps;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GrimoireTD.Tests.CreepManagerTests
{
    public class CreepManagerTests
    {
        //Primitives and Basic Objects
        private float idleTimeToTrackAfterSpawnEnd = 5f;

        private float wave1Timing1 = 0.5f;
        private float wave1Timing2 = 1f;
        private float wave1Timing3 = 2f;

        private float wave2Timing1 = 0.6f;

        //Model and Frame Updater
        private FrameUpdaterStub frameUpdater;

        private IReadOnlyGameModel gameModel = Substitute.For<IGameModel>();

        private IReadOnlyGameStateManager gameStateManager = Substitute.For<IReadOnlyGameStateManager>();

        //Other Deps Passed To Ctor or SetUp
        private List<IWaveTemplate> waves;

        private IWaveTemplate waveTemplate1 = Substitute.For<IWaveTemplate>();
        private IWaveTemplate waveTemplate2 = Substitute.For<IWaveTemplate>();

        private IWave wave1 = Substitute.For<IWave>();
        private IWave wave2 = Substitute.For<IWave>();

        private ICreepTemplate wave1CreepTemplate1 = Substitute.For<ICreepTemplate>();
        private ICreep wave1Creep1 = Substitute.For<ICreep>();
        private ICreepTemplate wave1CreepTemplate2 = Substitute.For<ICreepTemplate>();
        private ICreep wave1Creep2 = Substitute.For<ICreep>();
        private ICreepTemplate wave1CreepTemplate3 = Substitute.For<ICreepTemplate>();
        private ICreep wave1Creep3 = Substitute.For<ICreep>();

        private ICreepTemplate wave2CreepTemplate1 = Substitute.For<ICreepTemplate>();
        private ICreep wave2Creep1 = Substitute.For<ICreep>();

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            //Model and Frame Updater
            DepsProv.TheModelObjectFrameUpdater = () =>
            {
                return frameUpdater;
            };

            gameModel.GameStateManager.Returns(gameStateManager);

            DepsProv.SetTheGameModel(gameModel);

            //Other Deps Passed To Ctor or SetUp
            waves = new List<IWaveTemplate>
            {
                waveTemplate1,
                waveTemplate2
            };

            waveTemplate1.GenerateWave().Returns(wave1);
            waveTemplate2.GenerateWave().Returns(wave2);

            wave1CreepTemplate1.GenerateCreep(Vector3.zero).Returns(wave1Creep1);
            wave1CreepTemplate2.GenerateCreep(Vector3.zero).Returns(wave1Creep2);
            wave1CreepTemplate3.GenerateCreep(Vector3.zero).Returns(wave1Creep3);

            wave2CreepTemplate1.GenerateCreep(Vector3.zero).Returns(wave2Creep1);
        }

        [SetUp]
        public void EachTestSetUp()
        {
            frameUpdater = new FrameUpdaterStub();

            gameStateManager.CurrentGameMode.Returns(GameMode.DEFEND);

            wave1.Spawns.Returns(new Dictionary<float, ICreepTemplate>
            {
                { wave1Timing1, wave1CreepTemplate1 },
                { wave1Timing2, wave1CreepTemplate2 },
                { wave1Timing3, wave1CreepTemplate3 }
            });

            wave2.Spawns.Returns(new Dictionary<float, ICreepTemplate>
            {
                { wave2Timing1, wave2CreepTemplate1 }
            });

            wave1.NextSpawnTime().Returns(wave1Timing1);
            wave2.NextSpawnTime().Returns(wave2Timing1);

            wave1.DequeueNextCreep().Returns(wave1CreepTemplate1);
            wave2.DequeueNextCreep().Returns(wave2CreepTemplate1);

            wave1.CreepsRemaining().Returns(true);
            wave2.CreepsRemaining().Returns(true);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            typeof(DepsProv).TypeInitializer.Invoke(null, null);
        }

        private CCreepManager ConstructAndSetUpSubject()
        {
            var subject = new CCreepManager();

            subject.SetUp(waves, idleTimeToTrackAfterSpawnEnd);

            return subject;
        }

        [Test]
        public void CreepManager_OnConstructionAndSetUp_HasAnEmptyCreepList()
        {
            var subject = ConstructAndSetUpSubject();

            Assert.AreEqual(0, subject.CreepList.Count);
        }

        [Test]
        public void FrameUpdate_WhenInBuildMode_DoesNotSpawnCreeps()
        {
            gameStateManager.CurrentGameMode.Returns(GameMode.BUILD);

            var subject = ConstructAndSetUpSubject();

            var eventTester = new EventTester<EAOnCreepSpawned>();
            subject.OnCreepSpawned += eventTester.Handler;

            frameUpdater.RunUpdate(wave1Timing1);
            frameUpdater.RunUpdate(wave2Timing1);

            Assert.AreEqual(0, subject.CreepList.Count);
            eventTester.AssertFired(false);
        }

        [Test]
        public void FrameUpdate_WhenInDefendMode_AddsCreepToCreepListWhenCreepShouldSpawn()
        {
            var subject = ConstructAndSetUpSubject();

            gameStateManager.OnEnterDefendMode += Raise.EventWith<EAOnEnterDefendMode>();

            frameUpdater.RunUpdate(wave1Timing1 / 1.5f);

            Assert.AreEqual(0, subject.CreepList.Count);

            frameUpdater.RunUpdate(wave1Timing1 / 1.5f);

            Assert.AreEqual(1, subject.CreepList.Count);
            Assert.AreEqual(wave1Creep1, subject.CreepList[0]);
        }

        [Test]
        public void FrameUpdate_WhenInDefendMode_FiresOnCreepSpawnedEventWhenCreepShouldSpawn()
        {
            var subject = ConstructAndSetUpSubject();

            var eventTester = new EventTester<EAOnCreepSpawned>();
            subject.OnCreepSpawned += eventTester.Handler;

            gameStateManager.OnEnterDefendMode += Raise.EventWith<EAOnEnterDefendMode>();

            frameUpdater.RunUpdate(wave1Timing1 / 1.5f);

            eventTester.AssertFired(false);

            frameUpdater.RunUpdate(wave1Timing1 / 1.5f);

            eventTester.AssertFired(1);
            eventTester.AssertResult(subject, args => args.NewCreep == wave1Creep1);
        }

        [Test]
        public void CreepManager_OnCreepDiedEventFromASpawnedCreep_RemovesCreepFromCreepList()
        {
            var subject = ConstructAndSetUpSubject();

            gameStateManager.OnEnterDefendMode += Raise.EventWith<EAOnEnterDefendMode>();

            frameUpdater.RunUpdate(wave1Timing1 / 1.5f);
            frameUpdater.RunUpdate(wave1Timing1 / 1.5f);

            Assert.AreEqual(1, subject.CreepList.Count);

            wave1Creep1.OnDied += Raise.Event();

            Assert.AreEqual(0, subject.CreepList.Count);
        }

        [Test]
        public void FrameUpdate_AfterSpawningFirstCreep_AddsSecondCreepToCreepListAtCorrectTime()
        {
            var subject = ConstructAndSetUpSubject();

            gameStateManager.OnEnterDefendMode += Raise.EventWith<EAOnEnterDefendMode>();

            frameUpdater.RunUpdate(wave1Timing1 / 1.9f);
            frameUpdater.RunUpdate(wave1Timing1 / 1.9f);

            wave1.NextSpawnTime().Returns(wave1Timing2);
            wave1.DequeueNextCreep().Returns(wave1CreepTemplate2);

            frameUpdater.RunUpdate((wave1Timing2 - wave1Timing1) / 1.9f);

            Assert.AreEqual(1, subject.CreepList.Count);

            frameUpdater.RunUpdate((wave1Timing2 - wave1Timing1) / 1.9f);

            Assert.AreEqual(2, subject.CreepList.Count);
            Assert.AreEqual(wave1Creep2, subject.CreepList[1]);
        }

        [Test]
        public void FrameUpdate_AfterSpawningFirstCreep_FiresOnCreepSpawnedEventWhenSecondCreepShouldSpawn()
        {
            var subject = ConstructAndSetUpSubject();

            var eventTester = new EventTester<EAOnCreepSpawned>();
            subject.OnCreepSpawned += eventTester.Handler;

            gameStateManager.OnEnterDefendMode += Raise.EventWith<EAOnEnterDefendMode>();

            frameUpdater.RunUpdate(wave1Timing1 / 1.9f);
            frameUpdater.RunUpdate(wave1Timing1 / 1.9f);

            wave1.NextSpawnTime().Returns(wave1Timing2);
            wave1.DequeueNextCreep().Returns(wave1CreepTemplate2);

            frameUpdater.RunUpdate((wave1Timing2 - wave1Timing1) / 1.9f);

            eventTester.AssertFired(1);

            frameUpdater.RunUpdate((wave1Timing2 - wave1Timing1) / 1.9f);

            eventTester.AssertFired(2);
            eventTester.AssertResult(subject, args => args.NewCreep == wave1Creep2);
        }

        [Test]
        public void FrameUpdate_AfterAllCreepsSpawnedAndDied_WaitsForIdleTimeToTrackThenFiresOnWaveOverEvent()
        {
            var subject = ConstructAndSetUpSubject();

            var eventTester = new EventTester<EAOnWaveOver>();
            subject.OnWaveOver += eventTester.Handler;

            gameStateManager.OnEnterDefendMode += Raise.EventWith<EAOnEnterDefendMode>();

            frameUpdater.RunUpdate(wave1Timing1 / 1.9f);
            frameUpdater.RunUpdate(wave1Timing1 / 1.9f);

            eventTester.AssertFired(false);

            wave1.NextSpawnTime().Returns(wave1Timing2);
            wave1.DequeueNextCreep().Returns(wave1CreepTemplate2);

            frameUpdater.RunUpdate((wave1Timing2 - wave1Timing1) / 1.9f);
            frameUpdater.RunUpdate((wave1Timing2 - wave1Timing1) / 1.9f);

            eventTester.AssertFired(false);

            wave1.NextSpawnTime().Returns(wave1Timing3);
            wave1.DequeueNextCreep().Returns(wave1CreepTemplate3);

            frameUpdater.RunUpdate((wave1Timing3 - wave1Timing2) / 1.9f);

            wave1.CreepsRemaining().Returns(false);
            wave1.NextSpawnTime().Returns(0f);

            frameUpdater.RunUpdate((wave1Timing3 - wave1Timing2) / 1.9f);

            eventTester.AssertFired(false);

            wave1Creep1.OnDied += Raise.Event();
            wave1Creep2.OnDied += Raise.Event();
            wave1Creep3.OnDied += Raise.Event();

            eventTester.AssertFired(false);

            frameUpdater.RunUpdate(idleTimeToTrackAfterSpawnEnd / 0.9f);

            eventTester.AssertFired(1);
        }

        [Test]
        public void FrameUpdate_AfterAllCreepsSpawnAndIdleTimeToTrackElapsed_WaitsForAllCreepsToDieThenFiresOnWaveOverEvent()
        {
            var subject = ConstructAndSetUpSubject();

            var eventTester = new EventTester<EAOnWaveOver>();
            subject.OnWaveOver += eventTester.Handler;

            gameStateManager.OnEnterDefendMode += Raise.EventWith<EAOnEnterDefendMode>();

            frameUpdater.RunUpdate(wave1Timing1 / 1.9f);
            frameUpdater.RunUpdate(wave1Timing1 / 1.9f);

            eventTester.AssertFired(false);

            wave1.NextSpawnTime().Returns(wave1Timing2);
            wave1.DequeueNextCreep().Returns(wave1CreepTemplate2);

            frameUpdater.RunUpdate((wave1Timing2 - wave1Timing1) / 1.9f);
            frameUpdater.RunUpdate((wave1Timing2 - wave1Timing1) / 1.9f);

            eventTester.AssertFired(false);

            wave1.NextSpawnTime().Returns(wave1Timing3);
            wave1.DequeueNextCreep().Returns(wave1CreepTemplate3);

            frameUpdater.RunUpdate((wave1Timing3 - wave1Timing2) / 1.9f);

            wave1.CreepsRemaining().Returns(false);
            wave1.NextSpawnTime().Returns(0f);

            frameUpdater.RunUpdate((wave1Timing3 - wave1Timing2) / 1.9f);

            eventTester.AssertFired(false);

            frameUpdater.RunUpdate(idleTimeToTrackAfterSpawnEnd / 0.9f);

            eventTester.AssertFired(false);

            wave1Creep1.OnDied += Raise.Event();
            wave1Creep2.OnDied += Raise.Event();
            wave1Creep3.OnDied += Raise.Event();

            frameUpdater.RunUpdate(0.00001f);

            eventTester.AssertFired(1);
        }

        [Test]
        public void FrameUpdate_AfterFirstWaveEndedAndGameEnteredDefendModeAgain_StartsSpawningSecondWave()
        {
            var subject = ConstructAndSetUpSubject();

            gameStateManager.OnEnterDefendMode += Raise.EventWith<EAOnEnterDefendMode>();

            frameUpdater.RunUpdate(wave1Timing1 / 1.9f);
            frameUpdater.RunUpdate(wave1Timing1 / 1.9f);

            wave1.NextSpawnTime().Returns(wave1Timing2);
            wave1.DequeueNextCreep().Returns(wave1CreepTemplate2);

            frameUpdater.RunUpdate((wave1Timing2 - wave1Timing1) / 1.9f);
            frameUpdater.RunUpdate((wave1Timing2 - wave1Timing1) / 1.9f);

            wave1.NextSpawnTime().Returns(wave1Timing3);
            wave1.DequeueNextCreep().Returns(wave1CreepTemplate3);

            frameUpdater.RunUpdate((wave1Timing3 - wave1Timing2) / 1.9f);

            wave1.CreepsRemaining().Returns(false);
            wave1.NextSpawnTime().Returns(0f);

            frameUpdater.RunUpdate((wave1Timing3 - wave1Timing2) / 1.9f);

            wave1Creep1.OnDied += Raise.Event();
            wave1Creep2.OnDied += Raise.Event();
            wave1Creep3.OnDied += Raise.Event();

            frameUpdater.RunUpdate(idleTimeToTrackAfterSpawnEnd / 0.9f);

            Assert.AreEqual(0, subject.CreepList.Count);

            gameStateManager.OnEnterBuildMode += Raise.EventWith<EAOnEnterBuildMode>();
            gameStateManager.CurrentGameMode.Returns(GameMode.BUILD);

            frameUpdater.RunUpdate(0.5f);

            gameStateManager.CurrentGameMode.Returns(GameMode.DEFEND);
            gameStateManager.OnEnterDefendMode += Raise.EventWith<EAOnEnterDefendMode>();

            frameUpdater.RunUpdate(wave2Timing1 / 1.9f);

            Assert.AreEqual(0, subject.CreepList.Count);

            frameUpdater.RunUpdate(wave2Timing1 / 1.9f);

            Assert.AreEqual(1, subject.CreepList.Count);
            Assert.AreEqual(wave2Creep1, subject.CreepList[0]);
        }

        [Test]
        public void CreepList_Always_IsSortedByDistanceFromEndAfterFrameUpdate()
        {
            var subject = ConstructAndSetUpSubject();

            gameStateManager.OnEnterDefendMode += Raise.EventWith<EAOnEnterDefendMode>();

            wave1Creep1.DistanceFromEnd.Returns(2f);
            wave1Creep2.DistanceFromEnd.Returns(3f);
            wave1Creep3.DistanceFromEnd.Returns(1f);

            frameUpdater.RunUpdate(wave1Timing1 / 1.9f);
            frameUpdater.RunUpdate(wave1Timing1 / 1.9f);

            wave1.NextSpawnTime().Returns(wave1Timing2);
            wave1.DequeueNextCreep().Returns(wave1CreepTemplate2);

            frameUpdater.RunUpdate((wave1Timing2 - wave1Timing1) / 1.9f);
            frameUpdater.RunUpdate((wave1Timing2 - wave1Timing1) / 1.9f);

            wave1.NextSpawnTime().Returns(wave1Timing3);
            wave1.DequeueNextCreep().Returns(wave1CreepTemplate3);

            frameUpdater.RunUpdate((wave1Timing3 - wave1Timing2) / 1.9f);

            wave1.CreepsRemaining().Returns(false);
            wave1.NextSpawnTime().Returns(0f);

            frameUpdater.RunUpdate((wave1Timing3 - wave1Timing2) / 1.9f);

            Assert.AreEqual(wave1Creep3, subject.CreepList[0]);
            Assert.AreEqual(wave1Creep1, subject.CreepList[1]);
            Assert.AreEqual(wave1Creep2, subject.CreepList[2]);
        }
    }
}