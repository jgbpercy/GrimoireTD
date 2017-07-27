using System;
using System.Collections.Generic;
using UnityEngine;

public class DamageTypeManager : SingletonMonobehaviour<DamageTypeManager> {

    [Serializable]
    private class MetaDamageTypeStructure
    {
        [SerializeField]
        public MetaDamageType metaDamageType;
        [SerializeField]
        public SpecificDamageType[] specificDamageTypes;
    }

    [SerializeField]
    private MetaDamageTypeStructure[] metaDamageTypes;

    private Dictionary<MetaDamageType, List<SpecificDamageType>> metaToSpecific;

    private Dictionary<SpecificDamageType, MetaDamageType> specificToMeta;

	private void Start ()
    {
        metaToSpecific = new Dictionary<MetaDamageType, List<SpecificDamageType>>();
        specificToMeta = new Dictionary<SpecificDamageType, MetaDamageType>();

        foreach (MetaDamageTypeStructure metaDamageTypeStructure in metaDamageTypes)
        {
            List<SpecificDamageType> specificDamageTypeList = new List<SpecificDamageType>();

            foreach (SpecificDamageType specificDamageType in metaDamageTypeStructure.specificDamageTypes)
            {
                specificDamageTypeList.Add(specificDamageType);

                specificToMeta.Add(specificDamageType, metaDamageTypeStructure.metaDamageType);
            }

            metaToSpecific.Add(metaDamageTypeStructure.metaDamageType, specificDamageTypeList);
        }
	}

    public static List<SpecificDamageType> GetSpecificDamageTypes(MetaDamageType metaDamageType)
    {
        return Instance.metaToSpecific[metaDamageType];
    }

    public static MetaDamageType GetMetaDamageType(SpecificDamageType specificDamageType)
    {
        return Instance.specificToMeta[specificDamageType];
    }
}
