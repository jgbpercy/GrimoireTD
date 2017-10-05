using NUnit.Framework;
using NSubstitute;
using GrimoireTD.Abilities.DefendMode;
using GrimoireTD.Abilities;
using GrimoireTD.DefendingEntities;

namespace GrimoireTD.Tests.DefendModeAbilityManagerTests
{
    public class DefendModeAbilityManagerTests
    {
        private IAbilities abilities;

        private IDefendingEntity attachedToDefendingEntity;

        private CDefendModeAbilityManager subject;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            abilities = Substitute.For<IAbilities>();

            attachedToDefendingEntity = Substitute.For<IDefendingEntity>();

            subject = new CDefendModeAbilityManager(abilities, attachedToDefendingEntity);
        }
    }
}