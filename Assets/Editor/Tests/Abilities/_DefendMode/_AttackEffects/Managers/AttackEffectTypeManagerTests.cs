using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NSubstitute;
using GrimoireTD.Abilities.DefendMode.AttackEffects;

namespace GrimoireTD.Tests.AttackEffectTypeManagerTests
{
    public class AttackEffectTypeManagerTests
    {
        private IBasicMetaDamageEffectType basicInternetDamage = Substitute.For<IBasicMetaDamageEffectType>();
        private IWeakMetaDamageEffectType weakInternetDamage = Substitute.For<IWeakMetaDamageEffectType>();
        private IStrongMetaDamageEffectType strongInternetDamage = Substitute.For<IStrongMetaDamageEffectType>();

        private ISpecificDamageEffectType lolCatDamage = Substitute.For<ISpecificDamageEffectType>();
        private ISpecificDamageEffectType redditDamage = Substitute.For<ISpecificDamageEffectType>();
        private ISpecificDamageEffectType geoCitiesDamage = Substitute.For<ISpecificDamageEffectType>();

        private IBasicMetaDamageEffectType basicNonNetworkedDamage = Substitute.For<IBasicMetaDamageEffectType>();
        private IWeakMetaDamageEffectType weakNonNetworkedDamage = Substitute.For<IWeakMetaDamageEffectType>();
        private IStrongMetaDamageEffectType strongNonNetworkedDamage = Substitute.For<IStrongMetaDamageEffectType>();

        private ISpecificDamageEffectType realityDamage = Substitute.For<ISpecificDamageEffectType>();

        private IAttributeModifierEffectType attributeModifierEffectType = Substitute.For<IAttributeModifierEffectType>();

        private IResistanceModifierEffectType resistanceModifierEffectType = Substitute.For<IResistanceModifierEffectType>();

        private List<IAttackEffectType> attackEffectTypeList;

        private IAttackEffectTypeManager subject;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            weakInternetDamage.BasicMetaDamageType.Returns(basicInternetDamage);
            strongInternetDamage.BasicMetaDamageType.Returns(basicInternetDamage);

            lolCatDamage.BasicMetaDamageEffectType.Returns(basicInternetDamage);
            redditDamage.BasicMetaDamageEffectType.Returns(basicInternetDamage);
            geoCitiesDamage.BasicMetaDamageEffectType.Returns(basicInternetDamage);

            weakNonNetworkedDamage.BasicMetaDamageType.Returns(basicNonNetworkedDamage);
            strongNonNetworkedDamage.BasicMetaDamageType.Returns(basicNonNetworkedDamage);

            realityDamage.BasicMetaDamageEffectType.Returns(basicNonNetworkedDamage);

            attackEffectTypeList = new List<IAttackEffectType>
            {
                basicInternetDamage,
                weakInternetDamage,
                strongInternetDamage,
                lolCatDamage,
                redditDamage,
                geoCitiesDamage,
                basicNonNetworkedDamage,
                weakNonNetworkedDamage,
                strongNonNetworkedDamage,
                realityDamage,
                attributeModifierEffectType,
                resistanceModifierEffectType
            };

            subject = new CAttackEffectTypeManager();

            subject.SetUp(attackEffectTypeList);
        }

        [Test]
        public void SpecificDamageTypes_AfterSetUp_ReturnsTheTypesThatWerePassed()
        {
            Assert.True(subject.SpecificDamageTypes.Contains(lolCatDamage));
            Assert.True(subject.SpecificDamageTypes.Contains(redditDamage));
            Assert.True(subject.SpecificDamageTypes.Contains(geoCitiesDamage));
            Assert.True(subject.SpecificDamageTypes.Contains(realityDamage));
        }

        [Test]
        public void MetaDamageTypes_AfterSetUp_ReturnsTheTypesThatWerePassed()
        {
            Assert.True(subject.MetaDamageTypes.Contains(basicInternetDamage));
            Assert.True(subject.MetaDamageTypes.Contains(weakInternetDamage));
            Assert.True(subject.MetaDamageTypes.Contains(strongInternetDamage));
            Assert.True(subject.MetaDamageTypes.Contains(basicNonNetworkedDamage));
            Assert.True(subject.MetaDamageTypes.Contains(weakNonNetworkedDamage));
            Assert.True(subject.MetaDamageTypes.Contains(strongNonNetworkedDamage));
        }

        [Test]
        public void BasicDamageTypes_AfterSetUp_ReturnsTheTypesThatWerePassed()
        {
            Assert.True(subject.BasicMetaDamageTypes.Contains(basicInternetDamage));
            Assert.True(subject.BasicMetaDamageTypes.Contains(basicNonNetworkedDamage));
        }

        [Test]
        public void WeakDamageTypes_AfterSetUp_ReturnsTheTypesThatWerePassed()
        {
            Assert.True(subject.WeakMetaDamageTypes.Contains(weakInternetDamage));
            Assert.True(subject.WeakMetaDamageTypes.Contains(weakNonNetworkedDamage));
        }

        [Test]
        public void StrongDamageTypes_AfterSetUp_ReturnsTheTypesThatWerePassed()
        {
            Assert.True(subject.StrongMetaDamageTypes.Contains(strongInternetDamage));
            Assert.True(subject.StrongMetaDamageTypes.Contains(strongNonNetworkedDamage));
        }

        [Test]
        public void ModifierEffectTypes_AfterSetUp_ReturnsTheTypesThatWerePassed()
        {
            Assert.True(subject.ModifierEffectTypes.Contains(attributeModifierEffectType));
            Assert.True(subject.ModifierEffectTypes.Contains(resistanceModifierEffectType));
        }

        [Test]
        public void AttributeModifierEffectTypes_AfterSetUp_ReturnsTheTypesThatWerePassed()
        {
            Assert.True(subject.AttributeEffectTypes.Contains(attributeModifierEffectType));
        }

        [Test]
        public void ResistanceModifierEffectTypes_AfterSetUp_ReturnsTheTypesThatWerePassed()
        {
            Assert.True(subject.ResistanceEffectTypes.Contains(resistanceModifierEffectType));
        }

        [Test]
        public void GetSpecificDamageTypes_PassedBasicMetaType_ReturnsTheSpecificTypesForThatMetaType()
        {
            var result = subject.GetSpecificDamageTypes(basicInternetDamage);

            Assert.True(result.Contains(lolCatDamage));
            Assert.True(result.Contains(redditDamage));
            Assert.True(result.Contains(geoCitiesDamage));

            Assert.False(result.Contains(realityDamage));
        }

        [Test]
        public void GetSpecificDamageTypes_PassedWeakMetaType_ReturnsTheSpecificTypesForThatMetaType()
        {
            var result = subject.GetSpecificDamageTypes(weakInternetDamage);

            Assert.True(result.Contains(lolCatDamage));
            Assert.True(result.Contains(redditDamage));
            Assert.True(result.Contains(geoCitiesDamage));

            Assert.False(result.Contains(realityDamage));
        }

        [Test]
        public void GetBasicMetaDamageType_PassedSpecificType_ReturnsTheBasicTypeForThatSpecificType()
        {
            Assert.AreEqual(subject.GetBasicMetaDamageType(lolCatDamage), basicInternetDamage);
        }

        [Test]
        public void GetWeakMetaDamageType_PassedSpecificType_ReturnsTheWeakTypeForThatSpecificType()
        {
            Assert.AreEqual(subject.GetWeakMetaDamageType(lolCatDamage), weakInternetDamage);
        }

        [Test]
        public void GetWeakMetaDamageType_PassedBasicType_ReturnsTheWeakTypeForThatBasicType()
        {
            Assert.AreEqual(subject.GetWeakMetaDamageType(basicInternetDamage), weakInternetDamage);
        }

        [Test]
        public void GetStrongMetaDamageType_PassedSpecificType_ReturnsTheStrongTypeForThatSpecificType()
        {
            Assert.AreEqual(subject.GetStrongMetaDamageType(lolCatDamage), strongInternetDamage);
        }

        [Test]
        public void GetStrongMetaDamageType_PassedBasicType_ReturnsTheStrongTypeForThatBasicType()
        {
            Assert.AreEqual(subject.GetStrongMetaDamageType(basicInternetDamage), strongInternetDamage);
        }
    }
}