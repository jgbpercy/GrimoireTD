using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using GrimoireTD.Creeps;
using GrimoireTD.Abilities.DefendMode.AttackEffects;
using GrimoireTD.Technical;
using GrimoireTD.Attributes;

namespace GrimoireTD.Tests.ResistancesTests
{
    public class ResistancesTests
    {
        private IReadOnlyAttackEffectTypeManager attackEffectTypeManager = Substitute.For<IAttackEffectTypeManager>();

        private IReadOnlyGameModel gameModel = Substitute.For<IReadOnlyGameModel>();

        private ICreep attachedToCreep = Substitute.For<ICreep>();

        private IBasicMetaDamageEffectType basicInternetDamage = Substitute.For<IBasicMetaDamageEffectType>();
        private IWeakMetaDamageEffectType weakInternetDamage = Substitute.For<IWeakMetaDamageEffectType>();
        private IStrongMetaDamageEffectType strongInternetDamage = Substitute.For<IStrongMetaDamageEffectType>();

        private float internetDamageEffectOfArmor = 0.03f;

        private ISpecificDamageEffectType lolCatDamage = Substitute.For<ISpecificDamageEffectType>();
        private ISpecificDamageEffectType redditDamage = Substitute.For<ISpecificDamageEffectType>();
        private ISpecificDamageEffectType geoCitiesDamage = Substitute.For<ISpecificDamageEffectType>();

        private IBasicMetaDamageEffectType basicNonNetworkedDamage = Substitute.For<IBasicMetaDamageEffectType>();
        private IWeakMetaDamageEffectType weakNonNetworkedDamage = Substitute.For<IWeakMetaDamageEffectType>();
        private IStrongMetaDamageEffectType strongNonNetworkedDamage = Substitute.For<IStrongMetaDamageEffectType>();

        private float nonNetworkedDamageEffectOfArmor = 0.05f;

        private ISpecificDamageEffectType realityDamage = Substitute.For<ISpecificDamageEffectType>();
        private ISpecificDamageEffectType goOutsideDamage = Substitute.For<ISpecificDamageEffectType>();
        private ISpecificDamageEffectType seeTheSunshineDamage = Substitute.For<ISpecificDamageEffectType>();

        private IBaseResistances baseResistances = Substitute.For<IBaseResistances>();

        private IResistanceModifier baseLolCatResistanceModifier = Substitute.For<IResistanceModifier>();
        private float baseLolCatResistanceMagnitude = 0.1f;
        private IResistanceModifier baseRedditResistanceModifier = Substitute.For<IResistanceModifier>();
        private float baseRedditResistanceMagnitude = 0.12f;
        private IResistanceModifier baseGeoCitiesResistanceModifier = Substitute.For<IResistanceModifier>();
        private float baseGeoCitiesResistanceMagnitude = 0.16f;

        private IResistanceModifier baseRealityResistanceModifier = Substitute.For<IResistanceModifier>();
        private float baseRealityResistanceMagnitude = 0.17f;
        private IResistanceModifier baseGoOutsideResistanceModifier = Substitute.For<IResistanceModifier>();
        private float baseGoOutsideResistanceMagnitude = 0.11f;
        private IResistanceModifier baseSeeTheSunshineResistanceModifier = Substitute.For<IResistanceModifier>();
        private float baseSeeTheSunshineResistanceMagnitude = 0.15f;

        private IBlockModifier baseLolCatBlockModifier = Substitute.For<IBlockModifier>();
        private int baseLolCatBlockMagnitude = 4;
        private IBlockModifier baseRedditBlockModifier = Substitute.For<IBlockModifier>();
        private int baseRedditBlockMagnitude = 6;
        private IBlockModifier baseGeoCitiesBlockModifer = Substitute.For<IBlockModifier>();
        private int baseGeoCitiesBlockMagnitude = 9;

        private IBlockModifier baseRealityBlockModifier = Substitute.For<IBlockModifier>();
        private int baseRealityBlockMagnitude = 2;
        private IBlockModifier baseGoOutsideBlockModifier = Substitute.For<IBlockModifier>();
        private int baseGoOutsideBlockMagnitude = 3;
        private IBlockModifier baseSeeTheSunshineBlockModifier = Substitute.For<IBlockModifier>();
        private int baseSeeTheSunshineBlockMagnitude = 6;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            attackEffectTypeManager.SpecificDamageTypes.Returns(new List<ISpecificDamageEffectType>
            {
                lolCatDamage,
                redditDamage,
                geoCitiesDamage,
                realityDamage,
                goOutsideDamage,
                seeTheSunshineDamage
            });

            attackEffectTypeManager.BasicMetaDamageTypes.Returns(new List<IBasicMetaDamageEffectType>
            {
                basicInternetDamage,
                basicNonNetworkedDamage
            });

            gameModel.AttackEffectTypeManager.Returns(attackEffectTypeManager);

            GameModels.Models.Add(gameModel);

            var specificInternetDamageTypes = new List<ISpecificDamageEffectType>
            {
                lolCatDamage,
                redditDamage,
                geoCitiesDamage
            };

            basicInternetDamage.SpecificDamageTypes.Returns(specificInternetDamageTypes);
            weakInternetDamage.SpecificDamageTypes.Returns(specificInternetDamageTypes);
            strongInternetDamage.SpecificDamageTypes.Returns(specificInternetDamageTypes);

            var specificNonNetworkedDamageTypes = new List<ISpecificDamageEffectType>
            {
                realityDamage,
                goOutsideDamage,
                seeTheSunshineDamage
            };

            basicNonNetworkedDamage.SpecificDamageTypes.Returns(specificNonNetworkedDamageTypes);
            weakNonNetworkedDamage.SpecificDamageTypes.Returns(specificNonNetworkedDamageTypes);
            strongNonNetworkedDamage.SpecificDamageTypes.Returns(specificNonNetworkedDamageTypes);

            weakInternetDamage.BasicMetaDamageType.Returns(basicInternetDamage);
            strongInternetDamage.BasicMetaDamageType.Returns(basicInternetDamage);
            basicInternetDamage.WeakMetaDamageType.Returns(weakInternetDamage);
            basicInternetDamage.StrongMetaDamageType.Returns(strongInternetDamage);

            weakNonNetworkedDamage.BasicMetaDamageType.Returns(basicNonNetworkedDamage);
            strongNonNetworkedDamage.BasicMetaDamageType.Returns(basicNonNetworkedDamage);
            basicNonNetworkedDamage.WeakMetaDamageType.Returns(weakNonNetworkedDamage);
            basicNonNetworkedDamage.StrongMetaDamageType.Returns(strongNonNetworkedDamage);

            baseResistances.GetResistanceModifier(lolCatDamage).Returns(baseLolCatResistanceModifier);
            baseResistances.GetResistanceModifier(redditDamage).Returns(baseRedditResistanceModifier);
            baseResistances.GetResistanceModifier(geoCitiesDamage).Returns(baseGeoCitiesResistanceModifier);

            baseLolCatResistanceModifier.Magnitude.Returns(baseLolCatResistanceMagnitude);
            baseRedditResistanceModifier.Magnitude.Returns(baseRedditResistanceMagnitude);
            baseGeoCitiesResistanceModifier.Magnitude.Returns(baseGeoCitiesResistanceMagnitude);

            baseResistances.GetResistanceModifier(realityDamage).Returns(baseRealityResistanceModifier);
            baseResistances.GetResistanceModifier(goOutsideDamage).Returns(baseGoOutsideResistanceModifier);
            baseResistances.GetResistanceModifier(seeTheSunshineDamage).Returns(baseSeeTheSunshineResistanceModifier);

            baseRealityResistanceModifier.Magnitude.Returns(baseRealityResistanceMagnitude);
            baseGoOutsideResistanceModifier.Magnitude.Returns(baseGoOutsideResistanceMagnitude);
            baseSeeTheSunshineResistanceModifier.Magnitude.Returns(baseSeeTheSunshineResistanceMagnitude);

            baseResistances.GetBlockModifier(lolCatDamage).Returns(baseLolCatBlockModifier);
            baseResistances.GetBlockModifier(redditDamage).Returns(baseRedditBlockModifier);
            baseResistances.GetBlockModifier(geoCitiesDamage).Returns(baseGeoCitiesBlockModifer);

            baseLolCatBlockModifier.Magnitude.Returns(baseLolCatBlockMagnitude);
            baseRedditBlockModifier.Magnitude.Returns(baseRedditBlockMagnitude);
            baseGeoCitiesBlockModifer.Magnitude.Returns(baseGeoCitiesBlockMagnitude);

            baseResistances.GetBlockModifier(realityDamage).Returns(baseRealityBlockModifier);
            baseResistances.GetBlockModifier(goOutsideDamage).Returns(baseGoOutsideBlockModifier);
            baseResistances.GetBlockModifier(seeTheSunshineDamage).Returns(baseSeeTheSunshineBlockModifier);

            baseRealityBlockModifier.Magnitude.Returns(baseRealityBlockMagnitude);
            baseGoOutsideBlockModifier.Magnitude.Returns(baseGoOutsideBlockMagnitude);
            baseSeeTheSunshineBlockModifier.Magnitude.Returns(baseSeeTheSunshineBlockMagnitude);

            basicInternetDamage.EffectOfArmor.Returns(internetDamageEffectOfArmor);

            basicNonNetworkedDamage.EffectOfArmor.Returns(nonNetworkedDamageEffectOfArmor);
        }

        [SetUp]
        public void EachTestSetUp()
        {
            attachedToCreep.CurrentArmor.Returns(0f);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            typeof(GameModels).TypeInitializer.Invoke(null, null);
        }

        private CResistances ConstructSubject()
        {
            return new CResistances(attachedToCreep, baseResistances);
        }

        [Test]
        public void GetResistance_AfterConstructionWithBaseResistances_ReturnsBaseValueForSpecificDamageType()
        {
            var subject = ConstructSubject();

            var result = subject.GetResistance(geoCitiesDamage).Value;

            Assert.AreEqual(baseGeoCitiesResistanceMagnitude, result);
        }

        [Test]
        public void GetResistance_AfterConstructionWithBaseResistances_ReturnsCorrectBasicResistance()
        {
            var subject = ConstructSubject();

            var result = subject.GetResistance(basicInternetDamage).Value;

            AssertExt.Approximately(0.12666667f, result);
        }

        [Test]
        public void GetResistance_AfterConstructionWithBaseResistances_ReturnsCorrectWeakResistance()
        {
            var subject = ConstructSubject();

            var result = subject.GetResistance(weakInternetDamage).Value;

            AssertExt.Approximately(0.16f, result);
        }

        [Test]
        public void GetResistance_AfterConstructionWithBaseResistances_ReturnsCorrectStrongResistance()
        {
            var subject = ConstructSubject();

            var result = subject.GetResistance(strongInternetDamage).Value;

            AssertExt.Approximately(0.1f, result);
        }

        [Test]
        public void GetBlock_AfterConstructionWithBaseResistances_ReturnsBaseValuesForSpecificDamageType()
        {
            var subject = ConstructSubject();

            var result = subject.GetBlock(geoCitiesDamage).Value;

            Assert.AreEqual(baseGeoCitiesBlockMagnitude, result);
        }

        [Test]
        public void GetBlock_AfterConstructionWithBaseResistances_ReturnsCorrectBasicBlock()
        {
            var subject = ConstructSubject();

            var result = subject.GetBlock(basicNonNetworkedDamage).Value;

            Assert.AreEqual(4, result);
        }

        [Test]
        public void GetBlock_AfterConstructionWithBaseResistances_ReturnsCorrectWeakBlock()
        {
            var subject = ConstructSubject();

            var result = subject.GetBlock(weakNonNetworkedDamage).Value;

            Assert.AreEqual(6, result);
        }

        [Test]
        public void GetBlock_AfterConstructioNWithBaseResistances_ReturnsCorrectStrongBlock()
        {
            var subject = ConstructSubject();

            var result = subject.GetBlock(strongNonNetworkedDamage).Value;

            Assert.AreEqual(2, result);
        }

        [Test]
        public void GetResistance_AfterConstructionWithBaseResistancesAndArmor_ReturnsResistanceForBasePlusArmor()
        {
            attachedToCreep.CurrentArmor.Returns(2.5f);

            var subject = ConstructSubject();

            var result = subject.GetResistance(redditDamage).Value;

            AssertExt.Approximately(0.1845224f, result);
        }

        [Test]
        public void GetResistanceWithoutArmor_Always_ReturnsCorrectSpecificResistance()
        {
            attachedToCreep.CurrentArmor.Returns(2.5f);

            var subject = ConstructSubject();

            var result = subject.GetResistanceWithoutArmor(redditDamage);

            AssertExt.Approximately(baseRedditResistanceMagnitude, result);
        }

        [Test]
        public void GetResistanceWithoutArmor_Always_ReturnsCorrectBasicResistance()
        {
            attachedToCreep.CurrentArmor.Returns(2.5f);

            var subject = ConstructSubject();

            var result = subject.GetResistanceWithoutArmor(basicInternetDamage);

            AssertExt.Approximately(0.12666667f, result);
        }

        [Test]
        public void GetResistanceWithoutArmor_Always_ReturnsCorrectWeakResistance()
        {
            attachedToCreep.CurrentArmor.Returns(2.5f);

            var subject = ConstructSubject();

            var result = subject.GetResistanceWithoutArmor(weakInternetDamage);

            AssertExt.Approximately(0.16f, result);
        }

        [Test]
        public void GetResistanceWithoutArmor_Always_ReturnsCorrectStrongResistance()
        {
            attachedToCreep.CurrentArmor.Returns(2.5f);

            var subject = ConstructSubject();

            var result = subject.GetResistanceWithoutArmor(strongInternetDamage);

            AssertExt.Approximately(0.1f, result);
        }

        [Test]
        public void AddResistanceModifier_Always_ChangesResistanceValue()
        {
            var subject = ConstructSubject();

            var addedModifier = Substitute.For<IResistanceModifier>();
            addedModifier.DamageType.Returns(goOutsideDamage);
            addedModifier.Magnitude.Returns(0.3f);

            subject.AddResistanceModifier(addedModifier);

            var result = subject.GetResistance(goOutsideDamage).Value;

            AssertExt.Approximately(0.377f, result);
        }

        [Test]
        public void AddResistanceModifier_Always_FiresOnAnyResistanceChangedEvent()
        {
            var subject = ConstructSubject();

            var eventTester = new EventTester<EAOnAnyResistanceChanged>();
            subject.OnAnyResistanceChanged += eventTester.Handler;

            var addedModifier = Substitute.For<IResistanceModifier>();
            addedModifier.DamageType.Returns(goOutsideDamage);
            addedModifier.Magnitude.Returns(0.3f);

            subject.AddResistanceModifier(addedModifier);

            eventTester.AssertFired(1);
            eventTester.AssertResult(subject, args => CustomMath.Approximately(0.377f, args.NewValue));
        }

        [Test]
        public void AddResistanceModifier_Always_FiresOnResistanceChangedEvent()
        {
            var subject = ConstructSubject();

            var eventTester = new EventTester<EAOnRecalculatorListChange<float>>();
            subject.GetResistance(goOutsideDamage).OnChange += eventTester.Handler;

            var addedModifier = Substitute.For<IResistanceModifier>();
            addedModifier.DamageType.Returns(goOutsideDamage);
            addedModifier.Magnitude.Returns(0.3f);

            subject.AddResistanceModifier(addedModifier);

            eventTester.AssertFired(1);
            eventTester.AssertResult(subject.GetResistance(goOutsideDamage), args => CustomMath.Approximately(0.377f, args.NewValue));
        }

        [Test]
        public void RemoveResistanceModifier_PassedModifierThatWasAddedPreviously_ChangesResistanceValue()
        {
            var subject = ConstructSubject();

            var addedModifier = Substitute.For<IResistanceModifier>();
            addedModifier.DamageType.Returns(goOutsideDamage);
            addedModifier.Magnitude.Returns(0.3f);

            subject.AddResistanceModifier(addedModifier);

            subject.RemoveResistanceModifer(addedModifier);

            var result = subject.GetResistance(goOutsideDamage).Value;

            AssertExt.Approximately(baseGoOutsideResistanceMagnitude, result);
        }

        [Test]
        public void RemoveResistanceModifier_PassedModifierThatWasAddedPreviously_FiresOnAnyResistanceChangedEvent()
        {
            var subject = ConstructSubject();

            var eventTester = new EventTester<EAOnAnyResistanceChanged>();
            subject.OnAnyResistanceChanged += eventTester.Handler;

            var addedModifier = Substitute.For<IResistanceModifier>();
            addedModifier.DamageType.Returns(goOutsideDamage);
            addedModifier.Magnitude.Returns(0.3f);

            subject.AddResistanceModifier(addedModifier);

            subject.RemoveResistanceModifer(addedModifier);

            eventTester.AssertFired(2);

            eventTester.AssertResults(argList => argList[1].NewValue == baseGoOutsideResistanceMagnitude);
        }

        [Test]
        public void RemoveResistanceModifier_PassedModifierThatWasAddedPreviously_FiresOnResistanceChangedEvent()
        {
            var subject = ConstructSubject();

            var eventTester = new EventTester<EAOnRecalculatorListChange<float>>();
            subject.GetResistance(goOutsideDamage).OnChange += eventTester.Handler;

            var addedModifier = Substitute.For<IResistanceModifier>();
            addedModifier.DamageType.Returns(goOutsideDamage);
            addedModifier.Magnitude.Returns(0.3f);

            subject.AddResistanceModifier(addedModifier);

            subject.RemoveResistanceModifer(addedModifier);

            eventTester.AssertFired(2);

            eventTester.AssertResults(argList => argList[1].NewValue == baseGoOutsideResistanceMagnitude);
        }


        [Test]
        public void RemoveResistanceModifier_PassedModifierNotPreviouslyAdded_ThrowsKeyNotFoundException()
        {
            var subject = ConstructSubject();

            var removedModifier = Substitute.For<IResistanceModifier>();

            Assert.Throws(typeof(KeyNotFoundException), () => subject.RemoveResistanceModifer(removedModifier));
        }

        [Test]
        public void AddBlockModifier_Always_ChangesBlockValue()
        {
            var subject = ConstructSubject();

            var addedBlockMagnitude = 4;

            var addedModifier = Substitute.For<IBlockModifier>();
            addedModifier.DamageType.Returns(seeTheSunshineDamage);
            addedModifier.Magnitude.Returns(addedBlockMagnitude);

            subject.AddBlockModifier(addedModifier);

            var result = subject.GetBlock(seeTheSunshineDamage).Value;

            Assert.AreEqual(baseSeeTheSunshineBlockMagnitude + addedBlockMagnitude, result);
        }

        [Test]
        public void AddBlockModifier_Always_FiresOnAnyBlockChangedEvent()
        {
            var subject = ConstructSubject();

            var eventTester = new EventTester<EAOnAnyBlockChanged>();
            subject.OnAnyBlockChanged += eventTester.Handler;

            var addedBlockMagnitude = 4;

            var addedModifier = Substitute.For<IBlockModifier>();
            addedModifier.DamageType.Returns(seeTheSunshineDamage);
            addedModifier.Magnitude.Returns(addedBlockMagnitude);

            subject.AddBlockModifier(addedModifier);

            eventTester.AssertFired(1);
            eventTester.AssertResult(subject, args => args.NewValue == baseSeeTheSunshineBlockMagnitude + addedBlockMagnitude);
        }

        [Test]
        public void AddBlockModifier_Always_FiresOnBlockChangedEvent()
        {
            var subject = ConstructSubject();

            var eventTester = new EventTester<EAOnRecalculatorListChange<int>>();
            subject.GetBlock(seeTheSunshineDamage).OnChange += eventTester.Handler;

            var addedBlockMagnitude = 4;

            var addedModifier = Substitute.For<IBlockModifier>();
            addedModifier.DamageType.Returns(seeTheSunshineDamage);
            addedModifier.Magnitude.Returns(addedBlockMagnitude);

            subject.AddBlockModifier(addedModifier);

            eventTester.AssertFired(1);
            eventTester.AssertResult(subject.GetBlock(seeTheSunshineDamage), args => args.NewValue == baseSeeTheSunshineBlockMagnitude + addedBlockMagnitude);
        }

        [Test]
        public void RemoveBlockModifier_PassedModifierThatWasAddedPreviously_ChangesBlockValue()
        {
            var subject = ConstructSubject();

            var addedModifier = Substitute.For<IBlockModifier>();
            addedModifier.DamageType.Returns(seeTheSunshineDamage);
            addedModifier.Magnitude.Returns(3);

            subject.AddBlockModifier(addedModifier);

            subject.RemoveBlockModifier(addedModifier);

            var result = subject.GetBlock(seeTheSunshineDamage).Value;

            Assert.AreEqual(baseSeeTheSunshineBlockMagnitude, result);
        }

        [Test]
        public void RemoveBlockModifier_PassedModifierThatWasAddedPreviously_FiresOnAnyBlockChangedEvent()
        {
            var subject = ConstructSubject();

            var eventTester = new EventTester<EAOnAnyBlockChanged>();
            subject.OnAnyBlockChanged += eventTester.Handler;

            var addedModifier = Substitute.For<IBlockModifier>();
            addedModifier.DamageType.Returns(seeTheSunshineDamage);
            addedModifier.Magnitude.Returns(3);

            subject.AddBlockModifier(addedModifier);

            subject.RemoveBlockModifier(addedModifier);

            eventTester.AssertFired(2);

            eventTester.AssertResults(argList => argList[1].NewValue == baseSeeTheSunshineBlockMagnitude);
        }

        [Test]
        public void RemoveBlockModifier_PassModifierThatWasAddedPreviously_FiresOnBlockChangedEvent()
        {
            var subject = ConstructSubject();

            var eventTester = new EventTester<EAOnRecalculatorListChange<int>>();
            subject.GetBlock(seeTheSunshineDamage).OnChange += eventTester.Handler;

            var addedModifier = Substitute.For<IBlockModifier>();
            addedModifier.DamageType.Returns(seeTheSunshineDamage);
            addedModifier.Magnitude.Returns(3);

            subject.AddBlockModifier(addedModifier);

            subject.RemoveBlockModifier(addedModifier);

            eventTester.AssertFired(2);

            eventTester.AssertResults(argList => argList[1].NewValue == baseSeeTheSunshineBlockMagnitude);
        }

        [Test]
        public void RemoveBlockModifier_PassedModifierNotPreviouslyAdded_ThrowsKeyNotFoundExeption()
        {
            var subject = ConstructSubject();

            var removedModifier = Substitute.For<IBlockModifier>();

            Assert.Throws(typeof(KeyNotFoundException), () => subject.RemoveBlockModifier(removedModifier));
        }

        [Test]
        public void GetResistance_AfterArmorChangedEvent_ChangesValueCorrectly()
        {
            attachedToCreep.CurrentArmor.Returns(3f);

            var subject = ConstructSubject();

            attachedToCreep.CurrentArmor.Returns(3.5f);
            attachedToCreep.OnArmorChanged += Raise.EventWith(new EAOnAttributeChanged(3.5f));

            var result = subject.GetResistance(geoCitiesDamage).Value;

            AssertExt.Approximately(0.244941f, result);
        }

        [Test]
        public void GetReistanceWithoutArmor_AfterArmorChangedEvent_DoesNotChangeArmorValue()
        {
            attachedToCreep.CurrentArmor.Returns(3f);

            var subject = ConstructSubject();

            attachedToCreep.CurrentArmor.Returns(3.5f);
            attachedToCreep.OnArmorChanged += Raise.EventWith(new EAOnAttributeChanged(3.5f));

            var result = subject.GetResistanceWithoutArmor(geoCitiesDamage);

            AssertExt.Approximately(0.16f, result);
        }
    }
}