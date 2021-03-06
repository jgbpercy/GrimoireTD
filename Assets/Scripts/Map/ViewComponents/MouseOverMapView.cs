﻿using UnityEngine;
using GrimoireTD.Technical;
using GrimoireTD.UI;
using GrimoireTD.Dependencies;

namespace GrimoireTD.Map
{
    public class MouseOverMapView : SingletonMonobehaviour<MouseOverMapView>
    {
        private InterfaceController interfaceController;

        [SerializeField]
        private Color tileHighlightNoAction;
        [SerializeField]
        private Color tileHighlightCanAction;

        [SerializeField]
        private Color tileHighlighterTestBright;

        [SerializeField]
        private Transform tileHighlighter;
        private MeshRenderer tileHighlighterRenderer;

        [SerializeField]
        private Transform structureGhost;
        private MeshFilter structureGhostFilter;
        private MeshRenderer structureGhostRenderer;
        private Vector3 structureGhostPositionOffset;

        private void Start()
        {
            interfaceController = InterfaceController.Instance;

            //tile highlighter
            tileHighlighterRenderer = tileHighlighter.GetComponent<MeshRenderer>();
            tileHighlighterRenderer.enabled = false;

            //tower ghost
            structureGhostFilter = structureGhost.GetComponent<MeshFilter>();
            structureGhostRenderer = structureGhost.GetComponent<MeshRenderer>();
            structureGhostRenderer.enabled = false;

            interfaceController.OnStructureToBuildSelected += OnSelectedStructureToBuildChange;
        }

        private void Update()
        {
            HighlightMouseOverHex();
        }

        private void HighlightMouseOverHex()
        {
            bool mouseRaycastHitMap = interfaceController.MouseRaycastHitMap;

            if (!mouseRaycastHitMap)
            {
                tileHighlighterRenderer.enabled = false;
                structureGhostRenderer.enabled = false;
                return;
            }

            InterfaceCursorMode cursorMode = interfaceController.CurrentCursorMode;
            IHexData mouseOverHex = interfaceController.MouseOverHex;
            Coord mouseOverCoord = interfaceController.MouseOverCoord;

            if (cursorMode == InterfaceCursorMode.BUILD)
            {
                tileHighlighterRenderer.enabled = false;

                if (DepsProv.TheMapData.CanBuildStructureAt(mouseOverCoord))
                {
                    structureGhostRenderer.enabled = true;

                    structureGhost.position = mouseOverCoord.ToPositionVector() + structureGhostPositionOffset;
                }
                else
                {
                    structureGhostRenderer.enabled = false;
                }
            }
            else if (cursorMode == InterfaceCursorMode.SELECT)
            {
                tileHighlighterRenderer.enabled = true;
                structureGhostRenderer.enabled = false;

                tileHighlighter.position = mouseOverCoord.ToPositionVector();

                if (mouseOverHex.StructureHere != null || mouseOverHex.UnitHere != null)
                {
                    tileHighlighterRenderer.material.color = tileHighlightCanAction;
                }
                else
                {
                    tileHighlighterRenderer.material.color = tileHighlightNoAction;
                }
            }
            else if (cursorMode == InterfaceCursorMode.EXECUTE_BUILD_MODE_ABILITY)
            {
                tileHighlighterRenderer.enabled = true;
                structureGhostRenderer.enabled = false;

                tileHighlighter.position = mouseOverCoord.ToPositionVector();

                if (interfaceController.SelectedBuildModeAbilityTargetingComponent.IsValidTarget(interfaceController.SelectedUnitInstance, mouseOverCoord))
                {
                    tileHighlighterRenderer.material.color = tileHighlighterTestBright;
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

        private void OnSelectedStructureToBuildChange(object sender, EAOnStructureToBuildSelected args)
        {
            //TODO: get and cache these at start (and also generally do better) #optimisation
            MeshFilter selectedStructureMeshFilter = MapEntitiesView.Instance.DefenderPrefabs[args.SelectedStructureTemplate].GetComponentInChildren<MeshFilter>();
            Transform selectedStructureGraphics = selectedStructureMeshFilter.transform;

            structureGhost.localScale = selectedStructureGraphics.localScale;
            structureGhostPositionOffset = selectedStructureGraphics.localPosition;

            structureGhostFilter.mesh = selectedStructureMeshFilter.sharedMesh;
            structureGhostRenderer.material = MapEntitiesView.Instance.DefenderPrefabs[args.SelectedStructureTemplate].GetComponentInChildren<MeshRenderer>().sharedMaterial;
            structureGhostRenderer.material.color = new Color(structureGhostRenderer.material.color.r, structureGhostRenderer.material.color.g, structureGhostRenderer.material.color.b, 0.45f);
        }
    }
}