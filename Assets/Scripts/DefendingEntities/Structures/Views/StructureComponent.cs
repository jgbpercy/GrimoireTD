﻿using UnityEngine;

namespace GrimoireTD.DefendingEntities.Structures
{
    public class StructureComponent : MonoBehaviour
    {
        private IStructure structureModel;

        public IStructure StructureModel
        {
            get
            {
                return structureModel;
            }
        }

        public void SetUp(IStructure structureModel)
        {
            this.structureModel = structureModel;
        }
    }
}