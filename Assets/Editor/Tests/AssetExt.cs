using NUnit.Framework;
using GrimoireTD.Technical;

namespace GrimoireTD.Tests
{
    public static class AssertExt
    {
        public static void Approximately(float expected, float actual)
        {
            Assert.True(CustomMath.Approximately(expected, actual), "Exp: " + expected + "\nGot: " + actual);
        }
    }
}