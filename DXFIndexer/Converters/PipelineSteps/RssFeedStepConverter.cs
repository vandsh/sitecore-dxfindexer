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
    public class RssFeedStepConverter : BasePipelineStepConverter
    {
        public RssFeedStepConverter(IItemModelRepository repository) : base(repository)
        {
            var templateId = Guid.Parse(Settings.GetSetting("DXF.RssFeedStepConverter.Id"));
            this.SupportedTemplateIds.Add(templateId);
        }

        protected override void AddPlugins(ItemModel source, PipelineStep pipelineStep)
        {
            var settings = new EndpointSettings();
            var endpointFrom = base.ConvertReferenceToModel<Endpoint>(source, RssStepItemModel.EndpointFrom);
            if (endpointFrom != null)
            {
                settings.EndpointFrom = endpointFrom;
            }
            pipelineStep.Plugins.Add(settings);
        }
    }
}
