using System;
using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using GrimoireTD.Abilities.BuildMode;
using GrimoireTD.Defenders.Units;
using GrimoireTD.Map;
using GrimoireTD.Defenders.Structures;
using GrimoireTD.Dependencies;

namespace GrimoireTD.Tests.BuildModeAbilityHexTargetingRuleServiceTests
{
    public class BuildModeAbilityHexTargetingRuleServiceTests
    {
        //Primitives and Basic Objects
        private Coord unitPosition = new Coord(0, 0);

        //Model and Frame Updater
        private IMapData mapData = Substitute.For<IMapData>();

        private IReadOnlyGameModel gameModel = Substitute.For<IReadOnlyGameModel>();

        //Other Objects Passed To Methods
        private IUnit unit = Substitute.For<IUnit>();

        private IStructure structure = Substitute.For<IStructure>();

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            //Model and Frame Updater
            gameModel.MapData.Returns(mapData);

            DepsProv.SetTheGameModel(gameModel);

            //Other Objects Passed To Methods
            unit.CoordPosition.Returns(unitPosition);
        }

        [SetUp]
        public void EachTestSetUp()
        {
            //Model and Frame Updater
            mapData.CanMoveUnitTo(Arg.Any<Coord>(), Arg.Any<List<Coord>>()).Returns(true);

            //Other Objects Passed To Methods
            unit.CachedDisallowedMovementDestinations.Returns(new List<Coord>());
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            typeof(DepsProv).TypeInitializer.Invoke(null, null);
        }

        [Test]
        public void RunRule_PassedValidMoveRuleAndOutOfRangeHex_ReturnsFalse()
        {
            var result = PlayerTargetsHexRuleService.RunRule(new ValidMoveArgs(
                unit,
                new Coord(3, 3),
                1
            ));

            Assert.False(result);
        }

        [Test]
        public void RunRule_PassedValidMoveRuleAndHexInRange_ReturnsFalseIfMapReturnsFalse()
        {
            mapData.CanMoveUnitTo(Arg.Any<Coord>(), Arg.Any<List<Coord>>()).Returns(false);

            var result = PlayerTargetsHexRuleService.RunRule(new ValidMoveArgs(
                unit,
                new Coord(0,1),
                4
            ));

            Assert.False(result);
        }

        [Test]
        public void RunRule_PassedValidMoveRuleAndHexInRange_ReturnsTrueIfMapReturnsTrue()
        {
            var result = PlayerTargetsHexRuleService.RunRule(new ValidMoveArgs(
                unit,
                new Coord(0, 1),
                4
            ));

            Assert.True(result);
        }

        [Test]
        public void RunRule_PassedValidMoveRuleAndNonUnit_ThrowsException()
        {
            Assert.Throws(typeof(ArgumentException), () => 
                PlayerTargetsHexRuleService.RunRule(new ValidMoveArgs(
                    structure,
                    new Coord(0, 1),
                    4
                ))
            );
        }

        [Test]
        public void RunRule_PassedHexIsInRangeRuleAndHexInRange_ReturnsTrue()
        {
            var result = PlayerTargetsHexRuleService.RunRule(new HexIsInRangeArgs(
                unit,
                new Coord(0, 1),
                4
            ));

            Assert.True(result);
        }

        [Test]
        public void RunRule_PassedHexIsInRangeRuleAndHexOutOfRange_ReturnsFalse()
        {
            var result = PlayerTargetsHexRuleService.RunRule(new HexIsInRangeArgs(
                unit,
                new Coord(3, 3),
                1
            ));

            Assert.False(result);
        }
    }
}