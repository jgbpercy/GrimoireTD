using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;

public class Creep : ITargetable, IFrameUpdatee {

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

        public PersistentEffect(AttackEffectType attackEffectType, int magnitude, float duration)
        {
            this.magnitude = magnitude;
            this.duration = duration;
            this.attackEffectType = attackEffectType;
            effectName = AttackEffectNames.NameOf(attackEffectType);
            elapsed = 0f;
        }

        public PersistentEffect(AttackEffect attackEffect)
        {
            magnitude = attackEffect.Magnitude;
            duration = attackEffect.Duration;
            attackEffectType = attackEffect.AttackEffectType;
            effectName = attackEffect.EffectName;
            elapsed = 0f;
        }

        public void ApplyNewEffect(AttackEffect attackEffect)
        {
            Assert.IsTrue(attackEffect.AttackEffectType == this.AttackEffectType);
            magnitude = attackEffect.Magnitude;
            duration = attackEffect.Duration;
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

    private CreepTemplate creepTemplate;

    private List<PersistentEffect> persistentEffects = new List<PersistentEffect>();

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

    public CreepTemplate CreepTemplate
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

    public List<PersistentEffect> PersistentEffects
    {
        get
        {
            return persistentEffects;
        }
    }

    public Creep(CreepTemplate template, Vector3 spawnPosition)
    {
        id = IdGen.GetNextId();

        currentSpeed = template.BaseSpeed;
        currentHitpoints = template.MaxHitpoints;
        currentPermanentMinusArmor = 0;

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

        bool speedChanged = false;

        for (int i = 0; i < persistentEffects.Count; i++)
            {
            persistentEffects[i].IncreaseElapsed();

            if ( persistentEffects[i].HasElapsed())
            {
                CDebug.Log(CDebug.combatLog, Id + " persistent effect ended: " + persistentEffects[i].EffectName);
                
                if ( OnPersistentEffectExpiredCallback != null )
                {
                    OnPersistentEffectExpiredCallback(i);
                }

                if (persistentEffects[i].AttackEffectType == AttackEffectType.FrostSlow || persistentEffects[i].AttackEffectType == AttackEffectType.MaimSlow || persistentEffects[i].AttackEffectType == AttackEffectType.TrapSlow || persistentEffects[i].AttackEffectType == AttackEffectType.DazeSlow )
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

    public void ApplyAttackEffects(AttackEffect[] attackEffects)
    {
        CDebug.Log(CDebug.combatLog, Id + " receiving " + attackEffects.Length + " attack effects:");

        foreach (AttackEffect attackEffect in attackEffects)
        {
            CDebug.Log(CDebug.combatLog, "Applying attack effect: " + attackEffect.EffectName + ", magnitude: "  + attackEffect.Magnitude + ", duration: " + attackEffect.Duration);

            switch (attackEffect.AttackEffectType)
            {
                case AttackEffectType.PiercingDamage:
                    CDebug.Log(CDebug.combatLog, "Dam Bl: " + creepTemplate.Resistances.DamageBlock + ", Phy Bl: " + creepTemplate.Resistances.PhysicalBlock + ", Pi Bl: " + creepTemplate.Resistances.PiercingBlock + ", Ar Fac: " + physicalResistFromArmor + ", Ar: " + GetActualArmor() + ", Phy Res: " + creepTemplate.Resistances.BasePhysicalResistance + ", Pi Res: " + creepTemplate.Resistances.PiercingResistance);
                    TakeDamage(Mathf.FloorToInt((attackEffect.Magnitude - creepTemplate.Resistances.DamageBlock - creepTemplate.Resistances.PhysicalBlock - creepTemplate.Resistances.PiercingBlock) * (1 - GetActualResistance(physicalResistFromArmor, creepTemplate.Resistances.BasePhysicalResistance, creepTemplate.Resistances.PiercingResistance))));
                    break;
                case AttackEffectType.BluntDamage:
                    CDebug.Log(CDebug.combatLog, "Dam Bl: " + creepTemplate.Resistances.DamageBlock + ", Phy Bl: " + creepTemplate.Resistances.PhysicalBlock + ", Bl Bl: " + creepTemplate.Resistances.BluntBlock + ", Ar Fac: " + physicalResistFromArmor + ", Ar: " + GetActualArmor() + ", Phy Res: " + creepTemplate.Resistances.BasePhysicalResistance + ", Bl Res: " + creepTemplate.Resistances.BluntResistance);
                    TakeDamage(Mathf.FloorToInt((attackEffect.Magnitude - creepTemplate.Resistances.DamageBlock - creepTemplate.Resistances.PhysicalBlock - creepTemplate.Resistances.BluntBlock) * (1 - GetActualResistance(physicalResistFromArmor, creepTemplate.Resistances.BasePhysicalResistance, creepTemplate.Resistances.BluntResistance))));
                    break;
                case AttackEffectType.PoisonDamage:
                    CDebug.Log(CDebug.combatLog, "Dam Bl: " + creepTemplate.Resistances.DamageBlock + ", Phy Bl: " + creepTemplate.Resistances.PhysicalBlock + ", Po Bl: " + creepTemplate.Resistances.PoisonBlock + ", Ar Fac: " + physicalResistFromArmor + ", Ar: " + GetActualArmor() + ", Phy Res: " + creepTemplate.Resistances.BasePhysicalResistance + ", Po Res: " + creepTemplate.Resistances.PoisonReistance);
                    TakeDamage(Mathf.FloorToInt((attackEffect.Magnitude - creepTemplate.Resistances.DamageBlock - creepTemplate.Resistances.PhysicalBlock - creepTemplate.Resistances.PoisonBlock) * (1 - GetActualResistance(physicalResistFromArmor, creepTemplate.Resistances.BasePhysicalResistance, creepTemplate.Resistances.PoisonReistance))));
                    break;
                case AttackEffectType.AcidDamage:
                    CDebug.Log(CDebug.combatLog, "Dam Bl: " + creepTemplate.Resistances.DamageBlock + ", Phy Bl: " + creepTemplate.Resistances.PhysicalBlock + ", Ac Bl: " + creepTemplate.Resistances.AcidBlock + ", Ar Fac: " + physicalResistFromArmor + ", Ar: " + GetActualArmor() + ", Phy Res: " + creepTemplate.Resistances.BasePhysicalResistance + ", Ac Res: " + creepTemplate.Resistances.AcidResistance);
                    TakeDamage(Mathf.FloorToInt((attackEffect.Magnitude - creepTemplate.Resistances.DamageBlock - creepTemplate.Resistances.PhysicalBlock - creepTemplate.Resistances.AcidBlock) * (1 - GetActualResistance(physicalResistFromArmor, creepTemplate.Resistances.BasePhysicalResistance, creepTemplate.Resistances.AcidResistance))));
                    break;
                case AttackEffectType.FireDamage:
                    CDebug.Log(CDebug.combatLog, "Dam Bl: " + creepTemplate.Resistances.DamageBlock + ", El Bl: " + creepTemplate.Resistances.ElementalBlock + ", Fi Bl: " + creepTemplate.Resistances.FireBlock + ", Ar Fac: " + elementalResistFromArmor + ", Ar: " + GetActualArmor() + ", El Res: " + creepTemplate.Resistances.BaseElementalResistance + ", Fi Res: " + creepTemplate.Resistances.FireResistance);
                    TakeDamage(Mathf.FloorToInt((attackEffect.Magnitude - creepTemplate.Resistances.DamageBlock - creepTemplate.Resistances.ElementalBlock - creepTemplate.Resistances.FireBlock) * (1 - GetActualResistance(elementalResistFromArmor, creepTemplate.Resistances.BaseElementalResistance, creepTemplate.Resistances.FireResistance))));
                    break;
                case AttackEffectType.ColdDamage:
                    CDebug.Log(CDebug.combatLog, "Dam Bl: " + creepTemplate.Resistances.DamageBlock + ", El Bl: " + creepTemplate.Resistances.ElementalBlock + ", Co Bl: " + creepTemplate.Resistances.ColdBlock + ", Ar Fac: " + elementalResistFromArmor + ", Ar: " + GetActualArmor() + ", El Res: " + creepTemplate.Resistances.BaseElementalResistance + ", Co Res: " + creepTemplate.Resistances.ColdResistance);
                    TakeDamage(Mathf.FloorToInt((attackEffect.Magnitude - creepTemplate.Resistances.DamageBlock - creepTemplate.Resistances.ElementalBlock - creepTemplate.Resistances.ColdBlock) * (1 - GetActualResistance(elementalResistFromArmor, creepTemplate.Resistances.BaseElementalResistance, creepTemplate.Resistances.ColdResistance))));
                    break;
                case AttackEffectType.LightningDamage:
                    CDebug.Log(CDebug.combatLog, "Dam Bl: " + creepTemplate.Resistances.DamageBlock + ", El Bl: " + creepTemplate.Resistances.ElementalBlock + ", Li Bl: " + creepTemplate.Resistances.LightningBlock + ", Ar Fac: " + elementalResistFromArmor + ", Ar: " + GetActualArmor() + ", El Res: " + creepTemplate.Resistances.BaseElementalResistance + ", Li Res: " + creepTemplate.Resistances.LightningResistance);
                    TakeDamage(Mathf.FloorToInt((attackEffect.Magnitude - creepTemplate.Resistances.DamageBlock - creepTemplate.Resistances.ElementalBlock - creepTemplate.Resistances.LightningBlock) * (1 - GetActualResistance(elementalResistFromArmor, creepTemplate.Resistances.BaseElementalResistance, creepTemplate.Resistances.LightningResistance))));
                    break;
                case AttackEffectType.EarthDamage:
                    CDebug.Log(CDebug.combatLog, "Dam Bl: " + creepTemplate.Resistances.DamageBlock + ", El Bl: " + creepTemplate.Resistances.ElementalBlock + ", Ea Bl: " + creepTemplate.Resistances.EarthBlock + ", Ar Fac: " + elementalResistFromArmor + ", Ar: " + GetActualArmor() + ", El Res: " + creepTemplate.Resistances.BaseElementalResistance + ", Ea Res: " + creepTemplate.Resistances.EarthResistance);
                    TakeDamage(Mathf.FloorToInt((attackEffect.Magnitude - creepTemplate.Resistances.DamageBlock - creepTemplate.Resistances.ElementalBlock - creepTemplate.Resistances.EarthBlock) * (1 - GetActualResistance(elementalResistFromArmor, creepTemplate.Resistances.BaseElementalResistance, creepTemplate.Resistances.EarthResistance))));
                    break;
                case AttackEffectType.FrostSlow:
                    ApplyPersistentEffect(attackEffect);
                    OnSpeedChange();
                    break;
                case AttackEffectType.MaimSlow:
                    ApplyPersistentEffect(attackEffect);
                    OnSpeedChange();
                    break;
                case AttackEffectType.TrapSlow:
                    ApplyPersistentEffect(attackEffect);
                    OnSpeedChange();
                    break;
                case AttackEffectType.DazeSlow:
                    ApplyPersistentEffect(attackEffect);
                    OnSpeedChange();
                    break;
                case AttackEffectType.ArmorReduction:
                    ApplyPersistentEffect(attackEffect);
                    break;
                case AttackEffectType.ArmorCorrosion:
                    currentPermanentMinusArmor += attackEffect.Magnitude;
                    CDebug.Log(CDebug.combatLog, Id + " permanent minus armor is now " + currentPermanentMinusArmor);
                    break;
            }
        }
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

    private void ApplyPersistentEffect(AttackEffect attackEffect)
    {
        PersistentEffect currentEffect = persistentEffects.Find(x => x.AttackEffectType == attackEffect.AttackEffectType);

        if (currentEffect == null)
        {
            CDebug.Log(CDebug.combatLog, "No current effect, applying effect");

            PersistentEffect newPersistentEffect = new PersistentEffect(attackEffect);

            persistentEffects.Add(newPersistentEffect);

            if ( OnNewPersistentEffectCallback != null )
            {
                OnNewPersistentEffectCallback(newPersistentEffect);
            }

            return;
        }

        if (currentEffect.Magnitude < attackEffect.Magnitude)
        {
            currentEffect.ApplyNewEffect(attackEffect);
            CDebug.Log(CDebug.combatLog, "Current effect with lower magnitude, applying effect");
            return;
        }
        else if (currentEffect.Magnitude == attackEffect.Magnitude)
        {
            if ( currentEffect.TimeRemaining() < attackEffect.Duration )
            {
                currentEffect.ApplyNewEffect(attackEffect);
                CDebug.Log(CDebug.combatLog, "Current effect with same magnitude and lower remaining duration, applying effect");
                return;
            }
        }

        CDebug.Log(CDebug.combatLog, "Current effect already better than new effect");
    }

    private float GetActualResistance(float resistanceFromArmor, float baseResistance, float specificResistance)
    {
        float actualResistance = 0f;

        for (int i = 0; i < GetActualArmor(); i++)
        {
            actualResistance = actualResistance + (1 - actualResistance) * resistanceFromArmor;
        }

        actualResistance = actualResistance + (1 - actualResistance) * baseResistance;
        actualResistance = actualResistance + (1 - actualResistance) * specificResistance;

        CDebug.Log(CDebug.combatLog, "Calculated actual resistance as " + actualResistance);
        return actualResistance;
    }

    private int GetActualArmor()
    {
        int minusArmor = GetCurrentEffectMagnitude(AttackEffectType.ArmorCorrosion) + currentPermanentMinusArmor;
        return CreepTemplate.Resistances.Armor - minusArmor;
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
        }

    }

    private void OnSpeedChange()
    {
        float frostSlowFactor = (1 - (GetCurrentEffectMagnitude(AttackEffectType.FrostSlow) / 100f));
        float maimSlowFactor = (1 - (GetCurrentEffectMagnitude(AttackEffectType.MaimSlow) / 100f));
        float trapSlowFactor = (1 - (GetCurrentEffectMagnitude(AttackEffectType.TrapSlow) / 100f));
        float dazeSlowFactor = (1 - (GetCurrentEffectMagnitude(AttackEffectType.DazeSlow) / 100f));

        currentSpeed = creepTemplate.BaseSpeed * frostSlowFactor * maimSlowFactor * trapSlowFactor * dazeSlowFactor;

        CDebug.Log(CDebug.combatLog, Id + " current speed is " + currentSpeed + " = " + creepTemplate.BaseSpeed + " * " + frostSlowFactor + " * " + maimSlowFactor + " * " + trapSlowFactor + " * " + dazeSlowFactor);
    }

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
