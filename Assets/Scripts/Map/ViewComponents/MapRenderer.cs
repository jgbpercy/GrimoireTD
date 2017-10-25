using UnityEngine;
using GrimoireTD.Technical;
using GrimoireTD.Dependencies;

namespace GrimoireTD.Map
{
    public class MapRenderer : SingletonMonobehaviour<MapRenderer>
    {
        [SerializeField]
        private bool debugOn;

        [SerializeField]
        private float textureRes = 512f;

        public const float HEX_OFFSET = 0.43301270189f;

        [SerializeField]
        private int[] textureOffsetForOffmapHexes;

        private Texture2D tileImages;

        [SerializeField]
        private MeshFilter mapGraphicsFilter;
        [SerializeField]
        private MeshRenderer mapGraphicsRenderer;
        [SerializeField]
        private MeshCollider mapGraphicsCollider;

        [SerializeField]
        private MeshFilter offMapGraphicsFilter;

        private IReadOnlyMapData mapData;

        [SerializeField]
        private int offMapHexThickness;

        private void Start()
        {
            tileImages = (Texture2D)mapGraphicsRenderer.material.mainTexture;

            mapData = DepsProv.TheMapData;

            mapData.OnMapCreated += InitialiseMap;

            //TODO subscribe to map change events (TODO have map change events!)            
        }

        private void InitialiseMap(object sender, EAOnMapCreated args)
        {
            Mesh mapMesh = new Mesh();

            Vector3[] vertices = new Vector3[mapData.Width * mapData.Height * 7];
            Vector3[] currentVertices = new Vector3[7];

            int[] polys = new int[((mapData.Width * mapData.Height * 6) + 2 * (3 * mapData.Width * mapData.Height - 2 * mapData.Width - 2 * mapData.Height + 1)) * 3];
            int[] currentPolys = new int[18];
            int neighboursCenterIndex;
            int[] joinPolys = new int[6];

            Vector3[] normals = new Vector3[mapData.Width * mapData.Height * 7];

            Vector2[] uvs = new Vector2[mapData.Width * mapData.Height * 7];
            Vector2[] currentUvs = new Vector2[7];
            float textureOffsetX = 0f;
            float textureOffsetY = 0f;
            float textureScaleMultiplierX = textureRes / tileImages.width;
            float textureScaleMultiplierY = textureRes / tileImages.height;

            Coord currentCoord;
            Vector3 centerOfCurrent;

            int currentVertexIndex = 0;
            int currentPolyIndex = 0;

            for (int x = 0; x < mapData.Width; x++)
            {
                for (int y = 0; y < mapData.Height; y++)
                {
                    currentCoord = new Coord(x, y);
                    centerOfCurrent = currentCoord.ToPositionVector();

                    //vertices (& normals)
                    currentVertices = HexVertices(centerOfCurrent);

                    for (int i = 0; i < 7; i++)
                    {
                        vertices[currentVertexIndex + i] = currentVertices[i];
                        normals[currentVertexIndex + i] = Vector3.forward;
                    }

                    //main polys
                    currentPolys = HexPolys(currentCoord);

                    for (int i = 0; i < 18; i++)
                    {
                        polys[currentPolyIndex + i] = currentPolys[i];
                    }

                    currentPolyIndex += 18;

                    //join polys
                    if (y % 2 == 0)
                    {
                        //down right
                        if (mapData.TryGetHexAt(new Coord(currentCoord.X, currentCoord.Y - 1)) != null)
                        {
                            neighboursCenterIndex = currentVertexIndex - 7;
                            joinPolys = GetJoinPolys(currentVertexIndex + 3, neighboursCenterIndex + 1, neighboursCenterIndex + 6, currentVertexIndex + 4);
                            for (int i = 0; i < 6; i++)
                            {
                                polys[currentPolyIndex + i] = joinPolys[i];
                            }
                            currentPolyIndex += 6;
                        }

                        //down left
                        if (mapData.TryGetHexAt(new Coord(currentCoord.X - 1, currentCoord.Y - 1)) != null)
                        {
                            neighboursCenterIndex = currentVertexIndex - 7 - mapData.Height * 7;
                            joinPolys = GetJoinPolys(currentVertexIndex + 4, neighboursCenterIndex + 2, neighboursCenterIndex + 1, currentVertexIndex + 5);
                            for (int i = 0; i < 6; i++)
                            {
                                polys[currentPolyIndex + i] = joinPolys[i];
                            }
                            currentPolyIndex += 6;

                        }

                        //left
                        if (mapData.TryGetHexAt(new Coord(currentCoord.X - 1, currentCoord.Y)) != null)
                        {
                            neighboursCenterIndex = currentVertexIndex - mapData.Height * 7;
                            joinPolys = GetJoinPolys(currentVertexIndex + 5, neighboursCenterIndex + 3, neighboursCenterIndex + 2, currentVertexIndex + 6);
                            for (int i = 0; i < 6; i++)
                            {
                                polys[currentPolyIndex + i] = joinPolys[i];
                            }
                            currentPolyIndex += 6;
                        }

                        //up left
                        if (mapData.TryGetHexAt(new Coord(currentCoord.X - 1, currentCoord.Y + 1)) != null)
                        {
                            neighboursCenterIndex = currentVertexIndex + 7 - mapData.Height * 7;
                            joinPolys = GetJoinPolys(currentVertexIndex + 6, neighboursCenterIndex + 4, neighboursCenterIndex + 3, currentVertexIndex + 1);
                            for (int i = 0; i < 6; i++)
                            {
                                polys[currentPolyIndex + i] = joinPolys[i];
                            }
                            currentPolyIndex += 6;
                        }

                    }
                    else
                    {
                        //down left
                        if (mapData.TryGetHexAt(new Coord(currentCoord.X, currentCoord.Y - 1)) != null)
                        {
                            neighboursCenterIndex = currentVertexIndex - 7;
                            joinPolys = GetJoinPolys(currentVertexIndex + 4, neighboursCenterIndex + 2, neighboursCenterIndex + 1, currentVertexIndex + 5);
                            for (int i = 0; i < 6; i++)
                            {
                                polys[currentPolyIndex + i] = joinPolys[i];
                            }
                            currentPolyIndex += 6;
                        }

                        //left
                        if (mapData.TryGetHexAt(new Coord(currentCoord.X - 1, currentCoord.Y)) != null)
                        {
                            neighboursCenterIndex = currentVertexIndex - 7 * mapData.Height;
                            joinPolys = GetJoinPolys(currentVertexIndex + 5, neighboursCenterIndex + 3, neighboursCenterIndex + 2, currentVertexIndex + 6);
                            for (int i = 0; i < 6; i++)
                            {
                                polys[currentPolyIndex + i] = joinPolys[i];
                            }
                            currentPolyIndex += 6;
                        }
                    }

                    textureOffsetX = mapData.GetHexAt(new Coord(x, y)).HexType.TextureOffsetX;
                    textureOffsetY = mapData.GetHexAt(new Coord(x, y)).HexType.TextureOffsetY;

                    currentUvs = HexUvs(textureOffsetX, textureScaleMultiplierX, textureOffsetY, textureScaleMultiplierY);

                    for (int i = 0; i < 7; i++)
                    {
                        uvs[currentVertexIndex + i] = currentUvs[i];
                    }

                    currentVertexIndex += 7;
                }
            }

            mapMesh.vertices = vertices;
            mapMesh.triangles = polys;
            mapMesh.normals = normals;
            mapMesh.uv = uvs;

            mapGraphicsFilter.mesh = mapMesh;
            mapGraphicsCollider.sharedMesh = mapGraphicsFilter.sharedMesh;

            //TODO: make this one function
            InitialiseOffMap();
        }

        private void InitialiseOffMap()
        {
            Mesh offMapMesh = new Mesh();

            int numberOfOffMapHexes = ((mapData.Width + 2 * offMapHexThickness) * (mapData.Height + 2 * offMapHexThickness)) - (mapData.Width * mapData.Height);

            Vector3[] vertices = new Vector3[numberOfOffMapHexes * 7];
            Vector3[] currentVertices = new Vector3[7];

            int[] polys = new int[numberOfOffMapHexes * 18];
            int[] currentPolys = new int[18];

            Vector3[] normals = new Vector3[numberOfOffMapHexes * 7];

            Vector2[] uvs = new Vector2[numberOfOffMapHexes * 7];
            Vector2[] currentUvs = new Vector2[7];
            float textureOffsetX = textureOffsetForOffmapHexes[0];
            float textureOffsetY = textureOffsetForOffmapHexes[1];
            float textureScaleMultiplierX = textureRes / tileImages.width;
            float textureScaleMultiplierY = textureRes / tileImages.height;

            Coord currentCoord;
            Vector3 centerOfCurrent;

            int currentVertexIndex = 0;
            int currentPolyIndex = 0;

            for (int x = 0; x < mapData.Width + 2 * offMapHexThickness; x++)
            {
                for (int y = 0; y < mapData.Height + 2 * offMapHexThickness; y++)
                {

                    if (x < offMapHexThickness || y < offMapHexThickness || x >= mapData.Width + offMapHexThickness || y >= mapData.Height + offMapHexThickness)
                    {

                        currentCoord = new Coord(x, y);
                        centerOfCurrent = currentCoord.ToPositionVector();

                        //vertices (& normals)
                        currentVertices = HexVertices(centerOfCurrent);

                        for (int i = 0; i < 7; i++)
                        {
                            vertices[currentVertexIndex + i] = currentVertices[i];
                            normals[currentVertexIndex + i] = Vector3.forward;
                        }

                        //main polys
                        currentPolys = HexPolys(currentVertexIndex);

                        for (int i = 0; i < 18; i++)
                        {
                            polys[currentPolyIndex + i] = currentPolys[i];
                        }

                        currentPolyIndex += 18;

                        currentUvs = HexUvs(textureOffsetX, textureScaleMultiplierX, textureOffsetY, textureScaleMultiplierY);

                        for (int i = 0; i < 7; i++)
                        {
                            uvs[currentVertexIndex + i] = currentUvs[i];
                        }

                        currentVertexIndex += 7;

                    }
                }
            }

            offMapMesh.vertices = vertices;
            offMapMesh.triangles = polys;
            offMapMesh.normals = normals;
            offMapMesh.uv = uvs;

            offMapGraphicsFilter.mesh = offMapMesh;

            offMapGraphicsFilter.transform.position = new Vector3(-2 * MapRenderer.HEX_OFFSET * offMapHexThickness, -0.75f * offMapHexThickness, 0f);
        }

        private Vector3[] HexVertices(Vector3 center)
        {
            Vector3[] vertices = new Vector3[7];

            vertices[0] = center;

            for (int i = 0; i < 6; i++)
            {
                vertices[i + 1] = new Vector3(center.x + 0.5f * Mathf.Sin(i * Mathf.PI / 3), center.y + 0.5f * Mathf.Cos(i * Mathf.PI / 3), 0f);
            }

            return vertices;
        }

        private int[] HexPolys(Coord currentCoord)
        {
            int[] polys = new int[18];

            int startOffset = (currentCoord.Y + currentCoord.X * mapData.Height) * 7;

            for (int i = 0; i < 6; i++)
            {
                polys[3 * i] = i + 1 + startOffset;
                polys[3 * i + 1] = i + 2 < 7 ? i + 2 + startOffset : 1 + startOffset;
                polys[3 * i + 2] = startOffset;
            }

            return polys;
        }

        private int[] HexPolys(int currentVertexIndex)
        {
            int[] polys = new int[18];

            int startOffset = currentVertexIndex;

            for (int i = 0; i < 6; i++)
            {
                polys[3 * i] = i + 1 + startOffset;
                polys[3 * i + 1] = i + 2 < 7 ? i + 2 + startOffset : 1 + startOffset;
                polys[3 * i + 2] = startOffset;
            }

            return polys;
        }

        private int[] GetJoinPolys(int vertex1, int vertex2, int vertex3, int vertex4)
        {
            int[] polys = new int[6];

            polys[0] = vertex1;
            polys[1] = vertex2;
            polys[2] = vertex3;
            polys[3] = vertex1;
            polys[4] = vertex3;
            polys[5] = vertex4;

            return polys;
        }

        private Vector3[] HexNormals()
        {
            Vector3[] normals = new Vector3[7];

            for (int i = 0; i < 7; i++)
            {
                normals[i] = Vector3.forward;
            }

            return normals;
        }

        private Vector2[] HexUvs(float textureOffsetX, float textureScaleMultiplierX, float textureOffsetY, float textureScaleMultiplierY)
        {
            Vector2[] uvs = new Vector2[7];

            uvs[0] = new Vector2((textureOffsetX + 0.5f) * textureScaleMultiplierX, (textureOffsetY + 0.5f) * textureScaleMultiplierY);
            uvs[1] = new Vector2((textureOffsetX + 0.5f) * textureScaleMultiplierX, (textureOffsetY + 1f) * textureScaleMultiplierY);
            uvs[2] = new Vector2((textureOffsetX + 0.5f + HEX_OFFSET) * textureScaleMultiplierX, (textureOffsetY + 0.75f) * textureScaleMultiplierY);
            uvs[3] = new Vector2((textureOffsetX + 0.5f + HEX_OFFSET) * textureScaleMultiplierX, (textureOffsetY + 0.25f) * textureScaleMultiplierY);
            uvs[4] = new Vector2((textureOffsetX + 0.5f) * textureScaleMultiplierX, textureOffsetY * textureScaleMultiplierY);
            uvs[5] = new Vector2((textureOffsetX + 0.5f - HEX_OFFSET) * textureScaleMultiplierX, (textureOffsetY + 0.25f) * textureScaleMultiplierY);
            uvs[6] = new Vector2((textureOffsetX + 0.5f - HEX_OFFSET) * textureScaleMultiplierX, (textureOffsetY + 0.75f) * textureScaleMultiplierY);

            return uvs;
        }
    }
}