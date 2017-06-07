using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class MouseOverMapView : SingletonMonobehaviour<MouseOverMapView> {

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
    private Transform rangeIndicator;
    private MeshRenderer rangeIndicatorRenderer;

    [SerializeField]
    private Transform structureGhost;
    private MeshFilter structureGhostFilter;
    private MeshRenderer structureGhostRenderer;
    private Vector3 structureGhostPositionOffset;

    private void Start()
    {
        CDebug.Log(CDebug.applicationLoading, "Mouseover Map View Start");

        interfaceController = InterfaceController.Instance;

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

        interfaceController.RegisterForStructureToBuildSelectedCallback(OnSelectedStructureChange);
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
            rangeIndicatorRenderer.enabled = false;
            return;
        }

        InterfaceCursorMode cursorMode = interfaceController.CurrentCursorMode;
        HexData mouseOverHex = interfaceController.MouseOverHex;
        Coord mouseOverCoord = interfaceController.MouseOverCoord;

        if (cursorMode == InterfaceCursorMode.BUILD)
        {
            rangeIndicatorRenderer.enabled = true;
            tileHighlighterRenderer.enabled = false;

            if (mouseOverHex.CanPlaceStructureHere())
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
        else if (cursorMode == InterfaceCursorMode.SELECT)
        {
            tileHighlighterRenderer.enabled = true;
            rangeIndicatorRenderer.enabled = false;
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
        else if ( cursorMode == InterfaceCursorMode.EXECUTE_BUILD_MODE_ABILITY)
        {
            tileHighlighterRenderer.enabled = true;
            rangeIndicatorRenderer.enabled = false;
            structureGhostRenderer.enabled = false;

            tileHighlighter.position = mouseOverCoord.ToPositionVector();

            if (interfaceController.HexTargetedAbilityToActivate.IsTargetSuitable(interfaceController.SelectedUnitLocation, mouseOverCoord))
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

    private void OnSelectedStructureChange(StructureTemplate selectedStructureTemplate)
    {
        //should get and cache these at start (and also generally do better)
        MeshFilter selectedStructureMeshFilter = selectedStructureTemplate.Prefab.GetComponentInChildren<MeshFilter>();
        Transform selectedStructureGraphics = selectedStructureMeshFilter.transform;

        structureGhost.localScale = selectedStructureGraphics.localScale;
        structureGhostPositionOffset = selectedStructureGraphics.localPosition;

        structureGhostFilter.mesh = selectedStructureMeshFilter.sharedMesh;
        structureGhostRenderer.material = selectedStructureTemplate.Prefab.GetComponentInChildren<MeshRenderer>().sharedMaterial;
        structureGhostRenderer.material.color = new Color(structureGhostRenderer.material.color.r, structureGhostRenderer.material.color.g, structureGhostRenderer.material.color.b, 0.45f);

        //TEMP HACK TEMP HACK TEMP HACK
        Assert.IsTrue((ProjectileAttackTemplate)selectedStructureTemplate.BaseAbilities[0] != null);
        float abilityZeroRange = ((ProjectileAttackTemplate)selectedStructureTemplate.BaseAbilities[0]).Range;

        rangeIndicator.localScale = new Vector3(abilityZeroRange * 2, rangeIndicator.localScale.y, abilityZeroRange * 2);
    }
}
