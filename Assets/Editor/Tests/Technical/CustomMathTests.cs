using NUnit.Framework;
using UnityEngine;
using GrimoireTD.Technical;

namespace GrimoireTD.Tests.CustomMathTests
{
    public class CustomMathTests
    {
        [Test]
        public void SignedOddRoot_PassedNegativeValueAndEvenRoot_ReturnsNaN()
        {
            var result = CustomMath.SignedRoot(-4, 2);

            Assert.IsNaN(result);
        }

        [Test]
        public void SignedOddRoot_PassedPositiveValueAndEvenRoot_ReturnsCorrectRoot()
        {
            var result = CustomMath.SignedRoot(4, 2);

            Assert.AreEqual(2, result);
        }

        [Test]
        public void SignedOddRoot_PassedNegativeValueAndOddRoot_ReturnsCorrectNegativeRoot()
        {
            var result = CustomMath.SignedRoot(-27, 3);

            Assert.AreEqual(-3, result);
        }

        [Test]
        public void SignedOddRoot_PassedPositiveValueAndOddRoot_ReturnsCorrectRoot()
        {
            var result = CustomMath.SignedRoot(27, 3);

            Assert.AreEqual(3, result);
        }

        [Test]
        public void Approximately_ForTwoVeryCloseFloats_ReturnsTrue()
        {
            var number = 0.3f;
            var closeNumber = number + 0.00000001f;

            var result = CustomMath.Approximately(number, closeNumber);

            Assert.True(result);
        }

        [Test]
        public void Approximately_ForTwoNotVeryCloseFloats_ReturnsFalse()
        {
            var number = 6f;
            var notVeryCloseNumber = number + 0.001f;

            var result = CustomMath.Approximately(number, notVeryCloseNumber);

            Assert.False(result);
        }

        [Test]
        public void Approximately_ForTwoVeryCloseVector2s_ReturnsTrue()
        {
            var vectorX = 3f;
            var vectorY = 4f;

            var vector = new Vector2(vectorX, vectorY);
            var closeVector = new Vector2(vectorX + 0.00000001f, vectorY + 0.00000001f);

            var result = CustomMath.Approximately(vector, closeVector);

            Assert.True(result);
        }

        [Test]
        public void Apporoximately_ForTwoNotVeryCloseVector2s_ReturnsFalse()
        {
            var vectorX = 3f;
            var vectorY = 4f;

            var vector = new Vector2(vectorX, vectorY);
            var notVeryCloseVector = new Vector2(vectorX + 0.001f, vectorY);

            var result = CustomMath.Approximately(vector, notVeryCloseVector);

            Assert.False(result);
        }

        [Test]
        public void Approximately_ForTwoVeryCloseVector3s_ReturnsTrue()
        {
            var vectorX = 3f;
            var vectorY = 4f;
            var vectorZ = 5f;

            var vector = new Vector3(vectorX, vectorY, vectorZ);
            var closeVector = new Vector3(vectorX + 0.00000001f, vectorY + 0.00000001f, vectorZ + 0.000000001f);

            var result = CustomMath.Approximately(vector, closeVector);

            Assert.True(result);
        }

        [Test]
        public void Apporoximately_ForTwoNotVeryCloseVector3s_ReturnsFalse()
        {
            var vectorX = 3f;
            var vectorY = 4f;
            var vectorZ = 5f;

            var vector = new Vector3(vectorX, vectorY, vectorZ);
            var notVeryCloseVector = new Vector3(vectorX + 0.001f, vectorY, vectorZ);

            var result = CustomMath.Approximately(vector, notVeryCloseVector);

            Assert.False(result);
        }

        [Test]
        public void Approximately_ForTwoVeryCloseVector4s_ReturnsTrue()
        {
            var vectorX = 3f;
            var vectorY = 4f;
            var vectorZ = 5f;
            var vectorW = 6f;

            var vector = new Vector4(vectorX, vectorY, vectorZ, vectorW);
            var closeVector = new Vector4(vectorX + 0.00000001f, vectorY + 0.00000001f, vectorZ + 0.000000001f, vectorW + 0.0000000001f);

            var result = CustomMath.Approximately(vector, closeVector);

            Assert.True(result);
        }

        [Test]
        public void Apporoximately_ForTwoNotVeryCloseVector4s_ReturnsFalse()
        {
            var vectorX = 3f;
            var vectorY = 4f;
            var vectorZ = 5f;
            var vectorW = 6f;

            var vector = new Vector4(vectorX, vectorY, vectorZ, vectorW);
            var notVeryCloseVector = new Vector4(vectorX + 0.001f, vectorY, vectorZ, vectorW);

            var result = CustomMath.Approximately(vector, notVeryCloseVector);

            Assert.False(result);
        }
    }
}