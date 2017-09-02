using UnityEngine;
using UnityEngine.UI;
using GrimoireTD.Economy;
using GrimoireTD.UI;

namespace GrimoireTD.DefendingEntities.Structures
{
    public class StructureEnhancementUIComponent : MonoBehaviour
    {
        private IStructure selectedStructure;

        private IStructureUpgrade upgrade;

        private IStructureEnhancement enhancement;

        private bool initialised = false;

        [SerializeField]
        private Text ownText;
        [SerializeField]
        private Text ownCostText;
        [SerializeField]
        private Color boughtColor;
        [SerializeField]
        private Color notBoughtColor;

        public void SetUp(IStructure structure, IStructureUpgrade upgrade, IStructureEnhancement enhancement)
        {
            if (initialised)
            {
                return;
            }
            initialised = true;

            selectedStructure = structure;
            this.upgrade = upgrade;
            this.enhancement = enhancement;

            SetDisplay();

            selectedStructure.OnUpgraded += OnStructureUpgraded;
        }

        private void SetDisplay()
        {
            ownText.text = enhancement.DescriptionText;
            ownCostText.text = enhancement.Cost.ToString(EconomyTransactionStringFormat.ShortNameLineBreaks, true);

            if (selectedStructure.EnhancementsChosen[enhancement] == true)
            {
                gameObject.GetComponent<Image>().color = boughtColor;
            }
            else
            {
                gameObject.GetComponent<Image>().color = notBoughtColor;
            }
        }

        private void OnStructureUpgraded(object sender, EAOnUpgraded args)
        {
            SetDisplay();
        }

        public void UIComponentClicked()
        {
            InterfaceController.Instance.ClickStructureEnhancement(selectedStructure, upgrade, enhancement);
        }

        private void OnDestroy()
        {
            if (selectedStructure != null)
            {
                selectedStructure.OnUpgraded -= OnStructureUpgraded;
            }
        }
    }
}