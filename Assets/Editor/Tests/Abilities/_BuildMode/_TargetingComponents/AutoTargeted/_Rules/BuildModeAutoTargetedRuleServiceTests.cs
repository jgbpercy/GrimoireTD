using NUnit.Framework;
using NSubstitute;
using GrimoireTD.Abilities.BuildMode;
using GrimoireTD.Map;

namespace GrimoireTD.Tests.BuildModeAutoTargetedRuleServiceTests
{
    public class BuildModeAutoTargetedRuleServiceTests
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

            var result = BuildModeAutoTargetedRuleService.RunRule(new SingleHexArgs(
                targetCoord,
                mapData
            ));

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(targetCoord, result[0]);
        }

        //TODO probably revisit if services should return empty list or null when no targets - not sure what's best yet just needs to be consistent 
        [Test]
        public void RunRule_PassedSingleHexAndNullTarget_ReturnsNull()
        {
            var result = BuildModeAutoTargetedRuleService.RunRule(new SingleHexArgs(
                null,
                mapData
            ));
            
            Assert.AreEqual(null, result);
        }
    }
}