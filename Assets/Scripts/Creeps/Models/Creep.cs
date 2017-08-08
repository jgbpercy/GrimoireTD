using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class Creep : IDefendModeTargetable, IFrameUpdatee {

    public class PersistentEffect
    {
        private int magnitude;

        private float duration;
        private float elapsed;

        private AttackEffectType attackEffectType;

        private string effectName;

        public int Magnitude
        {
            get
            {
                return magnitude;
            }
        }

        public float Duration
        {
            get
            {
                return duration;
            }
        }

        public float Elapsed
        {
            get
            {
                return elapsed;
            }
        }

        public AttackEffectType AttackEffectType
        {
            get
            {
                return attackEffectType;
            }
        }

        public string EffectName
        {
            get
            {
                return effectName;
            }
        }

        public PersistentEffect(AttackEffect attackEffect, int actualMagnitude, float actualDuration)
        {
            magnitude = actualMagnitude;
            duration = actualDuration;
            attackEffectType = attackEffect.AttackEffectType;
            effectName = attackEffect.EffectName;
            elapsed = 0f;
        }

        public void ApplyNewEffect(AttackEffect attackEffect, int actualMagnitude, float actualDuration)
        {
            Assert.IsTrue(attackEffect.AttackEffectType == AttackEffectType);

            magnitude = actualMagnitude;
            duration = actualDuration;
            elapsed = 0f;
        }

        public bool HasElapsed()
        {
            return elapsed >= duration;
        }

        public void IncreaseElapsed()
        {
            elapsed += Time.deltaTime;
        }

        public float TimeRemaining()
        {
            return duration - elapsed;
        }
    }

    private int id;

    private const float physicalResistFromArmor = 0.05f;
    private const float elementalResistFromArmor = 0.03f;

    private Vector3 currentDestinationVector;
    private int currentDestinationPathNode;

    private float currentSpeed;
    private int currentHitpoints;
    private int currentPermanentMinusArmor;

    private Vector3 position;
    private float distanceFromEnd;

    private ICreepTemplate creepTemplate;

    private List<PersistentEffect> persistentEffects;

    private Action OnHealthChangedCallback;
    private Action OnDiedCallback;

    private Action<PersistentEffect> OnNewPersistentEffectCallback;
    private Action<int> OnPersistentEffectExpiredCallback;

    public string Id
    {
        get
        {
            return "C-" + id;
        }
    }

    public string GetId()
    {
        return Id;
    }

    public string GetName()
    {
        return creepTemplate.NameInGame;
    }

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

    public ICreepTemplate CreepTemplate
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

    public IReadOnlyList<PersistentEffect> PersistentEffects
    {
        get
        {
            return persistentEffects;
        }
    }

    public Creep(ICreepTemplate template, Vector3 spawnPosition)
    {
        id = IdGen.GetNextId();

        currentSpeed = template.BaseSpeed;
        currentHitpoints = template.MaxHitpoints;
        currentPermanentMinusArmor = 0;

        position = spawnPosition;

        currentDestinationPathNode = MapGenerator.Instance.Map.CreepPath.Count - 2;
        currentDestinationVector = MapGenerator.Instance.Map.CreepPath[currentDestinationPathNode].ToPositionVector();

        creepTemplate = template;

        persistentEffects = new List<PersistentEffect>();

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
            currentDestinationVector = MapGenerator.Instance.Map.CreepPath[currentDestinationPathNode].ToPositionVector();
        }

        position = Vector3.MoveTowards(position, currentDestinationVector, currentSpeed * Time.deltaTime);

        bool speedChanged = false;

        //TODO: make persistent effects a callback list?
        for (int i = 0; i < persistentEffects.Count; i++)
        {
            persistentEffects[i].IncreaseElapsed();

            if ( persistentEffects[i].HasElapsed())
            {
                CDebug.Log(CDebug.combatLog, Id + " persistent effect ended: " + persistentEffects[i].EffectName);

                OnPersistentEffectExpiredCallback?.Invoke(i);

                if (persistentEffects[i].AttackEffectType is SlowEffectType )
                {
                    speedChanged = true;
                }
            }
        }

        persistentEffects.RemoveAll(x => x.HasElapsed());

        if (speedChanged)
        {
            OnSpeedChange();
        }
    }

    public void ApplyAttackEffects(IEnumerable<AttackEffect> attackEffects, DefendingEntity sourceDefendingEntity)
    {
        CDebug.Log(CDebug.combatLog, Id + " receiving attack effects:");

        foreach (AttackEffect attackEffect in attackEffects)
        {
            CDebug.Log(CDebug.combatLog, "Applying attack effect: " + attackEffect.EffectName + 
                ", base magnitude: "  + attackEffect.BaseMagnitude + 
                ", base duration: " + attackEffect.BaseDuration +
                ", actual magnitude: " + attackEffect.GetActualMagnitude(sourceDefendingEntity) +
                ", actual duration: " + attackEffect.GetActualDuration(sourceDefendingEntity)
                );

            DamageEffectType damageEffectType = attackEffect.AttackEffectType as DamageEffectType;
            if ( damageEffectType != null )
            {
                ApplyDamageEffect(attackEffect, sourceDefendingEntity, damageEffectType);
                continue;
            }

            PersistentEffectType persistentEffectType = attackEffect.AttackEffectType as PersistentEffectType;
            if (persistentEffectType != null)
            {
                ApplyPersistentEffect(attackEffect, sourceDefendingEntity, persistentEffectType);
                continue;
            }

            InstantEffectType instantEffectType = attackEffect.AttackEffectType as InstantEffectType;
            if ( instantEffectType != null)
            {
                ApplyInstantEffect(attackEffect, sourceDefendingEntity);
                continue;
            }

            throw new Exception("Unhandled AttackEffectType");
        }
    }

    private void ApplyDamageEffect(AttackEffect attackEffect, DefendingEntity sourceDefendingEntity, DamageEffectType damageEffectType)
    {
        float magnitude = attackEffect.GetActualMagnitude(sourceDefendingEntity);
        int block = creepTemplate.Resistances.GetBlock(damageEffectType);
        float resistance = creepTemplate.Resistances.GetResistance(damageEffectType, GetCurrentMinusArmor());

        CDebug.Log(CDebug.combatLog, "Bl: " + block + ", Res: " + resistance);

        TakeDamage(
            Mathf.RoundToInt(
                (magnitude - block) * (1 - resistance)
            )
        );
    }

    private int GetCurrentEffectMagnitude(AttackEffectType effectType)
    {
        PersistentEffect currentEffect = persistentEffects.Find(x => x.AttackEffectType == effectType);

        if (currentEffect != null)
        {
            return currentEffect.Magnitude;
        }

        return 0;
    }

    private void ApplyPersistentEffect(AttackEffect attackEffect, DefendingEntity sourceDefendingEntity, PersistentEffectType persistentEffectType)
    {
        PersistentEffect currentEffect = persistentEffects.Find(x => x.AttackEffectType == attackEffect.AttackEffectType);

        int newEffectMagnitude = Mathf.RoundToInt(attackEffect.GetActualMagnitude(sourceDefendingEntity));
        float newEffectDuration = attackEffect.GetActualDuration(sourceDefendingEntity);

        bool isSpeedChange = persistentEffectType is SlowEffectType;

        if (currentEffect == null)
        {
            CDebug.Log(CDebug.combatLog, "No current effect, applying effect");

            PersistentEffect newPersistentEffect = new PersistentEffect(attackEffect, newEffectMagnitude, newEffectDuration);

            persistentEffects.Add(newPersistentEffect);

            OnNewPersistentEffectCallback?.Invoke(newPersistentEffect);

            if (isSpeedChange) { OnSpeedChange(); }

            return;
        }

        if (currentEffect.Magnitude < newEffectMagnitude)
        {
            currentEffect.ApplyNewEffect(attackEffect, newEffectMagnitude, newEffectDuration);

            CDebug.Log(CDebug.combatLog, "Current effect with lower magnitude, applying effect");

            if (isSpeedChange) { OnSpeedChange(); }

            return;
        }
        else if (currentEffect.Magnitude == newEffectMagnitude)
        {
            if ( currentEffect.TimeRemaining() < newEffectDuration)
            {
                currentEffect.ApplyNewEffect(attackEffect, newEffectMagnitude, newEffectDuration);

                CDebug.Log(CDebug.combatLog, "Current effect with same magnitude and lower remaining duration, applying effect");

                if (isSpeedChange) { OnSpeedChange(); }

                return;
            }
        }

        CDebug.Log(CDebug.combatLog, "Current effect already better than new effect");
    }

    private void ApplyInstantEffect(AttackEffect attackEffect, DefendingEntity sourceDefendingEntity)
    {
        ArmorCorrosionEffectType armorCorrosionEffect = attackEffect.AttackEffectType as ArmorCorrosionEffectType;
        if( armorCorrosionEffect != null )
        {
            currentPermanentMinusArmor += Mathf.RoundToInt(attackEffect.GetActualMagnitude(sourceDefendingEntity));
            CDebug.Log(CDebug.combatLog, Id + " permanent minus armor is now " + currentPermanentMinusArmor);
            return;
        }

        throw new Exception("Unhandled InstantEffectType");
    }

    private int GetCurrentMinusArmor()
    {
        int minusArmor = 0;

        IReadOnlyList<ArmorReductionEffectType> armorReductionTypes = AttackEffectTypeManager.Instance.ArmorReductionTypes;

        foreach (PersistentEffect persistentEffect in persistentEffects)
        {
            if (armorReductionTypes.Contains(persistentEffect.AttackEffectType))
            {
                minusArmor += persistentEffect.Magnitude;
            }
        }

        minusArmor += currentPermanentMinusArmor;

        CDebug.Log(CDebug.combatLog, Id + " current minus armor is " + minusArmor);

        return minusArmor;
    }

    private void TakeDamage(int damage)
    {
        currentHitpoints -= damage;

        CDebug.Log(CDebug.combatLog, Id + " takes " + damage + " damage, leaving " + CurrentHitpoints);

        if ( currentHitpoints < 1 )
        {
            currentHitpoints = 0;
        }

        OnHealthChangedCallback();

        if (currentHitpoints == 0)
        {
            CDebug.Log(CDebug.combatLog, Id + " died");
            OnDiedCallback();
            CreepTemplate.Bounty.DoTransaction();
        }
    }

    private void OnSpeedChange()
    {
        float speedFactor = 1;

        foreach (SlowEffectType slowEffectType in AttackEffectTypeManager.Instance.SlowTypes)
        {
            float speedFactorForType = (1 - GetCurrentEffectMagnitude(slowEffectType) / 100f);
            speedFactor *= speedFactorForType;
            CDebug.Log(CDebug.combatLog, "Speed change: " + slowEffectType.EffectName() + " applying speed factor of " + speedFactorForType + ". Overall factor now " + speedFactor);
        }
        
        currentSpeed = creepTemplate.BaseSpeed * speedFactor;

        CDebug.Log(CDebug.combatLog, Id + " current speed is " + currentSpeed + " = " + creepTemplate.BaseSpeed + " * " + speedFactor);
    }

    //TODO: make callback? Or change? Seems bad
    public void GameObjectDestroyed()
    {
        ModelObjectFrameUpdater.Instance.DeregisterAsModelObjectFrameUpdatee(this);
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

    public void RegisterForOnNewPersistentEffectCallback(Action<PersistentEffect> callback)
    {
        OnNewPersistentEffectCallback += callback;
    }

    public void DeregisterForOnNewPersistentEffectCallback(Action<PersistentEffect> callback)
    {
        OnNewPersistentEffectCallback -= callback;
    }

    public void RegisterForOnPersistentEffectExpiredCallback(Action<int> callback)
    {
        OnPersistentEffectExpiredCallback += callback;
    }

    public void DeregisterForOnPersistentEffectExpiredCallback(Action<int> callback)
    {
        OnPersistentEffectExpiredCallback -= callback;
    }
}
