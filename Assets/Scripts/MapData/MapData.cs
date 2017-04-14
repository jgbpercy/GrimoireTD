using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HexType
{
    HEX_GRASS,
    HEX_ROCK
}

public class MapData {

    //private HexData[,] hexes;

    private Dictionary<Coord, HexData> hexes;

    private int width;
    private int height;

    private List<GraphEdge> graphEdges;

    private bool debugOn = false;

    private class GraphEdge
    {
        private Coord fromVertex;
        private Coord toVertex;

        public Coord FromVertex
        {
            get
            {
                return fromVertex;
            }
        }

        public Coord ToVertex
        {
            get
            {
                return toVertex;
            }
        }

        public GraphEdge(Coord fromVertex, Coord toVertex)
        {
            this.fromVertex = fromVertex;
            this.toVertex = toVertex;
        }

    }

    public int Width
    {
        get
        {
            return width;
        }
    }

    public int Height
    {
        get
        {
            return height;
        }
    }


    public MapData()
    {
        if (debugOn) Debug.Log("Call to default MapData constructor");

        width = 15;
        height = 10;
        GenerateEmptyMap();
        GenerateGraphEdges();
    }

    public MapData(int width, int height)
    {
        if (debugOn) Debug.Log("Call to specific MapData constructor: (" + width + ", " + height + ")");

        this.width = width;
        this.height = height;
        GenerateEmptyMap();
        GenerateGraphEdges();
    }

    public MapData(Texture2D mapImage, Dictionary<Color32, HexType> colorsToTypesDictionary)
    {
        if (debugOn) Debug.Log("Call to MapData constructor from image");

        width = mapImage.width / 2;
        height = mapImage.height / 2;

        if (debugOn) Debug.Log("Map dimensions: (" + width + ", " + height + ")");

        hexes = new Dictionary<Coord, HexData>();

        Color32[] allPixels = mapImage.GetPixels32();
        int xPixelCoord;
        int yPixelCoord;

        if (debugOn) Debug.Log("Pixels in array " + allPixels.GetLength(0));

        HexType currentHexType;
        bool foundHexType;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                xPixelCoord = y % 2 == 0 ? 2 * x : 2 * x + 1;
                yPixelCoord = 2 * y;

                if (debugOn) Debug.Log("Setting tile (" + x + ", " + y + ") from pixel (" + xPixelCoord + ", " + yPixelCoord + ")");

                if (debugOn) Debug.Log("Pixel Color: " + allPixels[xPixelCoord + yPixelCoord * width * 2]);

                foundHexType = colorsToTypesDictionary.TryGetValue(allPixels[xPixelCoord + yPixelCoord * width * 2], out currentHexType);

                hexes.Add(new Coord(x,y), foundHexType ? new HexData(currentHexType) : new HexData());
            }
        }

        GenerateGraphEdges();

    }

    private void GenerateEmptyMap()
    {
        hexes = new Dictionary<Coord, HexData>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                hexes.Add(new Coord(x, y), new HexData());
            }
        }
        
    }

    private void GenerateGraphEdges()
    {
        if (debugOn) Debug.Log("Generating Graph Edges");

        graphEdges = new List<GraphEdge>();

        List<Coord> currentNeighbours = new List<Coord>();
        Coord currentCoord;

        foreach ( KeyValuePair<Coord, HexData> hex in hexes)
        {
            currentCoord = hex.Key;

            if (debugOn) Debug.Log("Generating edges for (" + currentCoord.X + ", " + currentCoord.Y + ")");

            if ( GetHexAt(currentCoord).isPathable() )
            {

                currentNeighbours = GetNeighboursOf(currentCoord);

                if (debugOn) Debug.Log("Hex is pathable! Finding pathable neighbours");

                foreach (Coord neighbour in currentNeighbours)
                {

                    if ( GetHexAt(neighbour) != null && GetHexAt(neighbour).isPathable() )
                    {
                        graphEdges.Add(new GraphEdge(currentCoord, neighbour));
                        if (debugOn) Debug.Log("Added edge: (" + currentCoord.X + ", " + currentCoord.Y + ") -> (" + neighbour.X + ", " + neighbour.Y + ")");
                    }
                }
            }
        }
    }

    public HexData TryGetHexAt(Coord hexCoord)
    {
        HexData hexAtCoords;

        bool foundHex = hexes.TryGetValue(hexCoord, out hexAtCoords);

        if (foundHex)
        {
            return hexAtCoords;
        }

        return null;
    }

    public HexData GetHexAt(Coord hexCoord)
    {
        return hexes[hexCoord];       
    }

    public List<Coord> GetPath(Coord fromCoord, Coord toCoord)
    {
        List<Coord> closedSet = new List<Coord>();
        List<Coord> openSet = new List<Coord>();

        List<GraphEdge> currentEdges = new List<GraphEdge>();

        Coord currentCoord;

        float tentativeGScore;

        if (debugOn) Debug.Log("Generating path from (" + fromCoord.X + ", " + fromCoord.Y + ") to (" + toCoord.X + ", " + toCoord.Y + ")");

        openSet.Add(fromCoord);
        hexes[fromCoord].pathingGScore = 0f;
        hexes[fromCoord].pathingFScore = HeuristicDistance(fromCoord, toCoord);

        while ( openSet.Count != 0 )
        {
            if (debugOn) Debug.Log("----A* iteration----");

            currentCoord = openSet[0];
            foreach ( Coord openSetMember in openSet )
            {
                if ( hexes[openSetMember].pathingFScore < hexes[currentCoord].pathingFScore )
                {
                    currentCoord = openSetMember;
                }
            }

            if (debugOn) Debug.Log("Current Coord is (" + currentCoord.X + ", " + currentCoord.Y + ")");

            if ( currentCoord.Equals(toCoord) )
            {
                if (debugOn) Debug.Log("----To Coord reached, reconstructing path----");
                return ReconstructPath(fromCoord, toCoord);
            }

            openSet.Remove(currentCoord);
            closedSet.Add(currentCoord);

            currentEdges = graphEdges.FindAll(x => x.FromVertex.Equals(currentCoord));

            if (debugOn) Debug.Log("Number of current edges: " + currentEdges.Count);
            
            foreach ( GraphEdge edge in currentEdges )
            {
                if (debugOn) Debug.Log("Neighbour (" + edge.ToVertex.X + ", " + edge.ToVertex.Y + ")...");

                if ( !closedSet.Contains(edge.ToVertex) )
                {

                    if (debugOn) Debug.Log("...was not in closed set...");
                    tentativeGScore = hexes[currentCoord].pathingGScore + 2 * MapLoader.HEX_OFFSET;
                    if ( !openSet.Contains(edge.ToVertex) )
                    {
                        if (debugOn) Debug.Log("...or open set...");
                        openSet.Add(edge.ToVertex);
                    }
                    else
                    {
                        if (debugOn) Debug.Log("...but was in open set...");
                    }
                    if ( tentativeGScore < hexes[edge.ToVertex].pathingGScore )
                    {
                        if (debugOn) Debug.Log("...tentative GScore " + tentativeGScore + " was below previous GScore of " + hexes[edge.ToVertex].pathingGScore + "...");
                        hexes[edge.ToVertex].pathingGScore = tentativeGScore;
                        hexes[edge.ToVertex].pathingCameFrom = currentCoord;
                        hexes[edge.ToVertex].pathingFScore = tentativeGScore + HeuristicDistance(edge.ToVertex, toCoord);
                        if (debugOn) Debug.Log("...set Came From to (" + hexes[edge.ToVertex].pathingCameFrom.X + ", " + hexes[edge.ToVertex].pathingCameFrom.Y + ") and FScore to " + hexes[edge.ToVertex].pathingFScore + ".");
                    }
                }
                else
                {
                    if (debugOn) Debug.Log("...was in closed set.");
                }
            }
        }

        return null;
    }

    private float HeuristicDistance(Coord fromCoord, Coord toCoord)
    {
        Vector3 fromCoordPosition = fromCoord.ToPositionVector();
        Vector3 toCoordPosition = toCoord.ToPositionVector();

        if (debugOn) Debug.Log("Calculating a heuristic distance between (" + fromCoord.X + ", " + fromCoord.Y + ") and (" + toCoord.X + ", " + toCoord.Y + ")");
        if (debugOn) Debug.Log("Positions: " + fromCoordPosition + " and " + toCoordPosition);

        if (debugOn) Debug.Log("Difference: " + (fromCoordPosition - toCoordPosition));

        if (debugOn) Debug.Log("Magnitude: " + Vector3.Magnitude(fromCoordPosition - toCoordPosition));

        return Vector3.Magnitude(fromCoordPosition - toCoordPosition);
    }

    private List<Coord> ReconstructPath(Coord fromCoord, Coord toCoord)
    {
        List<Coord> path = new List<Coord>();

        Coord currentCoord = toCoord;

        while ( !currentCoord.Equals(fromCoord) )
        {
            if (debugOn) Debug.Log("Adding (" + currentCoord.X + ", " + currentCoord.Y + ") to path");
            path.Add(currentCoord);
            currentCoord = hexes[currentCoord].pathingCameFrom;
        }

        if (debugOn) Debug.Log("Adding (" + fromCoord.X + ", " + fromCoord.Y + ") to path");
        path.Add(fromCoord);

        return path;
    }

    private List<Coord> GetNeighboursOf(Coord hexCoord)
    {
        List<Coord> neighbourList = new List<Coord>();

        int x = hexCoord.X;
        int y = hexCoord.Y;

        int xOffset = y % 2 == 0 ? -1 : 0;

        Coord upLeft = new Coord(x + xOffset, y + 1);
        if (TryGetHexAt(upLeft) != null)
        {
            neighbourList.Add(upLeft);
        }

        Coord upRight = new Coord(x + xOffset + 1, y + 1);
        if (TryGetHexAt(upRight) != null)
        {
            neighbourList.Add(upRight);
        }

        Coord xRight = new Coord(x + 1, y);
        if (TryGetHexAt(xRight) != null)
        {
            neighbourList.Add(xRight);
        }

        Coord downRight = new Coord(x + xOffset + 1, y - 1);
        if (TryGetHexAt(downRight) != null)
        {
            neighbourList.Add(downRight);
        }

        Coord downLeft = new Coord(x + xOffset, y - 1);
        if (TryGetHexAt(downLeft) != null)
        {
            neighbourList.Add(downLeft);
        }

        Coord xLeft = new Coord(x - 1, y);
        if (TryGetHexAt(xLeft) != null)
        {
            neighbourList.Add(xLeft);
        }

        return neighbourList;
    }

}
