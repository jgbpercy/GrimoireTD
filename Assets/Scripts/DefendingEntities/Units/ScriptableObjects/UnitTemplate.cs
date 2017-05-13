﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewUnit", menuName = "Structures and Units/Unit")]
public class UnitTemplate : DefendingEntityTemplate {

    [SerializeField]
    private int experienceToLevelUp = 100;

    [SerializeField]
    private UnitTalent[] unitTalents;

    public int ExperienceToLevelUp
    {
        get
        {
            return experienceToLevelUp;
        }
    }

    public UnitTalent[] UnitTalents
    {
        get
        {
            return unitTalents;
        }
    }

    public virtual Unit GenerateUnit(Vector3 position)
    {
        return new Unit(this, position);
    }
}
