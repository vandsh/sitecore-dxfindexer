using System;
using System.Collections.Generic;
using System.Linq;
using DXFIndexer.Plugins;
using Sitecore.Configuration;
using Sitecore.ContentSearch;
using Sitecore.Data.Items;
using Sitecore.DataExchange.Attributes;
using Sitecore.DataExchange.Contexts;
using Sitecore.DataExchange.Models;
using Sitecore.DataExchange.Plugins;
using Sitecore.DataExchange.Processors.PipelineSteps;
using Sitecore.StringExtensions;

namespace DXFIndexer.Processors.PipelineSteps
{
    [RequiredEndpointPlugins(typeof(WriteToTagSettings))]
    public class WriteToTagProcessor : BasePipelineStepProcessor
    {
        public WriteToTagProcessor()
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
            var splitChar = Settings.GetSetting("DXF.WriteToTagProcessor.SplitChar");

            var writeToTagSettings = pipelineContext.CurrentPipelineStep.GetPlugin<WriteToTagSettings>();
            var dataSetToWriteToTag = pipelineContext.GetPlugin<IterableDataSettings>();
            if (writeToTagSettings != null)
            {
                try
                {
                    //yes we are assuming that anything in the datasettings is indexable, thats where they are going anyways
                    var baseIndexables = dataSetToWriteToTag.Data.Cast<IIndexable>().ToList();

                    var allTags = new List<string>();

                    foreach (var baseIndexable in baseIndexables)
                    {
                        try
                        {
                            var fieldValue = baseIndexable.GetFieldByName(writeToTagSettings.SourceField).Value;
                            if (fieldValue != null)
                            {
                                var sourceFieldValue = baseIndexable.GetFieldByName(writeToTagSettings.SourceField)
                                    .Value.ToString();
                                if (!sourceFieldValue.IsNullOrEmpty())
                                {
                                    var sourceFieldValueSplit = sourceFieldValue.Split(splitChar[0]);
                                    foreach (var splitFieldValue in sourceFieldValueSplit)
                                    {
                                        allTags.Add(splitFieldValue);
                                    }
                                }
                            }
                            else
                            {
                                logger.Debug(string.Format("{1} field is empty for: {0}", baseIndexable.Id, writeToTagSettings.SourceField));
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.Warn("Issue parsing tag for: " + baseIndexable.Id);
                        }

                    }

                    var disctinctTags = allTags.GroupBy(gb => gb).Select(g => new
                    {
                        Name = g.Key.Trim(),
                        Count = g.Count()
                    });

                    logger.Info("Tags Found: " + disctinctTags.Count());
                    foreach (var disctinctTag in disctinctTags)
                    {
                        //The below line is only executed when the kenexa job is executed
                        var existing = writeToTagSettings.CreatePath.Axes.GetDescendants().FirstOrDefault(i => i.Fields[writeToTagSettings.DestinationField].Value == disctinctTag.Name.ToString());
                        if (existing != null)
                        {
                            using (new Sitecore.SecurityModel.SecurityDisabler())
                            {
                                try
                                {
                                    existing.Editing.BeginEdit();
                                    existing.Fields[writeToTagSettings.CountField].SetValue(disctinctTag.Count.ToString(), false);
                                    logger.Debug("Existing Tag Found: " + existing.Name);
                                    existing.Editing.EndEdit();
                                }
                                catch (Exception ex)
                                {
                                    logger.Error(string.Format("Editing existing tag {1} Failed: {0}", ex.Message, existing.Name));
                                }
                            }
                        }
                        else
                        {
                            using (new Sitecore.SecurityModel.SecurityDisabler())
                            {
                                try
                                {
                                    logger.Debug("Creating new Tag: " + disctinctTag.Name.ToString());
                                    var newTag = writeToTagSettings.CreatePath.Add(ItemUtil.ProposeValidItemName(disctinctTag.Name.ToString()),writeToTagSettings.DestinationTemplate);
                                    newTag.Editing.BeginEdit();
                                    newTag.Fields[writeToTagSettings.DestinationField].SetValue(disctinctTag.Name.ToString(), false);
                                    newTag.Fields[writeToTagSettings.CountField].SetValue(disctinctTag.Count.ToString(), false);
                                    newTag.Editing.EndEdit();
                                }
                                catch (Exception ex)
                                {
                                    logger.Error(string.Format("Creating new tag {1} Failed: {0}", ex.Message,
                                        disctinctTag.Name));
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.Error("Tags Processor Failed: " + ex.Message);
                }

            }
        }
    }
}
