using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Assertions;
using GrimoireTD.Abilities.BuildMode;
using GrimoireTD.Creeps;
using GrimoireTD.Defenders.Structures;
using GrimoireTD.Defenders.Units;
using GrimoireTD.Map;
using GrimoireTD.Technical;
using GrimoireTD.Dependencies;
using GrimoireTD.Economy;

namespace GrimoireTD.UI
{
    public enum InterfaceCursorMode
    {
        BUILD,
        SELECT,
        EXECUTE_BUILD_MODE_ABILITY
    }

    //TODO: Handle individual defender selection (for Defender-targetted Build Mode Abilities)
    public class InterfaceController : SingletonMonobehaviour<InterfaceController>, IInterfaceController
    {
        [SerializeField]
        private float tempDebugTimeScale = 1f;

        [SerializeField]
        private LayerMask mainRayHitLayer;

        [SerializeField]
        private float maxRayFromCameraDistance = 100f;

        private IStructureTemplate selectedStructureTemplate;

        private IBuildModeAbility selectedBuildModeAbility = null;

        private Ray cameraToMouseRay;
        private RaycastHit mouseHit;

        private ICreep mouseOverCreep;

        private Camera mainCamera;

        private IReadOnlyMapData mapData;
        private IReadOnlyGameStateManager gameStateManager;

        public InterfaceCursorMode CurrentCursorMode { get; private set; }

        public IHexData MouseOverHex { get; private set; }
        public Coord MouseOverCoord { get; private set; }

        public IPlayerTargetedComponent SelectedBuildModeAbilityTargetingComponent { get; private set; }

        public IUnit SelectedUnitInstance { get; private set; }
        public IStructure SelectedStructureInstance { get; private set; }

        public event EventHandler<EAOnDefenderSelected> OnDefenderSelected;
        public event EventHandler<EAOnDefenderDeselected> OnDefenderDeselected;

        public event EventHandler<EAOnStructureToBuildSelected> OnStructureToBuildSelected;
        public event EventHandler<EAOnStructureToBuildDeselected> OnStructureToBuildDeselected;

        public event EventHandler<EAOnCreepSelected> OnCreepSelected;
        public event EventHandler<EAOnCreepDeselected> OnCreepDeselected;

        public event EventHandler<EAOnEnterDefendModePlayerAction> OnEnterDefendModePlayerAction;

        public event EventHandler<EAOnCreateUnitPlayerAction> OnCreateUnitPlayerAction;
        public event EventHandler<EAOnBuildStructurePlayerAction> OnBuildStructurePlayerAction;

        public bool MouseRaycastHitMap
        {
            get
            {
                return MouseOverCoord != null;
            }
        }

        public Coord SelectedUnitLocation
        {
            get
            {
                return mapData.WhereAmI(SelectedUnitInstance);
            }
        }

        private void Awake()
        {
            CurrentCursorMode = InterfaceCursorMode.SELECT;
        }

        private void Start()
        {
            Time.timeScale = tempDebugTimeScale;

            //map
            mapData = DepsProv.TheMapData;

            //gamestate
            gameStateManager = DepsProv.TheGameStateManager;

            //camera
            mainCamera = Camera.main;

            //default cursor mode
            SetCursorModeSelect();

            gameStateManager.OnEnterBuildMode += OnEnterBuildMode;
            gameStateManager.OnEnterDefendMode += OnEnterDefendMode;
        }

        private void Update()
        {
            MouseOverCoord = null;
            MouseOverHex = null;
            mouseOverCreep = null;

            InterfaceControllerRaycast();

            if (Input.GetMouseButtonDown(0))
            {
                HandleMouseClick();
            }

            if (Input.GetMouseButtonDown(1))
            {
                SetCursorModeSelect();
                DeselectDefenders();
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
                MouseOverCoord = Coord.PositionVectorToCoord(new Vector3(mouseHit.point.x, mouseHit.point.y, 0f));
                MouseOverHex = mapData.GetHexAt(MouseOverCoord);
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
            if (MouseOverCoord == null && mouseOverCreep == null)
            {
                return;
            }

            if (CurrentCursorMode == InterfaceCursorMode.SELECT && mouseOverCreep != null)
            {
                SelectCreep(mouseOverCreep);
                DeselectDefenders();
                return;
            }

            if (
                CurrentCursorMode == InterfaceCursorMode.SELECT &&
                (MouseOverHex.StructureHere != null || MouseOverHex.UnitHere != null)
            )
            {
                SelectHexContents(MouseOverHex);
                DeselectCreep();
                return;
            }
            else if (CurrentCursorMode == InterfaceCursorMode.SELECT)
            {
                DeselectDefenders();
                DeselectCreep();
            }

            if (CurrentCursorMode == InterfaceCursorMode.BUILD && mapData.CanBuildStructureAt(MouseOverCoord))
            {
                OnBuildStructurePlayerAction?.Invoke(this, new EAOnBuildStructurePlayerAction(MouseOverCoord, selectedStructureTemplate));
            }

            if (
                CurrentCursorMode == InterfaceCursorMode.EXECUTE_BUILD_MODE_ABILITY &&
                SelectedBuildModeAbilityTargetingComponent.IsValidTarget(SelectedUnitInstance, MouseOverCoord)
            )
            {
                selectedBuildModeAbility.ExecuteAbility(SelectedUnitInstance, MouseOverCoord);

                SetCursorModeSelect();

                SelectHexContents(MouseOverHex);
            }

        }

        public void ButtonClickDefend()
        {
            OnEnterDefendModePlayerAction?.Invoke(this, new EAOnEnterDefendModePlayerAction());
        }

        private void OnEnterBuildMode(object sender, EAOnEnterBuildMode args)
        {
            SetCursorModeSelect();
        }

        private void OnEnterDefendMode(object sender, EAOnEnterDefendMode args)
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

            OnStructureToBuildSelected?.Invoke(this, new EAOnStructureToBuildSelected(selectedStructureTemplate));
        }

        private void SelectHexContents(IHexData hex)
        {
            SelectedStructureInstance = hex.StructureHere;
            SelectedUnitInstance = hex.UnitHere;

            OnDefenderSelected?.Invoke(this, new EAOnDefenderSelected(SelectedStructureInstance, SelectedUnitInstance));
        }

        private void DeselectDefenders()
        {
            SelectedStructureInstance = null;
            SelectedUnitInstance = null;
            selectedStructureTemplate = null;

            OnDefenderDeselected?.Invoke(this, new EAOnDefenderDeselected());

            OnStructureToBuildDeselected?.Invoke(this, new EAOnStructureToBuildDeselected());
        }

        private void SelectCreep(ICreep creep)
        {
            OnCreepSelected?.Invoke(this, new EAOnCreepSelected(creep));
        }

        private void DeselectCreep()
        {
            OnCreepDeselected?.Invoke(this, new EAOnCreepDeselected());
        }

        private void SetCursorModeBuild()
        {
            DeselectDefenders();

            CurrentCursorMode = InterfaceCursorMode.BUILD;
        }

        private void SetCursorModeSelect()
        {
            CurrentCursorMode = InterfaceCursorMode.SELECT;
        }

        public void ActivateBuildModeAbility(IBuildModeAbility abilityToActivate)
        {
            if (!abilityToActivate.BuildModeAbilityTemplate.Cost.CanDoTransaction())
            {
                return;
            }

            var playerTargetedComponent = abilityToActivate.TargetingComponent as IPlayerTargetedComponent;
            if (playerTargetedComponent != null)
            {
                CurrentCursorMode = InterfaceCursorMode.EXECUTE_BUILD_MODE_ABILITY;

                SelectedBuildModeAbilityTargetingComponent = playerTargetedComponent;
                selectedBuildModeAbility = abilityToActivate;

                //TODO: #optimisation: only do for a movement ability? Also, make an event/callback?
                SelectedUnitInstance.RegenerateCachedDisallowedMovementDestinations();
            }
            else
            {
                abilityToActivate.ExecuteAbility(SelectedUnitInstance, SelectedUnitInstance.CoordPosition);

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
    }
}