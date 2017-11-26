namespace System.Collections.Generic
{
    public class EAOnCallbackListRemove<T> : EventArgs
    {
        public readonly T RemovedItem;

        public EAOnCallbackListRemove(T removedItem)
        {
            RemovedItem = removedItem;
        }
    }
}