using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;
using System.Collections;

public enum InterfaceCursorMode
{
    BUILD,
    SELECT,
    EXECUTE_BUILD_MODE_ABILITY
}

public class InterfaceController : SingletonMonobehaviour<InterfaceController> {

    [SerializeField]
    private LayerMask tileMapLayer;

    private InterfaceCursorMode cursorMode = InterfaceCursorMode.SELECT;

    private StructureTemplate selectedStructureTemplate;

    private Structure selectedStructureInstance = null;

    private Unit selectedUnitInstance = null;

    private HexTargetedBuildModeAbility hexTargetedAbilityToActivate = null;

    [SerializeField]
    private float maxRayFromCameraDistance = 100f;

    private Ray cameraToMouseRay;
    private RaycastHit mouseHitAgainstTileMap;
    private bool mouseRaycastHitMapBool;

    private Coord mouseOverCoord;
    private HexData mouseOverHex;

    private Camera mainCamera;

    [SerializeField]
    private StructureTemplate[] structureTemplates;

    private MapData map;
    private GameStateManager gameStateManager;

    private Action<Structure, Unit> OnDefendingEntitySelectedCallback;
    private Action OnDefendingEntityDeselectedCallback;

    private Action<StructureTemplate> OnStructureToBuildSelectedCallback;

    public StructureTemplate[] StructureTemplates
    {
        get
        {
            return structureTemplates;
        }
    }

    public bool MouseRaycastHitMap
    {
        get
        {
            return mouseRaycastHitMapBool;
        }
    }

    public InterfaceCursorMode CurrentCursorMode
    {
        get
        {
            return cursorMode;
        }
    }

    public HexData MouseOverHex
    {
        get
        {
            return mouseOverHex;
        }
    }

    public Coord MouseOverCoord
    {
        get
        {
            return mouseOverCoord;
        }
    }

    public HexTargetedBuildModeAbility HexTargetedAbilityToActivate
    {
        get
        {
            return hexTargetedAbilityToActivate;
        }
    }

    public Unit SelectedUnitInstance
    {
        get
        {
            return selectedUnitInstance;
        }
    }

    public Coord SelectedUnitLocation
    {
        get
        {
            return map.WhereAmI(selectedUnitInstance);
        }
    }

    public Structure SelectedStructureInstance
    {
        get
        {
            return selectedStructureInstance;
        }
    }

    private void Start()
    {
        CDebug.Log(CDebug.applicationLoading, "Interface Controller Start");

        //map
        map = MapGenerator.Instance.Map;

        //gamestate
        gameStateManager = GameStateManager.Instance;

        //camera
        mainCamera = Camera.main;

        //default tower to build
        SelectStructureToBuild(0);
        SetCursorModeSelect();

        gameStateManager.RegisterForOnEnterBuildModeCallback(OnEnterBuildMode);
        gameStateManager.RegisterForOnEnterDefendModeCallback(OnEnterDefendMode);

    }

    void Update() {

        if (!EventSystem.current.IsPointerOverGameObject())
        {
            cameraToMouseRay = mainCamera.ScreenPointToRay(Input.mousePosition);
            mouseRaycastHitMapBool = Physics.Raycast(cameraToMouseRay, out mouseHitAgainstTileMap, maxRayFromCameraDistance, tileMapLayer);
            mouseOverCoord = Coord.PositionVectorToCoord(new Vector3(mouseHitAgainstTileMap.point.x, mouseHitAgainstTileMap.point.y, 0f));
            mouseOverHex = map.GetHexAt(mouseOverCoord);
        }
        else
        {
            mouseRaycastHitMapBool = false;
        }

        if (Input.GetMouseButtonDown(0))
        {
            HandleMouseClick();
        }

        if (Input.GetMouseButtonDown(1))
        {
            SetCursorModeSelect();
            DeselectStructure();
        }
    }


    private void HandleMouseClick()
    {
        if (!mouseRaycastHitMapBool)
        {
            return;
        }

        if (cursorMode == InterfaceCursorMode.SELECT && ( mouseOverHex.StructureHere != null || MouseOverHex.UnitHere != null ))
        {
            SelectHexContents(mouseOverHex);
            return;
        }
        else if ( cursorMode != InterfaceCursorMode.EXECUTE_BUILD_MODE_ABILITY )
        {
            DeselectStructure();
        }

        if (cursorMode == InterfaceCursorMode.BUILD)
        {
            map.TryBuildStructureAt(mouseOverCoord, selectedStructureTemplate);
        }

        if (cursorMode == InterfaceCursorMode.EXECUTE_BUILD_MODE_ABILITY && hexTargetedAbilityToActivate.IsTargetSuitable(map.WhereAmI(selectedUnitInstance), mouseOverCoord))
        {
            CDebug.Log(CDebug.buildModeAbilities, "InterfaceController detected hex targeted build mode ability click");
            hexTargetedAbilityToActivate.ExecuteAbility(map.WhereAmI(selectedUnitInstance), mouseOverCoord, selectedUnitInstance);
            SetCursorModeSelect();
            SelectHexContents(mouseOverHex);
        }

    }

    public void ButtonClickDefend()
    {
        CDebug.Log(CDebug.gameState, "Interface Controller registered defend button click");

        gameStateManager.CurrentGameMode = GameMode.DEFEND;
    }

    private void OnEnterBuildMode()
    {

    }

    private void OnEnterDefendMode()
    {
        SetCursorModeSelect();
    }

    public void SelectStructureToBuild(int structureIndex)
    {
        SetCursorModeBuild();

        selectedStructureTemplate = structureTemplates[structureIndex];

        OnStructureToBuildSelectedCallback(selectedStructureTemplate);
    }

    private void SelectHexContents(HexData hex)
    {
        selectedStructureInstance = hex.StructureHere;
        selectedUnitInstance = hex.UnitHere;

        if (OnDefendingEntitySelectedCallback != null)
        {
            OnDefendingEntitySelectedCallback(selectedStructureInstance, selectedUnitInstance);
        }
    }

    private void DeselectStructure()
    {
        selectedStructureInstance = null;

        if (OnDefendingEntityDeselectedCallback != null)
        {
            OnDefendingEntityDeselectedCallback();
        }
    }

    private void SetCursorModeBuild()
    {
        DeselectStructure();

        cursorMode = InterfaceCursorMode.BUILD;
    }

    private void SetCursorModeSelect()
    {
        cursorMode = InterfaceCursorMode.SELECT;
    }

    //NOTE this is hardcoded to be unit activated - need to split back out in SelectedDefendingEntitiesView to do structures
    public void ActivateBuildModeAbility(int index)
    {
        BuildModeAbility abilityToActivate = SelectedDefendingEntitiesView.Instance.TrackedBuildModeAbilities[index];

        if ( !EconomyManager.Instance.CanDoTransaction(abilityToActivate.BuildModeAbilityTemplate.Cost))
        {
            return;
        }

        if (abilityToActivate is HexTargetedBuildModeAbility)
        {
            CDebug.Log(CDebug.buildModeAbilities, "Hex Targeted ability activated");
            cursorMode = InterfaceCursorMode.EXECUTE_BUILD_MODE_ABILITY;
            hexTargetedAbilityToActivate = abilityToActivate as HexTargetedBuildModeAbility;
        }
        else
        {
            Coord unitCoord = map.WhereAmI(selectedUnitInstance);
            abilityToActivate.ExecuteAbility(unitCoord, unitCoord, selectedUnitInstance);
            SetCursorModeSelect();
        }
    }

    public void ExecuteHexTargetedBuildModeAbility()
    {
        SetCursorModeSelect();
    }

    public void RegisterForOnDefendingEntitySelectedCallback(Action<Structure, Unit> callback)
    {
        OnDefendingEntitySelectedCallback += callback;
    }

    public void DeregisterForOnDefendingEntitySelectedCallback(Action<Structure, Unit> callback)
    {
        OnDefendingEntitySelectedCallback -= callback;
    }

    public void RegisterForOnDefendingEntityDeselectedCallback(Action callback)
    {
        OnDefendingEntityDeselectedCallback += callback;
    }

    public void DeregisterForOnDefendingEntityDeselectedCallback(Action callback)
    {
        OnDefendingEntityDeselectedCallback -= callback;
    }

    public void RegisterForStructureToBuildSelectedCallback(Action<StructureTemplate> callback)
    {
        OnStructureToBuildSelectedCallback += callback;
    }

    public void DeregisterForStructureToBuildSelectedCallback(Action<StructureTemplate> callback)
    {
        OnStructureToBuildSelectedCallback -= callback;
    }
}
