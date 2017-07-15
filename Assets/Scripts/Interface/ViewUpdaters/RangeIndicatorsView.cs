﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeIndicatorsView : SingletonMonobehaviour<RangeIndicatorsView> {

    private List<GameObject> rangeIndicators;
    private Dictionary<GameObject, MeshRenderer> rangeIndicatorRenderers;

    private InterfaceController interfaceController;

    private int indicatorsToRender = 0;

    [SerializeField]
    private GameObject rangeIndicatorPrefab;

    [SerializeField]
    private Color[] indicatorColors;

    [SerializeField]
    private Transform rangeIndicatorPositioner;

	void Start ()
    {
        rangeIndicators = new List<GameObject>();
        rangeIndicatorRenderers = new Dictionary<GameObject, MeshRenderer>();

        interfaceController = InterfaceController.Instance;

        interfaceController.RegisterForStructureToBuildSelectedCallback(OnSelectedStructureChange);
	}
	
	void Update ()
    {
        bool mouseRaycastHitMap = interfaceController.MouseRaycastHitMap;

        if (!mouseRaycastHitMap)
        {
            rangeIndicators.ForEach(x => rangeIndicatorRenderers[x].enabled = false);
            return;
        }

        InterfaceCursorMode cursorMode = interfaceController.CurrentCursorMode;
        HexData mouseOverHex = interfaceController.MouseOverHex;
        Coord mouseOverCoord = interfaceController.MouseOverCoord;

        /*Optimisation: 
         * Have callbacks from interface controller for stuff like OnMouseOverMap and OnMouseOverHexChange
         * and use these to calculate and cache range indicator settings until the mouse over hex changes
         * */
        if (cursorMode == InterfaceCursorMode.SELECT)
        {
            indicatorsToRender = 0;
            List<float> ranges = new List<float>();
            
            if ( mouseOverHex.StructureHere != null )
            {
                mouseOverHex.StructureHere.DefendModeAbilities().ForEach(x => {

                    DMTargetingComponent targetingComponent = x.DefendModeAbilityTemplate.TargetingComponent;

                    if (targetingComponent is TargetingComponentFloatRange)
                    {
                        ranges.Add(((TargetingComponentFloatRange)targetingComponent).GetActualRange(mouseOverHex.StructureHere));
                    }
                });
            }

            if ( mouseOverHex.UnitHere != null )
            {
                mouseOverHex.UnitHere.DefendModeAbilities().ForEach(x => {

                    DMTargetingComponent targetingComponent = x.DefendModeAbilityTemplate.TargetingComponent;

                    if (targetingComponent is TargetingComponentFloatRange)
                    {
                        ranges.Add(((TargetingComponentFloatRange)targetingComponent).GetActualRange(mouseOverHex.UnitHere));
                    }
                });
            }

            for (int i = 0; i < ranges.Count; i++)
            {
                SetUpRangeIndicator(i, ranges[i]);

                indicatorsToRender = i + 1;
            }
        }
        else if (cursorMode == InterfaceCursorMode.EXECUTE_BUILD_MODE_ABILITY)
        {
            indicatorsToRender = 0;
        }
        else if ( cursorMode != InterfaceCursorMode.BUILD)
        {
            throw new System.Exception("Unhandled cursor mode");
        }

        for (int i = 0; i < indicatorsToRender; i++)
        {
            rangeIndicatorRenderers[rangeIndicators[i]].enabled = true;
        }
        for (int j = indicatorsToRender; j < rangeIndicators.Count; j++)
        {
            rangeIndicatorRenderers[rangeIndicators[j]].enabled = false;
        }

        rangeIndicatorPositioner.position = mouseOverCoord.ToPositionVector();
    }

    private void OnSelectedStructureChange(StructureTemplate selectedStructureTemplate)
    {
        int i = 0;

        foreach (AbilityTemplate abilityTemplate in selectedStructureTemplate.BaseCharacteristics.Abilities)
        {
            if ( abilityTemplate is DefendModeAbilityTemplate && ((DefendModeAbilityTemplate)abilityTemplate).TargetingComponent is TargetingComponentFloatRange )
            {
                TargetingComponentFloatRange targetingComponentFloatRange = ((DefendModeAbilityTemplate)abilityTemplate).TargetingComponent as TargetingComponentFloatRange;

                SetUpRangeIndicator(i, targetingComponentFloatRange.BaseRange);

                i++;
            }
        }

        indicatorsToRender = i;
    }

    private void SetUpRangeIndicator(int index, float range)
    {
        if (rangeIndicators.Count <= index)
        {
            GameObject newRangeIndicator = Instantiate(rangeIndicatorPrefab);
            newRangeIndicator.transform.SetParent(rangeIndicatorPositioner);

            newRangeIndicator.transform.localPosition = new Vector3();

            rangeIndicators.Add(newRangeIndicator);
            rangeIndicatorRenderers.Add(newRangeIndicator, newRangeIndicator.GetComponent<MeshRenderer>());

            rangeIndicatorRenderers[newRangeIndicator].material.color = indicatorColors[index % indicatorColors.Length];
        }

        rangeIndicators[index].transform.localScale = new Vector3(
            range * 2,
            rangeIndicators[index].transform.localScale.y,
            range * 2
        );
    }
}