﻿using System;
using System.Collections.Generic;
using UnityEngine;
using GrimoireTD.Map;

namespace GrimoireTD.Abilities.BuildMode
{
    public class SoBuildModeTargetingComponent : ScriptableObject, IBuildModeTargetingComponent
    {
        [SerializeField]
        protected int range;

        public int Range
        {
            get
            {
                return range;
            }
        }

        public virtual IReadOnlyList<IBuildModeTargetable> FindTargets(Coord position)
        {
            throw new NotImplementedException("Base BMTargetingComponent cannot find targets and should not be used.");
        }
    }
}