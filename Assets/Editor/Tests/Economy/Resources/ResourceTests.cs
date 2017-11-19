using NUnit.Framework;
using NSubstitute;
using GrimoireTD.Economy;

namespace GrimoireTD.Tests.ResourceTests
{
    public class ResourceTests
    {
        //Primitives and Basic Objects
        private int maxAmount = 100;

        //Template Deps
        IResourceTemplate template = Substitute.For<IResourceTemplate>();

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            //Template Deps
            template.MaxAmount.Returns(maxAmount);
        }

        private CResource ConstructSubject()
        {
            return new CResource(template);
        }

        [Test]
        public void Ctor_Always_CreatesResourceWithAmountOwnedZero()
        {
            var subject = ConstructSubject();

            Assert.AreEqual(0, subject.AmountOwned);
        }

        [Test]
        public void CanDoTransaction_PassedAmountThatWouldTakeAmountOwnedBelowZero_ReturnsFalse()
        {
            var subject = ConstructSubject();

            var result = subject.CanDoTransaction(-1);

            Assert.False(result);
        }

        [Test]
        public void CanDoTransaction_PassedAmountThatWouldTakeAmountOwnedAboveMaxAmount_ReturnsFalse()
        {
            var subject = ConstructSubject();

            var result = subject.CanDoTransaction(101);

            Assert.False(result);
        }

        [Test]
        public void CanDoTransaction_PassedAmountThatWouldTakeAmountOwnedToAValueBetweenZeroAndMax_ReturnsTrue()
        {
            var subject = ConstructSubject();

            var result = subject.CanDoTransaction(100);

            Assert.True(result);
        }

        [Test]
        public void DoTransaction_Always_ChangesAmountOwnedByAmountPassed()
        {
            var subject = ConstructSubject();

            subject.DoTransaction(5);

            Assert.AreEqual(5, subject.AmountOwned);
        }

        [Test]
        public void DoTransaction_Always_FiresOnResourceChangedEventWithCorrectNewAmountAndAmountChanged()
        {
            var subject = ConstructSubject();

            var eventTester = new EventTester<EAOnResourceChanged>();
            subject.OnResourceChanged += eventTester.Handler;

            subject.DoTransaction(10);

            eventTester.AssertFired(1);
            eventTester.AssertResult(subject, args => args.ByAmount == 10 && args.ToAmount == 10);
        }
    }
}