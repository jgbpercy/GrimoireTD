using System.Collections.Generic;
using UnityEngine;
using GrimoireTD.Abilities;
using GrimoireTD.Abilities.DefendMode;
using GrimoireTD.DefendingEntities.Structures;
using GrimoireTD.Technical;
using GrimoireTD.Map;
using GrimoireTD.UI;

namespace GrimoireTD.DefendingEntities
{
    public class RangeIndicatorsView : SingletonMonobehaviour<RangeIndicatorsView>
    {
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

        private void Start()
        {
            rangeIndicators = new List<GameObject>();
            rangeIndicatorRenderers = new Dictionary<GameObject, MeshRenderer>();

            interfaceController = InterfaceController.Instance;

            interfaceController.OnStructureToBuildSelected += OnSelectedStructureChange;
        }

        private void Update()
        {
            bool mouseRaycastHitMap = interfaceController.MouseRaycastHitMap;

            if (!mouseRaycastHitMap)
            {
                rangeIndicators.ForEach(x => rangeIndicatorRenderers[x].enabled = false);
                return;
            }

            InterfaceCursorMode cursorMode = interfaceController.CurrentCursorMode;
            IHexData mouseOverHex = interfaceController.MouseOverHex;
            Coord mouseOverCoord = interfaceController.MouseOverCoord;

            /*Optimisation: 
             * Have callbacks from interface controller for stuff like OnMouseOverMap and OnMouseOverHexChange
             * and use these to calculate and cache range indicator settings until the mouse over hex changes
             * */
            if (cursorMode == InterfaceCursorMode.SELECT)
            {
                indicatorsToRender = 0;
                List<float> ranges = new List<float>();

                if (mouseOverHex.StructureHere != null)
                {
                    foreach (IDefendModeAbility defendModeAbility in mouseOverHex.StructureHere.Abilities.DefendModeAbilities())
                    {
                        //TODO: referencing SOs here - make interfaces?
                        SoFloatRangeArgsTemplate floatRangeTargetingArgs = defendModeAbility.DefendModeAbilityTemplate.TargetingComponent.TargetingRule as SoFloatRangeArgsTemplate;
                        if (floatRangeTargetingArgs != null)
                        {
                            ranges.Add(floatRangeTargetingArgs.GetActualRange(mouseOverHex.StructureHere));
                        }
                    }
                }

                if (mouseOverHex.UnitHere != null)
                {
                    foreach (IDefendModeAbility defendModeAbility in mouseOverHex.UnitHere.Abilities.DefendModeAbilities())
                    {
                        SoFloatRangeArgsTemplate floatRangeTargetingArgs = defendModeAbility.DefendModeAbilityTemplate.TargetingComponent.TargetingRule as SoFloatRangeArgsTemplate;
                        if (floatRangeTargetingArgs != null)
                        {
                            ranges.Add(floatRangeTargetingArgs.GetActualRange(mouseOverHex.UnitHere));
                        }
                    }
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
            else if (cursorMode != InterfaceCursorMode.BUILD)
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

        private void OnSelectedStructureChange(object sender, EAOnStructureToBuildSelected args)
        {
            int i = 0;

            foreach (IAbilityTemplate abilityTemplate in args.SelectedStructureTemplate.BaseCharacteristics.Abilities)
            {
                IDefendModeAbilityTemplate defendModeAbilityTemplate = abilityTemplate as IDefendModeAbilityTemplate;
                if (defendModeAbilityTemplate != null)
                {
                    SoFloatRangeArgsTemplate floatRangeTargetingArgs = defendModeAbilityTemplate.TargetingComponent.TargetingRule as SoFloatRangeArgsTemplate;
                    if (floatRangeTargetingArgs != null)
                    {
                        SetUpRangeIndicator(i, floatRangeTargetingArgs.GetActualRange(null));

                        i++;
                    }
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
}