using System;
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

        private class PathingData
        {
            public float FScore = Mathf.Infinity;
            public float GScore = Mathf.Infinity;
            public Coord CameFrom = null;
        }

        public static Func<
            Dictionary<Coord, IHexData>, 
            Coord, 
            Coord, 
            List<Coord>> 
        GetPath = (hexes, start, end) =>
        {
            return GetPathWithUnpathableList(hexes, start, end, new List<Coord>());
        };

        public static Func<
            Dictionary<Coord, IHexData>, 
            Coord, 
            Coord, 
            List<Coord>, 
            List<Coord>>
        GetPathWithUnpathableList = (hexes, start, end, unpathableCoords) =>
        {
            return GetPathWithUnpathableAndNewlyPathableLists(hexes, start, end, unpathableCoords, new List<Coord>());
        };

        public static Func<
            Dictionary<Coord, IHexData>,
            Coord,
            Coord,
            List<Coord>,
            List<Coord>,
            List<Coord>>
        GetPathWithUnpathableAndNewlyPathableLists = (hexes, start, end, unpathableCoords, newlyPathableCoords) =>
        {
            var graphEdges = GraphEdges(hexes, unpathableCoords, newlyPathableCoords);

            return CalculatePath(start, end, hexes.Keys, graphEdges);
        };

        /*  Obviously GetDisallowed Coords doesn't really need the path, it could recalc it from the other args, but for now
         *  anything calling this (tests aside) will have the path anyway
         */
        public static Func<
            IReadOnlyList<Coord>,
            Dictionary<Coord, IHexData>,
            Coord,
            Coord,
            List<Coord>>
        GetDisallowedCoords = (path, hexes, start, end) =>
        {
            return GetDisallowedCoordsWithNewlyPathableCoords(path, hexes, start, end, new List<Coord>());
        };

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
        public static Func<
            IReadOnlyList<Coord>,
            Dictionary<Coord, IHexData>,
            Coord,
            Coord,
            List<Coord>,
            List<Coord>>
        GetDisallowedCoordsWithNewlyPathableCoords = (path, hexes, start, end, newlyPathableCoords) =>
        {
            var disallowedList = new List<Coord>();

            foreach (var coord in path)
            {
                if (GetPathWithUnpathableAndNewlyPathableLists(hexes, start, end, new List<Coord> { coord }, newlyPathableCoords) == null)
                {
                    disallowedList.Add(coord);
                }
            }

            return disallowedList;
        };

        private static List<GraphEdge> GraphEdges(Dictionary<Coord, IHexData> hexes, List<Coord> newlyUnpathableCoords, List<Coord> newlyPathableCoords)
        {
            var graphEdges = new List<GraphEdge>();

            var currentNeighbours = new List<Coord>();
            Coord currentCoord;
            IHexData currentHex;

            foreach (var hex in hexes)
            {
                currentCoord = hex.Key;
                currentHex = hex.Value;

                if (IsPathable(currentCoord, currentHex, newlyUnpathableCoords, newlyPathableCoords))
                {
                    currentNeighbours = CMapData.GetUncheckedNeighboursOf(currentCoord);

                    foreach (var neighbour in currentNeighbours)
                    {
                        if (hexes.ContainsKey(neighbour) && IsPathable(neighbour, hexes[neighbour], newlyUnpathableCoords, newlyPathableCoords))
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

        private static List<Coord> CalculatePath(Coord fromCoord, Coord toCoord, IEnumerable<Coord> coords, List<GraphEdge> graphEdges)
        {
            var pathingData = new Dictionary<Coord, PathingData>();

            var closedSet = new List<Coord>();
            var openSet = new List<Coord>();

            var currentEdges = new List<GraphEdge>();

            Coord currentCoord;

            float tentativeGScore;

            foreach (var coord in coords)
            {
                pathingData.Add(coord, new PathingData());
            }

            openSet.Add(fromCoord);
            pathingData[fromCoord].GScore = 0f;
            pathingData[fromCoord].FScore = HeuristicDistance(fromCoord, toCoord);

            while (openSet.Count != 0)
            {
                currentCoord = openSet[0];
                foreach (var openSetMember in openSet)
                {
                    if (pathingData[openSetMember].FScore < pathingData[currentCoord].FScore)
                    {
                        currentCoord = openSetMember;
                    }
                }

                if (currentCoord.Equals(toCoord))
                {
                    return ReconstructPath(pathingData, fromCoord, toCoord);
                }

                openSet.Remove(currentCoord);
                closedSet.Add(currentCoord);

                currentEdges = graphEdges.FindAll(x => x.FromVertex.Equals(currentCoord));

                foreach (var edge in currentEdges)
                {
                    if (!closedSet.Contains(edge.ToVertex))
                    {
                        tentativeGScore = pathingData[currentCoord].GScore + 2 * MapRenderer.HEX_OFFSET;

                        if (!openSet.Contains(edge.ToVertex))
                        {
                            openSet.Add(edge.ToVertex);
                        }

                        if (tentativeGScore < pathingData[edge.ToVertex].GScore)
                        {
                            pathingData[edge.ToVertex].GScore = tentativeGScore;
                            pathingData[edge.ToVertex].CameFrom = currentCoord;
                            pathingData[edge.ToVertex].FScore = tentativeGScore + HeuristicDistance(edge.ToVertex, toCoord);
                        }
                    }
                }
            }

            return null;
        }

        private static List<Coord> ReconstructPath(Dictionary<Coord, PathingData> pathingData, Coord fromCoord, Coord toCoord)
        {
            var path = new List<Coord>();

            var currentCoord = toCoord;

            while (!currentCoord.Equals(fromCoord))
            {
                path.Add(currentCoord);
                currentCoord = pathingData[currentCoord].CameFrom;
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