using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using GrimoireTD.Abilities.DefendMode;
using GrimoireTD.Creeps;
using GrimoireTD.DefendingEntities;

namespace GrimoireTD.Tests.DefendModeTargetingComponentTests
{
    public class DefendModeTargetingComponentTests
    {
        private IDefendingEntity attachedToDefendingEntity = Substitute.For<IDefendingEntity>();

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

            defendModeTargetingArgs = new DefendModeTargetingArgs(attachedToDefendingEntity);

            template.TargetingRule
                .GenerateArgs(attachedToDefendingEntity)
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
            var result = subject.FindTargets(attachedToDefendingEntity);

            Assert.AreEqual(result, targetList);
        }
    }
}