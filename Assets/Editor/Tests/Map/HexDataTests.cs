using NUnit.Framework;
using NSubstitute;
using GrimoireTD.Map;
using GrimoireTD.Defenders.Units;
using GrimoireTD.Defenders.Structures;
using GrimoireTD.Defenders.DefenderEffects;
using System.Linq;
using System.Collections.Generic;

namespace GrimoireTD.Tests.HexDataTests
{
    public class HexDataTests
    {
        //Other Deps Passed To Ctor or SetUp
        private IHexType pathableHexType = Substitute.For<IHexType>();
        private IHexType unpathableHexType = Substitute.For<IHexType>();

        private IHexType buildableHexType = Substitute.For<IHexType>();
        private IHexType unbuildableHexType = Substitute.For<IHexType>();

        private IHexType occupyableHexType = Substitute.For<IHexType>();
        private IHexType unoccupyableHexType = Substitute.For<IHexType>();

        //Other Objects Passed To Methods
        IUnit unit = Substitute.For<IUnit>();

        IStructure structure = Substitute.For<IStructure>();

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            //Other Deps Passed To Ctor or SetUp
            pathableHexType.IsPathableByCreeps.Returns(true);
            unpathableHexType.IsPathableByCreeps.Returns(false);

            buildableHexType.IsBuildable.Returns(true);
            unbuildableHexType.IsBuildable.Returns(false);

            occupyableHexType.UnitCanOccupy.Returns(true);
            unoccupyableHexType.UnitCanOccupy.Returns(false);
        }

        [Test]
        public void IsPathableByCreeps_HexTypeNotPathable_ReturnsFalse()
        {
            var subject = new CHexData(unpathableHexType);

            Assert.False(subject.IsPathableByCreeps());
        }

        [Test]
        public void IsPathableByCreeps_UnitOnHex_ReturnsFalse()
        {
            var subject = new CHexData(pathableHexType);

            subject.PlaceUnitHere(unit);

            Assert.False(subject.IsPathableByCreeps());
        }

        [Test]
        public void IsPathableByCreeps_StrucutureOnHext_ReturnsFalse()
        {
            var subject = new CHexData(pathableHexType);

            subject.BuildStructureHere(structure);

            Assert.False(subject.IsPathableByCreeps());
        }

        [Test]
        public void IsPathableByCreeps_PathableHexTypeAndNoUnitOrStructure_ReturnsTrue()
        {
            var subject = new CHexData(pathableHexType);

            Assert.True(subject.IsPathableByCreeps());
        }

        [Test]
        public void IsPathableByCreepsWithUnitRemoved_HexTypeNotPathable_ReturnsFalse()
        {
            var subject = new CHexData(unpathableHexType);

            Assert.False(subject.IsPathableByCreepsWithUnitRemoved());
        }

        [Test]
        public void IsPathableByCreepsWithUnitRemoved_StructureOnHex_ReturnsFalse()
        {
            var subject = new CHexData(pathableHexType);

            subject.BuildStructureHere(structure);

            Assert.False(subject.IsPathableByCreepsWithUnitRemoved());
        }

        [Test]
        public void IsPathableByCreepsWithUnitRemoved_UnitOnHex_ReturnsTrue()
        {
            var subject = new CHexData(pathableHexType);

            subject.PlaceUnitHere(unit);

            Assert.True(subject.IsPathableByCreepsWithUnitRemoved());
        }

        [Test]
        public void IsPathableByCreepsWithTypePathable_DefenderOnHex_ReturnsFalse()
        {
            var subject = new CHexData(unpathableHexType);

            subject.PlaceUnitHere(unit);

            Assert.False(subject.IsPathableByCreepsWithTypePathable());
        }

        [Test]
        public void IsPathableByCreepsWithTypePathable_NoDefenderOnHex_ReturnsTrue()
        {
            var subject = new CHexData(unpathableHexType);

            Assert.True(subject.IsPathableByCreepsWithTypePathable());
        }

        [Test]
        public void IsPathableByCreepsWithStructureRemoved_HexTypeNotPathable_ReturnsFalse()
        {
            var subject = new CHexData(unpathableHexType);

            Assert.False(subject.IsPathableByCreepsWithStructureRemoved());
        }   

        [Test]
        public void IsPathableByCreepsWithStructureRemoved_UnitOnHex_ReturnsFalse()
        {
            var subject = new CHexData(pathableHexType);

            subject.PlaceUnitHere(unit);

            Assert.False(subject.IsPathableByCreepsWithStructureRemoved());
        }

        [Test]
        public void IsPathableByCreepsWithStructureRemoved_StructureOnHex_ReturnsTrue()
        {
            var subject = new CHexData(pathableHexType);

            subject.BuildStructureHere(structure);

            Assert.True(subject.IsPathableByCreepsWithStructureRemoved());
        }

        [Test]
        public void CanPlaceStructureHere_HexTypeNotBuildable_ReturnsFalse()
        {
            var subject = new CHexData(unbuildableHexType);

            Assert.False(subject.CanBuildStructureHere());
        }

        [Test]
        public void CanPlaceStructureHere_StructureOnHex_ReturnsFalse()
        {
            var subject = new CHexData(buildableHexType);

            subject.BuildStructureHere(structure);

            Assert.False(subject.CanBuildStructureHere());
        }

        [Test]
        public void CanPlaceStructureHere_HexTypeBuildableAndNoStructure_ReturnsTrue()
        {
            var subject = new CHexData(buildableHexType);

            subject.PlaceUnitHere(unit);

            Assert.True(subject.CanBuildStructureHere());
        }

        [Test]
        public void CanPlaceUnitHere_HexTypeNotOccupyable_ReturnsFalse()
        {
            var subject = new CHexData(unoccupyableHexType);

            Assert.False(subject.CanPlaceUnitHere());
        }

        [Test]
        public void CanPlaceUnitHere_UnitOnHex_ReturnsFalse()
        {
            var subject = new CHexData(occupyableHexType);

            subject.PlaceUnitHere(unit);

            Assert.False(subject.CanPlaceUnitHere());
        }

        [Test]
        public void CanPlaceUnitHere_HexTypeOccupyableAndNoUnit_ReturnsTrue()
        {
            var subject = new CHexData(occupyableHexType);

            subject.BuildStructureHere(structure);

            Assert.True(subject.CanPlaceUnitHere());
        }

        [Test]
        public void IsEmpty_UnitOnHex_ReturnsFalse()
        {
            var subject = new CHexData(pathableHexType);

            subject.PlaceUnitHere(unit);

            Assert.False(subject.IsEmpty());
        }

        [Test]
        public void IsEmpty_StructureOnHex_ReturnsFalse()
        {
            var subject = new CHexData(pathableHexType);

            subject.BuildStructureHere(structure);

            Assert.False(subject.IsEmpty());
        }

        [Test]
        public void IsEmpty_NoStructureOrUnit_ReturnsTrue()
        {
            var subject = new CHexData(pathableHexType);

            Assert.True(subject.IsEmpty());
        }

        [Test]
        public void BuildStrutureHere_Always_AddStructureToHex()
        {
            var subject = new CHexData(buildableHexType);

            subject.BuildStructureHere(structure);

            Assert.AreEqual(structure, subject.StructureHere);
        }

        [Test]
        public void PlaceUnitHere_Always_AddsUnitToHex()
        {
            var subject = new CHexData(pathableHexType);

            subject.PlaceUnitHere(unit);

            Assert.AreEqual(unit, subject.UnitHere);
        }

        [Test]
        public void RemoveUnitHere_Always_RemovesUnitFromHex()
        {
            var subject = new CHexData(pathableHexType);

            subject.PlaceUnitHere(unit);

            subject.RemoveUnitHere();

            Assert.Null(subject.UnitHere);
        }

        [Test]
        public void AddDefenderAura_Always_AddsDefenderAuraToHex()
        {
            var subject = new CHexData(pathableHexType);

            var aura = Substitute.For<IDefenderAura>();

            subject.AddDefenderAura(aura);

            Assert.True(subject.DefenderAurasHere.Contains(aura));
        }

        [Test]
        public void AddDefenderAura_Always_FiresEvent()
        {
            var subject = new CHexData(pathableHexType);

            var eventTester = new EventTester<EAOnCallbackListAdd<IDefenderAura>>();
            subject.DefenderAurasHere.OnAdd += eventTester.Handler;

            var aura = Substitute.For<IDefenderAura>();

            subject.AddDefenderAura(aura);

            eventTester.AssertFired(true);
            eventTester.AssertResult(subject.DefenderAurasHere, arg => arg.AddedItem == aura);
        }

        [Test]
        public void DefenderAuraCleared_Always_RemovesAuraFromHex()
        {
            var subject = new CHexData(pathableHexType);

            var aura = Substitute.For<IDefenderAura>();

            subject.AddDefenderAura(aura);

            aura.OnClearDefenderAura += Raise.EventWith(new EAOnClearDefenderAura(aura));

            Assert.False(subject.DefenderAurasHere.Contains(aura));
        }

        [Test]
        public void DefenderAuraCleared_Always_FiresEvent()
        {
            var subject = new CHexData(pathableHexType);

            var eventTester = new EventTester<EAOnCallbackListRemove<IDefenderAura>>();
            subject.DefenderAurasHere.OnRemove += eventTester.Handler;

            var aura = Substitute.For<IDefenderAura>();

            subject.AddDefenderAura(aura);

            aura.OnClearDefenderAura += Raise.EventWith(new EAOnClearDefenderAura(aura));

            eventTester.AssertFired(1);
            eventTester.AssertResult(subject.DefenderAurasHere, args => args.RemovedItem == aura);
        }
    }
}