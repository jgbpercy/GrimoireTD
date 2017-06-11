using System;
using UnityEngine;

//all currently assumes tileScale = 1 whops
[Serializable]
public class Coord : object
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

        Coord z = obj as Coord;
        if ((object)z == null)
        {
            return false;
        }

        return (x == z.x) && (y == z.y);
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
            return new Vector3(x * 2f * MapRenderer.HEX_OFFSET * MapRenderer.tileScale, y * 0.75f * MapRenderer.tileScale, 0f);
        }
        else
        {
            return new Vector3(x * 2f * MapRenderer.HEX_OFFSET * MapRenderer.tileScale + MapRenderer.HEX_OFFSET, y * 0.75f * MapRenderer.tileScale, 0f);
        }
    }

    public Vector3 ToFirePointVector()
    {
        return new Vector3(ToPositionVector().x, ToPositionVector().y, -0.5f);
    }

    public override string ToString()
    {
        return "(" + x + ", " + y + ")";
    }

    public static Coord PositionVectorToCoord(Vector3 positionVector)
    {
        int mainYSection = Mathf.FloorToInt((positionVector.y + 0.5f) / 1.5f);
        float ySubSection = (positionVector.y + 0.5f) % 1.5f;

        //bool debugOn = Input.GetButtonDown("Fire1");

        int mainXSection;
        float normalisedY;

        int x = 0;
        int y = 0;

        if ( 0.25f <= ySubSection && ySubSection < 0.75f )
        {
            //if (debugOn) Debug.Log("Lower Major Section Hit");
            y = mainYSection * 2;
            x = Mathf.FloorToInt((positionVector.x + MapRenderer.HEX_OFFSET) / (MapRenderer.HEX_OFFSET * 2));
        }
        else if ( 1f <= ySubSection && ySubSection < 1.5f )
        {
            //if (debugOn) Debug.Log("Upper Major Section Hit");
            y = mainYSection * 2 + 1;
            x = Mathf.FloorToInt(positionVector.x / (MapRenderer.HEX_OFFSET * 2));
        }
        else if ( 0f <= ySubSection && ySubSection < 0.25f )
        {
            //if (debugOn) Debug.Log("Lower Minor Section Hit");
            mainXSection = Mathf.FloorToInt(positionVector.x / MapRenderer.HEX_OFFSET) + 1;
            normalisedY = ((positionVector.y + 1f) % 0.25f) * (MapRenderer.HEX_OFFSET / 0.25f);
            //if (debugOn) Debug.Log("mainXSection: " + mainXSection);
            //if (debugOn) Debug.Log("normalisedY: " + normalisedY);

            if (mainXSection % 2 == 0)
            {
                if (normalisedY + ((positionVector.x + MapRenderer.HEX_OFFSET) % MapRenderer.HEX_OFFSET) > MapRenderer.HEX_OFFSET )
                {
                    y = mainYSection * 2;
                    x = mainXSection / 2;
                }
                else
                {
                    y = mainYSection * 2 - 1;
                    x = mainXSection / 2 - 1;
                }
            }
            else
            {
                if (normalisedY > ((positionVector.x + MapRenderer.HEX_OFFSET) % MapRenderer.HEX_OFFSET) )
                {
                    y = mainYSection * 2;
                    x = mainXSection / 2;
                }
                else
                {
                    y = mainYSection * 2 - 1;
                    x = mainXSection / 2;
                }
            }
        }
        else if ( 0.75f <= ySubSection && ySubSection < 1f)
        {
            //if (debugOn) Debug.Log("Upper Minor Section Hit");
            mainXSection = Mathf.FloorToInt(positionVector.x / MapRenderer.HEX_OFFSET) + 1;
            normalisedY = ((positionVector.y + 1f) % 0.25f) * (MapRenderer.HEX_OFFSET / 0.25f);
            //if (debugOn) Debug.Log("mainXSection: " + mainXSection);
            //if (debugOn) Debug.Log("normalisedY: " + normalisedY);

            if (mainXSection % 2 == 0)
            {
                if (normalisedY > ((positionVector.x + MapRenderer.HEX_OFFSET) % MapRenderer.HEX_OFFSET))
                {
                    y = mainYSection * 2 + 1;
                    x = mainXSection / 2 - 1;
                }
                else
                {
                    y = mainYSection * 2;
                    x = mainXSection / 2;
                }
            }
            else
            {
                if (normalisedY + ((positionVector.x + MapRenderer.HEX_OFFSET) % MapRenderer.HEX_OFFSET) > MapRenderer.HEX_OFFSET)
                {
                    y = mainYSection * 2 + 1;
                    x = mainXSection / 2;
                }
                else
                {
                    y = mainYSection * 2;
                    x = mainXSection / 2;
                }
            }
        }

        return new Coord(x, y);
    }
}

