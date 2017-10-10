using NUnit.Framework;
using NSubstitute;
using GrimoireTD.Attributes;

namespace GrimoireTD.Tests.AttributeTests
{
    public abstract class AttributeTests
    {
        protected float modifierMagnitude = 2f;

        protected IAttributeModifier modifier = Substitute.For<IAttributeModifier>();

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            modifier.Magnitude.Returns(modifierMagnitude);
        }

        protected abstract CAttribute ConstructSubject();

        [Test]
        public void AddModifier_Always_FiresOnAttributeChangedEvent()
        {
            var subject = ConstructSubject();

            var eventTester = new EventTester<EAOnAttributeChanged>();
            subject.OnAttributeChanged += eventTester.Handler;

            subject.AddModifier(modifier);

            eventTester.AssertFired(1);
        }

        [Test]
        public void TryRemoveModifier_IfModifierNotPresent_ReturnsFalse()
        {
            var subject = ConstructSubject();

            var result = subject.TryRemoveModifier(modifier);

            Assert.False(result);
        }

        [Test]
        public void TryRemoveModifier_IfModifierNotPresent_DoesNotFireEvent()
        {
            var subject = ConstructSubject();

            var eventTester = new EventTester<EAOnAttributeChanged>();
            subject.OnAttributeChanged += eventTester.Handler;

            subject.TryRemoveModifier(modifier);

            eventTester.AssertFired(false);
        }

        [Test]
        public void TryRemoveModifier_IfModifierPresent_ReturnsTrue()
        {
            var subject = ConstructSubject();

            subject.AddModifier(modifier);

            var result = subject.TryRemoveModifier(modifier);

            Assert.True(result);
        }

        [Test]
        public void TryRemoveModifier_IfModifierPresent_FiresEvent()
        {
            var subject = ConstructSubject();

            subject.AddModifier(modifier);

            var eventTester = new EventTester<EAOnAttributeChanged>();
            subject.OnAttributeChanged += eventTester.Handler;

            var result = subject.TryRemoveModifier(modifier);

            eventTester.AssertFired(1);
        }
    }
}