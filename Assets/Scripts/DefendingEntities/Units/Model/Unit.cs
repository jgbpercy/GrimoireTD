using UnityEngine;
using System;

public class Unit : DefendingEntity
{
    private UnitTemplate unitTemplate;

    private Action<Coord> onMovedCallback;

    private float timeIdle = 0f;
    private float timeActive = 0f;

    private int experience = 0;
    private int fatigue = 0;

    private Action OnExperienceFatigueChangedCallback;

    public override string Id
    {
        get
        {
            return "U-" + id;
        }
    }

    public UnitTemplate UnitTemplate
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

    public float TimeIdle
    {
        get
        {
            return timeIdle;
        }
    }

    public float TimeActive
    {
        get
        {
            return timeActive;
        }
    }

    public int Experience
    {
        get
        {
            return experience;
        }
    }

    public int Fatigue
    {
        get
        {
            return fatigue;
        }
    }

    public Unit(UnitTemplate unitTemplate, Vector3 position) : base(unitTemplate)
    {
        id = IdGen.GetNextId();

        this.unitTemplate = unitTemplate;

        DefendingEntityView.Instance.CreateUnit(this, position);

        timeIdle = 0f;
        timeActive = 0f;
        experience = 0;
        fatigue = 0;

        GameStateManager.Instance.RegisterForOnEnterBuildModeCallback(OnEnterBuildMode);
    }

    public override string UIText()
    {
        return unitTemplate.Description;
    }

    public void TrackTime(bool wasIdle, float time)
    {
        if (wasIdle)
        {
            timeIdle += time;
        }
        else
        {
            timeActive += time;
        }
    }

    private void OnEnterBuildMode()
    {
        CDebug.Log(CDebug.experienceAndFatigue, Id + " entered build mode:");
        CDebug.Log(CDebug.experienceAndFatigue, "Time Active: " + timeActive.ToString("0.0"));
        CDebug.Log(CDebug.experienceAndFatigue, "Time Idle: " + timeIdle.ToString("0.0"));

        experience += Mathf.RoundToInt((timeActive / (timeActive + timeIdle)) * 100);

        fatigue += Mathf.RoundToInt((timeActive / (timeActive + timeIdle)) * 10) - 5;

        fatigue = Mathf.Max(fatigue, 0);

        if (OnExperienceFatigueChangedCallback != null)
        {
            OnExperienceFatigueChangedCallback();
        }

        timeIdle = 0f;
        timeActive = 0f;
    }

    public void RegisterForOnMovedCallback(Action<Coord> callback)
    {
        onMovedCallback += callback;
    }

    public void DeregisterForOnMovedCallback(Action<Coord> callback)
    {
        onMovedCallback -= callback;
    }

    public void RegisterForExperienceFatigueChangedCallback(Action callback)
    {
        OnExperienceFatigueChangedCallback += callback;
    }

    public void DeregisterForExperienceFatigueChangedCallback(Action callback)
    {
        OnExperienceFatigueChangedCallback -= callback;
    }
}
