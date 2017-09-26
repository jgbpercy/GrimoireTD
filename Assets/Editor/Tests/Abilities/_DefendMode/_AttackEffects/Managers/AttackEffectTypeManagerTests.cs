using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NSubstitute;
using GrimoireTD.Abilities.DefendMode.AttackEffects;

namespace GrimoireTD.Tests.AttackEffectTypeManagerTests
{
    public class AttackEffectTypeManagerTests
    {
        private IBasicMetaDamageEffectType basicInternetDamage;
        private IWeakMetaDamageEffectType weakInternetDamage;
        private IStrongMetaDamageEffectType strongInternetDamage;

        private ISpecificDamageEffectType lolCatDamage;
        private ISpecificDamageEffectType redditDamage;
        private ISpecificDamageEffectType geoCitiesDamage;

        private IBasicMetaDamageEffectType basicNonNetworkedDamage;
        private IWeakMetaDamageEffectType weakNonNetworkedDamage;
        private IStrongMetaDamageEffectType strongNonNetworkedDamage;

        private ISpecificDamageEffectType realityDamage;

        private IAttributeModifierEffectType attributeModifierEffectType;

        private IResistanceModifierEffectType resistanceModifierEffectType;

        private List<IAttackEffectType> attackEffectTypeList;

        private IAttackEffectTypeManager subject;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            basicInternetDamage = Substitute.For<IBasicMetaDamageEffectType>();
            weakInternetDamage = Substitute.For<IWeakMetaDamageEffectType>();
            strongInternetDamage = Substitute.For<IStrongMetaDamageEffectType>();

            lolCatDamage = Substitute.For<ISpecificDamageEffectType>();
            redditDamage = Substitute.For<ISpecificDamageEffectType>();
            geoCitiesDamage = Substitute.For<ISpecificDamageEffectType>();

            basicNonNetworkedDamage = Substitute.For<IBasicMetaDamageEffectType>();
            weakNonNetworkedDamage = Substitute.For<IWeakMetaDamageEffectType>();
            strongNonNetworkedDamage = Substitute.For<IStrongMetaDamageEffectType>();

            realityDamage = Substitute.For<ISpecificDamageEffectType>();

            attributeModifierEffectType = Substitute.For<IAttributeModifierEffectType>();
            resistanceModifierEffectType = Substitute.For<IResistanceModifierEffectType>();

            weakInternetDamage.BasicMetaDamageType.Returns(basicInternetDamage);
            strongInternetDamage.BasicMetaDamageType.Returns(basicInternetDamage);

            lolCatDamage.BasicMetaDamageEffectType.Returns(basicInternetDamage);
            redditDamage.BasicMetaDamageEffectType.Returns(basicInternetDamage);
            geoCitiesDamage.BasicMetaDamageEffectType.Returns(basicInternetDamage);

            weakNonNetworkedDamage.BasicMetaDamageType.Returns(basicNonNetworkedDamage);
            strongNonNetworkedDamage.BasicMetaDamageType.Returns(basicNonNetworkedDamage);

            realityDamage.BasicMetaDamageEffectType.Returns(basicNonNetworkedDamage);

            attackEffectTypeList = new List<IAttackEffectType>();

            attackEffectTypeList.Add(basicInternetDamage);
            attackEffectTypeList.Add(weakInternetDamage);
            attackEffectTypeList.Add(strongInternetDamage);
            attackEffectTypeList.Add(lolCatDamage);
            attackEffectTypeList.Add(redditDamage);
            attackEffectTypeList.Add(geoCitiesDamage);
            attackEffectTypeList.Add(basicNonNetworkedDamage);
            attackEffectTypeList.Add(weakNonNetworkedDamage);
            attackEffectTypeList.Add(strongNonNetworkedDamage);
            attackEffectTypeList.Add(realityDamage);
            attackEffectTypeList.Add(attributeModifierEffectType);
            attackEffectTypeList.Add(resistanceModifierEffectType);

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