using System.Collections.Generic;
using UnityEngine;

namespace GrimoireTD.Map
{
    public static class PathingService
    {
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

        public static List<Coord> GeneratePath(IMapData map, Dictionary<Coord, IHexData> hexes, Coord start, Coord end)
        {
            return GeneratePath(map, hexes, start, end, new List<Coord>());
        }

        public static List<Coord> GeneratePath(IMapData map, Dictionary<Coord, IHexData> hexes, Coord start, Coord end, List<Coord> unpathableCoords)
        {
            return GeneratePath(map, hexes, start, end, unpathableCoords, new List<Coord>());
        }

        public static List<Coord> GeneratePath(IMapData map, Dictionary<Coord, IHexData> hexes, Coord start, Coord end, List<Coord> unpathableCoords, List<Coord> newlyPathableCoords)
        {
            List<GraphEdge> graphEdges = GraphEdges(map, hexes, unpathableCoords, newlyPathableCoords);

            return CalculatePath(hexes, start, end, graphEdges);
        }

        public static List<Coord> DisallowedCoords(IReadOnlyList<Coord> path, IMapData map, Dictionary<Coord, IHexData> hexes, Coord start, Coord end)
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
        public static List<Coord> DisallowedCoords(IReadOnlyList<Coord> path, IMapData map, Dictionary<Coord, IHexData> hexes, Coord start, Coord end, List<Coord> newlyPathableCoords)
        {
            List<Coord> disallowedList = new List<Coord>();

            foreach (Coord coord in path)
            {
                if (GeneratePath(map, hexes, start, end, new List<Coord> { coord }, newlyPathableCoords) == null)
                {
                    disallowedList.Add(coord);
                }
            }

            return disallowedList;
        }

        private static List<GraphEdge> GraphEdges(IMapData map, Dictionary<Coord, IHexData> hexes, List<Coord> newlyUnpathableCoords, List<Coord> newlyPathableCoords)
        {
            List<GraphEdge> graphEdges = new List<GraphEdge>();

            List<Coord> currentNeighbours = new List<Coord>();
            Coord currentCoord;
            IHexData currentHex;

            foreach (KeyValuePair<Coord, IHexData> hex in hexes)
            {
                currentCoord = hex.Key;
                currentHex = hex.Value;

                if (IsPathable(currentCoord, currentHex, newlyUnpathableCoords, newlyPathableCoords))
                {
                    currentNeighbours = map.GetNeighboursOf(currentCoord);

                    foreach (Coord neighbour in currentNeighbours)
                    {
                        if (map.GetHexAt(neighbour) != null && IsPathable(neighbour, map.GetHexAt(neighbour), newlyUnpathableCoords, newlyPathableCoords))
                        {
                            graphEdges.Add(new GraphEdge(currentCoord, neighbour));

                        }
                    }
                }
            }

            return graphEdges;
        }

        private static bool IsPathable(Coord coord, IHexData hex, List<Coord> newlyUnpathableCoords, List<Coord> newlyPathableCoords)
        {
            //bool debugThis = false;

            if (hex.IsPathableByCreeps() && !newlyUnpathableCoords.Contains(coord))
            {
                return true;
            }

            if (newlyPathableCoords.Contains(coord))
            {
                return true;
            }

            return false;
        }

        private static List<Coord> CalculatePath(Dictionary<Coord, IHexData> hexes, Coord fromCoord, Coord toCoord, List<GraphEdge> graphEdges)
        {
            List<Coord> closedSet = new List<Coord>();
            List<Coord> openSet = new List<Coord>();

            List<GraphEdge> currentEdges = new List<GraphEdge>();

            Coord currentCoord;

            float tentativeGScore;

            foreach (KeyValuePair<Coord, IHexData> hex in hexes)
            {
                hex.Value.ResetPathingData();
            }

            openSet.Add(fromCoord);
            hexes[fromCoord].pathingGScore = 0f;
            hexes[fromCoord].pathingFScore = HeuristicDistance(fromCoord, toCoord);

            while (openSet.Count != 0)
            {
                currentCoord = openSet[0];
                foreach (Coord openSetMember in openSet)
                {
                    if (hexes[openSetMember].pathingFScore < hexes[currentCoord].pathingFScore)
                    {
                        currentCoord = openSetMember;
                    }
                }

                if (currentCoord.Equals(toCoord))
                {
                    return ReconstructPath(hexes, fromCoord, toCoord);
                }

                openSet.Remove(currentCoord);
                closedSet.Add(currentCoord);

                currentEdges = graphEdges.FindAll(x => x.FromVertex.Equals(currentCoord));

                foreach (GraphEdge edge in currentEdges)
                {
                    if (!closedSet.Contains(edge.ToVertex))
                    {

                        tentativeGScore = hexes[currentCoord].pathingGScore + 2 * MapRenderer.HEX_OFFSET;

                        if (!openSet.Contains(edge.ToVertex))
                        {
                            openSet.Add(edge.ToVertex);
                        }

                        if (tentativeGScore < hexes[edge.ToVertex].pathingGScore)
                        {
                            hexes[edge.ToVertex].pathingGScore = tentativeGScore;
                            hexes[edge.ToVertex].pathingCameFrom = currentCoord;
                            hexes[edge.ToVertex].pathingFScore = tentativeGScore + HeuristicDistance(edge.ToVertex, toCoord);
                        }
                    }
                }
            }

            return null;

        }

        private static List<Coord> ReconstructPath(Dictionary<Coord, IHexData> hexes, Coord fromCoord, Coord toCoord)
        {
            List<Coord> path = new List<Coord>();

            Coord currentCoord = toCoord;

            while (!currentCoord.Equals(fromCoord))
            {
                path.Add(currentCoord);
                currentCoord = hexes[currentCoord].pathingCameFrom;
            }

            path.Add(fromCoord);

            return path;
        }

        private static float HeuristicDistance(Coord fromCoord, Coord toCoord)
        {
            return Vector3.Magnitude(fromCoord.ToPositionVector() - toCoord.ToPositionVector());
        }
    }
}