using UnityEngine;
using GrimoireTD.Technical;
using GrimoireTD.ChannelDebug;

namespace GrimoireTD.DefendingEntities.Structures
{
    //TODO: the available 
    public class SelectStructureToBuildView : SingletonMonobehaviour<SelectStructureToBuildView>
    {
        [SerializeField]
        private GameObject structurePanelPrefab;
        [SerializeField]
        private GameObject structureSelectPanel;

        private void Start()
        {
            CDebug.Log(CDebug.applicationLoading, "Structure Select View Start");

            GameModels.Models[0].RegisterForOnSetUpCallback(SetUpStructurePanel);
        }

        private void SetUpStructurePanel()
        {
            foreach (IStructureTemplate structureTemplate in GameModels.Models[0].BuildableStructureTemplates)
            {
                GameObject structurePanel = Instantiate(structurePanelPrefab) as GameObject;
                structurePanel.transform.SetParent(structureSelectPanel.transform, false);

                structurePanel.GetComponent<SelectStructureToBuildUIElement>().SetUp(structureTemplate);
            }
        }
    }
}