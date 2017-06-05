using UnityEngine;
using UnityEngine.UI;

public class SelectStructureToBuildUIElement : MonoBehaviour {

    private StructureTemplate structureTemplate;

    private bool initialised = false;

    [SerializeField]
    private Text structureNameText;
    [SerializeField]
    private Text structureDescriptionText;

    public void SetUp(StructureTemplate structureTemplate)
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
