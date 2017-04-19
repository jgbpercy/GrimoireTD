using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Assertions;
using System;

public class InterfaceController : SingletonMonobehaviour<InterfaceController> {

    private enum CursorMode
    {
        BUILD,
        SELECT
    }

    [SerializeField]
    private LayerMask tileMapLayer;

    private CursorMode cursorMode = CursorMode.SELECT;

    private StructureTemplate selectedStructureTemplate;

    private Structure selectedStructureInstance = null;

    [SerializeField]
    private Color tileHighlightNoAction;
    [SerializeField]
    private Color tileHighlightCanAction;

    [SerializeField]
    private StructureTemplate[] structureTemplates;
    [SerializeField]
    private GameObject structurePanelPrefab;
    [SerializeField]
    private GameObject structureSelectPanel;

    [SerializeField]
    private float maxRayFromCameraDistance = 100f;

    private Ray cameraToMouseRay;
    private RaycastHit mouseHitAgainstTileMap;
    private bool mouseRaycastHitMapBool;

    private Coord mouseOverCoord;
    private HexData mouseOverHex;

    private Camera mainCamera;

    [SerializeField]
    private Transform tileHighlighter;
    private MeshRenderer tileHighlighterRenderer;

    [SerializeField]
    private Transform rangeIndicator;
    private MeshRenderer rangeIndicatorRenderer;

    [SerializeField]
    private Transform structureGhost;
    private MeshFilter structureGhostFilter;
    private MeshRenderer structureGhostRenderer;
    private Vector3 structureGhostPositionOffset;

    [SerializeField]
    private Text modeText;

    [SerializeField]
    private GameObject selectedStructurePanel;
    [SerializeField]
    private Text selectedStructureName;
    [SerializeField]
    private Text selectedStructureText;

    private MapData map;
    private GameStateManager gameStateManager;

    private List<GameObject> enableGameObjectsInBuildMode;
    private List<GameObject> enableGameObjectsInDefendMode;

    private Action<DefendingEntity> OnDefendingEntitySelectedCallback;
    private Action OnDefendingEntityDeselectedCallback;

    private void Awake()
    {
        enableGameObjectsInBuildMode = new List<GameObject>();
        enableGameObjectsInDefendMode = new List<GameObject>();
    }

    private void Start ()
    {
        CDebug.Log(CDebug.applicationLoading, "Interface Controller Start");

        //map
        map = MapGenerator.Instance.Map;

        //gamestate
        gameStateManager = GameStateManager.Instance;

        //camera
        mainCamera = Camera.main;

        //tile highlighter
        tileHighlighterRenderer = tileHighlighter.GetComponent<MeshRenderer>();
        tileHighlighterRenderer.enabled = false;

        //range indicator
        rangeIndicatorRenderer = rangeIndicator.GetComponent<MeshRenderer>();
        rangeIndicatorRenderer.enabled = false;

        //tower ghost
        structureGhostFilter = structureGhost.GetComponent<MeshFilter>();
        structureGhostRenderer = structureGhost.GetComponent<MeshRenderer>();
        structureGhostRenderer.enabled = false;

        //set up tower panel
        SetUpStructurePanel();

        //default tower to build
        SelectStructureToBuild(0);
        SetCursorModeSelect();

        gameStateManager.RegisterForOnEnterBuildModeCallback( OnEnterBuildMode );
        gameStateManager.RegisterForOnEnterDefendModeCallback(OnEnterDefendMode);

    }
	
	void Update () {

        if ( !EventSystem.current.IsPointerOverGameObject() )
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

        HighlightMouseOverHex();

        if ( Input.GetMouseButtonDown(0) )
        {
            HandleMouseClick();
        }

        if ( Input.GetMouseButtonDown(1) )
        {
            SetCursorModeSelect();
            DeselectStructure();
        }

        if (Input.GetButtonDown("DebugSpawnWave") && gameStateManager.CurrentGameMode == GameMode.DEFEND )
        {
            CreepManager.Instance.InfDebugSpawnWave = true;
        }

        if (Input.GetButtonDown("DebugSpawn") && gameStateManager.CurrentGameMode == GameMode.DEFEND )
        {
            CreepManager.Instance.InfDebugSpawn = true;
        }        
    }

    private void HighlightMouseOverHex()
    {

        if (!mouseRaycastHitMapBool)
        {
            tileHighlighterRenderer.enabled = false;
            structureGhostRenderer.enabled = false;
            rangeIndicatorRenderer.enabled = false;
            return;
        }

        if ( cursorMode == CursorMode.BUILD )
        {
            rangeIndicatorRenderer.enabled = true;
            tileHighlighterRenderer.enabled = false;

            if ( mouseOverHex.CanAddStructureHere() )
            {
                structureGhostRenderer.enabled = true;

                structureGhost.position = mouseOverCoord.ToPositionVector() + structureGhostPositionOffset;

                rangeIndicatorRenderer.material.color = tileHighlightCanAction;
                rangeIndicator.position = mouseOverCoord.ToPositionVector();
            }
            else
            {
                structureGhostRenderer.enabled = false;

                rangeIndicatorRenderer.material.color = tileHighlightNoAction;
                rangeIndicator.position = mouseOverCoord.ToPositionVector();
            }
        }
        else if ( cursorMode == CursorMode.SELECT )
        {
            tileHighlighterRenderer.enabled = true;
            rangeIndicatorRenderer.enabled = false;
            structureGhostRenderer.enabled = false;

            tileHighlighter.position = mouseOverCoord.ToPositionVector();

            if ( mouseOverHex.StructureHere != null )
            {
                tileHighlighterRenderer.material.color = tileHighlightCanAction;
            }
            else
            {
                tileHighlighterRenderer.material.color = tileHighlightNoAction;
            }
        }
        else
        {
            throw new System.Exception("Unhandled cursor mode");
        }

    }

    private void HandleMouseClick()
    {
        if ( !mouseRaycastHitMapBool )
        {
            return;
        }

        if ( cursorMode == CursorMode.SELECT && mouseOverHex.StructureHere != null )
        {
            SelectBuiltStructure();
            return;
        }
        else
        {
            DeselectStructure();
        }
       
        if ( cursorMode == CursorMode.BUILD )
        {
            map.TryBuildStructureAt(mouseOverCoord, selectedStructureTemplate);
        }

    }

    public void buttonClickDefend()
    {
        CDebug.Log(CDebug.gameState, "Interface Controller register defend button click");

        gameStateManager.CurrentGameMode = GameMode.DEFEND;
    }

    private void OnEnterBuildMode()
    {
        foreach (GameObject enableInDefendMode in enableGameObjectsInDefendMode)
        {
            enableInDefendMode.SetActive(false);
        }
        foreach (GameObject enabledInBuildMode in enableGameObjectsInBuildMode)
        {
            enabledInBuildMode.SetActive(true);
        }

        modeText.text = "Mode: Build";
    }

    private void OnEnterDefendMode()
    {
        SetCursorModeSelect();

        foreach (GameObject enableInDefendMode in enableGameObjectsInDefendMode)
        {
            enableInDefendMode.SetActive(true);
        }
        foreach (GameObject enabledInBuildMode in enableGameObjectsInBuildMode)
        {
            enabledInBuildMode.SetActive(false);
        }

        modeText.text = "Mode: Defend";

    }

    //TODO make proper callback to model?
    public void RegisterAsEnabledInMode(GameObject gameObject, GameMode gameMode)
    {
        if ( gameMode == GameMode.BUILD )
        {
            enableGameObjectsInBuildMode.Add(gameObject);
        }
        else
        {
            enableGameObjectsInDefendMode.Add(gameObject);
        }
        
    }

    private void SetUpStructurePanel()
    {
        for (int i = 0; i < structureTemplates.Length; i++)
        {
            GameObject structurePanel = Instantiate(structurePanelPrefab) as GameObject;

            structurePanel.transform.SetParent(structureSelectPanel.transform, false);

            StructureUIElement structurePanelUIScript = structurePanel.GetComponent<StructureUIElement>();
            structurePanelUIScript.indexInStructureList = i;

            Text structureName = structurePanel.transform.GetChild(0).GetComponent<Text>();
            structureName.text = structureTemplates[i].NameInGame;
            
            Text structureDescription = structurePanel.transform.GetChild(1).GetComponent<Text>();
            structureDescription.text = structureTemplates[i].UIText();
        }

    }

    public void SelectStructureToBuild(int structureIndex)
    {
        SetCursorModeBuild();

        selectedStructureTemplate = structureTemplates[structureIndex];

        //should get and cache these at start (and also generally do better)
        MeshFilter selectedStructureMeshFilter = selectedStructureTemplate.StructurePrefab.GetComponentInChildren<MeshFilter>();
        Transform selectedStructureGraphics = selectedStructureMeshFilter.transform;

        structureGhost.localScale = selectedStructureGraphics.localScale;
        structureGhostPositionOffset = selectedStructureGraphics.localPosition;

        structureGhostFilter.mesh = selectedStructureMeshFilter.sharedMesh;
        structureGhostRenderer.material = selectedStructureTemplate.StructurePrefab.GetComponentInChildren<MeshRenderer>().sharedMaterial;
        structureGhostRenderer.material.color = new Color(structureGhostRenderer.material.color.r, structureGhostRenderer.material.color.g, structureGhostRenderer.material.color.b, 0.45f);

        //TEMP HACK TEMP HACK TEMP HACK
        Assert.IsTrue((ProjectileAttackTemplate)selectedStructureTemplate.BaseAbilities[0] != null);
        float abilityZeroRange = ((ProjectileAttackTemplate)selectedStructureTemplate.BaseAbilities[0]).Range;

        rangeIndicator.localScale = new Vector3(abilityZeroRange * 2, rangeIndicator.localScale.y, abilityZeroRange * 2);
    }

    private void SelectBuiltStructure()
    {
        selectedStructureInstance = mouseOverHex.StructureHere;

        selectedStructurePanel.SetActive(true);

        selectedStructureName.text = selectedStructureInstance.StructureClassTemplate.NameInGame;

        selectedStructureText.text = selectedStructureInstance.UIText();

        if ( OnDefendingEntitySelectedCallback != null )
        {
            OnDefendingEntitySelectedCallback(selectedStructureInstance);
        }
    }

    private void DeselectStructure()
    {
        selectedStructureInstance = null;

        selectedStructurePanel.SetActive(false);

        if ( OnDefendingEntityDeselectedCallback != null )
        {
            OnDefendingEntityDeselectedCallback();
        }
    }

    private void SetCursorModeBuild()
    {
        DeselectStructure();

        cursorMode = CursorMode.BUILD;
    }

    private void SetCursorModeSelect()
    {
        cursorMode = CursorMode.SELECT;
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
}
