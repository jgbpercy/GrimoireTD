using UnityEngine.Assertions;
using UnityEngine;

namespace GrimoireTD.Technical
{
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

        public static bool Approximately(Vector2 firstVector2, Vector2 secondVector2)
        {
            if (!Mathf.Approximately(firstVector2.x, secondVector2.x)) return false;
            if (!Mathf.Approximately(firstVector2.y, secondVector2.y)) return false;

            return true;
        }

        public static bool Approximately(Vector3 firstVector3, Vector3 secondVector3)
        {
            if (!Mathf.Approximately(firstVector3.x, secondVector3.x)) return false;
            if (!Mathf.Approximately(firstVector3.y, secondVector3.y)) return false;
            if (!Mathf.Approximately(firstVector3.z, secondVector3.z)) return false;

            return true;
        }

        public static bool Approximately(Vector4 firstVector4, Vector4 secondVector4)
        {
            if (!Mathf.Approximately(firstVector4.x, secondVector4.x)) return false;
            if (!Mathf.Approximately(firstVector4.y, secondVector4.y)) return false;
            if (!Mathf.Approximately(firstVector4.z, secondVector4.z)) return false;
            if (!Mathf.Approximately(firstVector4.w, secondVector4.w)) return false;

            return true;
        }
    }
}