using UnityEngine;

namespace GrimoireTD.Economy
{
    [CreateAssetMenu(fileName = "NewResource", menuName = "Economy/Resource")]
    public class SoResourceTemplate : ScriptableObject, IResourceTemplate
    {
        [SerializeField]
        private string nameInGame;

        [SerializeField]
        private string shortName;

        [SerializeField]
        private int maxAmount;

        public string NameInGame
        {
            get
            {
                return nameInGame;
            }
        }

        public string ShortName
        {
            get
            {
                return shortName;
            }
        }

        public int MaxAmount
        {
            get
            {
                return maxAmount;
            }
        }

        public IResource GenerateResource()
        {
            return new CResource(this);
        }
    }
}