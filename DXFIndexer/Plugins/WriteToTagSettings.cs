using Sitecore.Data.Items;
using Sitecore.DataExchange;

namespace DXFIndexer.Plugins
{
    public class WriteToTagSettings : IPlugin
    {
        public Item CreatePath { get; set; }
        public string SourceField { get; set; }
        public string DestinationField { get; set; }
        public string CountField { get; set; }
        public TemplateItem DestinationTemplate { get; set; }
    }
}
