using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using GrimoireTD.Defenders.Structures;
using GrimoireTD.Defenders.Units;
using GrimoireTD.Levels;
using GrimoireTD.UI;
using GrimoireTD.Dependencies;

namespace GrimoireTD.Map
{
    public class CMapData : IMapData
    {
        //Private fields
        private Dictionary<Coord, IHexData> hexes;

        public int Width { get; private set; }
        public int Height { get; private set; }

        private List<Coord> _path;

        private List<Coord> disallowedCoordsForUnitOrStructurePlacement;

        public event EventHandler<EAOnMapCreated> OnMapCreated;
        public event EventHandler<EAOnPathGeneratedOrChanged> OnPathGeneratedOrChanged;

        public event EventHandler<EAOnUnitCreated> OnUnitCreated;
        public event EventHandler<EAOnStructureCreated> OnStructureCreated;

        public IReadOnlyList<IHexType> HexTypes { get; private set; }

        //Properties
        public IReadOnlyList<Coord> CreepPath
        {
            get
            {
                return _path;
            }
        }

        private List<Coord> path
        {
            set
            {
                _path = value;

                disallowedCoordsForUnitOrStructurePlacement = PathingService.GetDisallowedCoords(_path, hexes, new Coord(0, 0), new Coord(Width - 1, Height - 1));

                OnPathGeneratedOrChanged?.Invoke(this, new EAOnPathGeneratedOrChanged(value));
            }
        }

        //Constructor
        public CMapData() { }

        //Set Up
        public void SetUp(
            Texture2D mapImage, 
            IDictionary<Color32, IHexType> colorsToTypesDictionary, 
            IEnumerable<IStartingStructure> startingStructures,
            IEnumerable<IStartingUnit> startingUnits
        )
        {
            //Register callbacks
            DepsProv.TheInterfaceController().OnBuildStructurePlayerAction += OnBuildStructurePlayerAction;
            DepsProv.TheInterfaceController().OnCreateUnitPlayerAction += OnCreateUnitPlayerAction;

            //Public Hex Type List
            HexTypes = colorsToTypesDictionary.Values.ToList();

            //Generate Map
            Width = mapImage.width / 2;
            Height = mapImage.height / 2;

            hexes = MapImageToMapDictionaryService.GetMapDictionary(mapImage, colorsToTypesDictionary);

            OnMapCreated?.Invoke(this, new EAOnMapCreated(this));

            TempRegeneratePath();

            //Starting Structures and Units
            foreach (var startingStructure in startingStructures)
            {
                BuildStructure(startingStructure.StartingPosition, startingStructure.StructureTemplate);
            }

            foreach (var startingUnit in startingUnits)
            {
                CreateUnit(startingUnit.StartingPosition, startingUnit.UnitTemplate);
            }
        }

        //Pathing
        //  until variable start/ends etc, to at least just store the hardcoded start and ends in one place
        private void TempRegeneratePath()
        {
            path = PathingService.GetPath(hexes, new Coord(0, 0), new Coord(Width - 1, Height - 1));
        }

        private void TempRegenerateDisallowedCoords()
        {
            disallowedCoordsForUnitOrStructurePlacement = PathingService.GetDisallowedCoords(CreepPath, hexes, new Coord(0, 0), new Coord(Width - 1, Height - 1));
        }

        //Public Hex/Coord queries
        public IHexData TryGetHexAt(Coord hexCoord)
        {
            IHexData hexAtCoord;

            var foundHex = hexes.TryGetValue(hexCoord, out hexAtCoord);

            if (foundHex)
            {
                return hexAtCoord;
            }

            return null;
        }

        public IHexData GetHexAt(Coord hexCoord)
        {
            return hexes[hexCoord];
        }

        /* TODO? #optimisation? A better versions of this / GetCoordsInRange with:
         * - a static unchecked version that gets possible coords in range (that should probably just be a Coord method)
         * - an extant version that gets all coords actually on the map in range
         */    
        public List<Coord> GetExtantNeighboursOf(Coord coord)
        {
            var neighbourList = new List<Coord>();

            var x = coord.X;
            var y = coord.Y;

            var xOffset = y % 2 == 0 ? -1 : 0;

            var upLeft = new Coord(x + xOffset, y + 1);
            if (TryGetHexAt(upLeft) != null)
            {
                neighbourList.Add(upLeft);
            }

            var upRight = new Coord(x + xOffset + 1, y + 1);
            if (TryGetHexAt(upRight) != null)
            {
                neighbourList.Add(upRight);
            }

            var xRight = new Coord(x + 1, y);
            if (TryGetHexAt(xRight) != null)
            {
                neighbourList.Add(xRight);
            }

            var downRight = new Coord(x + xOffset + 1, y - 1);
            if (TryGetHexAt(downRight) != null)
            {
                neighbourList.Add(downRight);
            }

            var downLeft = new Coord(x + xOffset, y - 1);
            if (TryGetHexAt(downLeft) != null)
            {
                neighbourList.Add(downLeft);
            }

            var xLeft = new Coord(x - 1, y);
            if (TryGetHexAt(xLeft) != null)
            {
                neighbourList.Add(xLeft);
            }

            return neighbourList;
        }

        public static List<Coord> GetUncheckedNeighboursOf(Coord coord)
        {
            var xOffset = coord.Y % 2 == 0 ? -1 : 0;

            return new List<Coord>
            {
                new Coord(coord.X + xOffset, coord.Y + 1), //up left
                new Coord(coord.X + xOffset + 1, coord.Y + 1), //up right
                new Coord(coord.X + 1, coord.Y), //right
                new Coord(coord.X + xOffset + 1, coord.Y - 1), //down right
                new Coord(coord.X + xOffset, coord.Y - 1), //down left
                new Coord(coord.X - 1, coord.Y), //left
            };
        }

        //Build / Create / Move
        //  #optimisation: cache all of the below CanX call when entering states where this info is needed 
        //  At time of writing, will be called each frame from MouseOverMapView

        public bool CanBuildStructureAt(Coord coord)
        {
            if (!GetHexAt(coord).CanBuildStructureHere() || disallowedCoordsForUnitOrStructurePlacement.Contains(coord))
            {
                return false;
            }
            
            return true;
        }

        private void OnBuildStructurePlayerAction(object sender, EAOnBuildStructurePlayerAction args)
        {
            BuildStructure(args.Position, args.StructureTemplate);
        }

        private void BuildStructure(Coord coord, IStructureTemplate structureTemplate)
        {
            if (!CanBuildStructureAt(coord))
            {
                throw new Exception("MapData attempted to build a structure where one couldn't be built. Some code is not checking properly if a structure can be built.");
            }

            var newStructure = structureTemplate.GenerateStructure(coord);

            GetHexAt(coord).BuildStructureHere(newStructure);

            if (CreepPath.Contains(coord))
            {
                TempRegeneratePath();
            }
            else
            {
                TempRegenerateDisallowedCoords();
            }

            OnStructureCreated?.Invoke(this, new EAOnStructureCreated(coord, newStructure));
        }

        // Unit Creation
        public bool CanCreateUnitAt(Coord coord)
        {
            if (!GetHexAt(coord).CanPlaceUnitHere() || disallowedCoordsForUnitOrStructurePlacement.Contains(coord))
            {
                return false;
            }

            return true;
        }

        private void OnCreateUnitPlayerAction(object sender, EAOnCreateUnitPlayerAction args)
        {
            CreateUnit(args.Position, args.UnitTemplate);
        }

        private void CreateUnit(Coord targetCoord, IUnitTemplate unitTemplate)
        {
            if (!CanCreateUnitAt(targetCoord))
            {
                throw new Exception("MapData attempted to create a unit where one couldn't be created. Some code is not checking properly if a unit can be created.");
            }

            var newUnit = unitTemplate.GenerateUnit(targetCoord);

            GetHexAt(targetCoord).PlaceUnitHere(newUnit);

            if (CreepPath.Contains(targetCoord))
            {
                TempRegeneratePath();
            }
            else
            {
                TempRegenerateDisallowedCoords();
            }

            newUnit.OnMoved += OnUnitMoved;

            OnUnitCreated?.Invoke(this, new EAOnUnitCreated(targetCoord, newUnit));
        }

        // Unit Movement
        public bool CanMoveUnitTo(Coord targetCoord, IReadOnlyList<Coord> disallowedCoordsForMove)
        {
            if (!GetHexAt(targetCoord).CanPlaceUnitHere())
            {
                return false;
            }

            if (disallowedCoordsForMove.Contains(targetCoord))
            {
                return false;
            }

            return true;
        }

        private void OnUnitMoved(object sender, EAOnMoved args)
        {
            if (!CanMoveUnitTo(args.ToPosition, args.Unit.CachedDisallowedMovementDestinations))
            {
                throw new Exception("MapData attempted to move a unit where it couldn't be move. Some code is not checking properly if a unit can be moved.");
            }

            GetHexAt(args.ToPosition).PlaceUnitHere(args.Unit);
            GetHexAt(args.OldPosition).RemoveUnitHere();

            TempRegeneratePath();
        }

        //Public non-changing helper methods
        public static bool CoordIsInRange(int range, Coord startHex, Coord targetHex)
        {
            return Distance(startHex, targetHex) <= range;
        }

        public static int Distance(Coord startHex, Coord targetHex)
        {
            var baseVerticalDistance = Mathf.Abs(startHex.Y - targetHex.Y);
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

            return baseVerticalDistance + Mathf.Max(baseHorizontalDistance - (baseVerticalDistance / 2), 0);
        }

        public List<Coord> GetCoordsInRange(int range, Coord startHex)
        {
            //TODO: #optimisation do this more efficiently. Find the Coords in a sensible way, not by looping through all possibles :)
            var coordsInRange = new List<Coord>();

            for (var x = 0; x < Width; x++)
            {
                for (var y = 0; y < Height; y++)
                {
                    var currentCoord = new Coord(x, y);

                    if (CoordIsInRange(range, startHex, currentCoord))
                    {
                        coordsInRange.Add(currentCoord);
                    }
                }
            }

            return coordsInRange;
        }

        public List<Coord> GetDisallowedMovementDestinationCoords(Coord fromCoord)
        {
            if (GetHexAt(fromCoord).IsPathableByCreepsWithUnitRemoved())
            {
                return PathingService.GetDisallowedCoordsWithNewlyPathableCoords(CreepPath, hexes, new Coord(0, 0), new Coord(Width - 1, Height - 1), new List<Coord> { fromCoord });
            }

            return disallowedCoordsForUnitOrStructurePlacement;
        }

        private List<Coord> GetDisallowedCoords(List<Coord> newlyPathableCoords)
        {
            return PathingService.GetDisallowedCoordsWithNewlyPathableCoords(CreepPath, hexes, new Coord(0, 0), new Coord(Width - 1, Height - 1), newlyPathableCoords);
        }
    }
}