using NUnit.Framework;
using NSubstitute;
using GrimoireTD.Attributes;

namespace GrimoireTD.Tests.AdditiveAttributeTests
{
    public class AdditiveAttributeTests : AttributeTests.AttributeTests
    {
        protected override CAttribute ConstructSubject()
        {
            return new CAdditiveAttribute("TestAttributeName");
        }

        [TestCase(0.2f, 0.2f, 0.2f, 0.6f)]
        [TestCase(0.3f, 0.4f, 0.5f, 1.2f)]
        public void Value_AfterModifierAreAdded_ReturnsCorrectValues(
            float modifierMag1,
            float modifierMag2,
            float modifierMag3,
            float expected)
        {
            var modifier1 = Substitute.For<IAttributeModifier>();
            var modifier2 = Substitute.For<IAttributeModifier>();
            var modifier3 = Substitute.For<IAttributeModifier>();

            modifier1.Magnitude.Returns(modifierMag1);
            modifier2.Magnitude.Returns(modifierMag2);
            modifier3.Magnitude.Returns(modifierMag3);

            var subject = ConstructSubject();

            subject.AddModifier(modifier1);
            subject.AddModifier(modifier2);
            subject.AddModifier(modifier3);

            AssertExt.Approximately(expected, subject.Value());
        }

        [TestCase(0.2f, 0.2f, 0.2f, 0.4f)]
        [TestCase(0.3f, 0.4f, 0.5f, 0.7f)]
        public void Value_AfterModifiersAreAddedAndRemoved_ReturnsCorrectValues(
            float modifierMag1,
            float modifierMag2,
            float modifierMag3,
            float expected)
        {
            var modifier1 = Substitute.For<IAttributeModifier>();
            var modifier2 = Substitute.For<IAttributeModifier>();
            var modifier3 = Substitute.For<IAttributeModifier>();

            modifier1.Magnitude.Returns(modifierMag1);
            modifier2.Magnitude.Returns(modifierMag2);
            modifier3.Magnitude.Returns(modifierMag3);

            var subject = ConstructSubject();

            subject.AddModifier(modifier1);
            subject.AddModifier(modifier2);
            subject.AddModifier(modifier3);

            subject.TryRemoveModifier(modifier3);

            AssertExt.Approximately(expected, subject.Value());
        }
    }
}