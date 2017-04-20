using UnityEngine;
using UnityEngine.UI;

public class StructureSelectView : SingletonMonobehaviour<StructureSelectView> {

    [SerializeField]
    private GameObject structurePanelPrefab;
    [SerializeField]
    private GameObject structureSelectPanel;

    private void Start()
    {
        CDebug.Log(CDebug.applicationLoading, "Structure Select View Start");

        SetUpStructurePanel();
    }

    private void SetUpStructurePanel()
    {
        StructureTemplate[] structureTemplates = InterfaceController.Instance.StructureTemplates;

        for (int i = 0; i < structureTemplates.Length; i++)
        {
            GameObject structurePanel = Instantiate(structurePanelPrefab) as GameObject;

            structurePanel.transform.SetParent(structureSelectPanel.transform, false);

            StructureUIElement structurePanelUIScript = structurePanel.GetComponent<StructureUIElement>();
            structurePanelUIScript.indexInStructureList = i;

            Text structureName = structurePanel.transform.GetChild(0).GetComponent<Text>();
            structureName.text = structureTemplates[i].NameInGame;

            Text structureDescription = structurePanel.transform.GetChild(1).GetComponent<Text>();
            structureDescription.text = structureTemplates[i].UIText();
        }
    }
}
