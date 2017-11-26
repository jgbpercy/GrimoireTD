using UnityEngine;

namespace GrimoireTD.Map
{
    public static class VectorExtensions
    {
        public static Coord ToCoord(this Vector3 vector)
        {
            return ToCoord(new Vector2(vector.x, vector.y));
        }

        public static Coord ToCoord(this Vector2 vector)
        {
            var mainYSection = Mathf.FloorToInt((vector.y + 0.5f) / 1.5f);
            var ySubSection = (vector.y + 0.5f) % 1.5f;

            int mainXSection;
            float normalisedY;

            var x = 0;
            var y = 0;

            if (0.25f <= ySubSection && ySubSection < 0.75f)
            {
                y = mainYSection * 2;
                x = Mathf.FloorToInt((vector.x + MapRenderer.HEX_OFFSET) / (MapRenderer.HEX_OFFSET * 2));
            }
            else if (1f <= ySubSection && ySubSection < 1.5f)
            {
                y = mainYSection * 2 + 1;
                x = Mathf.FloorToInt(vector.x / (MapRenderer.HEX_OFFSET * 2));
            }
            else if (0f <= ySubSection && ySubSection < 0.25f)
            {
                mainXSection = Mathf.FloorToInt(vector.x / MapRenderer.HEX_OFFSET) + 1;
                normalisedY = ((vector.y + 1f) % 0.25f) * (MapRenderer.HEX_OFFSET / 0.25f);

                if (mainXSection % 2 == 0)
                {
                    if (normalisedY + ((vector.x + MapRenderer.HEX_OFFSET) % MapRenderer.HEX_OFFSET) > MapRenderer.HEX_OFFSET)
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
                    if (normalisedY > ((vector.x + MapRenderer.HEX_OFFSET) % MapRenderer.HEX_OFFSET))
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
            else if (0.75f <= ySubSection && ySubSection < 1f)
            {
                mainXSection = Mathf.FloorToInt(vector.x / MapRenderer.HEX_OFFSET) + 1;
                normalisedY = ((vector.y + 1f) % 0.25f) * (MapRenderer.HEX_OFFSET / 0.25f);

                if (mainXSection % 2 == 0)
                {
                    if (normalisedY > ((vector.x + MapRenderer.HEX_OFFSET) % MapRenderer.HEX_OFFSET))
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
                    if (normalisedY + ((vector.x + MapRenderer.HEX_OFFSET) % MapRenderer.HEX_OFFSET) > MapRenderer.HEX_OFFSET)
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
}