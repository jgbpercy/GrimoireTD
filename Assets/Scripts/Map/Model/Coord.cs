using System;
using UnityEngine;
using GrimoireTD.Abilities.BuildMode;

namespace GrimoireTD.Map
{
    [Serializable]
    public class Coord : object, IBuildModeTargetable
    {
        [SerializeField]
        private int x;
        [SerializeField]
        private int y;

        public virtual int X
        {
            get
            {
                return x;
            }
        }

        public virtual int Y
        {
            get
            {
                return y;
            }
        }

        public Coord CoordPosition
        {
            get
            {
                return this;
            }
        }

        public Coord(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var z = obj as Coord;
            if ((object)z == null)
            {
                return false;
            }

            return (x == z.x) && (y == z.y);
        }

        public static bool operator == (Coord coord1, Coord coord2)
        {
            return (coord1.x == coord2.x) && (coord1.y == coord2.y);
        }

        public static bool operator != (Coord coord1, Coord coord2)
        {
            return !((coord1.x == coord2.x) && (coord1.y == coord2.y));
        }

        public bool Equals(Coord z)
        {
            if ((object)z == null)
            {
                return false;
            }

            return (x == z.x) && (y == z.y);
        }

        public override int GetHashCode()
        {
            return x ^ y;
        }

        public Vector3 ToPositionVector()
        {
            if (y % 2 == 0)
            {
                return new Vector3(x * 2f * MapRenderer.HEX_OFFSET, y * 0.75f, 0f);
            }
            else
            {
                return new Vector3(x * 2f * MapRenderer.HEX_OFFSET + MapRenderer.HEX_OFFSET, y * 0.75f, 0f);
            }
        }

        public Vector3 ToFirePointVector()
        {
            var positionVector = ToPositionVector();

            return new Vector3(positionVector.x, positionVector.y, -0.5f);
        }

        public override string ToString()
        {
            return "(" + x + ", " + y + ")";
        }
    }
}