﻿using UnityEngine;
using GrimoireTD.Technical;
using GrimoireTD.Dependencies;

namespace GrimoireTD.Defenders.Structures
{
    public class SelectStructureToBuildView : SingletonMonobehaviour<SelectStructureToBuildView>
    {
        [SerializeField]
        private GameObject structurePanelPrefab;
        [SerializeField]
        private GameObject structureSelectPanel;

        private void Start()
        {
            DepsProv.TheGameModel.OnGameModelSetUp += SetUpStructurePanel;
        }

        private void SetUpStructurePanel(object sender, EAOnGameModelSetUp args)
        {
            foreach (var structureTemplate in args.GameModel.BuildableStructureTemplates)
            {
                var structurePanel = Instantiate(structurePanelPrefab) as GameObject;
                structurePanel.transform.SetParent(structureSelectPanel.transform, false);
                structurePanel.transform.localScale = Vector3.one;

                structurePanel.GetComponent<SelectStructureToBuildUIElement>().SetUp(structureTemplate);
            }
        }
    }
}