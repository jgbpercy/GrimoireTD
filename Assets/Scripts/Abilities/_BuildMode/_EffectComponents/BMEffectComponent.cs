﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class BMEffectComponent : ScriptableObject {

    public virtual void ExecuteEffect(DefendingEntity executingEntity, List<IBuildModeTargetable> targets)
    {
        throw new NotImplementedException("Base BMEffectComponent cannot executre effect and should not be used.");
    }
}