using UnityEngine.Assertions;
using UnityEngine;

public static class CustomMath
{
    public static float SignedOddRoot(float value, int nthRoot)
    {
        Assert.IsTrue(nthRoot % 2 == 1);

        int sign = value < 0 ? -1 : 1;

        float exponent = 1f / (float)nthRoot;

        float rawRoot = Mathf.Pow(Mathf.Abs(value), exponent);

        return rawRoot * sign;
    }
}
