namespace System.Collections.Generic
{
    public class EAOnCallbackListRemove<T>
    {
        public readonly T RemovedItem;

        public EAOnCallbackListRemove(T removedItem)
        {
            RemovedItem = removedItem;
        }
    }
}