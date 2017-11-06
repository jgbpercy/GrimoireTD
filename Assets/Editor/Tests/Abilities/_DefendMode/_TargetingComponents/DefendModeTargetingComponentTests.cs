using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using GrimoireTD.Abilities.DefendMode;
using GrimoireTD.Creeps;
using GrimoireTD.Defenders;

namespace GrimoireTD.Tests.DefendModeTargetingComponentTests
{
    public class DefendModeTargetingComponentTests
    {
        private IDefender attachedToDefender = Substitute.For<IDefender>();

        private IDefendModeTargetable target = Substitute.For<IDefendModeTargetable>();

        private IDefendModeTargetingComponentTemplate template = Substitute.For<IDefendModeTargetingComponentTemplate>();

        private DefendModeTargetingArgs defendModeTargetingArgs;

        private List<IDefendModeTargetable> targetList;

        private CDefendModeTargetingComponent subject;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            targetList = new List<IDefendModeTargetable>
            {
                target
            };

            defendModeTargetingArgs = new DefendModeTargetingArgs(attachedToDefender);

            template.TargetingRule
                .GenerateArgs(attachedToDefender)
                .Returns(defendModeTargetingArgs);

            DefendModeTargetingRuleService.RunRule = (args) =>
            {
                if (args == defendModeTargetingArgs)
                {
                    return targetList;
                }
                else
                {
                    return null;
                }
            };

            subject = new CDefendModeTargetingComponent(template);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            typeof(DefendModeTargetingRuleService).TypeInitializer.Invoke(null, null);
        }

        [Test]
        public void FindTargets_PassedValidInput_ReturnsRuleResultForTheArgsGeneratedFromThisInput()
        {
            var result = subject.FindTargets(attachedToDefender);

            Assert.AreEqual(result, targetList);
        }
    }
}