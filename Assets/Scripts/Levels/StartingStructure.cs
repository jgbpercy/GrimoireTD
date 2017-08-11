using System;
using UnityEngine;
using GrimoireTD.DefendingEntities.Structures;
using GrimoireTD.Map;

namespace GrimoireTD.Levels
{
    [Serializable]
    public class StartingStructure
    {
        [SerializeField]
        private SoStructureTemplate structureTemplate;
        [SerializeField]
        private Coord startingPosition;

        public IStructureTemplate StructureTemplate
        {
            get
            {
                return structureTemplate;
            }
        }

        public Coord StartingPosition
        {
            get
            {
                return startingPosition;
            }
        }
    }
}