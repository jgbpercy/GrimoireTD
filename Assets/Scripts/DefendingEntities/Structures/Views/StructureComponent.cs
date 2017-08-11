﻿using UnityEngine;

namespace GrimoireTD.DefendingEntities.Structures
{
    public class StructureComponent : MonoBehaviour
    {
        private Structure structureModel;

        public Structure StructureModel
        {
            get
            {
                return structureModel;
            }
        }

        public void SetUp(Structure structureModel)
        {
            this.structureModel = structureModel;
        }
    }
}