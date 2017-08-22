using System;
using System.Collections.Generic;
using UnityEngine;
using GrimoireTD.Economy;
using GrimoireTD.UI;
using GrimoireTD.Map;
using GrimoireTD.Levels;
using GrimoireTD.Abilities.DefendMode.AttackEffects;
using GrimoireTD.ChannelDebug;
using GrimoireTD.DefendingEntities.Structures;

namespace GrimoireTD.Technical
{
    public class GameModelLoader : SingletonMonobehaviour<GameModelLoader>
    {
        [Serializable]
        public class ColorToHexType
        {
            [SerializeField]
            private Color32 color;

            [SerializeField]
            public SoHexType hexType;

            public Color32 Color
            {
                get
                {
                    return color;
                }
            }

            public IHexType HexType
            {
                get
                {
                    return hexType;
                }
            }
        }

        public IGameModel GameModel { get; private set; }

        [SerializeField]
        private SoLevel level;

        [SerializeField]
        private SoResourceTemplate[] resourceTemplates;

        [SerializeField]
        private ColorToHexType[] hexColorsAndTypes;

        [SerializeField]
        private SoAttackEffectType[] attackEffectTypes;

        [SerializeField]
        private SoStructureTemplate[] buildableStructureTemplates;

        [SerializeField]
        private float trackIdleTimeAfterSpawns;

        [SerializeField]
        private float unitFatigueFactorInfelctionPoint;
        [SerializeField]
        private float unitFatigueFactorShallownessMultiplier;

        private void Awake()
        {
            CDebug.InitialiseDebugChannels();

            GameModel = new CGameModel();
        }

        private void Start()
        {
            Debug.Log("Model Loader Start");

            MapRenderer.Instance.enabled = true;
            MapEntitiesView.Instance.enabled = true;
            EconomyView.Instance.enabled = true;
            InterfaceController.Instance.enabled = true;
        }

        private void Update()
        {
            Dictionary<Color32, IHexType> colorsToTypesDictionary = new Dictionary<Color32, IHexType>();
            List<IHexType> hexTypeList = new List<IHexType>();

            foreach (ColorToHexType colorToType in hexColorsAndTypes)
            {
                colorsToTypesDictionary.Add(colorToType.Color, colorToType.hexType);
                hexTypeList.Add(colorToType.hexType);
            }

            GameModel.SetUp
            (
                level,
                resourceTemplates,
                hexTypeList,
                attackEffectTypes,
                colorsToTypesDictionary,
                buildableStructureTemplates,
                trackIdleTimeAfterSpawns,
                unitFatigueFactorInfelctionPoint,
                unitFatigueFactorShallownessMultiplier
            );

            enabled = false;
        }
    }
}