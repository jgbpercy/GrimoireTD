using UnityEngine;
using UnityEngine.UI;
using GrimoireTD.UI;

namespace GrimoireTD.Defenders.Structures
{
    public class SelectStructureToBuildUIElement : MonoBehaviour
    {
        private IStructureTemplate structureTemplate;

        private bool initialised = false;

        [SerializeField]
        private Text structureNameText;
        [SerializeField]
        private Text structureDescriptionText;

        public void SetUp(IStructureTemplate structureTemplate)
        {
            if (initialised)
            {
                return;
            }
            initialised = true;

            this.structureTemplate = structureTemplate;

            structureNameText.text = structureTemplate.StartingNameInGame;
            structureDescriptionText.text = structureTemplate.UIText();
        }

        public void SendClickToInterfaceController()
        {
            InterfaceController.Instance.SelectStructureToBuild(structureTemplate);
        }
    }
}