using System;
using DXFIndexer.ItemModels;
using DXFIndexer.Plugins;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.DataExchange.Converters.PipelineSteps;
using Sitecore.DataExchange.Models;
using Sitecore.DataExchange.Repositories;
using Sitecore.Services.Core.Model;

namespace DXFIndexer.Converters.PipelineSteps
{
    public class WriteToTagStepConverter : BasePipelineStepConverter
    {
        public WriteToTagStepConverter(IItemModelRepository repository) : base(repository)
        {
            var templateId = Guid.Parse(Settings.GetSetting("DXF.WriteToTagStepConverter.Id"));
            this.SupportedTemplateIds.Add(templateId);
        }

        protected override void AddPlugins(ItemModel source, PipelineStep pipelineStep)
        {
            var settings = new WriteToTagSettings();
            var database = Settings.GetSetting("DXF.WriteToTagStepConverter.Db");
            using (var db = new DatabaseSwitcher(Database.GetDatabase(database)))
            {
                var createPath = base.GetStringValue(source, WriteToTagSettingsItemModel.CreatePath);
                if (createPath != null)
                {
                    var parentTagItem = Sitecore.Context.Data.Database.GetItem(createPath);
                    settings.CreatePath = parentTagItem;
                }

                var destinationTemplate = base.GetStringValue(source, WriteToTagSettingsItemModel.DestinationTemplate);
                if (createPath != null)
                {
                    var desiredTemplate = Sitecore.Context.Data.Database.GetItem(ID.Parse(destinationTemplate));
                    settings.DestinationTemplate = (TemplateItem) desiredTemplate;
                }
            }

            var sourceField = base.GetStringValue(source, WriteToTagSettingsItemModel.SourceField);
            var destinationField = base.GetStringValue(source, WriteToTagSettingsItemModel.DestinationField);
            var countField = base.GetStringValue(source, WriteToTagSettingsItemModel.CountField);

            settings.SourceField = sourceField;
            settings.DestinationField = destinationField;
            settings.CountField = countField;

            pipelineStep.Plugins.Add(settings);
        }
    }
}
