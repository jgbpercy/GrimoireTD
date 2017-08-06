public interface IResourceTransaction {

    IResource Resource { get; }

    int Amount { get; }

    bool CanDoTransaction();

    void DoTransaction();
}
