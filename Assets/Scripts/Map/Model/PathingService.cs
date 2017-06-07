using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PathingService {

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

    public static List<Coord> GeneratePath(MapData map, Dictionary<Coord, HexData> hexes, Coord start, Coord end)
    {
        return GeneratePath(map, hexes, start, end, new List<Coord>());
    }

    public static List<Coord> GeneratePath(MapData map, Dictionary<Coord, HexData> hexes, Coord start, Coord end, List<Coord> unpathableCoords)
    {
        return GeneratePath(map, hexes, start, end, unpathableCoords, new List<Coord>());
    }

    public static List<Coord> GeneratePath(MapData map, Dictionary<Coord, HexData> hexes, Coord start, Coord end, List<Coord> unpathableCoords, List<Coord> newlyPathableCoords)
    {
        List<GraphEdge> graphEdges = GraphEdges(map, hexes, unpathableCoords, newlyPathableCoords);

        return CalculatePath(hexes, start, end, graphEdges);
    }

    public static List<Coord> DisallowedCoords(List<Coord> path, MapData map, Dictionary<Coord, HexData> hexes, Coord start, Coord end)
    {
        return DisallowedCoords(path, map, hexes, start, end, new List<Coord>());
    }

    /*Optimisation: 
     * when assessing a path coord, just try and generate from the 
     * previous one in the current path to the next one in the current path. Should be shorter in 
     * almost all cases; need to double check this works though.
     * OR!
     * Calulate from the two coords from each half that have the shortest heuristic distance from each other? (but
     * that matrix could be quite big)
     * OR!
     * You can probably do something where you start with a non-zero open set?
    */
    public static List<Coord> DisallowedCoords(List<Coord> path, MapData map, Dictionary<Coord, HexData> hexes, Coord start, Coord end, List<Coord> newlyPathableCoords)
    {
        List<Coord> disallowedList = new List<Coord>();

        foreach (Coord coord in path) 
        {
            CDebug.Log(CDebug.pathing, "----Assessing " + coord + " for disallow list, " + newlyPathableCoords.Count + " newly pathable coord----");

            if ( GeneratePath(map, hexes, start, end, new List<Coord> { coord }, newlyPathableCoords) == null )
            {
                disallowedList.Add(coord);
            }
        }

        CDebug.Log(CDebug.pathing, "----Returning " + disallowedList.Count + " disallowed coords----");
        return disallowedList;
    }

    private static List<GraphEdge> GraphEdges(MapData map, Dictionary<Coord, HexData> hexes, List<Coord> newlyUnpathableCoords, List<Coord> newlyPathableCoords)
    {
        CDebug.Log(CDebug.pathing, "----Generating Graph Edges----");

        List<GraphEdge> graphEdges = new List<GraphEdge>();

        List<Coord> currentNeighbours = new List<Coord>();
        Coord currentCoord;
        HexData currentHex;

        foreach (KeyValuePair<Coord, HexData> hex in hexes)
        {
            currentCoord = hex.Key;
            currentHex = hex.Value;

            string debugString = "";

            if (CDebug.pathing.Enabled) { debugString += "Generating edges for " + currentCoord + " | "; }

            if ( IsPathable(currentCoord, currentHex, newlyUnpathableCoords, newlyPathableCoords) )
            {
                currentNeighbours = map.GetNeighboursOf(currentCoord);

                if (CDebug.pathing.Enabled) { debugString += "Hex pathable! Finding pathable neighbours: ";  }

                foreach (Coord neighbour in currentNeighbours)
                {

                    if (map.GetHexAt(neighbour) != null && IsPathable(neighbour, map.GetHexAt(neighbour), newlyUnpathableCoords, newlyPathableCoords) )
                    {
                        graphEdges.Add(new GraphEdge(currentCoord, neighbour));

                        if (CDebug.pathing.Enabled) { debugString += "Added edge: " + currentCoord + " -> " + neighbour + " | "; }
                    }
                }
            }

            CDebug.Log(CDebug.pathing, debugString);
        }

        return graphEdges;
    }

    private static bool IsPathable(Coord coord, HexData hex, List<Coord> newlyUnpathableCoords, List<Coord> newlyPathableCoords)
    {
        //bool debugThis = false;

        if (hex.IsPathableByCreeps() && !newlyUnpathableCoords.Contains(coord))
        {
            return true;
        }

        if (newlyPathableCoords.Contains(coord) )
        {
            return true;
        }

        return false;
    }

    private static List<Coord> CalculatePath(Dictionary<Coord, HexData> hexes, Coord fromCoord, Coord toCoord, List<GraphEdge> graphEdges)
    {
        List<Coord> closedSet = new List<Coord>();
        List<Coord> openSet = new List<Coord>();

        List<GraphEdge> currentEdges = new List<GraphEdge>();

        Coord currentCoord;

        float tentativeGScore;

        foreach (KeyValuePair<Coord,HexData> hex in hexes)
        {
            hex.Value.ResetPathingData();
        }

        CDebug.Log(CDebug.pathing, "Generating path from " + fromCoord + " to " + toCoord);

        openSet.Add(fromCoord);
        hexes[fromCoord].pathingGScore = 0f;
        hexes[fromCoord].pathingFScore = HeuristicDistance(fromCoord, toCoord);

        while (openSet.Count != 0)
        {
            CDebug.Log(CDebug.pathing, "----A* iteration----");

            currentCoord = openSet[0];
            foreach (Coord openSetMember in openSet)
            {
                if (hexes[openSetMember].pathingFScore < hexes[currentCoord].pathingFScore)
                {
                    currentCoord = openSetMember;
                }
            }

            CDebug.Log(CDebug.pathing, "Current Coord is " + currentCoord);

            if (currentCoord.Equals(toCoord))
            {
                CDebug.Log(CDebug.pathing, "----To Coord reached, reconstructing path----");
                return ReconstructPath(hexes, fromCoord, toCoord);
            }

            openSet.Remove(currentCoord);
            closedSet.Add(currentCoord);

            currentEdges = graphEdges.FindAll(x => x.FromVertex.Equals(currentCoord));

            string debugString = "";

            if (CDebug.pathing.Enabled) { debugString += "Number of current edges = " + currentEdges.Count + " | "; }

            foreach (GraphEdge edge in currentEdges)
            {
                if (CDebug.pathing.Enabled) { debugString += "Neighbour " + edge.ToVertex + ".."; }

                if (!closedSet.Contains(edge.ToVertex))
                {

                    if (CDebug.pathing.Enabled) { debugString += "..was not in closed set.."; }

                    tentativeGScore = hexes[currentCoord].pathingGScore + 2 * MapRenderer.HEX_OFFSET;
                    if (!openSet.Contains(edge.ToVertex))
                    {
                        if (CDebug.pathing.Enabled) { debugString += "..or open set.."; }
                        openSet.Add(edge.ToVertex);
                    }
                    else
                    {
                        if (CDebug.pathing.Enabled) { debugString += "...but was in open set..."; }
                    }
                    if (tentativeGScore < hexes[edge.ToVertex].pathingGScore)
                    {
                        if (CDebug.pathing.Enabled) { debugString += "..tentative GScore " + tentativeGScore + " was below previous GScore of " + hexes[edge.ToVertex].pathingGScore + ".."; }
                        hexes[edge.ToVertex].pathingGScore = tentativeGScore;
                        hexes[edge.ToVertex].pathingCameFrom = currentCoord;
                        hexes[edge.ToVertex].pathingFScore = tentativeGScore + HeuristicDistance(edge.ToVertex, toCoord);
                        if (CDebug.pathing.Enabled) { debugString += "..set Came From to (" + hexes[edge.ToVertex].pathingCameFrom.X + ", " + hexes[edge.ToVertex].pathingCameFrom.Y + ") and FScore to " + hexes[edge.ToVertex].pathingFScore + "."; }
                    }
                }
                else
                {
                    if (CDebug.pathing.Enabled) { debugString += "..was in closed set."; }
                }
            }

            CDebug.Log(CDebug.pathing, debugString);
        }

        return null;

    }

    private static List<Coord> ReconstructPath(Dictionary<Coord, HexData> hexes, Coord fromCoord, Coord toCoord)
    {
        List<Coord> path = new List<Coord>();

        Coord currentCoord = toCoord;

        while (!currentCoord.Equals(fromCoord))
        {
            CDebug.Log(CDebug.pathing, "Adding " + currentCoord + " to path");
            path.Add(currentCoord);
            currentCoord = hexes[currentCoord].pathingCameFrom;
        }

        CDebug.Log(CDebug.pathing, "Adding " + fromCoord + " to path");
        path.Add(fromCoord);

        return path;
    }

    private static float HeuristicDistance(Coord fromCoord, Coord toCoord)
    {
        Vector3 fromCoordPosition = fromCoord.ToPositionVector();
        Vector3 toCoordPosition = toCoord.ToPositionVector();

        string debugString = "";

        if (CDebug.pathing.Enabled) { debugString += "Calculating a heuristic distance between " + fromCoord + " and " + toCoord + " | "; }
        if (CDebug.pathing.Enabled) { debugString += "Positions: " + fromCoordPosition + " and " + toCoordPosition; }

        if (CDebug.pathing.Enabled) { debugString += "Difference: " + (fromCoordPosition - toCoordPosition); }

        if (CDebug.pathing.Enabled) { debugString += "Magnitude: " + Vector3.Magnitude(fromCoordPosition - toCoordPosition); }

        CDebug.Log(CDebug.pathing, debugString);

        return Vector3.Magnitude(fromCoordPosition - toCoordPosition);
    }
}
