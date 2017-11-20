using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXFIndexer.Classes.Attributes;
using DXFIndexer.Classes.Base;

namespace DXFIndexer.Classes.RSS
{
    public class RssIndexable : BaseIndexable
    {
        [ExcludeFromIndex]
        public RssIndexable(string publishDate, string subject, string summary, string authors, string categories, string baseUri)
        {
            PublishDate = publishDate;
            Subject = subject;
            Summary = summary;
            Authors = authors;
            Categories = categories;
            BaseUri = baseUri;
        }

        [IndexableId]
        public string ArticleId {
            get
            {
                return string.Format("{0}{1}", Subject.Replace(" ", ""), PublishDate.Replace("/", "").Replace("-", "").Replace(":", "").Replace(" ", ""));
            }
        }
        public string PublishDate { get; set; }
        public string Subject { get; set; }
        public string Summary { get; set; }
        public string Authors { get; set; }
        public string Categories { get; set; }
        [ExcludeFromIndex]
        public string BaseUri { get; set; }

    }
}