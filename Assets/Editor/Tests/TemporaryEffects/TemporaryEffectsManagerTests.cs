using NUnit.Framework;
using NSubstitute;
using GrimoireTD.Dependencies;
using GrimoireTD.TemporaryEffects;
using System;

namespace GrimoireTD.Tests.TemporaryEffectsManagerTests
{
    public class TemporaryEffectsManagerTests
    {
        //Primitives and Basic Objects
        private float effectDefaultMagnitude = 10f;
        private float effectDefaultDuration = 12f;

        private string effectName = "effectName";

        private string supersedingEffectName = "supersedingEffectName";

        private string notSupersedingEffectName = "notSupersedingEffectName";

        //Other Deps Passed To Ctor or SetUp
        private object effectKey = Substitute.For<object>();

        private EventHandler<EAOnTemporaryEffectEnd> endEffectHandler;

        [Test]
        public void ApplyEffect_NoCurrentEffectWithSameKey_AddsEffect()
        {
            var subject = new CTemporaryEffectsManager();

            subject.ApplyEffect(effectKey, effectDefaultMagnitude, effectDefaultDuration, effectName, () => { }, endEffectHandler);

            Assert.AreEqual(1, subject.EffectList.Count);
            Assert.AreEqual(effectName, subject.EffectList[0].EffectName);
            Assert.AreEqual(effectDefaultDuration, subject.EffectList[0].Duration);
            Assert.AreEqual(effectDefaultMagnitude, subject.EffectList[0].Magnitude);
        }

        [Test]
        public void ApplyEffect_NoCurrentEffectWithSameKey_InvokesOnApplyCallback()
        {
            var subject = new CTemporaryEffectsManager();

            var applyCallbackFired = false;

            Action applyCallback = () =>
            {
                applyCallbackFired = true;
            };

            subject.ApplyEffect(effectKey, effectDefaultMagnitude, effectDefaultDuration, effectName, applyCallback, endEffectHandler);

            Assert.True(applyCallbackFired);
        }

        [Test]
        public void ApplyEffect_NoCurrentEffectWithSameKey_FiresOnNewTemporaryEffectEvent()
        {
            var subject = new CTemporaryEffectsManager();

            var eventTester = new EventTester<EAOnNewTemporaryEffect>();
            subject.OnNewTemporaryEffect += eventTester.Handler;

            subject.ApplyEffect(effectKey, effectDefaultMagnitude, effectDefaultDuration, effectName, () => { }, endEffectHandler);

            eventTester.AssertFired(1);
            eventTester.AssertResult(subject, args => args.NewEffect.EffectName == effectName);
        }

        [Test]
        public void ApplyEffect_CurrentEffectLowerMagnitude_AddsEffect()
        {
            var subject = new CTemporaryEffectsManager();

            subject.ApplyEffect(effectKey, effectDefaultMagnitude, effectDefaultDuration, effectName, () => { }, endEffectHandler);

            subject.ApplyEffect(effectKey, effectDefaultMagnitude + 3f, effectDefaultDuration, supersedingEffectName, () => { }, endEffectHandler);

            Assert.AreEqual(1, subject.EffectList.Count);
            Assert.AreEqual(supersedingEffectName, subject.EffectList[0].EffectName);
            AssertExt.Approximately(effectDefaultDuration, subject.EffectList[0].Duration);
            AssertExt.Approximately(effectDefaultMagnitude + 3f, subject.EffectList[0].Magnitude);
        }

        [Test]
        public void ApplyEffect_CurrentEffectLowerMagnitude_InvokesOnApplyCallback()
        {
            var subject = new CTemporaryEffectsManager();

            var applyCallbackFired = false;

            Action applyCallback = () =>
            {
                applyCallbackFired = true;
            };

            subject.ApplyEffect(effectKey, effectDefaultMagnitude, effectDefaultDuration, effectName, () => { }, endEffectHandler);

            subject.ApplyEffect(effectKey, effectDefaultMagnitude + 3f, effectDefaultDuration, supersedingEffectName, applyCallback, endEffectHandler);

            Assert.True(applyCallbackFired);
        }

        [Test]
        public void ApplyEffect_CurrentEffectLowerMagnitude_FiresOnNewTemporaryEffectEvent()
        {
            var subject = new CTemporaryEffectsManager();

            var eventTester = new EventTester<EAOnNewTemporaryEffect>();
            subject.OnNewTemporaryEffect += eventTester.Handler;

            subject.ApplyEffect(effectKey, effectDefaultMagnitude, effectDefaultDuration, effectName, () => { }, endEffectHandler);

            subject.ApplyEffect(effectKey, effectDefaultMagnitude + 3f, effectDefaultDuration, supersedingEffectName, () => { }, endEffectHandler);

            eventTester.AssertFired(2);
            eventTester.AssertResults(args => args[1].NewEffect.EffectName == supersedingEffectName);
        }

        [Test]
        public void ApplyEffect_CurrentEffectLowerMagnitude_EndsCurrentEffect()
        {
            var subject = new CTemporaryEffectsManager();

            subject.ApplyEffect(effectKey, effectDefaultMagnitude, effectDefaultDuration, effectName, () => { }, endEffectHandler);

            var firstEffect = subject.EffectList[0];

            var eventTester = new EventTester<EAOnTemporaryEffectEnd>();
            firstEffect.OnTemporaryEffectEnd += eventTester.Handler;

            subject.ApplyEffect(effectKey, effectDefaultMagnitude + 3, effectDefaultDuration, supersedingEffectName, () => { }, endEffectHandler);

            eventTester.AssertFired(1);
            eventTester.AssertResult(firstEffect, args => args.EndedEffect == firstEffect);
        }

        [Test]
        public void ApplyEffect_CurrentEffectAproxSameMagnitudeButLowerDuration_AddsEffect()
        {
            var subject = new CTemporaryEffectsManager();

            subject.ApplyEffect(effectKey, effectDefaultMagnitude, effectDefaultDuration, effectName, () => { }, endEffectHandler);

            subject.ApplyEffect(effectKey, effectDefaultMagnitude, effectDefaultDuration + 3f, supersedingEffectName, () => { }, endEffectHandler);

            Assert.AreEqual(1, subject.EffectList.Count);
            Assert.AreEqual(supersedingEffectName, subject.EffectList[0].EffectName);
            AssertExt.Approximately(effectDefaultDuration + 3f, subject.EffectList[0].Duration);
            AssertExt.Approximately(effectDefaultMagnitude, subject.EffectList[0].Magnitude);
        }

        [Test]
        public void ApplyEffect_CurrentEffectAproxSameMagnitudeButLowerDuration_InvokesOnApplyCallback()
        {
            var subject = new CTemporaryEffectsManager();

            var applyCallbackFired = false;

            Action applyCallback = () =>
            {
                applyCallbackFired = true;
            };

            subject.ApplyEffect(effectKey, effectDefaultMagnitude, effectDefaultDuration, effectName, () => { }, endEffectHandler);

            subject.ApplyEffect(effectKey, effectDefaultMagnitude, effectDefaultDuration + 3f, supersedingEffectName, applyCallback, endEffectHandler);

            Assert.True(applyCallbackFired);
        }

        [Test]
        public void ApplyEffect_CurrentEffectAproxSameMagnitudeButLowerDuration_FiresOnNewTemporaryEffectEvent()
        {
            var subject = new CTemporaryEffectsManager();

            var eventTester = new EventTester<EAOnNewTemporaryEffect>();
            subject.OnNewTemporaryEffect += eventTester.Handler;

            subject.ApplyEffect(effectKey, effectDefaultMagnitude, effectDefaultDuration, effectName, () => { }, endEffectHandler);

            subject.ApplyEffect(effectKey, effectDefaultMagnitude, effectDefaultDuration + 3f, supersedingEffectName, () => { }, endEffectHandler);

            eventTester.AssertFired(2);
            eventTester.AssertResults(args => args[1].NewEffect.EffectName == supersedingEffectName);
        }

        [Test]
        public void ApplyEffect_CurrentEffectAproxSameMagnitudeButLowerDuration_EndsCurrentEffect()
        {
            var subject = new CTemporaryEffectsManager();

            subject.ApplyEffect(effectKey, effectDefaultMagnitude, effectDefaultDuration, effectName, () => { }, endEffectHandler);

            var firstEffect = subject.EffectList[0];

            var eventTester = new EventTester<EAOnTemporaryEffectEnd>();
            firstEffect.OnTemporaryEffectEnd += eventTester.Handler;

            subject.ApplyEffect(effectKey, effectDefaultMagnitude, effectDefaultDuration + 3f, supersedingEffectName, () => { }, endEffectHandler);

            eventTester.AssertFired(1);
            eventTester.AssertResult(firstEffect, args => args.EndedEffect == firstEffect);
        }

        [Test]
        public void ApplyEffect_CurrentEffectGreaterMagnitude_DoesNotApplyNewEffect()
        {
            var subject = new CTemporaryEffectsManager();

            subject.ApplyEffect(effectKey, effectDefaultMagnitude, effectDefaultDuration, effectName, () => { }, endEffectHandler);
        
            subject.ApplyEffect(effectKey, effectDefaultMagnitude - 1f, effectDefaultDuration, notSupersedingEffectName, () => { }, endEffectHandler);

            Assert.AreEqual(1, subject.EffectList.Count);
            Assert.AreEqual(effectName, subject.EffectList[0].EffectName);
            Assert.AreEqual(effectDefaultDuration, subject.EffectList[0].Duration);
            Assert.AreEqual(effectDefaultMagnitude, subject.EffectList[0].Magnitude);
        }

        [Test]
        public void ApplyEffect_CurrentEffectGreaterMagnitude_DoesNotInvokeOnApplyCallback()
        {
            var subject = new CTemporaryEffectsManager();

            var applyCallbackFired = false;

            Action applyCallback = () =>
            {
                applyCallbackFired = true;
            };

            subject.ApplyEffect(effectKey, effectDefaultMagnitude, effectDefaultDuration, effectName, () => { }, endEffectHandler);

            subject.ApplyEffect(effectKey, effectDefaultMagnitude - 1f, effectDefaultDuration, notSupersedingEffectName, applyCallback, endEffectHandler);

            Assert.False(applyCallbackFired);
        }

        [Test]
        public void ApplyEffect_CurrentEffectGreaterMagnitude_DoesNotFireOnNewTemporaryEffectEvent()
        {
            var subject = new CTemporaryEffectsManager();

            subject.ApplyEffect(effectKey, effectDefaultMagnitude, effectDefaultDuration, effectName, () => { }, endEffectHandler);

            var eventTester = new EventTester<EAOnNewTemporaryEffect>();
            subject.OnNewTemporaryEffect += eventTester.Handler;

            subject.ApplyEffect(effectKey, effectDefaultMagnitude - 1f, effectDefaultDuration, notSupersedingEffectName, () => { }, endEffectHandler);

            eventTester.AssertFired(false);
        }

        [Test]
        public void ApplyEffect_CurrentEffectGreaterMagnitude_DoesNotEndCurrentEffect()
        {
            var subject = new CTemporaryEffectsManager();

            subject.ApplyEffect(effectKey, effectDefaultMagnitude, effectDefaultDuration, effectName, () => { }, endEffectHandler);

            var firstEffect = subject.EffectList[0];

            var eventTester = new EventTester<EAOnTemporaryEffectEnd>();
            firstEffect.OnTemporaryEffectEnd += eventTester.Handler;

            subject.ApplyEffect(effectKey, effectDefaultMagnitude - 1f, effectDefaultDuration, notSupersedingEffectName, () => { }, endEffectHandler);

            eventTester.AssertFired(false);
        }

        [Test]
        public void ApplyEffect_CurrentEffectAproxSameMagnitudeGreaterDuration_DoesNotApplyNewEffect()
        {
            var subject = new CTemporaryEffectsManager();

            subject.ApplyEffect(effectKey, effectDefaultMagnitude, effectDefaultDuration, effectName, () => { }, endEffectHandler);

            subject.ApplyEffect(effectKey, effectDefaultMagnitude, effectDefaultDuration - 1f, notSupersedingEffectName, () => { }, endEffectHandler);

            Assert.AreEqual(1, subject.EffectList.Count);
            Assert.AreEqual(effectName, subject.EffectList[0].EffectName);
            Assert.AreEqual(effectDefaultDuration, subject.EffectList[0].Duration);
            Assert.AreEqual(effectDefaultMagnitude, subject.EffectList[0].Magnitude);
        }

        [Test]
        public void ApplyEffect_CurrentEffectAproxSameMagnitudeGreaterDuration_DoesNotInvokeOnApplyCallback()
        {
            var subject = new CTemporaryEffectsManager();

            var applyCallbackFired = false;

            Action applyCallback = () =>
            {
                applyCallbackFired = true;
            };

            subject.ApplyEffect(effectKey, effectDefaultMagnitude, effectDefaultDuration, effectName, () => { }, endEffectHandler);

            subject.ApplyEffect(effectKey, effectDefaultMagnitude, effectDefaultDuration - 1f, notSupersedingEffectName, applyCallback, endEffectHandler);

            Assert.False(applyCallbackFired);
        }

        [Test]
        public void ApplyEffect_CurrentEffectAproxSameMagnitudeGreaterDuration_DoesNotFireOnNewTemporaryEffectEvent()
        {
            var subject = new CTemporaryEffectsManager();

            subject.ApplyEffect(effectKey, effectDefaultMagnitude, effectDefaultDuration, effectName, () => { }, endEffectHandler);

            var eventTester = new EventTester<EAOnNewTemporaryEffect>();
            subject.OnNewTemporaryEffect += eventTester.Handler;

            subject.ApplyEffect(effectKey, effectDefaultMagnitude, effectDefaultDuration - 1f, notSupersedingEffectName, () => { }, endEffectHandler);

            eventTester.AssertFired(false);
        }

        [Test]
        public void ApplyEffect_CurrentEffectAproxSameMagnitudeGreaterDuration_DoesNotEndCurrentEffect()
        {
            var subject = new CTemporaryEffectsManager();

            subject.ApplyEffect(effectKey, effectDefaultMagnitude, effectDefaultDuration, effectName, () => { }, endEffectHandler);

            var firstEffect = subject.EffectList[0];

            var eventTester = new EventTester<EAOnTemporaryEffectEnd>();
            firstEffect.OnTemporaryEffectEnd += eventTester.Handler;

            subject.ApplyEffect(effectKey, effectDefaultMagnitude, effectDefaultDuration - 1f, notSupersedingEffectName, () => { }, endEffectHandler);

            eventTester.AssertFired(false);
        }

        [Test]
        public void ApplyEffect_DifferentKeyToExistingEffects_AppliesEffect()
        {
            var subject = new CTemporaryEffectsManager();

            subject.ApplyEffect(effectKey, effectDefaultMagnitude, effectDefaultDuration, effectName, () => { }, endEffectHandler);

            subject.ApplyEffect(new object(), effectDefaultMagnitude - 1f, effectDefaultDuration - 1f, "other name", () => { }, endEffectHandler);

            Assert.AreEqual(2, subject.EffectList.Count);
            Assert.AreEqual(effectName, subject.EffectList[0].EffectName);
            AssertExt.Approximately(effectDefaultDuration, subject.EffectList[0].Duration);
            AssertExt.Approximately(effectDefaultMagnitude, subject.EffectList[0].Magnitude);
        }
    }
}