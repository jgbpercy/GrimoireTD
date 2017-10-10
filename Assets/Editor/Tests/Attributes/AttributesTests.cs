using System;
using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using GrimoireTD.Attributes;

namespace GrimoireTD.Tests.AttributesTests
{
    public class AttributesTests
    {
        private INamedAttributeModifier<DEAttrName> damageModifier = Substitute.For<INamedAttributeModifier<DEAttrName>>();

        private INamedAttributeModifier<DEAttrName> cooldownModifier = Substitute.For<INamedAttributeModifier<DEAttrName>>();

        private INamedAttributeModifier<DEAttrName> rangeModifier = Substitute.For<INamedAttributeModifier<DEAttrName>>();

        private IAttribute fakeDamageBonusAttribute = Substitute.For<IAttribute>();
        private IAttribute fakeCooldownAttribute = Substitute.For<IAttribute>();

        private Dictionary<DEAttrName, IAttribute> attributeSetupDictionary;

        private CAttributes<DEAttrName> subject;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            damageModifier.AttributeName.Returns(DEAttrName.damageBonus);
            cooldownModifier.AttributeName.Returns(DEAttrName.cooldownReduction);
            rangeModifier.AttributeName.Returns(DEAttrName.rangeBonus);

            attributeSetupDictionary = new Dictionary<DEAttrName, IAttribute>
            {
                { DEAttrName.damageBonus, fakeDamageBonusAttribute },
                { DEAttrName.cooldownReduction, fakeCooldownAttribute }
            };

            subject = new CAttributes<DEAttrName>(attributeSetupDictionary);
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

            var eventTester = new EventTester<EAOnAnyAttributeChanged<DEAttrName>>();
            subject.OnAnyAttributeChanged += eventTester.Handler;

            subject.AddModifier(cooldownModifier);

            eventTester.AssertFired(1);
            eventTester.AssertResult(
                subject, 
                (args) => args.AttributeName == DEAttrName.cooldownReduction && args.NewValue == newValue
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

            var eventTester = new EventTester<EAOnAnyAttributeChanged<DEAttrName>>();
            subject.OnAnyAttributeChanged += eventTester.Handler;

            subject.TryRemoveModifier(cooldownModifier);

            eventTester.AssertFired(1);
            eventTester.AssertResult(
                subject,
                args => args.AttributeName == DEAttrName.cooldownReduction && args.NewValue == newValue
            );
        }

        [Test]
        public void Get_Always_ReturnsRequestedAttribute()
        {
            var result = subject.Get(DEAttrName.cooldownReduction);

            Assert.AreEqual(fakeCooldownAttribute, result);
        }
    }
}