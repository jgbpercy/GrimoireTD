using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using GrimoireTD.Abilities.BuildMode;
using GrimoireTD.Map;

namespace GrimoireTD.Tests.AutoTargetedComponentTests
{
    public class AutoTargetedComponentTests
    {
        private Coord targetCoord = new Coord(2, 2);

        private IAutoTargetedComponentTemplate template = Substitute.For<IAutoTargetedComponentTemplate>();

        private IBuildModeTargetable target = Substitute.For<IBuildModeTargetable>();

        private List<IBuildModeTargetable> targetList;

        private BuildModeAutoTargetedArgs buildModeAutoTargetedArgs;

        private CAutoTargetedComponent subject;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            buildModeAutoTargetedArgs = new BuildModeAutoTargetedArgs(targetCoord);

            template.TargetingRule.GenerateArgs(targetCoord).Returns(buildModeAutoTargetedArgs);

            targetList = new List<IBuildModeTargetable>
            {
                target
            };

            BuildModeAutoTargetedRuleService.RunRule = (args) =>
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
            typeof(BuildModeAutoTargetedRuleService).TypeInitializer.Invoke(null, null);
        }

        [Test]
        public void FindTargets_PassedValidInput_ReturnsRuleResultForTheArgsGeneratedFromThisInput()
        {
            var result = subject.FindTargets(targetCoord);

            Assert.AreEqual(result, targetList);
        }
    }
}