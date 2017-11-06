using System;
using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using GrimoireTD.Attributes;

namespace GrimoireTD.Tests.AttributesTests
{
    public class AttributesTests
    {
        private INamedAttributeModifier<DeAttrName> damageModifier = Substitute.For<INamedAttributeModifier<DeAttrName>>();

        private INamedAttributeModifier<DeAttrName> cooldownModifier = Substitute.For<INamedAttributeModifier<DeAttrName>>();

        private INamedAttributeModifier<DeAttrName> rangeModifier = Substitute.For<INamedAttributeModifier<DeAttrName>>();

        private IAttribute fakeDamageBonusAttribute = Substitute.For<IAttribute>();
        private IAttribute fakeCooldownAttribute = Substitute.For<IAttribute>();

        private Dictionary<DeAttrName, IAttribute> attributeSetupDictionary;

        private CAttributes<DeAttrName> subject;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            damageModifier.AttributeName.Returns(DeAttrName.damageBonus);
            cooldownModifier.AttributeName.Returns(DeAttrName.cooldownReduction);
            rangeModifier.AttributeName.Returns(DeAttrName.rangeBonus);

            attributeSetupDictionary = new Dictionary<DeAttrName, IAttribute>
            {
                { DeAttrName.damageBonus, fakeDamageBonusAttribute },
                { DeAttrName.cooldownReduction, fakeCooldownAttribute }
            };

            subject = new CAttributes<DeAttrName>(attributeSetupDictionary);
        }

        [SetUp]
        public void EachTestSetUp()
        {
            fakeCooldownAttribute.ClearReceivedCalls();
            fakeDamageBonusAttribute.ClearReceivedCalls();
        }

        [Test]
        public void AddModifier_PassedAttributeNameNotInDictionary_ThrowsException()
        {
            Assert.Throws(typeof(ArgumentException), () => subject.AddModifier(rangeModifier));
        }

        [Test]
        public void AddModifier_PassedValidModifier_AddsModifierToCorrectAttribute()
        {
            subject.AddModifier(cooldownModifier);

            fakeCooldownAttribute.Received(1).AddModifier(cooldownModifier);
        }

        [Test]
        public void AddModifier_PassedValidModifer_FiresOnAnyAttributeChangedEventWithNewValue()
        {
            var newValue = 5f;

            fakeCooldownAttribute.Value().Returns(newValue);

            var eventTester = new EventTester<EAOnAnyAttributeChanged<DeAttrName>>();
            subject.OnAnyAttributeChanged += eventTester.Handler;

            subject.AddModifier(cooldownModifier);

            eventTester.AssertFired(1);
            eventTester.AssertResult(
                subject, 
                (args) => args.AttributeName == DeAttrName.cooldownReduction && args.NewValue == newValue
            );
        }

        [Test]
        public void TryRemoveModifier_PassedAttributeNameNotInDictionary_ThrowsException()
        {
            Assert.Throws(typeof(ArgumentException), () => subject.TryRemoveModifier(rangeModifier));
        }

        [Test]
        public void TryRemoveModifier_PassedValidModifier_AttemptsToRemoveFromAttribute()
        {
            subject.TryRemoveModifier(cooldownModifier);

            fakeCooldownAttribute.Received(1).TryRemoveModifier(cooldownModifier);
        }

        [Test]
        public void TryRemoveModifier_PassedValidModifierThatReturnsFalseFromAttributeTryRemove_ReturnsFalse()
        {
            fakeCooldownAttribute.TryRemoveModifier(cooldownModifier).Returns(false);

            var result = subject.TryRemoveModifier(cooldownModifier);

            Assert.False(result);
        }

        [Test]
        public void TryRemoveModifier_PassedValidModifierThatReturnsTrueFromAttributeTryRemove_ReturnsTrue()
        {
            fakeCooldownAttribute.TryRemoveModifier(cooldownModifier).Returns(true);

            var result = subject.TryRemoveModifier(cooldownModifier);

            Assert.True(result);
        }

        [Test]
        public void TryRemoveModifier_PassedValidModifierThatReturnsTrueFromAttributeTryRemove_FiresOnAnyAttributeChangedEventWithNewValue()
        {
            var newValue = 3f;

            fakeCooldownAttribute.TryRemoveModifier(cooldownModifier).Returns(true);

            fakeCooldownAttribute.Value().Returns(newValue);

            var eventTester = new EventTester<EAOnAnyAttributeChanged<DeAttrName>>();
            subject.OnAnyAttributeChanged += eventTester.Handler;

            subject.TryRemoveModifier(cooldownModifier);

            eventTester.AssertFired(1);
            eventTester.AssertResult(
                subject,
                args => args.AttributeName == DeAttrName.cooldownReduction && args.NewValue == newValue
            );
        }

        [Test]
        public void Get_Always_ReturnsRequestedAttribute()
        {
            var result = subject.Get(DeAttrName.cooldownReduction);

            Assert.AreEqual(fakeCooldownAttribute, result);
        }
    }
}