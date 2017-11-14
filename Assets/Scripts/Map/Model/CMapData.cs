using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using GrimoireTD.Defenders;
using GrimoireTD.Defenders.Structures;
using GrimoireTD.Defenders.Units;
using GrimoireTD.Levels;
using GrimoireTD.UI;

namespace GrimoireTD.Map
{
    public class CMapData : IMapData
    {
        //Private fields
        private Dictionary<Coord, IHexData> hexes;

        private Dictionary<IDefender, Coord> defenderPositions;

        public int Width { get; private set; }
        public int Height { get; private set; }

        //TODO: why have I done this? Fix?
        private List<Coord> _path;

        private List<Coord> disallowedCoords;

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

        private List<Coord> Path
        {
            set
            {
                _path = value;

                OnPathGeneratedOrChanged?.Invoke(this, new EAOnPathGeneratedOrChanged(value));

                disallowedCoords = PathingService.DisallowedCoords(_path, this, hexes, new Coord(0, 0), new Coord(Width - 1, Height - 1));
            }
        }

        //Constructor
        public CMapData()
        {
            hexes = new Dictionary<Coord, IHexData>();
            defenderPositions = new Dictionary<IDefender, Coord>();
        }

        //Set Up
        public void SetUp(
            Texture2D mapImage, 
            IDictionary<Color32, IHexType> colorsToTypesDictionary, 
            IEnumerable<IStartingStructure> startingStructures,
            IEnumerable<IStartingUnit> startingUnits
        )
        {
            //Register callbacks
            InterfaceController.Instance.OnBuildStructurePlayerAction += OnBuildStructurePlayerAction;
            InterfaceController.Instance.OnCreateUnitPlayerAction += OnCreateUnitPlayerAction;

            //Public Hex List
            HexTypes = colorsToTypesDictionary.Values.ToList();

            //Generate Map
            Width = mapImage.width / 2;
            Height = mapImage.height / 2;

            Color32[] allPixels = mapImage.GetPixels32();
            int xPixelCoord;
            int yPixelCoord;

            IHexType currentHexType;
            bool foundHexType;

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    xPixelCoord = y % 2 == 0 ? 2 * x : 2 * x + 1;
                    yPixelCoord = 2 * y;

                    foundHexType = colorsToTypesDictionary.TryGetValue(allPixels[xPixelCoord + yPixelCoord * Width * 2], out currentHexType);

                    Assert.IsTrue(foundHexType);

                    hexes.Add(new Coord(x, y), new CHexData(currentHexType));
                }
            }

            OnMapCreated?.Invoke(this, new EAOnMapCreated(this));

            TempRegeneratePath();

            foreach (IStartingStructure startingStructure in startingStructures)
            {
                if (!CanBuildStructureAt(startingStructure.StartingPosition))
                {
                    throw new Exception("Attempted to add Starting Structure at invalid position (" + startingStructure.StartingPosition.X + ", " + startingStructure.StartingPosition.Y + ")");
                }
                else
                {
                    BuildStructure(startingStructure.StartingPosition, startingStructure.StructureTemplate);
                }
            }

            foreach (IStartingUnit startingUnit in startingUnits)
            {
                if (!CanCreateUnitAt(startingUnit.StartingPosition))
                {
                    throw new Exception("Attempted to add Starting Unit at invalid position (" + startingUnit.StartingPosition.X + ", " + startingUnit.StartingPosition.Y + ")");
                }
                else
                {
                    CreateUnit(startingUnit.StartingPosition, startingUnit.UnitTemplate);
                }
            }
        }

        //Pathing
        //  until variable start/ends etc, to at least just store the hardcoded start and ends in one place
        private void TempRegeneratePath()
        {
            Path = PathingService.GeneratePath(this, hexes, new Coord(0, 0), new Coord(Width - 1, Height - 1));
        }

        private void TempRegenerateDisallowedCoords()
        {
            disallowedCoords = PathingService.DisallowedCoords(CreepPath, this, hexes, new Coord(0, 0), new Coord(Width - 1, Height - 1));
        }

        //Public Hex/Coord queries
        public IHexData TryGetHexAt(Coord hexCoord)
        {
            IHexData hexAtCoords;

            bool foundHex = hexes.TryGetValue(hexCoord, out hexAtCoords);

            if (foundHex)
            {
                return hexAtCoords;
            }

            return null;
        }

        public IHexData GetHexAt(Coord hexCoord)
        {
            return hexes[hexCoord];
        }

        public List<Coord> GetNeighboursOf(Coord hexCoord)
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

        //Build / Create / Move
        //  Optimisation: cache all of the below CanX call when entering states where this info is needed 
        //  At time of writing, will be called each frame from MouseOverMapView

        public bool CanBuildStructureAt(Coord coord)
        {
            if (!GetHexAt(coord).CanPlaceStructureHere())
            {
                return false;
            }

            if (disallowedCoords.Contains(coord))
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

            IStructure newStructure = structureTemplate.GenerateStructure(coord);

            GetHexAt(coord).AddStructureHere(newStructure);

            defenderPositions.Add(newStructure, coord);

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

        //  Unit Creation
        public bool CanCreateUnitAt(Coord coord)
        {
            if (!GetHexAt(coord).CanPlaceUnitHere())
            {
                //CDebug.Log(something)
                return false;
            }

            if (disallowedCoords.Contains(coord))
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

            IUnit newUnit = unitTemplate.GenerateUnit(targetCoord);

            GetHexAt(targetCoord).PlaceUnitHere(newUnit);

            defenderPositions.Add(newUnit, targetCoord);

            if (CreepPath.Contains(targetCoord))
            {
                TempRegeneratePath();
            }
            else
            {
                TempRegenerateDisallowedCoords();
            }

            newUnit.OnMoved += (object sender, EAOnMoved args) => MoveUnitTo(args.ToPosition, newUnit, newUnit.CachedDisallowedMovementDestinations);

            OnUnitCreated?.Invoke(this,new EAOnUnitCreated(targetCoord, newUnit));
        }

        //  Unit Movement
        public bool CanMoveUnitTo(Coord targetCoord, IReadOnlyList<Coord> disallowedCoordsForMove)
        {
            if (!GetHexAt(targetCoord).CanPlaceUnitHere())
            {
                //CDebug.Log(something)
                return false;
            }

            if (disallowedCoordsForMove.Contains(targetCoord))
            {
                return false;
            }

            return true;
        }

        private void MoveUnitTo(Coord targetCoord, IUnit unit, IReadOnlyList<Coord> disallowedCoordsForMove)
        {
            if (!CanMoveUnitTo(targetCoord, disallowedCoordsForMove))
            {
                throw new Exception("MapData attempted to move a unit where it couldn't be move. Some code is not checking properly if a unit can be moved.");
            }

            GetHexAt(targetCoord).PlaceUnitHere(unit);
            GetHexAt(defenderPositions[unit]).RemoveUnitHere();

            defenderPositions.Remove(unit);
            defenderPositions.Add(unit, targetCoord);

            TempRegeneratePath();
        }

        //Public non-changing helper methods
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

            return baseVerticalDistance + Mathf.Max(baseHorizontalDistance - (baseVerticalDistance / 2), 0);
        }

        public List<Coord> GetCoordsInRange(int range, Coord startHex)
        {
            //TODO: do this more efficiently. Find the Coords in a sensible way, not by looping through all possibles :)
            List<Coord> coordsInRange = new List<Coord>();

            Coord currentCoord;

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    currentCoord = new Coord(x, y);

                    if (HexIsInRange(range, startHex, currentCoord))
                    {
                        coordsInRange.Add(currentCoord);
                    }
                }
            }

            return coordsInRange;
        }

        public Coord WhereAmI(IDefender defender)
        {
            return defenderPositions[defender];
        }

        public List<Coord> GetDisallowedMovementDestinationCoords(Coord fromCoord)
        {
            if (GetHexAt(fromCoord).IsPathableByCreepsWithUnitRemoved())
            {
                return PathingService.DisallowedCoords(CreepPath, this, hexes, new Coord(0, 0), new Coord(Width - 1, Height - 1), new List<Coord> { fromCoord });
            }

            return disallowedCoords;
        }

        private List<Coord> GetDisallowedCoords(List<Coord> newlyPathableCoords)
        {
            return PathingService.DisallowedCoords(CreepPath, this, hexes, new Coord(0, 0), new Coord(Width - 1, Height - 1), newlyPathableCoords);
        }
    }
}