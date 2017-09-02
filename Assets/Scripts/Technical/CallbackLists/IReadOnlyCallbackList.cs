namespace System.Collections.Generic
{
    public interface IReadOnlyCallbackList<T> : IEnumerable<T>, IReadOnlyCollection<T>, IReadOnlyList<T>
    {
        event EventHandler<EAOnCallbackListAdd<T>> OnAdd;
        event EventHandler<EAOnCallbackListRemove<T>> OnRemove;
    }
}