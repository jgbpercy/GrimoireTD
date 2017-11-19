namespace System.Collections.Generic
{
    public static class CollectionExt
    {
        public static ICollection<T> CombineCollection<T>(ICollection<T> collection1, ICollection<T> collection2)
        {
            var combined = new T[collection1.Count + collection2.Count];

            collection1.CopyTo(combined, 0);
            collection2.CopyTo(combined, collection2.Count);

            return combined;
        }
    }
}