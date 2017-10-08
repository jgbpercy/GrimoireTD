using System.Linq;
using NUnit.Framework;
using NSubstitute;
using GrimoireTD.Abilities;
using GrimoireTD.DefendingEntities;
using GrimoireTD.Dependencies;
using GrimoireTD.Abilities.DefendMode;
using GrimoireTD.Abilities.BuildMode;

namespace GrimoireTD.Tests.AbilitiesTests
{
    public class AbilitiesTests
    {
        private IDefendingEntity attachedToDefendingEntity = Substitute.For<IDefendingEntity>();

        private IDefendModeAbilityManager defendModeAbilityManager = Substitute.For<IDefendModeAbilityManager>();

        private IDefendModeAbilityTemplate defendModeAbilityTemplate = Substitute.For<IDefendModeAbilityTemplate>();

        private IBuildModeAbilityTemplate buildModeAbilityTemplate = Substitute.For<IBuildModeAbilityTemplate>();

        private IDefendModeAbility defendModeAbility = Substitute.For<IDefendModeAbility>();

        private IBuildModeAbility buildModeAbility = Substitute.For<IBuildModeAbility>();

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            DependencyProvider.DefendModeAbilityManager = (abilities, defendingEntity) =>
            {
                return defendModeAbilityManager;
            };

            defendModeAbility.AbilityTemplate.Returns(defendModeAbilityTemplate);
            defendModeAbility.DefendModeAbilityTemplate.Returns(defendModeAbilityTemplate);

            buildModeAbility.AbilityTemplate.Returns(buildModeAbilityTemplate);
            buildModeAbility.BuildModeAbilityTemplate.Returns(buildModeAbilityTemplate);
        }

        [SetUp]
        public void EachTestSetup()
        {

        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            typeof(DependencyProvider).TypeInitializer.Invoke(null, null);
        }

        private CAbilities ConstructSubject()
        {
            return new CAbilities(
                attachedToDefendingEntity
            );
        }

        [Test]
        public void AddAbility_PassedAnyAbility_AddsAbilityToPublicList()
        {
            var subject = ConstructSubject();

            subject.AddAbility(defendModeAbility);

            Assert.True(subject.AbilityList.Values.Contains(defendModeAbility));
        }

        [Test]
        public void AddAbility_PassedAnyAbility_FiresOnAbilityAddedEvent()
        {
            var subject = ConstructSubject();

            var eventTester = new EventTester<EAOnAbilityAdded>();
            subject.OnAbilityAdded += eventTester.Handler;

            subject.AddAbility(defendModeAbility);

            eventTester.AssertFired(1);
        }

        [Test]
        public void AddAbility_PassedBuildModeAbility_FiresOnBuildModeAbilityAddedEvent()
        {
            var subject = ConstructSubject();

            var eventTester = new EventTester<EAOnBuildModeAbilityAdded>();
            subject.OnBuildModeAbilityAdded += eventTester.Handler;

            subject.AddAbility(buildModeAbility);

            eventTester.AssertFired(1);
        }

        [Test]
        public void AddAbility_PassedDefendModeAbility_FiresOnDefendModeAbilityAddedEvent()
        {
            var subject = ConstructSubject();

            var eventTester = new EventTester<EAOnDefendModeAbilityAdded>();
            subject.OnDefendModeAbilityAdded += eventTester.Handler;

            subject.AddAbility(defendModeAbility);

            eventTester.AssertFired(1);
        }

        [Test]
        public void BuildModeAbilities_Always_ReturnsAllBuildModeAbilities()
        {
            var subject = ConstructSubject();

            var buildModeAbilityTwo = Substitute.For<IBuildModeAbility>();

            subject.AddAbility(buildModeAbility);
            subject.AddAbility(buildModeAbilityTwo);
            subject.AddAbility(defendModeAbility);

            var result = subject.BuildModeAbilities();

            Assert.True(result.Contains(buildModeAbility));
            Assert.True(result.Contains(buildModeAbilityTwo));
            Assert.AreEqual(2, result.Count);
        }

        [Test]
        public void DefendModeAbilities_Always_ReturnsAllDefendModeAbilitiesAndNoBuildModeAbilities()
        {
            var subject = ConstructSubject();

            var defendModeAbilityTwo = Substitute.For<IDefendModeAbility>();

            subject.AddAbility(defendModeAbility);
            subject.AddAbility(defendModeAbilityTwo);
            subject.AddAbility(buildModeAbility);

            var result = subject.DefendModeAbilities();

            Assert.True(result.Contains(defendModeAbility));
            Assert.True(result.Contains(defendModeAbilityTwo));
            Assert.AreEqual(2, result.Count);
        }

        [Test]
        public void TryRemoveAbility_PassedAbilityNotPresent_ReturnsFalse()
        {
            var subject = ConstructSubject();

            subject.AddAbility(buildModeAbility);

            var result = subject.TryRemoveAbility(defendModeAbility);

            Assert.False(result);
        }

        [Test]
        public void TryRemoveAbility_PassedAbilityThatIsPresent_ReturnsTrue()
        {
            var subject = ConstructSubject();

            subject.AddAbility(buildModeAbility);

            var result = subject.TryRemoveAbility(buildModeAbility);

            Assert.True(result);
        }

        [Test]
        public void TryRemoveAbility_PassedAbilityThatIsPresent_RemovesAbilityFromPublicList()
        {
            var subject = ConstructSubject();

            subject.AddAbility(buildModeAbility);
            subject.AddAbility(defendModeAbility);

            subject.TryRemoveAbility(buildModeAbility);

            Assert.False(subject.AbilityList.Values.Contains(buildModeAbility));
            Assert.AreEqual(1, subject.AbilityList.Count);
        }

        [Test]
        public void TryRemoveAbility_PassedAbilityThatIsPresent_FiresOnAbilityRemovedEvent()
        {
            var subject = ConstructSubject();

            var eventTester = new EventTester<EAOnAbilityRemoved>();
            subject.OnAbilityRemoved += eventTester.Handler;

            subject.AddAbility(buildModeAbility);

            subject.TryRemoveAbility(buildModeAbility);

            eventTester.AssertFired(1);
            eventTester.AssertResult(subject, x => x.Ability == buildModeAbility);
        }

        [Test]
        public void TryRemoveAbility_PassedBuildModeAbilityThatIsPresent_FiresOnBuildModeAbilityRemovedEvent()
        {
            var subject = ConstructSubject();

            var eventTester = new EventTester<EAOnBuildModeAbilityRemoved>();
            subject.OnBuildModeAbilityRemoved += eventTester.Handler;

            subject.AddAbility(buildModeAbility);

            subject.TryRemoveAbility(buildModeAbility);

            eventTester.AssertFired(1);
            eventTester.AssertResult(subject, x => x.BuildModeAbility == buildModeAbility);
        }

        [Test]
        public void TryRemoveAbility_PassedDefendModeAbilityThatIsPresent_FiresOnDefendModeAbilityRemovedEvent()
        {
            var subject = ConstructSubject();

            var eventTester = new EventTester<EAOnDefendModeAbilityRemoved>();
            subject.OnDefendModeAbilityRemoved += eventTester.Handler;

            subject.AddAbility(defendModeAbility);

            subject.TryRemoveAbility(defendModeAbility);

            eventTester.AssertFired(1);
            eventTester.AssertResult(subject, x => x.DefendModeAbility == defendModeAbility);
        }

        [Test]
        public void TryRemoveAbility_PassedTemplateForAbilityThatIsNotPresent_ReturnsFalse()
        {
            var nonPresentTemplate = Substitute.For<IBuildModeAbilityTemplate>();

            var subject = ConstructSubject();

            subject.AddAbility(buildModeAbility);

            var result = subject.TryRemoveAbility(nonPresentTemplate);

            Assert.False(result);
        }

        [Test]
        public void TryRemoveAbility_PassedTemplateForAbilityThatIsPresent_ReturnsTrue()
        {
            var subject = ConstructSubject();

            subject.AddAbility(buildModeAbility);

            var result = subject.TryRemoveAbility(buildModeAbilityTemplate);

            Assert.True(result);
        }

        [Test]
        public void TryRemoveAbility_PassedTemplateForAbilityThatIsPresent_RemovesAbilityFromPublicList()
        {
            var subject = ConstructSubject();

            subject.AddAbility(buildModeAbility);
            subject.AddAbility(defendModeAbility);

            subject.TryRemoveAbility(buildModeAbilityTemplate);

            Assert.False(subject.AbilityList.Values.Contains(buildModeAbility));
            Assert.AreEqual(1, subject.AbilityList.Count);
        }

        [Test]
        public void TryRemoveAbility_PassedTemplateForAbilityThatIsPresent_FiresOnAbilityRemovedEvent()
        {
            var subject = ConstructSubject();

            var eventTester = new EventTester<EAOnAbilityRemoved>();
            subject.OnAbilityRemoved += eventTester.Handler;

            subject.AddAbility(buildModeAbility);

            subject.TryRemoveAbility(buildModeAbilityTemplate);

            eventTester.AssertFired(1);
            eventTester.AssertResult(subject, x => x.Ability == buildModeAbility);
        }

        [Test]
        public void TryRemoveAbility_PassedTemplateForBuildModeAbilityThatIsPresent_FiresOnBuildModeAbilityRemovedEvent()
        {
            var subject = ConstructSubject();

            var eventTester = new EventTester<EAOnBuildModeAbilityRemoved>();
            subject.OnBuildModeAbilityRemoved += eventTester.Handler;

            subject.AddAbility(buildModeAbility);

            subject.TryRemoveAbility(buildModeAbilityTemplate);

            eventTester.AssertFired(1);
            eventTester.AssertResult(subject, x => x.BuildModeAbility == buildModeAbility);
        }

        [Test]
        public void TryRemoveAbility_PassedTemplateDefendModeAbilityThatIsPresent_FiresOnDefendModeAbilityRemovedEvent()
        {
            var subject = ConstructSubject();

            var eventTester = new EventTester<EAOnDefendModeAbilityRemoved>();
            subject.OnDefendModeAbilityRemoved += eventTester.Handler;

            subject.AddAbility(defendModeAbility);

            subject.TryRemoveAbility(defendModeAbilityTemplate);

            eventTester.AssertFired(1);
            eventTester.AssertResult(subject, x => x.DefendModeAbility == defendModeAbility);
        }
    }
}