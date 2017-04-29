using System;
using UnityEngine;

public class Creep : ITargetable, IFrameUpdatee {

    private Vector3 currentDestinationVector;
    private int currentDestinationPathNode;

    private float currentSpeed;

    private int currentHitpoints;

    private Vector3 position;
    private float distanceFromEnd;

    private CreepTemplate creepTemplate;

    private Action OnHealthChangedCallback;
    private Action OnDiedCallback;

    public float CurrentSpeed
    {
        get
        {
            return currentSpeed;
        }
    }

    public int CurrentHitpoints
    {
        get
        {
            return currentHitpoints;
        }
    }

    public Vector3 Position
    {
        get
        {
            return position;
        }
    }

    public CreepTemplate CreepClassTemplate
    {
        get
        {
            return creepTemplate;
        }
    }

    public float DistanceFromEnd
    {
        get
        {
            return distanceFromEnd;
        }
    }

    public Creep(CreepTemplate template, Vector3 spawnPosition)
    {
        currentSpeed = template.BaseSpeed;
        currentHitpoints = template.MaxHitpoints;

        position = spawnPosition;

        currentDestinationPathNode = MapGenerator.Instance.Map.Path.Count - 2;
        currentDestinationVector = MapGenerator.Instance.Map.Path[currentDestinationPathNode].ToPositionVector();

        creepTemplate = template;

        ModelObjectFrameUpdater.Instance.RegisterAsModelObjectFrameUpdatee(this);

        CreepView.Instance.CreateCreep(this);
    }

    public Vector3 TargetPosition()
    {
        //TODO unhardcode this height
        return new Vector3(position.x, position.y, -0.4f);
    }

    public void ModelObjectFrameUpdate()
    {
        float distanceFromCurrentDestination = Vector3.Magnitude(position - currentDestinationVector);

        distanceFromEnd = currentDestinationPathNode * MapRenderer.HEX_OFFSET + distanceFromCurrentDestination;

        if (distanceFromCurrentDestination < currentSpeed * Time.deltaTime)
        {
            currentDestinationPathNode = currentDestinationPathNode - 1 < 0 ? 0 : currentDestinationPathNode - 1;
            currentDestinationVector = MapGenerator.Instance.Map.Path[currentDestinationPathNode].ToPositionVector();
        }

        position = Vector3.MoveTowards(position, currentDestinationVector, currentSpeed * Time.deltaTime);
    }

    public void TakeDamage(int damage)
    {
        currentHitpoints -= damage;

        if ( currentHitpoints < 1 )
        {
            currentHitpoints = 0;
        }

        OnHealthChangedCallback();

        if (currentHitpoints == 0)
        {
            OnDiedCallback();
        }

    }

    public void RegisterForOnHealthChangedCallback(Action callback)
    {
        OnHealthChangedCallback += callback;
    }

    public void DeregisterForOnHealthChangedCallback(Action callback)
    {
        OnHealthChangedCallback -= callback;
    }

    public void RegisterForOnDiedCallback(Action callback)
    {
        OnDiedCallback += callback;
    }

    public void DeregisterForOnDiedCallback(Action callback)
    {
        OnDiedCallback -= callback;
    }

    public void GameObjectDestroyed()
    {
        ModelObjectFrameUpdater.Instance.DeregisterAsModelObjectFrameUpdatee(this);
    }
}
