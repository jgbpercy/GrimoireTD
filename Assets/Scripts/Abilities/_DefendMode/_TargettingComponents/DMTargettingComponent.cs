﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class DMTargetingComponent : ScriptableObject {

    public virtual List<IDefendModeTargetable> FindTargets(DefendingEntity attachedToDefendingEntity)
    {
        throw new NotImplementedException("Base DMTargetingComponent cannot find targets and should not be used.");
    }
}