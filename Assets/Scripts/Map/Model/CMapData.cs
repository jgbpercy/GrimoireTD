﻿using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using GrimoireTD.DefendingEntities;
using GrimoireTD.DefendingEntities.Structures;
using GrimoireTD.DefendingEntities.Units;
using GrimoireTD.ChannelDebug;
using GrimoireTD.Levels;
using GrimoireTD.UI;

namespace GrimoireTD.Map
{
    public class CMapData : IMapData
    {
        //Private fields
        private Dictionary<Coord, IHexData> hexes;

        private Dictionary<IDefendingEntity, Coord> defendingEntityPositions;

        public int Width { get; private set; }
        public int Height { get; private set; }

        private List<Coord> _path;

        private List<Coord> disallowedCoords;

        private Action OnPathGeneratedOrChangedCallback;

        private Action OnMapCreatedCallback;

        private Action<IUnit, Coord> OnUnitCreatedCallback;
        private Action<IStructure, Coord> OnStructureCreatedCallback;

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
                CDebug.Log(CDebug.pathing, "The _path was changed");
                _path = value;

                OnPathGeneratedOrChangedCallback?.Invoke();

                CDebug.Log(CDebug.pathing, "The Path property called update disallowed coords");
                disallowedCoords = PathingService.DisallowedCoords(_path, this, hexes, new Coord(0, 0), new Coord(Width - 1, Height - 1));

            }
        }

        //Constructor
        public CMapData()
        {
            hexes = new Dictionary<Coord, IHexData>();
            defendingEntityPositions = new Dictionary<IDefendingEntity, Coord>();
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
            InterfaceController.Instance.RegisterForOnBuildStructureUserAction(BuildStructure);
            InterfaceController.Instance.RegisterForOnCreateUnitUserAction(CreateUnit);

            //Public Hex List
            HexTypes = colorsToTypesDictionary.Values.ToList();

            //Generate Map
            CDebug.Log(CDebug.mapGeneration, "Call to MapData set up with image");

            Width = mapImage.width / 2;
            Height = mapImage.height / 2;

            CDebug.Log(CDebug.mapGeneration, "Map dimensions: (" + Width + ", " + Height + ")");

            Color32[] allPixels = mapImage.GetPixels32();
            int xPixelCoord;
            int yPixelCoord;

            CDebug.Log(CDebug.mapGeneration, "Pixels in array " + allPixels.GetLength(0));

            IHexType currentHexType;
            bool foundHexType;

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    xPixelCoord = y % 2 == 0 ? 2 * x : 2 * x + 1;
                    yPixelCoord = 2 * y;

                    CDebug.Log(CDebug.mapGeneration, "Setting tile (" + x + ", " + y + ") from pixel (" + xPixelCoord + ", " + yPixelCoord + ")");

                    CDebug.Log(CDebug.mapGeneration, "Pixel Color: " + allPixels[xPixelCoord + yPixelCoord * Width * 2]);

                    foundHexType = colorsToTypesDictionary.TryGetValue(allPixels[xPixelCoord + yPixelCoord * Width * 2], out currentHexType);
                    if (!foundHexType)
                    {
                        CDebug.Log(CDebug.mapGeneration, "didn't find hex for coord: " + x + ", " + y);
                        CDebug.Log(CDebug.mapGeneration, "R:" + allPixels[xPixelCoord + yPixelCoord * Width * 2].r);
                        CDebug.Log(CDebug.mapGeneration, "G:" + allPixels[xPixelCoord + yPixelCoord * Width * 2].g);
                        CDebug.Log(CDebug.mapGeneration, "B:" + allPixels[xPixelCoord + yPixelCoord * Width * 2].b);
                        CDebug.Log(CDebug.mapGeneration, "A:" + allPixels[xPixelCoord + yPixelCoord * Width * 2].a);
                    }

                    Assert.IsTrue(foundHexType);

                    hexes.Add(new Coord(x, y), new CHexData(currentHexType));
                }
            }

            OnMapCreatedCallback?.Invoke();

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
            CDebug.Log(CDebug.pathing, "TempRegeneratePath called the Pathing service");
            Path = PathingService.GeneratePath(this, hexes, new Coord(0, 0), new Coord(Width - 1, Height - 1));

            if (CDebug.pathing.Enabled)
            {
                string debugString = "";
                _path.ForEach(x => debugString += x + " ");
                CDebug.Log(CDebug.pathing, "The path is now: " + debugString);
            }
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
                CDebug.Log(CDebug.pathing, "CanBuildStructureAt found " + coord + " in disallowed coords");
                return false;
            }

            return true;
        }

        private void BuildStructure(Coord coord, IStructureTemplate structureTemplate)
        {
            if (!CanBuildStructureAt(coord))
            {
                throw new Exception("MapData attempted to build a structure where one couldn't be built. Some code is not checking properly if a structure can be built.");
            }

            IStructure newStructure = structureTemplate.GenerateStructure(coord);

            GetHexAt(coord).AddStructureHere(newStructure);

            defendingEntityPositions.Add(newStructure, coord);

            if (CreepPath.Contains(coord))
            {
                CDebug.Log(CDebug.pathing, "TryBuildStructureAt found coord " + coord + " in Path and called TempRegeneratePath");
                TempRegeneratePath();
            }
            else
            {
                TempRegenerateDisallowedCoords();
            }

            OnStructureCreatedCallback?.Invoke(newStructure, coord);
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
                CDebug.Log(CDebug.pathing, "CanCreateUnitAt found " + coord + " in disallowed coords");
                return false;
            }

            return true;
        }

        private void CreateUnit(Coord targetCoord, IUnitTemplate unitTemplate)
        {
            if (!CanCreateUnitAt(targetCoord))
            {
                throw new Exception("MapData attempted to create a unit where one couldn't be created. Some code is not checking properly if a unit can be created.");
            }

            IUnit newUnit = unitTemplate.GenerateUnit(targetCoord);

            GetHexAt(targetCoord).PlaceUnitHere(newUnit);

            defendingEntityPositions.Add(newUnit, targetCoord);

            if (CreepPath.Contains(targetCoord))
            {
                TempRegeneratePath();
            }
            else
            {
                TempRegenerateDisallowedCoords();
            }

            newUnit.RegisterForOnMovedCallback(coord => MoveUnitTo(coord, newUnit, newUnit.CachedDisallowedMovementDestinations));

            OnUnitCreatedCallback?.Invoke(newUnit, targetCoord);
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
                CDebug.Log(CDebug.pathing, "CanMoveUnitTo found " + targetCoord + " in disallowed coords for move");
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
            GetHexAt(defendingEntityPositions[unit]).RemoveUnitHere();

            defendingEntityPositions.Remove(unit);
            defendingEntityPositions.Add(unit, targetCoord);

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

            CDebug.Log(CDebug.distanceCalculations, "Ver: " + baseVerticalDistance);
            CDebug.Log(CDebug.distanceCalculations, "Hor: " + baseHorizontalDistance);

            return baseVerticalDistance + Mathf.Max(baseHorizontalDistance - (baseVerticalDistance / 2), 0);
        }

        public List<Coord> CoordsInRange(int range, Coord startHex)
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

        public Coord WhereAmI(IDefendingEntity defendingEntity)
        {
            return defendingEntityPositions[defendingEntity];
        }

        public List<Coord> GetDisallowedCoordsAfterUnitMove(Coord fromCoord)
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

        //Callbacks


        public void RegisterForOnPathGeneratedOrChangedCallback(Action callback)
        {
            OnPathGeneratedOrChangedCallback += callback;
        }

        public void DeregisterForOnPathGeneratedOrChangedCallback(Action callback)
        {
            OnPathGeneratedOrChangedCallback -= callback;
        }

        public void RegisterForOnMapCreatedCallback(Action callback)
        {
            OnMapCreatedCallback += callback;
        }

        public void DeregisterForOnMapCreatedCallback(Action callback)
        {
            OnMapCreatedCallback -= callback;
        }

        public void RegisterForOnUnitCreatedCallback(Action<IUnit, Coord> callback)
        {
            OnUnitCreatedCallback += callback;
        }

        public void DeregisterForOnUnitCreatedCallback(Action<IUnit, Coord> callback)
        {
            OnUnitCreatedCallback -= callback;
        }

        public void RegisterForOnStructureCreatedCallback(Action<IStructure, Coord> callback)
        {
            OnStructureCreatedCallback += callback;
        }

        public void DeregisterForOnStructureCreatedCallback(Action<IStructure, Coord> callback)
        {
            OnStructureCreatedCallback -= callback;
        }
    }
}