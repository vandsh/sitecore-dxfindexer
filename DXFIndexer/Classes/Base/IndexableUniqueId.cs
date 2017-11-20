using System;
using Sitecore.ContentSearch;

namespace DXFIndexer.Classes.Base
{
    public class IndexableUniqueId : IIndexableUniqueId
    {
        private IIndexableId Id;
        private Guid guid;

        public IndexableUniqueId(IIndexableId Id, Guid guid)
        {
            this.Id = Id;
            this.guid = guid;
        }
        public IIndexableId GroupId
        {
            get { return Id; }
        }

        public object Value
        {
            get { return guid; }
        }
    }
}
