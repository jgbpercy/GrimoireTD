﻿using System;
using UnityEngine;
using UnityEngine.Assertions;

public class MoveAbility : HexTargetedBuildModeAbility
{
    private MoveAbilityTemplate moveAbilityTemplate;

    public MoveAbilityTemplate MoveAbilityTemplate
    {
        get
        {
            return moveAbilityTemplate;
        }
    }

    public MoveAbility(MoveAbilityTemplate template) : base(template)
    {
        moveAbilityTemplate = template;
    }

    public override bool ExecuteAbility(Coord fromCoord, Coord targetCoord, DefendingEntity executingEntity)
    {
        CDebug.Log(CDebug.buildModeAbilities, "Called MoveAbility ExecuteAbility");

        Assert.IsTrue(executingEntity is Unit);
        Unit executingUnit = (Unit)executingEntity;

        if ( MapGenerator.Instance.Map.TryMoveUnitTo(fromCoord, targetCoord, (Unit)executingEntity, moveAbilityTemplate.Cost ) ) {
            if ( executingUnit.OnMovedCallback != null )
            {
                executingUnit.OnMovedCallback(targetCoord);
            }
            return true;
        }

        CDebug.Log(CDebug.buildModeAbilities, "Returned false from MoveAbility ExecuteAbility");

        return false;
    }

    public override string UIText()
    {
        throw new NotImplementedException();
    }

    public override bool IsTargetSuitable(Coord fromCoord, Coord targetCoord)
    {
        if (!base.IsTargetSuitable(fromCoord, targetCoord))
        {
            return false;
        }

        return MapGenerator.Instance.Map.GetHexAt(targetCoord).HexType.UnitCanOccupy;
    }
}
