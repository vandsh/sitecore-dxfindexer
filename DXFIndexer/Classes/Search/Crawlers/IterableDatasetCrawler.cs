using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.ContentSearch;

namespace DXFIndexer.Classes.Search.Crawlers
{
    public class IterableDatasetCrawler<T> : FlatDataCrawler<IIndexable>
    {
        private IEnumerable<T> _repository;
        public IterableDatasetCrawler(IEnumerable<T> items)
        {
            _repository = items;
        }
        protected override IIndexable GetIndexableAndCheckDeletes(IIndexableUniqueId indexableUniqueId)
        {
            throw new NotImplementedException();
        }

        protected override IIndexable GetIndexable(IIndexableUniqueId indexableUniqueId)
        {
            throw new NotImplementedException();
        }

        protected override bool IndexUpdateNeedDelete(IIndexable indexable)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<IIndexableUniqueId> GetIndexablesToUpdateOnDelete(IIndexableUniqueId indexableUniqueId)
        {
            //return empty
            var uidList = _repository.Cast<IIndexable>().Select(bi => bi.UniqueId);
            return uidList;
        }

        protected override IEnumerable<IIndexable> GetItemsToIndex()
        {
            var itemsToIndex = _repository.Cast<IIndexable>();
            return itemsToIndex;
        }
    }
}
