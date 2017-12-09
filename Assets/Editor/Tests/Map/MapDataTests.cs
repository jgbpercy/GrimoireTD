using NUnit.Framework;
using NSubstitute;
using GrimoireTD.Map;
using System.Collections.Generic;
using UnityEngine;
using GrimoireTD.Levels;
using GrimoireTD.UI;
using GrimoireTD.Dependencies;
using GrimoireTD.Defenders.Structures;
using GrimoireTD.Defenders.Units;
using System.Linq;

namespace GrimoireTD.Tests.MapDataTests
{
    public class MapDataTests
    {
        /* Default map in these tests:
         * 3   X 0 0 0
         * 2  0 X X 0
         * 1   0 X 0 0
         * 0  0 0 0 0
         *    0 1 2 3
         */

        //Instance Dependency Provider Deps
        private IModelInterfaceController interfaceController;

        //Other Objects Passed To Methods
        private Dictionary<Coord, IHexData> mapDictionary;

        private IHexData hexAt00 = Substitute.For<IHexData>();
        private IHexData hexAt10 = Substitute.For<IHexData>();
        private IHexData hexAt20 = Substitute.For<IHexData>();
        private IHexData hexAt30 = Substitute.For<IHexData>();

        private IHexData hexAt01 = Substitute.For<IHexData>();
        private IHexData hexAt11 = Substitute.For<IHexData>();
        private IHexData hexAt21 = Substitute.For<IHexData>();
        private IHexData hexAt31 = Substitute.For<IHexData>();

        private IHexData hexAt02 = Substitute.For<IHexData>();
        private IHexData hexAt12 = Substitute.For<IHexData>();
        private IHexData hexAt22 = Substitute.For<IHexData>();
        private IHexData hexAt32 = Substitute.For<IHexData>();

        private IHexData hexAt03 = Substitute.For<IHexData>();
        private IHexData hexAt13 = Substitute.For<IHexData>();
        private IHexData hexAt23 = Substitute.For<IHexData>();
        private IHexData hexAt33 = Substitute.For<IHexData>();

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            //Model and Frame Updater
            DepsProv.TheInterfaceController = () => interfaceController;

            //Instance Dependency Provider Deps
            mapDictionary = new Dictionary<Coord, IHexData>
            {
                { new Coord(0, 0), hexAt00 },
                { new Coord(1, 0), hexAt10 },
                { new Coord(2, 0), hexAt20 },
                { new Coord(3, 0), hexAt30 },

                { new Coord(0, 1), hexAt01 },
                { new Coord(1, 1), hexAt11 },
                { new Coord(2, 1), hexAt21 },
                { new Coord(3, 1), hexAt31 },

                { new Coord(0, 2), hexAt02 },
                { new Coord(1, 2), hexAt12 },
                { new Coord(2, 2), hexAt22 },
                { new Coord(3, 2), hexAt32 },

                { new Coord(0, 3), hexAt03 },
                { new Coord(1, 3), hexAt13 },
                { new Coord(2, 3), hexAt23 },
                { new Coord(3, 3), hexAt33 },
            };

            MapImageToMapDictionaryService.GetMapDictionary = (mapImage, colorsToTypesDictionary) => mapDictionary;
        }

        [SetUp]
        public void EachTestSetUp()
        {
            interfaceController = Substitute.For<IModelInterfaceController>();

            foreach (var hex in mapDictionary)
            {
                hex.Value.IsPathableByCreeps().Returns(true);
                hex.Value.IsPathableByCreepsWithStructureRemoved().Returns(true);
                hex.Value.IsPathableByCreepsWithTypePathable().Returns(true);
                hex.Value.IsPathableByCreepsWithUnitRemoved().Returns(true);

                hex.Value.CanBuildStructureHere().Returns(true);
                hex.Value.CanPlaceUnitHere().Returns(true);

                if (hex.Key == new Coord(1, 1) || hex.Key == new Coord(1, 2) || hex.Key == new Coord(2, 2) || hex.Key == new Coord(0, 3))
                {
                    hex.Value.IsPathableByCreeps().Returns(false);
                    hex.Value.IsPathableByCreepsWithStructureRemoved().Returns(false);
                    hex.Value.IsPathableByCreepsWithUnitRemoved().Returns(false);
                }
            }
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            typeof(MapImageToMapDictionaryService).TypeInitializer.Invoke(null, null);
            typeof(PathingService).TypeInitializer.Invoke(null, null);
            typeof(DepsProv).TypeInitializer.Invoke(null, null);
        }

        private CMapData ConstructAndSetUpSubject()
        {
            return ConstructAndSetUpSubject(new List<IStartingStructure>(), new List<IStartingUnit>());
        }

        private CMapData ConstructAndSetUpSubject(List<IStartingStructure> startingStructures, List<IStartingUnit> startingUnits)
        {
            var subject = new CMapData();

            SetUpSubject(subject, startingStructures, startingUnits);

            return subject;
        }

        private void SetUpSubject(CMapData subject, List<IStartingStructure> startingStructures, List<IStartingUnit> startingUnits)
        {
            var mapImage = new Texture2D(8, 8);

            var colorsToTypesDictionary = new Dictionary<Color32, IHexType>();

            subject.SetUp(
                mapImage,
                colorsToTypesDictionary,
                startingStructures,
                startingUnits);
        }

        [Test]
        public void SetUp_Always_FiresOnMapCreatedEvent()
        {
            var subject = new CMapData();

            var eventTester = new EventTester<EAOnMapCreated>();
            subject.OnMapCreated += eventTester.Handler;

            SetUpSubject(subject, new List<IStartingStructure>(), new List<IStartingUnit>());

            eventTester.AssertFired(1);
            eventTester.AssertResult(subject, args => args.MapData == subject);
        }

        [Test]
        public void SetUp_Always_FiresOnPathGeneratedOrChangedEvent()
        {
            var subject = new CMapData();

            var eventTester = new EventTester<EAOnPathGeneratedOrChanged>();
            subject.OnPathGeneratedOrChanged += eventTester.Handler;

            SetUpSubject(subject, new List<IStartingStructure>(), new List<IStartingUnit>());

            eventTester.AssertFired(1);
        }

        [Test]
        public void SetUp_Always_AddsStartingStructuresToHexes()
        {
            var structureTemplate = Substitute.For<IStructureTemplate>();
            var structure = Substitute.For<IStructure>();
            var startingStructure = Substitute.For<IStartingStructure>();

            structureTemplate.GenerateStructure(Arg.Any<Coord>()).Returns(structure);

            startingStructure.StartingPosition.Returns(new Coord(3, 0));
            startingStructure.StructureTemplate.Returns(structureTemplate);

            ConstructAndSetUpSubject(new List<IStartingStructure> { startingStructure }, new List<IStartingUnit>());

            hexAt30.Received(1).BuildStructureHere(structure);
        }

        [Test]
        public void SetUp_Always_FiresOnStructureCreatedEventForStartingStructures()
        {
            var structureTemplate = Substitute.For<IStructureTemplate>();
            var structure = Substitute.For<IStructure>();
            var startingStructure = Substitute.For<IStartingStructure>();

            structureTemplate.GenerateStructure(Arg.Any<Coord>()).Returns(structure);

            startingStructure.StartingPosition.Returns(new Coord(3, 0));
            startingStructure.StructureTemplate.Returns(structureTemplate);

            var subject = new CMapData();

            var eventTester = new EventTester<EAOnStructureCreated>();
            subject.OnStructureCreated += eventTester.Handler;

            SetUpSubject(subject, new List<IStartingStructure> { startingStructure }, new List<IStartingUnit>());

            eventTester.AssertFired(1);
            eventTester.AssertResult(subject, args => args.StructureCreated == structure && args.Position == new Coord(3, 0));
        }

        [Test]
        public void SetUp_Always_AddsStartingUnitsToHexes()
        {
            var unitTemplate = Substitute.For<IUnitTemplate>();
            var unit = Substitute.For<IUnit>();
            var startingUnit = Substitute.For<IStartingUnit>();

            unitTemplate.GenerateUnit(Arg.Any<Coord>()).Returns(unit);

            startingUnit.StartingPosition.Returns(new Coord(3, 0));
            startingUnit.UnitTemplate.Returns(unitTemplate);
            
            ConstructAndSetUpSubject(new List<IStartingStructure>(), new List<IStartingUnit> { startingUnit });

            hexAt30.Received(1).PlaceUnitHere(unit);
        }

        [Test]
        public void SetUp_Always_FiresOnUnitCreatedEventForStartingUnits()
        {
            var unitTemplate = Substitute.For<IUnitTemplate>();
            var unit = Substitute.For<IUnit>();
            var startingUnit = Substitute.For<IStartingUnit>();

            unitTemplate.GenerateUnit(Arg.Any<Coord>()).Returns(unit);

            startingUnit.StartingPosition.Returns(new Coord(3, 0));
            startingUnit.UnitTemplate.Returns(unitTemplate);

            var subject = new CMapData();

            var eventTester = new EventTester<EAOnUnitCreated>();
            subject.OnUnitCreated += eventTester.Handler;

            SetUpSubject(subject, new List<IStartingStructure>(), new List<IStartingUnit> { startingUnit });

            eventTester.AssertFired(1);
            eventTester.AssertResult(subject, args => args.UnitCreated == unit && args.Position == new Coord(3, 0));
        }

        [Test]
        public void SetUp_Always_FetchesPathFromPathingService()
        {
            var subject = ConstructAndSetUpSubject();

            Assert.AreEqual(new Coord(0, 0), subject.CreepPath[subject.CreepPath.Count - 1]);
            Assert.AreEqual(new Coord(3, 3), subject.CreepPath[0]);
        }

        [Test]
        public void TryGetHexAt_ForCoordNotOnMap_ReturnsNull()
        {
            var subject = ConstructAndSetUpSubject();

            var result = subject.TryGetHexAt(new Coord(5, 5));

            Assert.Null(result);
        }

        [Test]
        public void TryGetHexAt_ForCoordOnMap_ReturnsHex()
        {
            var subject = ConstructAndSetUpSubject();

            var result = subject.TryGetHexAt(new Coord(2, 2));

            Assert.AreEqual(hexAt22, result);
        }

        [Test]
        public void GetHexAt_ForCoordOnMap_ReturnsHex()
        {
            var subject = ConstructAndSetUpSubject();

            var result = subject.GetHexAt(new Coord(1, 1));

            Assert.AreEqual(hexAt11, result);
        }

        [Test]
        public void GetExtantNeighboursOf_ForHexesInMiddle_ReturnsSixNeighbours()
        {
            var subject = ConstructAndSetUpSubject();

            var result = subject.GetExtantNeighboursOf(new Coord(1, 1));

            Assert.Contains(new Coord(1, 0), result);
            Assert.Contains(new Coord(2, 0), result);
            Assert.Contains(new Coord(2, 1), result);
            Assert.Contains(new Coord(2, 2), result);
            Assert.Contains(new Coord(1, 2), result);
            Assert.Contains(new Coord(0, 1), result);
        }

        [Test]
        public void GetExtantNeighboursOf_ForHexesOnEdge_ReturnsExtantNeighbours()
        {
            var subject = ConstructAndSetUpSubject();

            var result = subject.GetExtantNeighboursOf(new Coord(1, 0));

            Assert.Contains(new Coord(0, 0), result);
            Assert.Contains(new Coord(0, 1), result);
            Assert.Contains(new Coord(1, 1), result);
            Assert.Contains(new Coord(2, 0), result);
        }

        [Test]
        public void GetUncheckedNeighboursOf_Always_ReturnsSixNeighbours()
        {
            var result = CMapData.GetUncheckedNeighboursOf(new Coord(0, 0));

            Assert.Contains(new Coord(-1, 1), result);
            Assert.Contains(new Coord(0, 1), result);
            Assert.Contains(new Coord(1, 0), result);
            Assert.Contains(new Coord(0, -1), result);
            Assert.Contains(new Coord(-1, -1), result);
            Assert.Contains(new Coord(-1, 0), result);
        }

        [TestCase(1, 1, 2, 1)]
        [TestCase(1, 2, 0, 1)]
        public void CoordIsInRange_PassedRange1AndAdjacentHexes_ReturnsTrue(int coord1X, int coord1Y, int coord2X, int coord2Y)
        {
            var result = CMapData.CoordIsInRange(1, new Coord(coord1X, coord1Y), new Coord(coord2X, coord2Y));

            Assert.True(result);
        }

        [TestCase(1, 1, 3, 2)]
        [TestCase(1, 2, 1, 0)]
        [Test]
        public void CoordIsInRange_PassedRange1AndHexes2Apart_ReturnsFalse(int coord1X, int coord1Y, int coord2X, int coord2Y)
        {
            var result = CMapData.CoordIsInRange(1, new Coord(coord1X, coord1Y), new Coord(coord2X, coord2Y));

            Assert.False(result);
        }

        [TestCase(0, 0, 3, 3, 5)]
        [TestCase(2, 0, 1, 3, 3)]
        public void Distance_Always_ReturnsDistanceBetweenTwoCoords(int coord1X, int coord1Y, int coord2X, int coord2Y, int expectedDistance)
        {
            var result = CMapData.Distance(new Coord(coord1X, coord1Y), new Coord(coord2X, coord2Y));

            Assert.AreEqual(expectedDistance, result);
        }

        [Test]
        public void GetCoordsInRange_Always_ReturnsAllExtantCoordsInRange()
        {
            var subject = ConstructAndSetUpSubject();

            var result = subject.GetCoordsInRange(2, new Coord(3, 1));

            Assert.Contains(new Coord(2, 0), result);
            Assert.Contains(new Coord(3, 0), result);
            Assert.Contains(new Coord(1, 1), result);
            Assert.Contains(new Coord(2, 1), result);
            Assert.Contains(new Coord(3, 1), result);
            Assert.Contains(new Coord(2, 2), result);
            Assert.Contains(new Coord(3, 2), result);
            Assert.Contains(new Coord(2, 3), result);
            Assert.Contains(new Coord(3, 3), result);
        }

        [Test]
        public void OnBuildStructurePlayerAction_Always_AddStructureToHex()
        {
            var structureTemplate = Substitute.For<IStructureTemplate>();
            var structure = Substitute.For<IStructure>();

            structure.CurrentName.Returns("blah");

            structureTemplate.GenerateStructure(Arg.Any<Coord>()).Returns(structure);

            ConstructAndSetUpSubject();

            interfaceController.OnBuildStructurePlayerAction += Raise.EventWith(new EAOnBuildStructurePlayerAction(new Coord(3, 1), structureTemplate));

            hexAt31.Received(1).BuildStructureHere(structure);
        }

        [Test]
        public void OnBuildStrucutrePlayerAction_Always_FiresOnStructureCreatedEvent()
        {
            var structureTemplate = Substitute.For<IStructureTemplate>();
            var structure = Substitute.For<IStructure>();

            structureTemplate.GenerateStructure(Arg.Any<Coord>()).Returns(structure);

            var subject = ConstructAndSetUpSubject();

            var eventTester = new EventTester<EAOnStructureCreated>();
            subject.OnStructureCreated += eventTester.Handler;

            interfaceController.OnBuildStructurePlayerAction += Raise.EventWith(new EAOnBuildStructurePlayerAction(new Coord(3, 1), structureTemplate));

            eventTester.AssertFired(1);
            eventTester.AssertResult(subject, args => args.Position == new Coord(3, 1) && args.StructureCreated == structure);
        }

        [Test]
        public void OnCreateUnitPlayerAction_Always_AddsUnitToHex()
        {
            var unitTemplate = Substitute.For<IUnitTemplate>();
            var unit = Substitute.For<IUnit>();

            unitTemplate.GenerateUnit(Arg.Any<Coord>()).Returns(unit);

            ConstructAndSetUpSubject();

            interfaceController.OnCreateUnitPlayerAction += Raise.EventWith(new EAOnCreateUnitPlayerAction(new Coord(3, 1), unitTemplate));

            hexAt31.Received(1).PlaceUnitHere(unit);
        }

        [Test]
        public void OnCreateUnitPlayerAction_Always_FirestOnUnitCreatedEvent()
        {
            var unitTemplate = Substitute.For<IUnitTemplate>();
            var unit = Substitute.For<IUnit>();

            unitTemplate.GenerateUnit(Arg.Any<Coord>()).Returns(unit);

            var subject = ConstructAndSetUpSubject();

            var eventTester = new EventTester<EAOnUnitCreated>();
            subject.OnUnitCreated += eventTester.Handler;

            interfaceController.OnCreateUnitPlayerAction += Raise.EventWith(new EAOnCreateUnitPlayerAction(new Coord(3, 1), unitTemplate));

            eventTester.AssertFired(1);
            eventTester.AssertResult(subject, args => args.UnitCreated == unit && args.Position == new Coord(3, 1));
        }

        [Test]
        public void OnBuildStructurePlayerAction_IfBuiltOnPath_FetchesNewPathFromService()
        {
            var structureTemplate = Substitute.For<IStructureTemplate>();
            var structure = Substitute.For<IStructure>();

            structureTemplate.GenerateStructure(Arg.Any<Coord>()).Returns(structure);

            var subject = ConstructAndSetUpSubject();

            hexAt21.IsPathableByCreeps().Returns(false);
            hexAt21.IsPathableByCreepsWithUnitRemoved().Returns(false);
            hexAt21.IsPathableByCreepsWithTypePathable().Returns(false);

            interfaceController.OnBuildStructurePlayerAction += Raise.EventWith(new EAOnBuildStructurePlayerAction(new Coord(2, 1), structureTemplate));

            Assert.True(subject.CreepPath.Contains(new Coord(3, 0)));
        }

        [Test]
        public void OnCreateUnitPlayerAction_IfCreatedOnPath_FetchesPathFromService()
        {
            var unitTemplate = Substitute.For<IUnitTemplate>();
            var unit = Substitute.For<IUnit>();

            unitTemplate.GenerateUnit(Arg.Any<Coord>()).Returns(unit);

            var subject = ConstructAndSetUpSubject();

            hexAt21.IsPathableByCreeps().Returns(false);
            hexAt21.IsPathableByCreepsWithStructureRemoved().Returns(false);
            hexAt21.IsPathableByCreepsWithTypePathable().Returns(false);

            interfaceController.OnCreateUnitPlayerAction += Raise.EventWith(new EAOnCreateUnitPlayerAction(new Coord(2, 1), unitTemplate));

            Assert.True(subject.CreepPath.Contains(new Coord(3, 0)));
        }

        [Test]
        public void OnBuildStructurePlayerAction_Always_FetchesDisallowedCoordsFromService()
        {
            var structureTemplate = Substitute.For<IStructureTemplate>();
            var structure = Substitute.For<IStructure>();

            structureTemplate.GenerateStructure(Arg.Any<Coord>()).Returns(structure);

            var subject = ConstructAndSetUpSubject();

            Assert.True(subject.CanCreateUnitAt(new Coord(2, 1)));
            Assert.True(subject.CanBuildStructureAt(new Coord(2, 1)));

            hexAt31.IsPathableByCreeps().Returns(false);
            hexAt31.IsPathableByCreepsWithUnitRemoved().Returns(false);
            hexAt31.IsPathableByCreepsWithTypePathable().Returns(false);

            interfaceController.OnBuildStructurePlayerAction += Raise.EventWith(new EAOnBuildStructurePlayerAction(new Coord(3, 1), structureTemplate));

            Assert.False(subject.CanCreateUnitAt(new Coord(2, 1)));
            Assert.False(subject.CanBuildStructureAt(new Coord(2, 1)));
        }

        [Test]
        public void OnCreateUnitPlayerAction_Always_FetchesDisallowedCoordsFromService()
        {
            var unitTemplate = Substitute.For<IUnitTemplate>();
            var unit = Substitute.For<IUnit>();

            unitTemplate.GenerateUnit(Arg.Any<Coord>()).Returns(unit);

            var subject = ConstructAndSetUpSubject();

            Assert.True(subject.CanCreateUnitAt(new Coord(2, 1)));
            Assert.True(subject.CanBuildStructureAt(new Coord(2, 1)));

            hexAt31.IsPathableByCreeps().Returns(false);
            hexAt31.IsPathableByCreepsWithStructureRemoved().Returns(false);
            hexAt31.IsPathableByCreepsWithTypePathable().Returns(false);

            interfaceController.OnCreateUnitPlayerAction += Raise.EventWith(new EAOnCreateUnitPlayerAction(new Coord(3, 1), unitTemplate));

            Assert.False(subject.CanCreateUnitAt(new Coord(2, 1)));
            Assert.False(subject.CanBuildStructureAt(new Coord(2, 1)));
        }
    }
}