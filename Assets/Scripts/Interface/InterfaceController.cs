using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public enum InterfaceCursorMode
{
    BUILD,
    SELECT
}

public class InterfaceController : SingletonMonobehaviour<InterfaceController> {

    [SerializeField]
    private LayerMask tileMapLayer;

    private InterfaceCursorMode cursorMode = InterfaceCursorMode.SELECT;

    private StructureTemplate selectedStructureTemplate;

    private Structure selectedStructureInstance = null;

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

    private Action<DefendingEntity> OnDefendingEntitySelectedCallback;
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

        //debug
        if (Input.GetButtonDown("DebugSpawnWave") && gameStateManager.CurrentGameMode == GameMode.DEFEND)
        {
            CreepManager.Instance.InfDebugSpawnWave = true;
        }

        if (Input.GetButtonDown("DebugSpawn") && gameStateManager.CurrentGameMode == GameMode.DEFEND)
        {
            CreepManager.Instance.InfDebugSpawn = true;
        }
    }


    private void HandleMouseClick()
    {
        if (!mouseRaycastHitMapBool)
        {
            return;
        }

        if (cursorMode == InterfaceCursorMode.SELECT && mouseOverHex.StructureHere != null)
        {
            SelectBuiltStructure();
            return;
        }
        else
        {
            DeselectStructure();
        }

        if (cursorMode == InterfaceCursorMode.BUILD)
        {
            map.TryBuildStructureAt(mouseOverCoord, selectedStructureTemplate);
        }

    }

    public void buttonClickDefend()
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

    private void SelectBuiltStructure()
    {
        selectedStructureInstance = mouseOverHex.StructureHere;

        if (OnDefendingEntitySelectedCallback != null)
        {
            OnDefendingEntitySelectedCallback(selectedStructureInstance);
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

    public void RegisterForOnDefendingEntitySelectedCallback(Action<DefendingEntity> callback)
    {
        OnDefendingEntitySelectedCallback += callback;
    }

    public void DeregisterForOnDefendingEntitySelectedCallback(Action<DefendingEntity> callback)
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
