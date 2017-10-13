using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using GrimoireTD.Creeps;
using GrimoireTD.Abilities.DefendMode.AttackEffects;

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

        private ISpecificDamageEffectType lolCatDamage = Substitute.For<ISpecificDamageEffectType>();
        private ISpecificDamageEffectType redditDamage = Substitute.For<ISpecificDamageEffectType>();
        private ISpecificDamageEffectType geoCitiesDamage = Substitute.For<ISpecificDamageEffectType>();

        private IBasicMetaDamageEffectType basicNonNetworkedDamage = Substitute.For<IBasicMetaDamageEffectType>();
        private IWeakMetaDamageEffectType weakNonNetworkedDamage = Substitute.For<IWeakMetaDamageEffectType>();
        private IStrongMetaDamageEffectType strongNonNetworkedDamage = Substitute.For<IStrongMetaDamageEffectType>();

        private ISpecificDamageEffectType realityDamage = Substitute.For<ISpecificDamageEffectType>();
        private ISpecificDamageEffectType goOutsideDamage = Substitute.For<ISpecificDamageEffectType>();
        private ISpecificDamageEffectType seeTheSunshineDamage = Substitute.For<ISpecificDamageEffectType>();

        private IBaseResistances baseResistances = Substitute.For<IBaseResistances>();

        private IResistanceModifier baseLolCatResistanceModifier = Substitute.For<IResistanceModifier>();
        private float baseLolCatResistanceMagnitude = 10f;
        private IResistanceModifier baseRedditResistanceModifier = Substitute.For<IResistanceModifier>();
        private float baseRedditResistanceMagnitude = 12f;
        private IResistanceModifier baseGeoCitiesResistanceModifier = Substitute.For<IResistanceModifier>();
        private float baseGeoCitiesResistanceMagnitude = 14f;

        private IResistanceModifier baseRealityResistanceModifier = Substitute.For<IResistanceModifier>();
        private float baseRealityResistanceMagnitude = 7f;
        private IResistanceModifier baseGoOutsideResistanceModifier = Substitute.For<IResistanceModifier>();
        private float baseGoOutsideResistanceMagnitude = 11f;
        private IResistanceModifier baseSeeTheSunshineResistanceModifier = Substitute.For<IResistanceModifier>();
        private float baseSeeTheSunshineResistanceMagnitude = 15f;

        private IBlockModifier baseLolCatBlockModifier = Substitute.For<IBlockModifier>();
        private int baseLolCatBlockMagnitude = 2;
        private IBlockModifier baseRedditBlockModifier = Substitute.For<IBlockModifier>();
        private int baseRedditBlockMagnitude = 3;
        private IBlockModifier baseGeoCitiesBlockModifer = Substitute.For<IBlockModifier>();
        private int baseGeoCitiesBlockMagnitude = 4;

        private IBlockModifier baseRealityBlockModifier = Substitute.For<IBlockModifier>();
        private int baseRealityBlockMagnitude = 5;
        private IBlockModifier baseGoOutsideBlockModifier = Substitute.For<IBlockModifier>();
        private int baseGoOutsideBlockMagnitude = 7;
        private IBlockModifier baseSeeTheSunshineBlockModifier = Substitute.For<IBlockModifier>();
        private int baseSeeTheSunshineBlockMagnitude = 9;

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

            basicInternetDamage.SpecificDamageTypes.Returns(new List<ISpecificDamageEffectType>
            {
                lolCatDamage,
                redditDamage,
                geoCitiesDamage
            });

            basicNonNetworkedDamage.SpecificDamageTypes.Returns(new List<ISpecificDamageEffectType>
            {
                realityDamage,
                goOutsideDamage,
                seeTheSunshineDamage
            });

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
        public void GetBlock_AfterConstructionWithBaseResistances_ReturnsBaseValuesForSpecificDamageType()
        {
            var subject = ConstructSubject();

            var result = subject.GetBlock(geoCitiesDamage).Value;

            Assert.AreEqual(baseGeoCitiesBlockMagnitude, result);
        }
    }
}