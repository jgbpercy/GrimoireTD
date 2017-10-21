namespace GrimoireTD.Creeps
{
    public interface IResistances : IReadOnlyResistances
    {
        void AddResistanceModifier(IResistanceModifier modifier);

        void AddBlockModifier(IBlockModifier modifier);

        void RemoveResistanceModifer(IResistanceModifier modifier);

        void RemoveBlockModifier(IBlockModifier modifier);

        void ReplaceResistanceModifier(IResistanceModifier outgoingModifier, IResistanceModifier incomingModifier);

        void ReplaceBlockModifier(IBlockModifier outgoingModifier, IBlockModifier incomingModifier);
    }
}