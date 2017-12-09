using NUnit.Framework;
using NSubstitute;
using System.Collections.Generic;
using GrimoireTD.Map;

namespace GrimoireTD.Tests.PathingServiceTests
{
    public class PathingServiceTests
    {
        private Dictionary<Coord, IHexData> SetUpHexes(List<Coord> unpathableCoords)
        {
            var hexAt00 = Substitute.For<IHexData>();
            var hexAt10 = Substitute.For<IHexData>();
            var hexAt20 = Substitute.For<IHexData>();
            var hexAt30 = Substitute.For<IHexData>();

            var hexAt01 = Substitute.For<IHexData>();
            var hexAt11 = Substitute.For<IHexData>();
            var hexAt21 = Substitute.For<IHexData>();
            var hexAt31 = Substitute.For<IHexData>();

            var hexAt02 = Substitute.For<IHexData>();
            var hexAt12 = Substitute.For<IHexData>();
            var hexAt22 = Substitute.For<IHexData>();
            var hexAt32 = Substitute.For<IHexData>();

            var hexAt03 = Substitute.For<IHexData>();
            var hexAt13 = Substitute.For<IHexData>();
            var hexAt23 = Substitute.For<IHexData>();
            var hexAt33 = Substitute.For<IHexData>();

            var hexes = new Dictionary<Coord, IHexData>
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

            foreach (var hex in hexes)
            {
                hex.Value.IsPathableByCreeps().Returns(true);
                hex.Value.IsPathableByCreepsWithStructureRemoved().Returns(true);
                hex.Value.IsPathableByCreepsWithTypePathable().Returns(true);
                hex.Value.IsPathableByCreepsWithUnitRemoved().Returns(true);

                if (unpathableCoords.Contains(hex.Key))
                {
                    hex.Value.IsPathableByCreeps().Returns(false);
                    hex.Value.IsPathableByCreepsWithStructureRemoved().Returns(false);
                    hex.Value.IsPathableByCreepsWithUnitRemoved().Returns(false);
                }
            }

            return hexes;
        }

        /* Map:
         * 3   X X X 0
         * 2  X X X 0
         * 1   X X X 0
         * 0  0 0 0 0
         *    0 1 2 3
         */
        [Test]
        public void GetPath_OnlyOnePossiblePath_ReturnsPath()
        {
            var hexes = SetUpHexes(new List<Coord>
            {
                new Coord(0, 1),
                new Coord(0, 2),
                new Coord(0, 3),

                new Coord(1, 1),
                new Coord(1, 2),
                new Coord(1, 3),

                new Coord(2, 1),
                new Coord(2, 2),
                new Coord(2, 3),
            });

            var result = PathingService.GetPath(hexes, new Coord(0,0), new Coord(3, 3));

            Assert.AreEqual(new Coord(0, 0), result[6]);
            Assert.AreEqual(new Coord(1, 0), result[5]);
            Assert.AreEqual(new Coord(2, 0), result[4]);
            Assert.AreEqual(new Coord(3, 0), result[3]);
            Assert.AreEqual(new Coord(3, 1), result[2]);
            Assert.AreEqual(new Coord(3, 2), result[1]);
            Assert.AreEqual(new Coord(3, 3), result[0]);
        }

        /* Map:
         * 3   X X X 0
         * 2  X X 0 0
         * 1   0 0 X 0
         * 0  0 X 0 0
         *    0 1 2 3
         */
        [Test]
        public void GetPath_MultiplePossiblePaths_ReturnsShortestPath()
        {
            var hexes = SetUpHexes(new List<Coord>
            {
                new Coord(0, 2),
                new Coord(0, 3),

                new Coord(1, 0),
                new Coord(1, 2),
                new Coord(1, 3),

                new Coord(2, 1),
                new Coord(2, 3),
            });

            var result = PathingService.GetPath(hexes, new Coord(0, 0), new Coord(3, 3));

            Assert.AreEqual(new Coord(0, 0), result[5]);
            Assert.AreEqual(new Coord(0, 1), result[4]);
            Assert.AreEqual(new Coord(1, 1), result[3]);
            Assert.AreEqual(new Coord(2, 2), result[2]);
            Assert.AreEqual(new Coord(3, 2), result[1]);
            Assert.AreEqual(new Coord(3, 3), result[0]);
        }

        /* Map:
         * 3   X 0 0 0
         * 2  0 X 0 0
         * 1   0 X 0 0
         * 0  0 0 X 0
         *    0 1 2 3
         */
        [Test]
        public void GetPath_NoPossiblePath_ReturnsNull()
        {
            var hexes = SetUpHexes(new List<Coord>
            {
                new Coord(0, 3),

                new Coord(1, 1),
                new Coord(1, 2),

                new Coord(2, 0),
            });

            var result = PathingService.GetPath(hexes, new Coord(0, 0), new Coord(3, 3));

            Assert.Null(result);
        }

        /* Map:
         * 3   U X X 0
         * 2  X X U 0
         * 1   0 0 X 0
         * 0  0 X 0 0
         *    0 1 2 3
         */
        [Test]
        public void GetPathWithUnpathableList_ShortestPathWouldHaveBeenThroughUnpathableCoord_ReturnsPathWithoutThatCoord()
        {
            var hexes = SetUpHexes(new List<Coord>
            {
                new Coord(0, 2),

                new Coord(1, 0),
                new Coord(1, 2),
                new Coord(1, 3),

                new Coord(2, 1),
                new Coord(2, 3),
            });

            var result = PathingService.GetPathWithUnpathableList(hexes, new Coord(0, 0), new Coord(3, 3), new List<Coord>
            {
                new Coord(0, 3),
                new Coord(2, 2),
            });

            Assert.AreEqual(new Coord(0, 0), result[7]);
            Assert.AreEqual(new Coord(0, 1), result[6]);
            Assert.AreEqual(new Coord(1, 1), result[5]);
            Assert.AreEqual(new Coord(2, 0), result[4]);
            Assert.AreEqual(new Coord(3, 0), result[3]);
            Assert.AreEqual(new Coord(3, 1), result[2]);
            Assert.AreEqual(new Coord(3, 2), result[1]);
            Assert.AreEqual(new Coord(3, 3), result[0]);
        }

        /* Map:
         * 3   X X X 0
         * 2  X X X 0
         * 1   X X X U
         * 0  0 0 0 0
         *    0 1 2 3
         */
        [Test]
        public void GetPathWithUnpathableList_OnlyPathWouldHaveBeenThroughUnpathableCoord_ReturnsNull()
        {
            var hexes = SetUpHexes(new List<Coord>
            {
                new Coord(0, 1),
                new Coord(0, 2),
                new Coord(0, 3),

                new Coord(1, 1),
                new Coord(1, 2),
                new Coord(1, 3),

                new Coord(2, 1),
                new Coord(2, 2),
                new Coord(2, 3),
            });

            var result = PathingService.GetPathWithUnpathableList(hexes, new Coord(0, 0), new Coord(3, 3), new List<Coord> { new Coord(3, 1) });

            Assert.Null(result);
        }

        /* Map:
         * 3   N X X 0
         * 2  X X U 0
         * 1   0 0 X 0
         * 0  0 X 0 0
         *    0 1 2 3
         */
        [Test]
        public void GetPathWithUnpathableAndNewlyPathableLists_ShortestPathWouldHaveBeenThroughUnpathableCoord_ReturnsPathWithoutThatCoord()
        {
            var hexes = SetUpHexes(new List<Coord>
            {
                new Coord(0, 2),
                new Coord(0, 3),

                new Coord(1, 0),
                new Coord(1, 2),
                new Coord(1, 3),

                new Coord(2, 1),
                new Coord(2, 3),
            });

            var result = PathingService.GetPathWithUnpathableAndNewlyPathableLists(
                hexes, 
                new Coord(0, 0), 
                new Coord(3, 3), 
                new List<Coord>
                {
                    new Coord(2, 2),
                },
                new List<Coord>
                {
                    new Coord(0, 3),
                });

            Assert.AreEqual(new Coord(0, 0), result[7]);
            Assert.AreEqual(new Coord(0, 1), result[6]);
            Assert.AreEqual(new Coord(1, 1), result[5]);
            Assert.AreEqual(new Coord(2, 0), result[4]);
            Assert.AreEqual(new Coord(3, 0), result[3]);
            Assert.AreEqual(new Coord(3, 1), result[2]);
            Assert.AreEqual(new Coord(3, 2), result[1]);
            Assert.AreEqual(new Coord(3, 3), result[0]);
        }

        /* Map:
         * 3   U X X 0
         * 2  X X N 0
         * 1   0 0 X 0
         * 0  0 X 0 0
         *    0 1 2 3
         */
        [Test]
        public void GetPathWithUnpathableAndNewlyPathableLists_ShortestPathWillBeThroughNewlyPathableCoord_ReturnsPathWithThatCoord()
        {
            var hexes = SetUpHexes(new List<Coord>
            {
                new Coord(0, 2),

                new Coord(1, 0),
                new Coord(1, 2),
                new Coord(1, 3),

                new Coord(2, 1),
                new Coord(2, 2),
                new Coord(2, 3),
            });

            var result = PathingService.GetPathWithUnpathableAndNewlyPathableLists(
                hexes, 
                new Coord(0, 0), 
                new Coord(3, 3),
                new List<Coord>
                {
                    new Coord(0, 3),
                },
                new List<Coord>
                {
                    new Coord(2, 2),
                });

            Assert.AreEqual(new Coord(0, 0), result[5]);
            Assert.AreEqual(new Coord(0, 1), result[4]);
            Assert.AreEqual(new Coord(1, 1), result[3]);
            Assert.AreEqual(new Coord(2, 2), result[2]);
            Assert.AreEqual(new Coord(3, 2), result[1]);
            Assert.AreEqual(new Coord(3, 3), result[0]);
        }

        /* Map:
         * 3   X X X 0
         * 2  X X 0 0
         * 1   0 0 0 0
         * 0  0 X X X
         *    0 1 2 3
         */
        [Test]
        public void GetDisallowedCoords_PathContainsCoordsThatWouldBlockAnyPathIfMadeUnpathable_ReturnsThoseCoords()
        {
            var hexes = SetUpHexes(new List<Coord>
            {
                new Coord(0, 2),
                new Coord(0, 3),

                new Coord(1, 0),
                new Coord(1, 2),
                new Coord(1, 3),

                new Coord(2, 0),
                new Coord(2, 3),

                new Coord(3, 0),
            });

            var path = PathingService.GetPath(hexes, new Coord(0, 0), new Coord(3, 3));

            var result = PathingService.GetDisallowedCoords(path, hexes, new Coord(0, 0), new Coord(3, 3));

            Assert.AreEqual(5, result.Count);

            Assert.Contains(new Coord(0, 0), result);
            Assert.Contains(new Coord(0, 1), result);
            Assert.Contains(new Coord(1, 1), result);
            Assert.Contains(new Coord(3, 2), result);
            Assert.Contains(new Coord(3, 3), result);
        }

        /* Map:
         * 3   X X X 0
         * 2  X X 0 0
         * 1   0 0 0 0
         * 0  0 N X X
         *    0 1 2 3
         */
        [Test]
        public void GetDisallowedCoordsWithNewlyPathableCoords_PathContainsACoordThatWouldOnlyBlockIfNewlyPathableCoordsWereStillUnpathable_DoesNotReturnThoseCoord()
        {
            var hexes = SetUpHexes(new List<Coord>
            {
                new Coord(0, 2),
                new Coord(0, 3),

                new Coord(1, 0),
                new Coord(1, 2),
                new Coord(1, 3),

                new Coord(2, 0),
                new Coord(2, 3),

                new Coord(3, 0),
            });

            var path = PathingService.GetPath(hexes, new Coord(0, 0), new Coord(3, 3));

            var result = PathingService.GetDisallowedCoordsWithNewlyPathableCoords(
                path, 
                hexes, 
                new Coord(0, 0), 
                new Coord(3, 3),
                new List<Coord>
                {
                    new Coord(1, 0),
                });

            Assert.AreEqual(4, result.Count);

            Assert.Contains(new Coord(0, 0), result);
            Assert.Contains(new Coord(1, 1), result);
            Assert.Contains(new Coord(3, 2), result);
            Assert.Contains(new Coord(3, 3), result);
        }
    }
}