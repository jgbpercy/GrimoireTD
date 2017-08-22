using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Assertions;
using GrimoireTD.Abilities.BuildMode;
using GrimoireTD.Creeps;
using GrimoireTD.DefendingEntities.Structures;
using GrimoireTD.DefendingEntities.Units;
using GrimoireTD.Map;
using GrimoireTD.Technical;
using GrimoireTD.ChannelDebug;

namespace GrimoireTD.UI
{
    public enum InterfaceCursorMode
    {
        BUILD,
        SELECT,
        EXECUTE_BUILD_MODE_ABILITY
    }

    //TODO: Handle individual defending entity selection (for DefendingEntity-targetted Build Mode Abilities)
    public class InterfaceController : SingletonMonobehaviour<InterfaceController>
    {
        [SerializeField]
        private float tempDebugTimeScale = 1f;

        [SerializeField]
        private LayerMask mainRayHitLayer;

        [SerializeField]
        private float maxRayFromCameraDistance = 100f;

        private InterfaceCursorMode cursorMode = InterfaceCursorMode.SELECT;

        private IStructureTemplate selectedStructureTemplate;

        private IStructure selectedStructureInstance = null;
        private IUnit selectedUnitInstance = null;

        private IPlayerTargetedComponent selectedBuildModeAbilityTargetingComponent = null;
        private IBuildModeAbility selectedBuildModeAbility = null;

        private Ray cameraToMouseRay;
        private RaycastHit mouseHit;

        private Coord mouseOverCoord;
        private IHexData mouseOverHex;

        private ICreep mouseOverCreep;

        private Camera mainCamera;

        private IReadOnlyMapData mapData;
        private IReadOnlyGameStateManager gameStateManager;

        private Action<IStructure, IUnit> OnDefendingEntitySelectedCallback;
        private Action OnDefendingEntityDeselectedCallback;

        private Action<IStructureTemplate> OnStructureToBuildSelectedCallback;
        private Action OnStructureToBuildDeselectedCallback;

        private Action<ICreep> OnCreepSelectedCallback;
        private Action OnCreepDeselectedCallback;

        private Action OnEnterDefendModeUserAction;

        private Action<Coord, IUnitTemplate> OnCreateUnitUserAction;
        private Action<Coord, IStructureTemplate> OnBuildStructureUserAction;

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

        public IHexData MouseOverHex
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

        public IPlayerTargetedComponent SelectedBuildModeAbilityTargetingComponent
        {
            get
            {
                return selectedBuildModeAbilityTargetingComponent;
            }
        }

        public IUnit SelectedUnitInstance
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
                return mapData.WhereAmI(selectedUnitInstance);
            }
        }

        public IStructure SelectedStructureInstance
        {
            get
            {
                return selectedStructureInstance;
            }
        }

        private void Start()
        {
            CDebug.Log(CDebug.applicationLoading, "Interface Controller Start");

            Time.timeScale = tempDebugTimeScale;

            //map
            mapData = GameModels.Models[0].MapData;

            //gamestate
            gameStateManager = GameModels.Models[0].GameStateManager;

            //camera
            mainCamera = Camera.main;

            //default cursor mode
            SetCursorModeSelect();

            gameStateManager.RegisterForOnEnterBuildModeCallback(OnEnterBuildMode);
            gameStateManager.RegisterForOnEnterDefendModeCallback(OnEnterDefendMode);
        }

        private void Update()
        {
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
                mouseOverHex = mapData.GetHexAt(mouseOverCoord);
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

            if (
                cursorMode == InterfaceCursorMode.SELECT &&
                (mouseOverHex.StructureHere != null || MouseOverHex.UnitHere != null)
            )
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

            if (cursorMode == InterfaceCursorMode.BUILD && mapData.CanBuildStructureAt(mouseOverCoord))
            {
                OnBuildStructureUserAction?.Invoke(mouseOverCoord, selectedStructureTemplate);
            }

            if (
                cursorMode == InterfaceCursorMode.EXECUTE_BUILD_MODE_ABILITY &&
                selectedBuildModeAbilityTargetingComponent.IsValidTarget(SelectedUnitInstance, mouseOverCoord)
            )
            {
                CDebug.Log(CDebug.buildModeAbilities, "InterfaceController detected hex targeted build mode ability click");

                selectedBuildModeAbility.ExecuteAbility(selectedUnitInstance, mouseOverCoord);

                SetCursorModeSelect();

                SelectHexContents(mouseOverHex);
            }

        }

        public void ButtonClickDefend()
        {
            CDebug.Log(CDebug.gameState, "Interface Controller registered defend button click");

            OnEnterDefendModeUserAction?.Invoke();
        }

        private void OnEnterBuildMode()
        {
            SetCursorModeSelect();
        }

        private void OnEnterDefendMode()
        {
            SetCursorModeSelect();
        }

        public void SelectStructureToBuild(IStructureTemplate selectedStructureTemplate)
        {
            if (!selectedStructureTemplate.Cost.CanDoTransaction())
            {
                return;
            }

            SetCursorModeBuild();

            this.selectedStructureTemplate = selectedStructureTemplate;

            OnStructureToBuildSelectedCallback?.Invoke(selectedStructureTemplate);
        }

        private void SelectHexContents(IHexData hex)
        {
            selectedStructureInstance = hex.StructureHere;
            selectedUnitInstance = hex.UnitHere;

            OnDefendingEntitySelectedCallback?.Invoke(selectedStructureInstance, selectedUnitInstance);
        }

        private void DeselectDefendingEntities()
        {
            selectedStructureInstance = null;
            selectedUnitInstance = null;
            selectedStructureTemplate = null;

            OnDefendingEntityDeselectedCallback?.Invoke();

            OnStructureToBuildDeselectedCallback?.Invoke();
        }

        private void SelectCreep(ICreep creep)
        {
            OnCreepSelectedCallback?.Invoke(creep);
        }

        private void DeselectCreep()
        {
            OnCreepDeselectedCallback?.Invoke();
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

        public void ActivateBuildModeAbility(IBuildModeAbility abilityToActivate)
        {
            if (!abilityToActivate.BuildModeAbilityTemplate.Cost.CanDoTransaction())
            {
                return;
            }

            IPlayerTargetedComponent playerTargetedComponent = abilityToActivate.BuildModeAbilityTemplate.TargetingComponent as IPlayerTargetedComponent;
            if (playerTargetedComponent != null)
            {
                CDebug.Log(CDebug.buildModeAbilities, "Player Targeted ability activated");

                cursorMode = InterfaceCursorMode.EXECUTE_BUILD_MODE_ABILITY;

                selectedBuildModeAbilityTargetingComponent = playerTargetedComponent;
                selectedBuildModeAbility = abilityToActivate;

                //TODO: optimisation: only do for a movement ability?
                selectedUnitInstance.RegenerateCachedDisallowedMovementDestinations();
            }
            else
            {
                abilityToActivate.ExecuteAbility(selectedUnitInstance, selectedUnitInstance.CoordPosition);

                SetCursorModeSelect();
            }
        }

        public void ExecuteHexTargetedBuildModeAbility()
        {
            SetCursorModeSelect();
        }

        public void ClickUnitTalent(IUnit unit, IUnitTalent unitTalent)
        {
            unit.TryLevelUp(unitTalent);
        }

        public void ClickStructureEnhancement(IStructure structure, IStructureUpgrade upgrade, IStructureEnhancement enhancement)
        {
            structure.TryUpgrade(upgrade, enhancement);
        }

        public void RegisterForOnDefendingEntitySelectedCallback(Action<IStructure, IUnit> callback)
        {
            OnDefendingEntitySelectedCallback += callback;
        }

        public void DeregisterForOnDefendingEntitySelectedCallback(Action<IStructure, IUnit> callback)
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

        public void RegisterForOnStructureToBuildSelectedCallback(Action<IStructureTemplate> callback)
        {
            OnStructureToBuildSelectedCallback += callback;
        }

        public void DeregisterForStructureToBuildSelectedCallback(Action<IStructureTemplate> callback)
        {
            OnStructureToBuildSelectedCallback -= callback;
        }

        public void RegisterForOnStructureToBuildDeselectedCallback(Action callback)
        {
            OnStructureToBuildDeselectedCallback += callback;
        }

        public void DeregisterForOnStructureToBuildDeselectedCallback(Action callback)
        {
            OnStructureToBuildDeselectedCallback -= callback;
        }

        public void RegisterForOnCreepSelectedCallback(Action<ICreep> callback)
        {
            OnCreepSelectedCallback += callback;
        }

        public void DeregisterForOnCreepSelectedCallback(Action<ICreep> callback)
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

        public void RegisterForOnEnterDefendModeUserAction(Action callback)
        {
            OnEnterDefendModeUserAction += callback;
        }

        public void DeregisterForOnEnterDefendModeUserAction(Action callback)
        {
            OnEnterDefendModeUserAction -= callback;
        }

        public void RegisterForOnCreateUnitUserAction(Action<Coord, IUnitTemplate>  callback)
        {
            OnCreateUnitUserAction += callback;
        }

        public void DeregisterForOnCreateUnitUserAction(Action<Coord, IUnitTemplate> callback)
        {
            OnCreateUnitUserAction -= callback;
        }

        public void RegisterForOnBuildStructureUserAction(Action<Coord, IStructureTemplate> callback)
        {
            OnBuildStructureUserAction += callback;
        }

        public void DeregisterForOnBuildStructureUserAction(Action<Coord, IStructureTemplate> callback)
        {
            OnBuildStructureUserAction -= callback;
        }
    }
}