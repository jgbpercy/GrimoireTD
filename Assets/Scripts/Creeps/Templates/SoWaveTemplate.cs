using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GrimoireTD.Creeps
{
    [CreateAssetMenu(fileName = "NewWave", menuName = "Levels/Wave")]
    public class SoWaveTemplate : ScriptableObject, IWaveTemplate
    {
        [SerializeField]
        private SSpawn[] spawns;

        public IReadOnlyList<ISpawn> Spawns
        {
            get
            {
                return spawns.ToList();
            }
        }

        public IWave GenerateWave()
        {
            return new CWave(this);
        }
    }
}