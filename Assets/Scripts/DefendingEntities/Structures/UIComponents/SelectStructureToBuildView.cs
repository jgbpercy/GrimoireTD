using UnityEngine;
using GrimoireTD.Technical;

namespace GrimoireTD.DefendingEntities.Structures
{
    public class SelectStructureToBuildView : SingletonMonobehaviour<SelectStructureToBuildView>
    {
        [SerializeField]
        private GameObject structurePanelPrefab;
        [SerializeField]
        private GameObject structureSelectPanel;

        private void Start()
        {
            GameModels.Models[0].OnGameModelSetUp += SetUpStructurePanel;
        }

        private void SetUpStructurePanel(object sender, EAOnGameModelSetUp args)
        {
            foreach (IStructureTemplate structureTemplate in args.GameModel.BuildableStructureTemplates)
            {
                GameObject structurePanel = Instantiate(structurePanelPrefab) as GameObject;
                structurePanel.transform.SetParent(structureSelectPanel.transform, false);

                structurePanel.GetComponent<SelectStructureToBuildUIElement>().SetUp(structureTemplate);
            }
        }
    }
}