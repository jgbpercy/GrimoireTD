using NUnit.Framework;
using GrimoireTD.Technical;
using UnityEngine;

namespace GrimoireTD.Tests
{
    public static class AssertExt
    {
        public static void Approximately(float expected, float actual)
        {
            Assert.True(CustomMath.Approximately(expected, actual), "Exp: " + expected + "\nGot: " + actual);
        }

        public static void Approximately(Vector2 expected, Vector2 actual)
        {
            Assert.True(CustomMath.Approximately(expected, actual), "Exp: " + expected.ToString("G4") + "\nGot: " + actual.ToString("G4"));
        }

        public static void Approximately(Vector3 expected, Vector3 actual)
        {
            Assert.True(CustomMath.Approximately(expected, actual), "Exp: " + expected.ToString("G4") + "\nGot: " + actual.ToString("G4"));
        }

        public static void Approximately(Vector4 expected, Vector4 actual)
        {
            Assert.True(CustomMath.Approximately(expected, actual), "Exp: " + expected.ToString("G4") + "\nGot: " + actual.ToString("G4"));
        }
    }
}