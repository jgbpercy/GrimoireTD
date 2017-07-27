using UnityEngine;

public class MapRenderer : SingletonMonobehaviour<MapRenderer> {

    [SerializeField]
    private bool debugOn;

    public static float tileScale = 1f;

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

    private MapData map;

    [SerializeField]
    private int offMapHexThickness;

    private void Start ()
    {
        CDebug.Log(CDebug.applicationLoading, "Map Renderer Start");

        tileImages = (Texture2D)mapGraphicsRenderer.material.mainTexture;

        map = MapGenerator.Instance.Map;

        //TODO subscribe to map change events

        InitialiseMap();

        InitialiseOffMap();
	}

    private void InitialiseMap()
    {
        if (debugOn) Debug.Log("Intialising Map");

        Mesh mapMesh = new Mesh();

        Vector3[] vertices = new Vector3[map.Width * map.Height * 7];
        Vector3[] currentVertices = new Vector3[7];

        int[] polys = new int[((map.Width * map.Height * 6) + 2 * (3 * map.Width * map.Height - 2*map.Width - 2*map.Height + 1)) * 3];
        int[] currentPolys = new int[18];
        int neighboursCenterIndex;
        int[] joinPolys = new int[6];

        Vector3[] normals = new Vector3[map.Width * map.Height * 7];

        Vector2[] uvs = new Vector2[map.Width * map.Height * 7];
        Vector2[] currentUvs = new Vector2[7];
        float textureOffsetX = 0f;
        float textureOffsetY = 0f;
        float textureScaleMultiplierX = textureRes / tileImages.width;
        float textureScaleMultiplierY = textureRes / tileImages.height;

        Coord currentCoord;
        Vector3 centerOfCurrent;

        int currentVertexIndex = 0;
        int currentPolyIndex = 0;

        for (int x = 0; x < map.Width; x++)
        {
            for (int y = 0; y < map.Height; y++)
            {
                currentCoord = new Coord(x, y);
                centerOfCurrent = currentCoord.ToPositionVector();

                //vertices (& normals)
                currentVertices = hexVertices(centerOfCurrent);

                for (int i = 0; i < 7; i++)
                {
                    vertices[currentVertexIndex + i] = currentVertices[i];
                    normals[currentVertexIndex + i] = Vector3.forward;
                }

                //main polys
                currentPolys = hexPolys(currentCoord);

                for (int i = 0; i < 18; i++)
                {
                    polys[currentPolyIndex + i] = currentPolys[i];
                }

                currentPolyIndex += 18;

                //join polys
                if ( y % 2 == 0 )
                {
                    //down right
                    if ( map.TryGetHexAt(new Coord(currentCoord.X, currentCoord.Y - 1)) != null )
                    {
                        neighboursCenterIndex = currentVertexIndex - 7;
                        joinPolys = getJoinPolys(currentVertexIndex + 3, neighboursCenterIndex + 1, neighboursCenterIndex + 6, currentVertexIndex + 4);
                        for (int i = 0; i < 6; i++)
                        {
                            polys[currentPolyIndex + i] = joinPolys[i];
                        }
                        currentPolyIndex += 6;
                    }

                    //down left
                    if ( map.TryGetHexAt(new Coord(currentCoord.X - 1, currentCoord.Y - 1)) != null)
                    {
                        neighboursCenterIndex = currentVertexIndex - 7 - map.Height * 7;
                        joinPolys = getJoinPolys(currentVertexIndex + 4, neighboursCenterIndex + 2, neighboursCenterIndex + 1, currentVertexIndex + 5);
                        for (int i = 0; i < 6; i++)
                        {
                            polys[currentPolyIndex + i] = joinPolys[i];
                        }
                        currentPolyIndex += 6;

                    }

                    //left
                    if ( map.TryGetHexAt(new Coord(currentCoord.X - 1, currentCoord.Y)) != null )
                    {
                        neighboursCenterIndex = currentVertexIndex - map.Height * 7;
                        joinPolys = getJoinPolys(currentVertexIndex + 5, neighboursCenterIndex + 3, neighboursCenterIndex + 2, currentVertexIndex + 6);
                        for (int i = 0; i < 6; i++)
                        {
                            polys[currentPolyIndex + i] = joinPolys[i];
                        }
                        currentPolyIndex += 6;
                    }

                    //up left
                    if (map.TryGetHexAt(new Coord(currentCoord.X - 1, currentCoord.Y + 1)) != null)
                    {
                        neighboursCenterIndex = currentVertexIndex + 7 - map.Height * 7;
                        joinPolys = getJoinPolys(currentVertexIndex + 6, neighboursCenterIndex + 4, neighboursCenterIndex + 3, currentVertexIndex + 1);
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
                    if (map.TryGetHexAt(new Coord(currentCoord.X, currentCoord.Y - 1)) != null)
                    {
                        neighboursCenterIndex = currentVertexIndex - 7;
                        joinPolys = getJoinPolys(currentVertexIndex + 4, neighboursCenterIndex + 2, neighboursCenterIndex + 1, currentVertexIndex + 5);
                        for (int i = 0; i < 6; i++)
                        {
                            polys[currentPolyIndex + i] = joinPolys[i];
                        }
                        currentPolyIndex += 6;
                    }

                    //left
                    if (map.TryGetHexAt(new Coord(currentCoord.X - 1, currentCoord.Y)) != null)
                    {
                        neighboursCenterIndex = currentVertexIndex - 7 * map.Height;
                        joinPolys = getJoinPolys(currentVertexIndex + 5, neighboursCenterIndex + 3, neighboursCenterIndex + 2, currentVertexIndex + 6);
                        for (int i = 0; i < 6; i++)
                        {
                            polys[currentPolyIndex + i] = joinPolys[i];
                        }
                        currentPolyIndex += 6;
                    }
                }

                textureOffsetX = map.GetHexAt(new Coord(x, y)).HexType.TextureOffsetX;
                textureOffsetY = map.GetHexAt(new Coord(x, y)).HexType.TextureOffsetY;
                     
                currentUvs = hexUvs(textureOffsetX, textureScaleMultiplierX, textureOffsetY, textureScaleMultiplierY);

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
    }

    private void InitialiseOffMap()
    {
        if (debugOn) Debug.Log("Intialising Off Map");

        Mesh offMapMesh = new Mesh();

        int numberOfOffMapHexes = ((map.Width + 2* offMapHexThickness) * (map.Height + 2 * offMapHexThickness)) - (map.Width * map.Height);

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

        for (int x = 0; x < map.Width + 2 * offMapHexThickness; x++)
        {
            for (int y = 0; y < map.Height + 2 * offMapHexThickness; y++)
            {

                if ( x < offMapHexThickness || y < offMapHexThickness || x >= map.Width + offMapHexThickness || y >= map.Height + offMapHexThickness )
                {

                    currentCoord = new Coord(x, y);
                    centerOfCurrent = currentCoord.ToPositionVector();

                    //vertices (& normals)
                    currentVertices = hexVertices(centerOfCurrent);

                    for (int i = 0; i < 7; i++)
                    {
                        vertices[currentVertexIndex + i] = currentVertices[i];
                        normals[currentVertexIndex + i] = Vector3.forward;
                    }

                    //main polys
                    currentPolys = hexPolys(currentVertexIndex);

                    for (int i = 0; i < 18; i++)
                    {
                        polys[currentPolyIndex + i] = currentPolys[i];
                    }

                    currentPolyIndex += 18;

                    currentUvs = hexUvs(textureOffsetX, textureScaleMultiplierX, textureOffsetY, textureScaleMultiplierY);

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



    private Vector3[] hexVertices(Vector3 center)
    {
        if (debugOn) Debug.Log("Generating Vertices");

        Vector3[] vertices = new Vector3[7];

        vertices[0] = center;
        if (debugOn) Debug.Log("Generated: " + vertices[0]);

        for (int i = 0; i < 6; i++)
        {
            vertices[i + 1] = new Vector3(center.x + 0.5f * Mathf.Sin(i*Mathf.PI/3), center.y + 0.5f * Mathf.Cos(i*Mathf.PI/3), 0f);
            if (debugOn) Debug.Log("Generated: " + vertices[i + 1]);
        }
        
        return vertices;
    }

    private int[] hexPolys(Coord currentCoord)
    {
        if (debugOn) Debug.Log("Generating Polys from Coord");

        int[] polys = new int[18];

        int startOffset = (currentCoord.Y + currentCoord.X * map.Height) * 7;

        for (int i = 0; i < 6; i++)
        {
            polys[3*i] = i + 1 + startOffset;
            polys[3*i + 1] = i + 2 < 7 ? i + 2 + startOffset : 1 + startOffset;
            polys[3*i + 2] = startOffset;
            if (debugOn) Debug.Log("Set Poly: (" + 3 * i + ": " + polys[3 * i] + ", " + (3 * i + 1) + ": " + polys[3 * i + 1] + ", " + (3 * i + 2) + ": " + polys[3 * i + 2] + ")" );
        }

        return polys;
    }

    private int[] hexPolys(int currentVertexIndex)
    {
        if (debugOn) Debug.Log("Generating Polys from index");

        int[] polys = new int[18];

        int startOffset = currentVertexIndex;

        for (int i = 0; i < 6; i++)
        {
            polys[3 * i] = i + 1 + startOffset;
            polys[3 * i + 1] = i + 2 < 7 ? i + 2 + startOffset : 1 + startOffset;
            polys[3 * i + 2] = startOffset;
            if (debugOn) Debug.Log("Set Poly: (" + 3 * i + ": " + polys[3 * i] + ", " + (3 * i + 1) + ": " + polys[3 * i + 1] + ", " + (3 * i + 2) + ": " + polys[3 * i + 2] + ")");
        }

        return polys;
    }

    private int[] getJoinPolys(int vertex1, int vertex2, int vertex3, int vertex4)
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

    private Vector3[] hexNormals()
    {
        if (debugOn) Debug.Log("Generating Normals");

        Vector3[] normals = new Vector3[7];

        for (int i = 0; i < 7; i++)
        {
            normals[i] = Vector3.forward;
        }

        return normals;
    }

    private Vector2[] hexUvs(float textureOffsetX, float textureScaleMultiplierX, float textureOffsetY, float textureScaleMultiplierY)
    {
        if (debugOn) Debug.Log("Generating UVs");

        Vector2[] uvs = new Vector2[7];

        uvs[0] = new Vector2((textureOffsetX + 0.5f) * textureScaleMultiplierX, (textureOffsetY + 0.5f) * textureScaleMultiplierY );
        uvs[1] = new Vector2((textureOffsetX + 0.5f) * textureScaleMultiplierX, (textureOffsetY + 1f) * textureScaleMultiplierY );
        uvs[2] = new Vector2((textureOffsetX + 0.5f + HEX_OFFSET) * textureScaleMultiplierX, (textureOffsetY + 0.75f) * textureScaleMultiplierY );
        uvs[3] = new Vector2((textureOffsetX + 0.5f + HEX_OFFSET) * textureScaleMultiplierX, (textureOffsetY + 0.25f) * textureScaleMultiplierY );
        uvs[4] = new Vector2((textureOffsetX + 0.5f) * textureScaleMultiplierX, textureOffsetY * textureScaleMultiplierY );
        uvs[5] = new Vector2((textureOffsetX + 0.5f - HEX_OFFSET) * textureScaleMultiplierX, (textureOffsetY + 0.25f) * textureScaleMultiplierY );
        uvs[6] = new Vector2((textureOffsetX + 0.5f - HEX_OFFSET) * textureScaleMultiplierX, (textureOffsetY + 0.75f) * textureScaleMultiplierY );

        if (debugOn)
        {
            Debug.Log("Generated UVs:");

            Debug.Log("0: " + uvs[0]);
            Debug.Log("1: " + uvs[1]);
            Debug.Log("2: " + uvs[2]);
            Debug.Log("3: " + uvs[3]);
            Debug.Log("4: " + uvs[4]);
            Debug.Log("5: " + uvs[5]);
            Debug.Log("6: " + uvs[6]);
        }
            
        return uvs;
    }
}
