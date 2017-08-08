public interface IEconomyTransaction {

    IResourceTransaction GetResourceTransaction(IResource resource);

    int GetTransactionAmount(IResource resource);

    bool CanDoTransaction();

    void DoTransaction();

    string ToString();

    string ToString(bool absolute);

    string ToString(EconomyTransactionStringFormat format, bool absolute);

    IEconomyTransaction Abs();

    IEconomyTransaction Add(IEconomyTransaction otherTransaction);

    IEconomyTransaction Subtract(IEconomyTransaction otherTransaction);

    IEconomyTransaction Multiply(int factor);

    IEconomyTransaction Multiply(float factor);

    IEconomyTransaction Multiply(float factor, RoundingMode roundingMode);
}

public enum EconomyTransactionStringFormat
{
    FullNameSingleLine,
    FullNameLineBreaks,
    ShortNameSingleLine,
    ShortNameLineBreaks
}

public enum RoundingMode
{
    NEAREST,
    UP,
    DOWN
}
