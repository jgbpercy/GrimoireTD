using UnityEngine;
using System;

public class Unit : DefendingEntity
{
    private UnitTemplate unitTemplate;

    private Action<Coord> onMovedCallback;

    public UnitTemplate UnitClassTemplate
    {
        get
        {
            return unitTemplate;
        }
    }

    public Action<Coord> OnMovedCallback
    {
        get
        {
            return onMovedCallback;
        }
    }

    public Unit(UnitTemplate unitTemplate, Vector3 position) : base(unitTemplate)
    {
        this.unitTemplate = unitTemplate;

        DefendingEntityView.Instance.CreateUnit(this, position);
    }

    public override string UIText()
    {
        return unitTemplate.Description;
    }

    public void RegisterForOnMovedCallback(Action<Coord> callback)
    {
        onMovedCallback += callback;
    }

    public void DeregisterForOnMovedCallback(Action<Coord> callback)
    {
        onMovedCallback -= callback;
    }

}
