using GrimoireTD.Abilities.BuildMode;
using GrimoireTD.Defenders.Structures;
using GrimoireTD.Defenders.Units;

namespace GrimoireTD.UI
{
    public interface IInterfaceController : IReadOnlyInterfaceController
    {
        void ActivateBuildModeAbility(IBuildModeAbility abilityToActivate);
        void ButtonClickDefend();
        void ClickStructureEnhancement(IStructure structure, IStructureUpgrade upgrade, IStructureEnhancement enhancement);
        void ClickUnitTalent(IUnit unit, IUnitTalent unitTalent);
        void ExecuteHexTargetedBuildModeAbility();
        void SelectStructureToBuild(IStructureTemplate selectedStructureTemplate);
    }
}