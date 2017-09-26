using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using GrimoireTD.ChannelDebug;
using GrimoireTD.Abilities.BuildMode;
using GrimoireTD.DefendingEntities.Units;
using GrimoireTD.Map;
using GrimoireTD.DefendingEntities.Structures;

namespace GrimoireTD.Tests.BuildModeAbilityHexTargetingRuleServiceTests
{
    public class BuildModeAbilityHexTargetingRuleServiceTests
    {
        private IUnit unit;

        private IStructure structure;

        private Coord unitPosition;

        private IMapData mapData;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            CDebug.InitialiseDebugChannels();

            unit = Substitute.For<IUnit>();

            structure = Substitute.For<IStructure>();

            mapData = Substitute.For<IMapData>();
        }

        [SetUp]
        public void EachTestSetUp()
        {
            mapData.CanMoveUnitTo(Arg.Any<Coord>(), Arg.Any<List<Coord>>()).Returns(true);

            unitPosition = new Coord(0, 0);

            unit.CoordPosition.Returns(unitPosition);

            unit.CachedDisallowedMovementDestinations.Returns(new List<Coord>());
        }

        [Test]
        public void RunRule_PassedValidMoveRuleAndOutOfRangeHex_ReturnsFalse()
        {
            var result = BuildModeHexTargetingRuleService.RunRule(new ValidMoveArgs(
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

            var result = BuildModeHexTargetingRuleService.RunRule(new ValidMoveArgs(
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
            var result = BuildModeHexTargetingRuleService.RunRule(new ValidMoveArgs(
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
            Assert.Throws(typeof(System.ArgumentException), () => 
                BuildModeHexTargetingRuleService.RunRule(new ValidMoveArgs(
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
            var result = BuildModeHexTargetingRuleService.RunRule(new HexIsInRangeArgs(
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
            var result = BuildModeHexTargetingRuleService.RunRule(new HexIsInRangeArgs(
                unit,
                new Coord(3, 3),
                mapData,
                1
            ));

            Assert.False(result);
        }
    }
}