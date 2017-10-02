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
        private IDefendingEntity attachedToDefendingEntity;

        private List<ICreep> creepList;

        private IDefendModeTargetable target;

        private List<IDefendModeTargetable> targetList;

        private DefendModeTargetingArgs defendModeTargetingArgs;

        private IDefendModeTargetingComponentTemplate template;

        private CDefendModeTargetingComponent subject;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            attachedToDefendingEntity = Substitute.For<IDefendingEntity>();

            creepList = new List<ICreep>();

            target = Substitute.For<IDefendModeTargetable>();

            targetList = new List<IDefendModeTargetable>
            {
                target
            };

            template = Substitute.For<IDefendModeTargetingComponentTemplate>();

            defendModeTargetingArgs = new DefendModeTargetingArgs(attachedToDefendingEntity);

            template.TargetingRule.GenerateArgs(attachedToDefendingEntity, creepList)
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
            var result = subject.FindTargets(attachedToDefendingEntity, creepList);

            Assert.AreEqual(result, targetList);
        }
    }
}