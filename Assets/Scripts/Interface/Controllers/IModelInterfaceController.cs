using System;

namespace GrimoireTD.UI
{
    public interface IModelInterfaceController
    {
        event EventHandler<EAOnBuildStructurePlayerAction> OnBuildStructurePlayerAction;
        event EventHandler<EAOnCreateUnitPlayerAction> OnCreateUnitPlayerAction;

        event EventHandler<EAOnCreepDeselected> OnCreepDeselected;
        event EventHandler<EAOnCreepSelected> OnCreepSelected;

        event EventHandler<EAOnDefenderDeselected> OnDefenderDeselected;
        event EventHandler<EAOnDefenderSelected> OnDefenderSelected;

        event EventHandler<EAOnEnterDefendModePlayerAction> OnEnterDefendModePlayerAction;
    }
}