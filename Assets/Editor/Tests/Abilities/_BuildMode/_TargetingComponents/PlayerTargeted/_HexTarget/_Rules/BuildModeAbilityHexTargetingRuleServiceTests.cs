using System;
using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using GrimoireTD.Abilities.BuildMode;
using GrimoireTD.DefendingEntities.Units;
using GrimoireTD.Map;
using GrimoireTD.DefendingEntities.Structures;

namespace GrimoireTD.Tests.BuildModeAbilityHexTargetingRuleServiceTests
{
    public class BuildModeAbilityHexTargetingRuleServiceTests
    {
        private Coord unitPosition = new Coord(0, 0);

        private IUnit unit = Substitute.For<IUnit>();

        private IStructure structure = Substitute.For<IStructure>();

        private IMapData mapData = Substitute.For<IMapData>();

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            unit.CoordPosition.Returns(unitPosition);
        }

        [SetUp]
        public void EachTestSetUp()
        {
            mapData.CanMoveUnitTo(Arg.Any<Coord>(), Arg.Any<List<Coord>>()).Returns(true);

            unit.CachedDisallowedMovementDestinations.Returns(new List<Coord>());
        }

        [Test]
        public void RunRule_PassedValidMoveRuleAndOutOfRangeHex_ReturnsFalse()
        {
            var result = PlayerTargetsHexRuleService.RunRule(new ValidMoveArgs(
                unit,
                new Coord(3, 3),
                mapData,
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
                mapData,
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
                mapData,
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
                    mapData,
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
                mapData,
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
                mapData,
                1
            ));

            Assert.False(result);
        }
    }
}