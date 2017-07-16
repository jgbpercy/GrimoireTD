using UnityEngine;
using UnityEngine.UI;

public class SelectStructureToBuildView : SingletonMonobehaviour<SelectStructureToBuildView> {

    [SerializeField]
    private GameObject structurePanelPrefab;
    [SerializeField]
    private GameObject structureSelectPanel;

    [SerializeField]
    private SoStructureTemplate[] buildableStructureTemplates;

    private void Start()
    {
        CDebug.Log(CDebug.applicationLoading, "Structure Select View Start");

        SetUpStructurePanel();
    }

    private void SetUpStructurePanel()
    {
        foreach (IStructureTemplate structureTemplate in buildableStructureTemplates)
        {
            GameObject structurePanel = Instantiate(structurePanelPrefab) as GameObject;
            structurePanel.transform.SetParent(structureSelectPanel.transform, false);

            structurePanel.GetComponent<SelectStructureToBuildUIElement>().SetUp(structureTemplate);
        }
    }
}
