using System.Collections.Generic;
using NUnit.Framework;

namespace GrimoireTD.Tests.CollectionExtTests
{
    public class CollectionExtTests
    {
        [Test]
        public void CombineCollections_PassedArrayAndList_ReturnsCollectionWithElementsFromBoth()
        {
            var list = new List<int> { 3, 4, 5 };

            var array = new int[] { 6, 7, 8 };

            var combined = CollectionExt.CombineCollection(list, array);

            Assert.AreEqual(6, combined.Count);

            Assert.True(combined.Contains(3));
            Assert.True(combined.Contains(4));
            Assert.True(combined.Contains(5));
            Assert.True(combined.Contains(6));
            Assert.True(combined.Contains(7));
            Assert.True(combined.Contains(8));
        }
    }
}