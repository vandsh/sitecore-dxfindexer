using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Xml;
using DXFIndexer.Classes.RSS;
using DXFIndexer.Plugins;
using Sitecore.ContentSearch;
using Sitecore.Data.Items;
using Sitecore.DataExchange.Attributes;
using Sitecore.DataExchange.Contexts;
using Sitecore.DataExchange.Extensions;
using Sitecore.DataExchange.Models;
using Sitecore.DataExchange.Plugins;
using Sitecore.DataExchange.Processors.PipelineSteps;
using Sitecore.StringExtensions;

namespace DXFIndexer.Processors.PipelineSteps
{
    [RequiredEndpointPlugins(typeof(RssFeedSettings))]
    public class ReadRssFeedProcessor : BasePipelineStepProcessor
    {
        public ReadRssFeedProcessor()
        {
        }

        public override void Process(PipelineStep pipelineStep, PipelineContext pipelineContext)
        {
            if (pipelineStep == null)
            {
                throw new ArgumentNullException(nameof(pipelineStep));
            }
            if (pipelineContext == null)
            {
                throw new ArgumentNullException(nameof(pipelineContext));
            }
            var logger = pipelineContext.PipelineBatchContext.Logger;

            EndpointSettings endpointSettings = pipelineStep.GetEndpointSettings();

            var rssFeedSettings = endpointSettings.EndpointFrom.GetPlugin<RssFeedSettings>();
            if (rssFeedSettings != null)
            {
                try
                {
                    //Read rss feed and make them into an indexable item
                    var indexableArticles = new List<RssIndexable>();
                    string url = rssFeedSettings.RssFeedUrl;
                    XmlReader reader = XmlReader.Create(url);
                    SyndicationFeed feed = SyndicationFeed.Load(reader);
                    reader.Close();
                    foreach (SyndicationItem item in feed.Items)
                    {
                        string publishDate = item.PublishDate.ToString();
                        string subject = item.Title.Text;
                        string summary = item.Summary.Text;
                        string categories = string.Join("/", item.Categories?.Select(c => c.Name));
                        string authors = string.Join(", ", item.Authors?.Select(a => a.Name));
                        string baseUri = item.BaseUri?.AbsoluteUri;
                        var rssIndexable = new RssIndexable(publishDate, subject, summary, authors, categories, baseUri);
                        indexableArticles.Add(rssIndexable);
                    }
                    //Stuff indexables into pipeline batch
                    var dataSettings = new IterableDataSettings(indexableArticles);
                    pipelineContext.Plugins.Add(dataSettings);
                }
                catch (Exception ex)
                {
                    logger.Error("RSS Processor Failed: " + ex.Message);
                }

            }
        }
    }
}
