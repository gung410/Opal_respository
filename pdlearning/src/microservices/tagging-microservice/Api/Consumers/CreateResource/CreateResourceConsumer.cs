using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Conexus.Opal.Microservice.Tagging.DataProviders;
using Conexus.Opal.Microservice.Tagging.Domain.Entities;
using Conexus.Opal.Microservice.Tagging.Events.ResourceSavedEvent;
using Thunder.Platform.Cqrs;

namespace Conexus.Opal.Microservice.Tagging.Consumers.CreateResource
{
    [OpalConsumer("microservice.events.metadata.create_resource")]
    public class CreateResourceConsumer : OpalMessageConsumer<CreateResourcePayload>
    {
        private readonly ITaggingDataProvider _taggingDataProvider;
        private readonly IThunderCqrs _thunderCqrs;

        public CreateResourceConsumer(
            ITaggingDataProvider taggingDataProvider,
            IThunderCqrs thunderCqrs)
        {
            _thunderCqrs = thunderCqrs;
            _taggingDataProvider = taggingDataProvider;
        }

        protected override async Task InternalHandleAsync(CreateResourcePayload message)
        {
            var resource = await _taggingDataProvider.GetResourceById(message.Id, message.CreatedBy);

            var tags = message.Tags;

            if (message.ResourceType == ResourceType.Community && (message.Tags == null || !message.Tags.Any()))
            {
                var tagFieldName = new List<string>()
                {
                    "pdActivityType", "serviceSchemeIds", "subjectAreaIds", "learningFrameworkIds", "learningDimensionIds", "learningAreaIds", "learningSubAreaIds",
                    "trackIds", "categoryIds", "teachingCourseStudyIds", "teachingLevels", "developmentalRoleIds", "learningMode"
                };

                var tagsinDynamicField = new List<Guid>();

                foreach (var name in tagFieldName)
                {
                    if (message.DynamicMetaData.ContainsKey(name) && Guid.TryParse(resource.DynamicMetaData[name].ToString(), out var id))
                    {
                        tagsinDynamicField.Add(id);
                    }
                }

                tags = tagsinDynamicField;
            }

            if (resource == null)
            {
                resource = new Resource
                {
                    ResourceId = message.Id,
                    ResourceType = message.ResourceType,
                    CreatedBy = message.CreatedBy,
                    MainSubjectAreaTagId = message.MainSubjectAreaTagId,
                    PreRequisties = message.PreRequisties,
                    ObjectivesOutCome = message.ObjectivesOutCome,
                    Tags = tags,
                    DynamicMetaData = message.ResourceType == ResourceType.Community ? message.Dictionary.Dictionary : message.DynamicMetaData,
                    SearchTags = message.SearchTags.ToList()
                };
            }
            else
            {
                resource.ResourceType = message.ResourceType;
                resource.MainSubjectAreaTagId = message.MainSubjectAreaTagId;
                resource.PreRequisties = message.PreRequisties;
                resource.ObjectivesOutCome = message.ObjectivesOutCome;
                resource.Tags = tags;
                resource.DynamicMetaData = message.ResourceType == ResourceType.Community ? message.Dictionary.Dictionary : message.DynamicMetaData;
                resource.SearchTags = message.SearchTags.ToList();
            }

            _taggingDataProvider.SaveResourceMetadata(resource, message.CreatedBy);
            await _thunderCqrs.SendEvent(new SaveResourceEvent(resource));
        }
    }
}
