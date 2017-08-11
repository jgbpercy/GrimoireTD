using UnityEngine;

namespace GrimoireTD.Map
{
    [CreateAssetMenu(fileName = "NewHexType", menuName = "Hexes/Hex Type")]
    public class SoHexType : ScriptableObject, IHexType
    {
        [SerializeField]
        private string nameInGame;

        [SerializeField]
        private int[] textureOffset;

        //temporary
        [SerializeField]
        private bool isBuildable;

        [SerializeField]
        private bool isPathableByCreeps;

        [SerializeField]
        private bool unitCanOccupy;

        public string NameInGame
        {
            get
            {
                return nameInGame;
            }
        }

        public int TextureOffsetX
        {
            get
            {
                return textureOffset[0];
            }
        }

        public int TextureOffsetY
        {
            get
            {
                return textureOffset[1];
            }
        }

        public bool IsBuildable
        {
            get
            {
                return isBuildable;
            }
        }

        public bool TypeIsPathableByCreeps
        {
            get
            {
                return isPathableByCreeps;
            }
        }

        public bool UnitCanOccupy
        {
            get
            {
                return unitCanOccupy;
            }
        }
    }
}