namespace System
{
    public interface INotifyOnChange<EA> where EA : EventArgs
    {
        event EventHandler<EA> OnChange;
    }
}