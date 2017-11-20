using System;
using DXFIndexer.ItemModels;
using Sitecore.Configuration;
using Sitecore.DataExchange.Converters.PipelineSteps;
using Sitecore.DataExchange.Models;
using Sitecore.DataExchange.Plugins;
using Sitecore.DataExchange.Repositories;
using Sitecore.Services.Core.Model;

namespace DXFIndexer.Converters.PipelineSteps
{
    public class IndexStepConverter : BasePipelineStepConverter
    {
        public IndexStepConverter(IItemModelRepository repository) : base(repository)
        {
            var templateId = Guid.Parse(Settings.GetSetting("DXF.IndexStepConverter.Id"));
            this.SupportedTemplateIds.Add(templateId);
        }

        protected override void AddPlugins(ItemModel source, PipelineStep pipelineStep)
        {
            var settings = new EndpointSettings();
            var endpointTo = base.ConvertReferenceToModel<Endpoint>(source, IndexStepItemModel.EndpointTo);
            if (endpointTo != null)
            {
                settings.EndpointTo = endpointTo;
            }
            pipelineStep.Plugins.Add(settings);
        }
    }
}
