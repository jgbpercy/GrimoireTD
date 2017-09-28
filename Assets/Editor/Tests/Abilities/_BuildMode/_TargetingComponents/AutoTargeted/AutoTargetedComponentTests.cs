using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using GrimoireTD.Abilities.BuildMode;
using GrimoireTD.Map;

namespace GrimoireTD.Tests.AutoTargetedComponentTests
{
    public class AutoTargetedComponentTests
    {
        private Coord targetCoord;

        private IReadOnlyMapData mapData;

        private BuildModeAutoTargetedArgs buildModeAutoTargetedArgs;

        private IAutoTargetedComponentTemplate template;

        private IBuildModeTargetable target;

        private List<IBuildModeTargetable> targetList;

        private CAutoTargetedComponent subject;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            targetCoord = new Coord(2, 2);

            mapData = Substitute.For<IReadOnlyMapData>();

            template = Substitute.For<IAutoTargetedComponentTemplate>();

            buildModeAutoTargetedArgs = new BuildModeAutoTargetedArgs(targetCoord, mapData);

            template.TargetingRule.GenerateArgs(targetCoord, mapData).Returns(buildModeAutoTargetedArgs);

            target = Substitute.For<IBuildModeTargetable>();

            targetList = new List<IBuildModeTargetable>
            {
                target
            };

            BuildModeAbilityAutoTargetedRuleService.RunRule = (args) =>
            {
                if (args == buildModeAutoTargetedArgs)
                {
                    return targetList;
                }
                else
                {
                    return null;
                }
            };

            subject = new CAutoTargetedComponent(template);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            typeof(BuildModeAbilityAutoTargetedRuleService).TypeInitializer.Invoke(null, null);
        }

        [Test]
        public void FindTargets_PassedValidInput_ReturnsRuleResultForTheArgsGeneratedFromThisInput()
        {
            var result = subject.FindTargets(targetCoord, mapData);

            Assert.AreEqual(result, targetList);
        }
    }
}