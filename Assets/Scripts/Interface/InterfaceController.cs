using UnityEngine;
using UnityEngine.EventSystems;
using System;
using UnityEngine.Assertions;

public enum InterfaceCursorMode
{
    BUILD,
    SELECT,
    EXECUTE_BUILD_MODE_ABILITY
}

public class InterfaceController : SingletonMonobehaviour<InterfaceController> {

    [SerializeField]
    private float debugTimeScale = 1f;

    [SerializeField]
    private LayerMask mainRayHitLayer;

    private InterfaceCursorMode cursorMode = InterfaceCursorMode.SELECT;

    private StructureTemplate selectedStructureTemplate;

    private Structure selectedStructureInstance = null;
    private Unit selectedUnitInstance = null;

    private HexTargetedBuildModeAbility hexTargetedAbilityToActivate = null;

    [SerializeField]
    private float maxRayFromCameraDistance = 100f;

    private Ray cameraToMouseRay;
    private RaycastHit mouseHit;

    private Coord mouseOverCoord;
    private HexData mouseOverHex;

    private Creep mouseOverCreep;

    private Camera mainCamera;

    private MapData map;
    private GameStateManager gameStateManager;

    private Action<Structure, Unit> OnDefendingEntitySelectedCallback;
    private Action OnDefendingEntityDeselectedCallback;

    private Action<StructureTemplate> OnStructureToBuildSelectedCallback;

    private Action<Creep> OnCreepSelectedCallback;
    private Action OnCreepDeselectedCallback;

    public bool MouseRaycastHitMap
    {
        get
        {
            return mouseOverCoord != null;
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

        Time.timeScale = debugTimeScale;

        //map
        map = MapGenerator.Instance.Map;

        //gamestate
        gameStateManager = GameStateManager.Instance;

        //camera
        mainCamera = Camera.main;

        //default cursor mode
        SetCursorModeSelect();

        gameStateManager.RegisterForOnEnterBuildModeCallback(OnEnterBuildMode);
        gameStateManager.RegisterForOnEnterDefendModeCallback(OnEnterDefendMode);

    }

    void Update() {

        mouseOverCoord = null;
        mouseOverHex = null;
        mouseOverCreep = null;

        InterfaceControllerRaycast();

        if (Input.GetMouseButtonDown(0))
        {
            HandleMouseClick();
        }

        if (Input.GetMouseButtonDown(1))
        {
            SetCursorModeSelect();
            DeselectDefendingEntities();
            DeselectCreep();
        }
    }

    private void InterfaceControllerRaycast()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        cameraToMouseRay = mainCamera.ScreenPointToRay(Input.mousePosition);
        bool mouseRaycastHit = Physics.Raycast(cameraToMouseRay, out mouseHit, maxRayFromCameraDistance, mainRayHitLayer);

        if (!mouseRaycastHit)
        {
            return;
        }

        if (mouseHit.transform.CompareTag("TileMap"))
        {
            mouseOverCoord = Coord.PositionVectorToCoord(new Vector3(mouseHit.point.x, mouseHit.point.y, 0f));
            mouseOverHex = map.GetHexAt(mouseOverCoord);
            return;
        }

        if (mouseHit.transform.CompareTag("Creep"))
        {
            mouseOverCreep = mouseHit.transform.GetComponent<CreepComponent>().CreepModel;
            Assert.IsNotNull(mouseOverCreep);
            return;
        }

        throw new Exception("Mouse Raycast hit unhandled");
    }


    private void HandleMouseClick()
    {
        if (mouseOverCoord == null && mouseOverCreep == null)
        {
            return;
        }

        if (cursorMode == InterfaceCursorMode.SELECT && mouseOverCreep != null)
        {
            SelectCreep(mouseOverCreep);
            DeselectDefendingEntities();
            return;
        }

        if (cursorMode == InterfaceCursorMode.SELECT && (mouseOverHex.StructureHere != null || MouseOverHex.UnitHere != null))
        {
            SelectHexContents(mouseOverHex);
            DeselectCreep();
            return;
        }
        else if (cursorMode == InterfaceCursorMode.SELECT)
        {
            DeselectDefendingEntities();
            DeselectCreep();
        }

        if (cursorMode == InterfaceCursorMode.BUILD)
        {
            map.TryBuildStructureAt(mouseOverCoord, selectedStructureTemplate, false);
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
        SetCursorModeSelect();
    }

    private void OnEnterDefendMode()
    {
        SetCursorModeSelect();
    }

    public void SelectStructureToBuild(StructureTemplate selectedStructureTemplate)
    {
        this.selectedStructureTemplate = selectedStructureTemplate;

        SetCursorModeBuild();

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

    private void DeselectDefendingEntities()
    {
        selectedStructureInstance = null;
        selectedUnitInstance = null;

        if (OnDefendingEntityDeselectedCallback != null)
        {
            OnDefendingEntityDeselectedCallback();
        }
    }

    private void SelectCreep(Creep creep)
    {
        if ( OnCreepSelectedCallback != null )
        {
            OnCreepSelectedCallback(creep);
        }
    }

    private void DeselectCreep()
    {
        if ( OnCreepDeselectedCallback != null)
        {
            OnCreepDeselectedCallback();
        }
    }

    private void SetCursorModeBuild()
    {
        DeselectDefendingEntities();

        cursorMode = InterfaceCursorMode.BUILD;
    }

    private void SetCursorModeSelect()
    {
        cursorMode = InterfaceCursorMode.SELECT;
    }

    public void ActivateBuildModeAbility(BuildModeAbility abilityToActivate)
    {
        if (!EconomyManager.Instance.CanDoTransaction(abilityToActivate.BuildModeAbilityTemplate.Cost))
        {
            return;
        }

        Coord unitCoord = map.WhereAmI(selectedUnitInstance);

        if (abilityToActivate is HexTargetedBuildModeAbility)
        {
            CDebug.Log(CDebug.buildModeAbilities, "Hex Targeted ability activated");

            cursorMode = InterfaceCursorMode.EXECUTE_BUILD_MODE_ABILITY;

            hexTargetedAbilityToActivate = abilityToActivate as HexTargetedBuildModeAbility;

            hexTargetedAbilityToActivate.RegenerateCachedDisallowedTargetHexes(map, unitCoord);
        }
        else
        {
            abilityToActivate.ExecuteAbility(unitCoord, unitCoord, selectedUnitInstance);

            SetCursorModeSelect();
        }
    }

    public void ExecuteHexTargetedBuildModeAbility()
    {
        SetCursorModeSelect();
    }

    public void ClickUnitTalent(Unit unit, UnitTalent unitTalent)
    {
        unit.TryLevelUp(unitTalent);
    }

    public void ClickStructureEnhancement(Structure structure, StructureUpgrade upgrade, StructureEnhancement enhancement)
    {
        structure.TryUpgrade(upgrade, enhancement, false);
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

    public void RegisterForOnCreepSelectedCallback(Action<Creep> callback)
    {
        OnCreepSelectedCallback += callback;
    }

    public void DeregisterForOnCreepSelectedCallback(Action<Creep> callback)
    {
        OnCreepSelectedCallback -= callback;
    }

    public void RegisterForOnCreepDeselectedCallback(Action callback)
    {
        OnCreepDeselectedCallback += callback;
    }

    public void DeregisterForOnCreepDeselectedCallback(Action callback)
    {
        OnCreepDeselectedCallback -= callback;
    }
}
