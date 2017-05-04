using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class MapData {

    private Dictionary<Coord, HexData> hexes;

    private Dictionary<DefendingEntity, Coord> defendingEntities = new Dictionary<DefendingEntity, Coord>();

    private int width;
    private int height;

    private List<GraphEdge> graphEdges;

    private List<Coord> path;

    public Action OnPathGeneratedOrChangedCallback;

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

    public List<Coord> Path
    {
        get
        {
            return path;
        }
    }

    public MapData(Texture2D mapImage, Dictionary<Color32, HexType> colorsToTypesDictionary)
    {
        CDebug.Log(CDebug.mapGeneration, "Call to MapData constructor from image");

        width = mapImage.width / 2;
        height = mapImage.height / 2;

        CDebug.Log(CDebug.mapGeneration, "Map dimensions: (" + width + ", " + height + ")");

        hexes = new Dictionary<Coord, HexData>();

        Color32[] allPixels = mapImage.GetPixels32();
        int xPixelCoord;
        int yPixelCoord;

        CDebug.Log(CDebug.mapGeneration, "Pixels in array " + allPixels.GetLength(0));

        HexType currentHexType;
        bool foundHexType;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                xPixelCoord = y % 2 == 0 ? 2 * x : 2 * x + 1;
                yPixelCoord = 2 * y;

                CDebug.Log(CDebug.mapGeneration, "Setting tile (" + x + ", " + y + ") from pixel (" + xPixelCoord + ", " + yPixelCoord + ")");

                CDebug.Log(CDebug.mapGeneration, "Pixel Color: " + allPixels[xPixelCoord + yPixelCoord * width * 2]);

                foundHexType = colorsToTypesDictionary.TryGetValue(allPixels[xPixelCoord + yPixelCoord * width * 2], out currentHexType);
                if ( !foundHexType )
                {
                    Debug.Log("didn't find hex for coord: " + x + ", " + y);
                    Debug.Log("R:" + allPixels[xPixelCoord + yPixelCoord * width * 2].r);
                    Debug.Log("G:" + allPixels[xPixelCoord + yPixelCoord * width * 2].g);
                    Debug.Log("B:" + allPixels[xPixelCoord + yPixelCoord * width * 2].b);
                    Debug.Log("A:" + allPixels[xPixelCoord + yPixelCoord * width * 2].a);
                }

                Assert.IsTrue(foundHexType);

                hexes.Add(new Coord(x,y), new HexData(currentHexType));
            }
        }
    }

    public void GeneratePath()
    {
        GenerateGraphEdges();

        path = GetPath(new Coord(0, 0), new Coord(width - 1, height - 1));

        if (OnPathGeneratedOrChangedCallback != null)
        {
            OnPathGeneratedOrChangedCallback();
        }
    }

    private void GenerateGraphEdges()
    {
        CDebug.Log(CDebug.mapGeneration, "Generating Graph Edges");

        graphEdges = new List<GraphEdge>();

        List<Coord> currentNeighbours = new List<Coord>();
        Coord currentCoord;

        foreach ( KeyValuePair<Coord, HexData> hex in hexes)
        {
            currentCoord = hex.Key;

            CDebug.Log(CDebug.mapGeneration, "Generating edges for (" + currentCoord.X + ", " + currentCoord.Y + ")");

            if ( GetHexAt(currentCoord).IsPathable() )
            {

                currentNeighbours = GetNeighboursOf(currentCoord);

                CDebug.Log(CDebug.mapGeneration, "Hex is pathable! Finding pathable neighbours");

                foreach (Coord neighbour in currentNeighbours)
                {

                    if ( GetHexAt(neighbour) != null && GetHexAt(neighbour).IsPathable() )
                    {
                        graphEdges.Add(new GraphEdge(currentCoord, neighbour));
                        CDebug.Log(CDebug.mapGeneration, "Added edge: (" + currentCoord.X + ", " + currentCoord.Y + ") -> (" + neighbour.X + ", " + neighbour.Y + ")");
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

    private List<Coord> GetPath(Coord fromCoord, Coord toCoord)
    {
        List<Coord> closedSet = new List<Coord>();
        List<Coord> openSet = new List<Coord>();

        List<GraphEdge> currentEdges = new List<GraphEdge>();

        Coord currentCoord;

        float tentativeGScore;

        CDebug.Log(CDebug.mapGeneration, "Generating path from (" + fromCoord.X + ", " + fromCoord.Y + ") to (" + toCoord.X + ", " + toCoord.Y + ")");

        openSet.Add(fromCoord);
        hexes[fromCoord].pathingGScore = 0f;
        hexes[fromCoord].pathingFScore = HeuristicDistance(fromCoord, toCoord);

        while ( openSet.Count != 0 )
        {
            CDebug.Log(CDebug.mapGeneration, "----A* iteration----");

            currentCoord = openSet[0];
            foreach ( Coord openSetMember in openSet )
            {
                if ( hexes[openSetMember].pathingFScore < hexes[currentCoord].pathingFScore )
                {
                    currentCoord = openSetMember;
                }
            }

            CDebug.Log(CDebug.mapGeneration, "Current Coord is (" + currentCoord.X + ", " + currentCoord.Y + ")");

            if ( currentCoord.Equals(toCoord) )
            {
                CDebug.Log(CDebug.mapGeneration, "----To Coord reached, reconstructing path----");
                return ReconstructPath(fromCoord, toCoord);
            }

            openSet.Remove(currentCoord);
            closedSet.Add(currentCoord);

            currentEdges = graphEdges.FindAll(x => x.FromVertex.Equals(currentCoord));

            CDebug.Log(CDebug.mapGeneration, "Number of current edges: " + currentEdges.Count);
            
            foreach ( GraphEdge edge in currentEdges )
            {
                CDebug.Log(CDebug.mapGeneration, "Neighbour (" + edge.ToVertex.X + ", " + edge.ToVertex.Y + ")...");

                if ( !closedSet.Contains(edge.ToVertex) )
                {

                    CDebug.Log(CDebug.mapGeneration, "...was not in closed set...");
                    tentativeGScore = hexes[currentCoord].pathingGScore + 2 * MapRenderer.HEX_OFFSET;
                    if ( !openSet.Contains(edge.ToVertex) )
                    {
                        CDebug.Log(CDebug.mapGeneration, "...or open set...");
                        openSet.Add(edge.ToVertex);
                    }
                    else
                    {
                        CDebug.Log(CDebug.mapGeneration, "...but was in open set...");
                    }
                    if ( tentativeGScore < hexes[edge.ToVertex].pathingGScore )
                    {
                        CDebug.Log(CDebug.mapGeneration, "...tentative GScore " + tentativeGScore + " was below previous GScore of " + hexes[edge.ToVertex].pathingGScore + "...");
                        hexes[edge.ToVertex].pathingGScore = tentativeGScore;
                        hexes[edge.ToVertex].pathingCameFrom = currentCoord;
                        hexes[edge.ToVertex].pathingFScore = tentativeGScore + HeuristicDistance(edge.ToVertex, toCoord);
                        CDebug.Log(CDebug.mapGeneration, "...set Came From to (" + hexes[edge.ToVertex].pathingCameFrom.X + ", " + hexes[edge.ToVertex].pathingCameFrom.Y + ") and FScore to " + hexes[edge.ToVertex].pathingFScore + ".");
                    }
                }
                else
                {
                    CDebug.Log(CDebug.mapGeneration, "...was in closed set.");
                }
            }
        }

        return null;
    }

    private float HeuristicDistance(Coord fromCoord, Coord toCoord)
    {
        Vector3 fromCoordPosition = fromCoord.ToPositionVector();
        Vector3 toCoordPosition = toCoord.ToPositionVector();

        CDebug.Log(CDebug.mapGeneration, "Calculating a heuristic distance between (" + fromCoord.X + ", " + fromCoord.Y + ") and (" + toCoord.X + ", " + toCoord.Y + ")");
        CDebug.Log(CDebug.mapGeneration, "Positions: " + fromCoordPosition + " and " + toCoordPosition);

        CDebug.Log(CDebug.mapGeneration, "Difference: " + (fromCoordPosition - toCoordPosition));

        CDebug.Log(CDebug.mapGeneration, "Magnitude: " + Vector3.Magnitude(fromCoordPosition - toCoordPosition));

        return Vector3.Magnitude(fromCoordPosition - toCoordPosition);
    }

    private List<Coord> ReconstructPath(Coord fromCoord, Coord toCoord)
    {
        List<Coord> path = new List<Coord>();

        Coord currentCoord = toCoord;

        while ( !currentCoord.Equals(fromCoord) )
        {
            CDebug.Log(CDebug.mapGeneration, "Adding (" + currentCoord.X + ", " + currentCoord.Y + ") to path");
            path.Add(currentCoord);
            currentCoord = hexes[currentCoord].pathingCameFrom;
        }

        CDebug.Log(CDebug.mapGeneration, "Adding (" + fromCoord.X + ", " + fromCoord.Y + ") to path");
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

    public void RegisterForOnPathGeneratedOrChangedCallback(Action callback)
    {
        OnPathGeneratedOrChangedCallback += callback;
    }

    public void DeregisterForOnPathGeneratedOrChangedCallback(Action callback)
    {
        OnPathGeneratedOrChangedCallback -= callback;
    }

    public bool TryBuildStructureAt(Coord coord, StructureTemplate structureTemplate)
    {
        if ( GetHexAt(coord).CanAddStructureHere() && EconomyManager.Instance.CanDoTransaction( structureTemplate.Cost ) )
        {
            Structure newStructure = structureTemplate.GenerateStructure(coord.ToPositionVector());

            GetHexAt(coord).AddStructureHere(newStructure);
            EconomyManager.Instance.DoTransaction(structureTemplate.Cost);
            defendingEntities.Add(newStructure, coord);

            return true;
        }

        return false;
    }

    public bool TryBuildStructureAtFree(Coord coord, StructureTemplate structureTemplate)
    {
        Assert.IsTrue(TryGetHexAt(coord) != null);
        Assert.IsTrue(structureTemplate != null);

        if ( GetHexAt(coord).CanAddStructureHere() )
        {
            Structure newStructure = structureTemplate.GenerateStructure(coord.ToPositionVector());

            GetHexAt(coord).AddStructureHere(newStructure);
            defendingEntities.Add(newStructure, coord);

            return true;
        }

        return false;
    }

    public bool TryCreateUnitAt(Coord coord, UnitTemplate unitTemplate)
    {
        if ( GetHexAt(coord).CanMoveUnitHere() )
        {
            Unit newUnit = unitTemplate.GenerateUnit(coord.ToPositionVector());

            GetHexAt(coord).MoveUnitHere(newUnit);
            defendingEntities.Add(newUnit, coord);

            return true;
        }

        return false;
    }

    public bool TryMoveUnitTo(Coord fromCoord, Coord targetCoord, Unit unit, EconomyTransaction cost)
    {
        if ( GetHexAt(targetCoord).CanMoveUnitHere() && EconomyManager.Instance.CanDoTransaction(cost) )
        {
            GetHexAt(targetCoord).MoveUnitHere(unit);
            GetHexAt(fromCoord).RemoveUnitHere();

            defendingEntities.Remove(unit);
            defendingEntities.Add(unit, targetCoord);

            EconomyManager.Instance.DoTransaction(cost);

            return true;
        }

        return false;
    }

    public static bool HexIsInRange(int range, Coord startHex, Coord targetHex)
    {
        return Distance(startHex, targetHex) <= range;
    }

    public static int Distance(Coord startHex, Coord targetHex)
    {
        int baseVerticalDistance = Mathf.Abs(startHex.Y - targetHex.Y);
        int baseHorizontalDistance;

        if (baseVerticalDistance % 2 == 0)
        {
            baseHorizontalDistance = Mathf.Abs(startHex.X - targetHex.X);
        }
        else
        {
            int fakeX;

            if (startHex.Y % 2 == 0)
            {
                fakeX = startHex.X > targetHex.X ? targetHex.X + 1 : targetHex.X;
            }
            else
            {
                fakeX = startHex.X > targetHex.X ? targetHex.X : targetHex.X - 1;
            }

            baseHorizontalDistance = Mathf.Abs(startHex.X - fakeX);
        }

        CDebug.Log(CDebug.distanceCalculations, "Ver: " + baseVerticalDistance);
        CDebug.Log(CDebug.distanceCalculations, "Hor: " + baseHorizontalDistance);

        return baseVerticalDistance + Mathf.Max(baseHorizontalDistance - (baseVerticalDistance / 2), 0);
    }

    public bool HexIsEmpty(Coord coord)
    {
        return GetHexAt(coord).UnitHere == null && GetHexAt(coord).StructureHere == null;
    }

    public bool HexHasUnit(Coord coord)
    {
        return GetHexAt(coord).UnitHere != null;
    }

    public bool HexHasStructure(Coord coord)
    {
        return GetHexAt(coord).StructureHere != null;
    }

    public Coord WhereAmI(DefendingEntity defendingEntity)
    {
        return defendingEntities[defendingEntity];
    }

}
