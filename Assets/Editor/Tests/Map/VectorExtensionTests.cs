using NUnit.Framework;
using NSubstitute;
using UnityEngine;
using GrimoireTD.Map;

namespace GrimoireTD.Tests.VectorExtensionTests
{
    public class VectorExtensionTests
    {
        [TestCase(0f, 0f, 0, 0)]
        [TestCase(0f, 0.49f, 0, 0)]
        [TestCase(0.4f, 0.49f, 0, 1)]
        [TestCase(0.4f, -0.25f, 0, 0)]
        [TestCase(0 - .3f, -0.3f, 0, 0)]
        public void Vector2ToCoord_Always_ReturnsCorrectCoord(
            float vectorX,
            float vectorY,
            int expectedCoordX,
            int expectedCoordY)
        {
            var vector = new Vector2(vectorX, vectorY);

            var expectedCoord = new Coord(expectedCoordX, expectedCoordY);

            var result = vector.ToCoord();

            Assert.AreEqual(expectedCoord, result);
        }

        [TestCase(0f, 0f, 0, 0)]
        [TestCase(0f, 0.49f, 0, 0)]
        [TestCase(0.4f, 0.49f, 0, 1)]
        [TestCase(0.4f, -0.25f, 0, 0)]
        [TestCase(0 - .3f, -0.3f, 0, 0)]
        public void Vector3ToCoord_Always_ReturnsCorrectCoord(
            float vectorX,
            float vectorY,
            int expectedCoordX,
            int expectedCoordY)
        {
            var vector = new Vector3(vectorX, vectorY, 0f);

            var expectedCoord = new Coord(expectedCoordX, expectedCoordY);

            var result = vector.ToCoord();

            Assert.AreEqual(expectedCoord, result);
        }
    }
}