using NUnit.Framework;
using GrimoireTD.Map;
using UnityEngine;

namespace GrimoireTD.Tests.CoordTest
{
    public class CoordTests
    {
        [Test]
        public void EqualsOperator_ForTwoCoordWithTheSameXAndY_ReturnsTrue()
        {
            var first = new Coord(3, 4);
            var second = new Coord(3, 4);

            var result = first == second;

            Assert.True(result);
        }

        [Test]
        public void EqualsOperator_ForTwoCoordsWithDifferentX_ReturnsFalse()
        {
            var first = new Coord(3, 4);
            var second = new Coord(4, 4);

            var result = first == second;

            Assert.False(result);
        }

        [Test]
        public void EqualsOperator_ForTwoCoordsWithDifferentY_ReturnsFalse()
        {
            var first = new Coord(3, 4);
            var second = new Coord(3, 5);

            var result = first == second;

            Assert.False(result);
        }

        [Test]
        public void EqualsOperator_BothNull_ReturnsTrue()
        {
            var first = null as Coord;
            var second = null as Coord;

            var result = first == second;

            Assert.True(result);
        }

        [Test]
        public void EqualsOperator_FirstNull_ReturnsFalse()
        {
            var first = null as Coord;
            var second = new Coord(5, 5);

            var result = first == second;

            Assert.False(result);
        }

        [Test]
        public void EqualsOperator_SecondNull_ReturnsFalse()
        {
            var first = new Coord(5, 6);
            var second = null as Coord;

            var result = first == second;

            Assert.False(result);
        }

        [Test]
        public void Equals_ForTwoCoordWithTheSameXAndY_ReturnsTrue()
        {
            var first = new Coord(3, 4);
            var second = new Coord(3, 4);

            var result = first.Equals(second);

            Assert.True(result);
        }

        [Test]
        public void Equals_ForTwoCoordsWithDifferentX_ReturnsFalse()
        {
            var first = new Coord(3, 4);
            var second = new Coord(8, 4);

            var result = first.Equals(second);

            Assert.False(result);
        }

        [Test]
        public void Equals_ForTwoCoordsWithDifferentY_ReturnsFalse()
        {
            var first = new Coord(3, 4);
            var second = new Coord(3, 2);

            var result = first.Equals(second);

            Assert.False(result);
        }

        [Test]
        public void Equals_WithNull_ReturnsFalse()
        {
            var first = new Coord(5, 6);
            var second = null as Coord;

            var result = first.Equals(second);

            Assert.False(result);
        }

        [Test]
        public void NotEqualsOperator_ForTwoCoordWithTheSameXAndY_ReturnsFalse()
        {
            var first = new Coord(3, 4);
            var second = new Coord(3, 4);

            var result = first != second;

            Assert.False(result);
        }

        [Test]
        public void NotEqualsOperator_ForTwoCoordsWithDifferentX_ReturnsTrue()
        {
            var first = new Coord(3, 4);
            var second = new Coord(6, 4);

            var result = first != second;

            Assert.True(result);
        }

        [Test]
        public void NotEqualsOperator_ForTwoCoordsWithDifferentY_ReturnsTrue()
        {
            var first = new Coord(3, 4);
            var second = new Coord(3, 1);

            var result = first != second;

            Assert.True(result);
        }

        [Test]
        public void ToPositionVector_ForEvenY_ReturnsCorrectVector()
        {
            var subject = new Coord(1, 2);

            var expected = new Vector3(2f * MapRenderer.HEX_OFFSET, 1.5f, 0f);

            AssertExt.Approximately(expected, subject.ToPositionVector());
        }

        [Test]
        public void ToPositionVector_ForOddY_ReturnsCorrectVector()
        {
            var subject = new Coord(1, 1);

            var expected = new Vector3(3f * MapRenderer.HEX_OFFSET, 0.75f, 0f);

            AssertExt.Approximately(expected, subject.ToPositionVector());
        }

        [Test]
        public void NotEqualsOperator_BothNull_ReturnsFalse()
        {
            var first = null as Coord;
            var second = null as Coord;

            var result = first != second;

            Assert.False(result);
        }

        [Test]
        public void NotEqualsOperator_FirstNull_ReturnsTrue()
        {
            var first = null as Coord;
            var second = new Coord(5, 5);

            var result = first != second;

            Assert.True(result);
        }

        [Test]
        public void NotEqualsOperator_SecondNull_ReturnsTrue()
        {
            var first = new Coord(5, 6);
            var second = null as Coord;

            var result = first != second;

            Assert.True(result);
        }

        [Test]
        public void ToString_Always_ReturnsExpetedString()
        {
            var subject = new Coord(1, 2);

            Assert.AreEqual("(1, 2)", subject.ToString());
        }
    }
}