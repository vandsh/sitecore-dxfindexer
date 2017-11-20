using System;
using System.Linq;
using DXFIndexer.Classes.Search.Crawlers;
using DXFIndexer.Plugins;
using Sitecore.ContentSearch;
using Sitecore.DataExchange.Attributes;
using Sitecore.DataExchange.Contexts;
using Sitecore.DataExchange.Extensions;
using Sitecore.DataExchange.Models;
using Sitecore.DataExchange.Plugins;
using Sitecore.DataExchange.Processors.PipelineSteps;

namespace DXFIndexer.Processors.PipelineSteps
{
    [RequiredEndpointPlugins(typeof(IndexSettings))]
    public class IndexStepProcessor : BasePipelineStepProcessor
    {
        public IndexStepProcessor(){}
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

            var endpointSettings = pipelineStep.GetEndpointSettings();
            var endpointTo = endpointSettings.EndpointTo;
            var indexSettings = endpointTo.GetPlugin<IndexSettings>();
            var indexName = indexSettings.IndexName;
            logger.Debug("Index Name: " + indexName);
            var dataSet = pipelineContext.GetPlugin<IterableDataSettings>();
            var baseIndexables = dataSet.Data.Cast<IIndexable>();
            var totalIndexables = baseIndexables.Count();

            logger.Info("Indexable Items found: " + totalIndexables);
            if (totalIndexables <= 0)
            {
                logger.Debug("No indexable items found, quitting step.");
                return;
            }
            try
            {
                var indexHandle = ContentSearchManager.GetIndex(indexName);
                indexHandle.Initialize();
                var flatCrawler = new IterableDatasetCrawler<IIndexable>(baseIndexables);
                using (var updateContext = indexHandle.CreateUpdateContext())
                {
                    logger.Debug("Crawler Initialize...");
                    flatCrawler.Initialize(indexHandle);
                    logger.Debug("Index Rebuild...");
                    indexHandle.Rebuild(IndexingOptions.ForcedIndexing);
                    logger.Debug("Crawler Rebuild...");
                    flatCrawler.RebuildFromRoot(updateContext, IndexingOptions.Default);
                    updateContext.Commit();
                    logger.Debug("Rebuild Complete!");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }
    }
}
