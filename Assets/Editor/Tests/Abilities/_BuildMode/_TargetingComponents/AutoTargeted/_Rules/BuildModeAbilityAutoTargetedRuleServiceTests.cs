using NUnit.Framework;
using NSubstitute;
using GrimoireTD.Abilities.BuildMode;
using GrimoireTD.Map;

namespace GrimoireTD.Tests.BuildModeAbilityAutoTargetedRuleServiceTests
{
    public class BuildModeAbilityAutoTargetedRuleServiceTests
    {
        private IReadOnlyMapData mapData;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            mapData = Substitute.For<IReadOnlyMapData>();
        }

        [Test]
        public void RunRule_PassedSingleHexAndOneTargetCoord_ReturnsListWithOneCoord()
        {
            var targetCoord = new Coord(3, 4);

            var result = BuildModeAbilityAutoTargetedRuleService.RunRule(new SingleHexArgs(
                targetCoord,
                mapData
            ));

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(targetCoord, result[0]);
        }
    }
}