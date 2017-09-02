namespace System.Collections.Generic
{
    public class EAOnCallbackListAdd<T> : EventArgs
    {
        public readonly T AddedItem;

        public EAOnCallbackListAdd(T addedItem)
        {
            AddedItem = addedItem;
        }
    }
}