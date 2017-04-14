using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum GameMode
{
    BUILD,
    DEFEND
}

public class InterfaceManager : MonoBehaviour {

    private enum CursorMode
    {
        BUILD,
        SELECT
    }

    [SerializeField]
    private LayerMask tileMapLayer;

    [SerializeField]
    private bool debugOn = false;

    private CursorMode cursorMode = CursorMode.SELECT;

    private GameObject selectedTowerPrefabToBuild;
    private Tower selectedTowerPrefabToBuildTowerScript;

    private Tower selectedStructureInstance = null;

    [SerializeField]
    private Color tileHighlightNoAction;
    [SerializeField]
    private Color tileHighlightCanAction;

    [SerializeField]
    private GameObject[] towerPrefabs;
    [SerializeField]
    private GameObject towerPanelPrefab;
    [SerializeField]
    private GameObject towerSelectPanel;

    [SerializeField]
    private float maxRayFromCameraDistance = 100f;

    private Ray cameraToMouseRay;
    private RaycastHit mouseHitAgainstTileMap;
    private bool mouseRaycastHitMapBool;

    private Coord mouseOverCoord;
    private HexData mouseOverHex;

    private Camera mainCamera;
    /*private Transform cameraTilter;
    private Transform cameraRotator;
    private Vector3 cameraRigTargetPosition;*/

    [SerializeField]
    private Transform tileHighlighter;
    private MeshRenderer tileHighlighterRenderer;

    [SerializeField]
    private Transform rangeIndicator;
    private MeshRenderer rangeIndicatorRenderer;

    [SerializeField]
    private Transform towerGhost;
    private MeshFilter towerGhostFilter;
    private MeshRenderer towerGhostRenderer;
    private Vector3 towerGhostPositionOffset;

    private EconomyManager economyManager;

    [SerializeField]
    private Text resourceText;

    [SerializeField]
    private Text modeText;

    [SerializeField]
    private GameObject selectedStructurePanel;
    [SerializeField]
    private Text selectedStructureName;
    [SerializeField]
    private Text selectedStructureText;

    private MapLoader mapLoader;
    private CreepManager creepManager;

    private List<GameObject> enableGameObjectsInBuildMode;
    private List<GameObject> enableGameObjectsInDefendMode;

    private GameMode currentGameMode;

    public GameMode CurrentGameMode 
    {
        get
        {
            return currentGameMode;
        }
    }

    private void Awake()
    {
        enableGameObjectsInBuildMode = new List<GameObject>();
        enableGameObjectsInDefendMode = new List<GameObject>();
    }

    void Start () {

        //starting mode
        currentGameMode = GameMode.BUILD;

        //own other manager components
        mapLoader = gameObject.GetComponent<MapLoader>();
        creepManager = gameObject.GetComponent<CreepManager>();
        economyManager = gameObject.GetComponent<EconomyManager>();

        //camera
        mainCamera = Camera.main;

        //tile highlighter
        //tileHighlighter = GameObject.Find("TileHighlighter").transform;
        tileHighlighterRenderer = tileHighlighter.GetComponent<MeshRenderer>();
        tileHighlighterRenderer.enabled = false;

        //range indicator
        //rangeIndicator = GameObject.Find("RangeIndicator").transform;
        rangeIndicatorRenderer = rangeIndicator.GetComponent<MeshRenderer>();
        rangeIndicatorRenderer.enabled = false;

        //tower ghost
        //towerGhost = GameObject.Find("TowerToBuildGhost").transform;
        towerGhostFilter = towerGhost.GetComponent<MeshFilter>();
        towerGhostRenderer = towerGhost.GetComponent<MeshRenderer>();
        towerGhostRenderer.enabled = false;

        //UI components
        /*
        resourceText = GameObject.Find("ResourceText").GetComponent<Text>();
        modeText = GameObject.Find("ModeText").GetComponent<Text>();
        towerSelectPanel = GameObject.Find("StructureSelectPanel");
        */

        //set up tower panel
        SetUpTowerPanel();

        //default tower to build
        SelectTowerToBuild(0);
        SetCursorModeSelect();

    }
	
	void Update () {

        if ( !EventSystem.current.IsPointerOverGameObject() )
        {
            cameraToMouseRay = mainCamera.ScreenPointToRay(Input.mousePosition);
            mouseRaycastHitMapBool = Physics.Raycast(cameraToMouseRay, out mouseHitAgainstTileMap, maxRayFromCameraDistance, tileMapLayer);
            mouseOverCoord = Coord.PositionVectorToCoord(new Vector3(mouseHitAgainstTileMap.point.x, mouseHitAgainstTileMap.point.y, 0f));
            mouseOverHex = mapLoader.Map.GetHexAt(mouseOverCoord);
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

        if (Input.GetButtonDown("DebugSpawnWave") && currentGameMode == GameMode.DEFEND )
        {
            creepManager.InfDebugSpawnWave = true;
        }

        if (Input.GetButtonDown("DebugSpawn") && currentGameMode == GameMode.DEFEND )
        {
            creepManager.InfDebugSpawn = true;
        }

        if ( !creepManager.WaveIsActive && currentGameMode == GameMode.DEFEND )
        {
            EnterBuildMode();
        }

        UIUpdateTasks();
        
    }

    private void HighlightMouseOverHex()
    {

        if (!mouseRaycastHitMapBool)
        {
            tileHighlighterRenderer.enabled = false;
            towerGhostRenderer.enabled = false;
            rangeIndicatorRenderer.enabled = false;
            return;
        }

        if ( cursorMode == CursorMode.BUILD )
        {
            rangeIndicatorRenderer.enabled = true;
            tileHighlighterRenderer.enabled = false;

            if ( mouseOverHex.canAddStructureHere() )
            {
                towerGhostRenderer.enabled = true;

                towerGhost.position = mouseOverCoord.ToPositionVector() + towerGhostPositionOffset;

                rangeIndicatorRenderer.material.color = tileHighlightCanAction;
                rangeIndicator.position = mouseOverCoord.ToPositionVector();
            }
            else
            {
                towerGhostRenderer.enabled = false;

                rangeIndicatorRenderer.material.color = tileHighlightNoAction;
                rangeIndicator.position = mouseOverCoord.ToPositionVector();
            }
        }
        else if ( cursorMode == CursorMode.SELECT )
        {
            tileHighlighterRenderer.enabled = true;
            rangeIndicatorRenderer.enabled = false;
            towerGhostRenderer.enabled = false;

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
            SelectStructure();
            return;
        }
        else
        {
            DeselectStructure();
        }

       
        if ( cursorMode == CursorMode.BUILD && mouseOverHex.canAddStructureHere() )
        {
            if ( economyManager.TryPayTowerCost(selectedTowerPrefabToBuild) )
            {
                mouseOverHex.tryAddStructureHere( Instantiate(selectedTowerPrefabToBuild, mouseOverCoord.ToPositionVector(), Quaternion.identity).GetComponent<Tower>() );
            }
        }

    }

    //to be remove when done better
    private void UIUpdateTasks()
    {
        resourceText.text = economyManager.ResourceUIText();

        modeText.text = "Mode: " + (currentGameMode == GameMode.BUILD ? "Build" : "Defend");
    }

    public void buttonClickDefend()
    {
        EnterDefendMode();
    }

    private bool EnterDefendMode()
    {
        currentGameMode = GameMode.DEFEND;
        SetCursorModeSelect();

        foreach(GameObject enableInDefendMode in enableGameObjectsInDefendMode)
        {
            enableInDefendMode.SetActive(true);
        } 
        foreach(GameObject enabledInBuildMode in enableGameObjectsInBuildMode)
        {
            enabledInBuildMode.SetActive(false);
        }

        creepManager.StartNextWave();

        return true;
    }

    private bool EnterBuildMode()
    {
        currentGameMode = GameMode.BUILD;

        foreach (GameObject enableInDefendMode in enableGameObjectsInDefendMode)
        {
            enableInDefendMode.SetActive(false);
        }
        foreach (GameObject enabledInBuildMode in enableGameObjectsInBuildMode)
        {
            enabledInBuildMode.SetActive(true);
        }

        return true;
    }

    public void registerAsEnabledInMode(GameObject gameObject, GameMode gameMode)
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

    private void SetUpTowerPanel()
    {
        for (int i = 0; i < towerPrefabs.Length; i++)
        {
            GameObject towerPanel = Instantiate(towerPanelPrefab) as GameObject;

            towerPanel.transform.SetParent(towerSelectPanel.transform, false);

            //removed when vertical layout thing used
            //RectTransform towerPanelTransform = (RectTransform)towerPanel.transform;

            //towerPanelTransform.Translate(0, -110 * i, 0);

            TowerUIElement towerPanelUIScript = towerPanel.GetComponent<TowerUIElement>();
            towerPanelUIScript.indexInTowerList = i;

            Tower towerPrefabTowerScript = towerPrefabs[i].GetComponent<Tower>();

            Text towerName = towerPanel.transform.GetChild(0).GetComponent<Text>();
            towerName.text = towerPrefabTowerScript.NameInGame;
            
            Text towerDescription = towerPanel.transform.GetChild(1).GetComponent<Text>();
            towerDescription.text = economyManager.TowerCostUIText(towerPrefabs[i]) + "\n";
            towerDescription.text += towerPrefabTowerScript.TowerStatsUIText();
        }

    }

    public void SelectTowerToBuild(int towerIndex)
    {
        SetCursorModeBuild();

        selectedTowerPrefabToBuild = towerPrefabs[towerIndex];
        selectedTowerPrefabToBuildTowerScript = selectedTowerPrefabToBuild.GetComponent<Tower>();

        //should get and cache these at start (and also generally do better)
        MeshFilter selectedPrefabFilter = selectedTowerPrefabToBuild.GetComponentInChildren<MeshFilter>();
        Transform selectedPrefabGraphics = selectedPrefabFilter.transform;

        towerGhost.localScale = selectedPrefabGraphics.localScale;
        towerGhostPositionOffset = selectedPrefabGraphics.localPosition;

        towerGhostFilter.mesh = selectedPrefabFilter.sharedMesh;
        towerGhostRenderer.material = selectedTowerPrefabToBuild.GetComponentInChildren<MeshRenderer>().sharedMaterial;
        towerGhostRenderer.material.color = new Color(towerGhostRenderer.material.color.r, towerGhostRenderer.material.color.g, towerGhostRenderer.material.color.b, 0.45f);

        rangeIndicator.localScale = new Vector3(selectedTowerPrefabToBuildTowerScript.Range * 2, rangeIndicator.localScale.y, selectedTowerPrefabToBuildTowerScript.Range * 2);
    }

    private void SelectStructure()
    {
        selectedStructureInstance = mouseOverHex.StructureHere;

        selectedStructurePanel.SetActive(true);

        selectedStructureName.text = selectedStructureInstance.NameInGame;

        selectedStructureText.text = selectedStructureInstance.TowerStatsUIText();
    }

    private void DeselectStructure()
    {
        selectedStructureInstance = null;

        selectedStructurePanel.SetActive(false);

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
}
