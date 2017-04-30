﻿using UnityEngine;

public class Structure : DefendingEntity {

    private StructureTemplate structureTemplate;

    public override string Id
    {
        get
        {
            return "S-" + id;
        }
    }

    public StructureTemplate StructureClassTemplate
    {
        get
        {
            return structureTemplate;
        }
    }

    public Structure(StructureTemplate structureTemplate, Vector3 position) : base(structureTemplate)
    {
        id = IdGen.GetNextId();

        this.structureTemplate = structureTemplate;

        DefendingEntityView.Instance.CreateStructure(this, position);
    }

    public override string UIText()
    {
        return structureTemplate.Description;
    }
}