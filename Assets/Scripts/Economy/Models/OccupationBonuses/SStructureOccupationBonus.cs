﻿using System;
using UnityEngine;
using GrimoireTD.Defenders.Structures;

namespace GrimoireTD.Economy
{
    /*TODO? Make something like an ITileHasThisCheckable interface by which you can generically 
     * query a tile for some criteria. So a single OccupationBonus class would just have a private
     * ITileHasThisCheckable, and Defender.GetHexOccupationBonus would just take an 
     * ITileHasThisCheckable. More bother than it is worth for now; current way is clearer and
     * almost same amount of code
    */
    [Serializable]
    public class SStructureOccupationBonus : SOccupationBonus, IStructureOccupationBonus
    {
        [SerializeField]
        private SoStructureTemplate structureTemplate;

        [SerializeField]
        private SoStructureUpgrade structureUpgradeLevel;

        public IStructureTemplate StructureTemplate
        {
            get
            {
                return structureTemplate;
            }
        }

        public IStructureUpgrade StructureUpgradeLevel
        {
            get
            {
                return structureUpgradeLevel;
            }
        }
    }
}