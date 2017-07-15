using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class MapData {

    //Private fields
    private Dictionary<Coord, HexData> hexes;

    private Dictionary<DefendingEntity, Coord> defendingEntityPositions = new Dictionary<DefendingEntity, Coord>();

    private int width;
    private int height;

    private List<Coord> _path;

    private List<Coord> disallowedCoords;

    private Action OnPathGeneratedOrChangedCallback;

    //Public properties
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
            return _path;
        }
        private set
        {
            CDebug.Log(CDebug.pathing, "The _path was changed");
            _path = value;

            if (OnPathGeneratedOrChangedCallback != null)
            {
                CDebug.Log(CDebug.pathing, "The callback for the _path changing was called");
                OnPathGeneratedOrChangedCallback();
            }

            CDebug.Log(CDebug.pathing, "The Path property called update disallowed coords");
            disallowedCoords = PathingService.DisallowedCoords(_path, this, hexes, new Coord(0, 0), new Coord(width - 1, height - 1));

        }
    }

    //Constructor
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

        TempRegeneratePath();
    }

    //Pathing
    //  until variable start/ends etc, to at least just store the hardcoded start and ends in one place
    private void TempRegeneratePath()
    {
        CDebug.Log(CDebug.pathing, "TempRegeneratePath called the Pathing service");
        Path = PathingService.GeneratePath(this, hexes, new Coord(0, 0), new Coord(width - 1, height - 1));

        if ( CDebug.pathing.Enabled)
        {
            string debugString = "";
            Path.ForEach(x => debugString += x + " ");
            CDebug.Log(CDebug.pathing, "The path is now: " + debugString);
        }
    }

    private void TempRegenerateDisallowedCoords()
    {
        disallowedCoords = PathingService.DisallowedCoords(Path, this, hexes, new Coord(0, 0), new Coord(width - 1, height - 1));
    }

    //Public Hex/Coord queries
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

    //Public change methods
    //  Optimisation: cache all of the below CanX call when entering states where this info is needed 
    //  At time of writing, will be called each frame from MouseOverMapView

    //  Building
    public bool CanBuildGenericStructureAt(Coord coord)
    {
        if (!GetHexAt(coord).CanPlaceStructureHere())
        {
            //CDebug.Log(something);
            return false;
        }

        if (disallowedCoords.Contains(coord))
        {
            CDebug.Log(CDebug.pathing, "CanBuildStructureAt found " + coord + " in disallowed coords");
            return false;
        }

        return true;
    }

    public bool CanBuildStructureAt(Coord coord, StructureTemplate structureTemplate, bool isFree)
    {
        if ( !CanBuildGenericStructureAt(coord) )
        {
            return false;
        }

        if (!isFree && !EconomyManager.Instance.CanDoTransaction(structureTemplate.Cost))
        {
            //CDebug.Log(something);
            return false;
        }

        return true;
    }

    public bool TryBuildStructureAt(Coord coord, StructureTemplate structureTemplate, bool isFree)
    {
        if ( !CanBuildStructureAt(coord, structureTemplate, isFree) )
        {
            return false;
        }

        Structure newStructure = structureTemplate.GenerateStructure(coord);

        GetHexAt(coord).AddStructureHere(newStructure);

        if (!isFree)
        {
            EconomyManager.Instance.DoTransaction(structureTemplate.Cost);
        }

        defendingEntityPositions.Add(newStructure, coord);

        if ( Path.Contains(coord) )
        {
            CDebug.Log(CDebug.pathing,"TryBuildStructureAt found coord " + coord + " in Path and called TempRegeneratePath");
            TempRegeneratePath();
        }
        else
        {
            TempRegenerateDisallowedCoords();
        }

        return true;
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

    public bool TryCreateUnitAt(Coord targetCoord, UnitTemplate unitTemplate)
    {
        if (!CanCreateUnitAt(targetCoord))
        {
            return false;
        }

        Unit newUnit = unitTemplate.GenerateUnit(targetCoord);

        GetHexAt(targetCoord).PlaceUnitHere(newUnit);

        defendingEntityPositions.Add(newUnit, targetCoord);

        if (Path.Contains(targetCoord))
        {
            TempRegeneratePath();
        }
        else
        {
            TempRegenerateDisallowedCoords();
        }

        return true;
    }

    //  Unit Movement
    public bool CanMoveUnitTo(Coord targetCoord, EconomyTransaction cost, List<Coord> disallowedCoordsForMove)
    {
        if (!GetHexAt(targetCoord).CanPlaceUnitHere())
        {
            //CDebug.Log(something)
            return false;
        }

            //CDebug.Log(something);
        if (!EconomyManager.Instance.CanDoTransaction(cost))
        {
            //CDebug.Log(something);
            return false;
        }

        if (disallowedCoordsForMove.Contains(targetCoord))
        {
            CDebug.Log(CDebug.pathing, "CanMoveUnitTo found " + targetCoord + " in disallowed coords for move");
            return false;
        }

        return true;
    }

    public bool TryMoveUnitTo(Coord fromCoord, Coord targetCoord, Unit unit, EconomyTransaction cost, List<Coord> disallowedCoordsForMove)
    {
        if ( !CanMoveUnitTo(targetCoord, cost, disallowedCoordsForMove) )
        {
            return false;
        }

        GetHexAt(targetCoord).PlaceUnitHere(unit);
        GetHexAt(fromCoord).RemoveUnitHere();

        defendingEntityPositions.Remove(unit);
        defendingEntityPositions.Add(unit, targetCoord);

        EconomyManager.Instance.DoTransaction(cost);

        TempRegeneratePath();

        return true;
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

    public static List<Coord> CoordsInRange(int range, Coord startHex)
    {
        //TODO: do this more efficiently. Find the Coords in a sensible way, not by looping through all possibles :)
        List<Coord> coordsInRange = new List<Coord>();

        Coord currentCoord;

        for (int x = 0; x < MapGenerator.Instance.Map.width; x++)
        {
            for (int y = 0; y < MapGenerator.Instance.Map.height; y++)
            {
                currentCoord = new Coord(x, y);

                if ( HexIsInRange(range, startHex, currentCoord))
                {
                    coordsInRange.Add(currentCoord);
                }
            }
        }

        return coordsInRange;
    }

    public Coord WhereAmI(DefendingEntity defendingEntity)
    {
        return defendingEntityPositions[defendingEntity];
    }

    public List<Coord> GetDisallowedCoordsAfterUnitMove(Coord fromCoord)
    {
        if ( GetHexAt(fromCoord).IsPathableByCreepsWithUnitRemoved() )
        {
            return PathingService.DisallowedCoords(Path, this, hexes, new Coord(0, 0), new Coord(width - 1, height - 1), new List<Coord> { fromCoord });
        }

        return disallowedCoords;
    }

    private List<Coord> GetDisallowedCoords(List<Coord> newlyPathableCoords)
    {
        return PathingService.DisallowedCoords(Path, this, hexes, new Coord(0, 0), new Coord(width - 1, height - 1), newlyPathableCoords);
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
}
