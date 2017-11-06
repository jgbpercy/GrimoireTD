using UnityEngine;

namespace GrimoireTD.Defenders.DefenderEffects
{
    public class SoDefenderEffectTemplate : ScriptableObject, IDefenderEffectTemplate
    {
        [SerializeField]
        private DefenderEffectAffectsType affects;

        [SerializeField]
        private string nameInGame;

        [SerializeField]
        private SoDefenderImprovement improvement;

        public DefenderEffectAffectsType Affects
        {
            get
            {
                return affects;
            }
        }

        public string NameInGame
        {
            get
            {
                return nameInGame;
            }
        }

        public IDefenderImprovement Improvement
        {
            get
            {
                return improvement;
            }
        }
    }
}