using System;
using NUnit.Framework;
using GrimoireTD.Dependencies;
using GrimoireTD.TemporaryEffects;

namespace GrimoireTD.Tests.TemporaryEffectTests
{
    public class TemporaryEffectTests
    {
        //Primitives and Basic Objects
        private float magnitude = 5f;
        private float duration = 3f;

        private string effectName = "Effect!";

        private float defaultDeltaTime = 0.2f;

        //Model and Frame Updater
        FrameUpdaterStub frameUpdater = new FrameUpdaterStub();

        //Other Deps Passed To Ctor or SetUp
        private object key = new object();

        private EventHandler<EAOnTemporaryEffectEnd> onEndEvent;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            //Model and Frame Updater
            DepsProv.TheModelObjectFrameUpdater = () => frameUpdater;
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            typeof(DepsProv).TypeInitializer.Invoke(null, null);
        }

        private CTemporaryEffect ConstructSubject()
        {
            return new CTemporaryEffect(key, magnitude, duration, effectName, onEndEvent);
        }

        [Test]
        public void FrameUpdate_Always_IncreasesElapsedTime()
        {
            var subject = ConstructSubject();

            frameUpdater.RunUpdate(defaultDeltaTime);

            Assert.AreEqual(defaultDeltaTime, subject.Elapsed);
        }

        [Test]
        public void TimeRemaining_Always_ReturnsDurationMinusElapsed()
        {
            var subject = ConstructSubject();

            frameUpdater.RunUpdate(defaultDeltaTime);

            Assert.AreEqual(duration - defaultDeltaTime, subject.TimeRemaining);
        }

        [Test]
        public void FrameUpdate_WhenElapsedExceedsDuration_FiresOnEndEvent()
        {
            var subject = ConstructSubject();

            var eventTester = new EventTester<EAOnTemporaryEffectEnd>();
            subject.OnTemporaryEffectEnd += eventTester.Handler;

            frameUpdater.RunUpdate((duration / 2) + 0.1f);

            eventTester.AssertFired(false);

            frameUpdater.RunUpdate((duration / 2) + 0.1f);

            eventTester.AssertFired(1);
        }

        [Test]
        public void EndNow_Always_FiresOnEndEvent()
        {
            var subject = ConstructSubject();

            var eventTester = new EventTester<EAOnTemporaryEffectEnd>();
            subject.OnTemporaryEffectEnd += eventTester.Handler;

            subject.EndNow();

            eventTester.AssertFired(1);
        }
    }
}