using NUnit.Framework;
using NSubstitute;
using GrimoireTD.Abilities;
using GrimoireTD.DefendingEntities;

namespace GrimoireTD.Tests.AbilitiesTests
{
    public class AbilitiesTests
    {
        private IDefendingEntity attachedToDefendingEntity;

        private CAbilities subject;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            attachedToDefendingEntity = Substitute.For<IDefendingEntity>();

            subject = new CAbilities(attachedToDefendingEntity);
        }
    }
}