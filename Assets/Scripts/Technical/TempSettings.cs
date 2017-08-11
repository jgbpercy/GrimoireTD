using UnityEngine;

namespace GrimoireTD.Technical
{
    public class TempSettings : SingletonMonobehaviour<TempSettings>
    {
        [SerializeField]
        private float trackIdleTimeAfterSpawns;

        [SerializeField]
        private float unitFatigueFactorInfelctionPoint;
        [SerializeField]
        private float unitFatigueFactorShallownessMultiplier;

        public float TrackIdleTimeAfterSpawns
        {
            get
            {
                return trackIdleTimeAfterSpawns;
            }
        }

        public float UnitFatigueFactorInfelctionPoint
        {
            get
            {
                return unitFatigueFactorInfelctionPoint;
            }
        }

        public float UnitFatigueFactorShallownessMultiplier
        {
            get
            {
                return unitFatigueFactorShallownessMultiplier;
            }
        }
    }
}