using System.Linq;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.Analytics.Application.Consumers.Learner.Messages;
using Microservice.Analytics.Application.Consumers.Learner.Messages.Payloads;
using Microservice.Analytics.Domain.Entities;
using Microsoft.Extensions.Logging;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Analytics.Application.Consumers.Learner
{
    [OpalConsumer("microservice.events.learner.user_activity.search_catalog")]
    public class LearnerActivitySearchCatalogConsumer : BaseLearnerActivityConsumer<LearnerActivitySearchCatalogPayload>
    {
        private readonly IRepository<SearchEngine> _searchEngineRepository;

        public LearnerActivitySearchCatalogConsumer(
            ILoggerFactory loggerFactory,
            IRepository<SearchEngine> searchEngineRepository,
            IRepository<SAM_UserHistory> userHistoryRepository) : base(loggerFactory, userHistoryRepository)
        {
            _searchEngineRepository = searchEngineRepository;
        }

        public override async Task InternalHandleAsync(BaseLearnerActivityMessage<LearnerActivitySearchCatalogPayload> message)
        {
            var latestHistoryItem = await this.GetLatestSAMUserHistoryItem(message);
            if (latestHistoryItem == null)
            {
                return;
            }

            var searchEngine = new SearchEngine()
            {
                UserId = message.UserId,
                UserHistoryId = latestHistoryItem.Id,
                DepartmentId = latestHistoryItem.DepartmentId,
                SearchText = message.Payload.SearchText,
                CreatedDate = message.Time,
                PdactivityTypeId = message.Payload.Tags.PdActivityType.FirstOrDefault(),
                CourseLevelId = message.Payload.Tags.CourseLevel.FirstOrDefault(),
                LearningModeId = message.Payload.Tags.LearningMode?.FirstOrDefault(),
                NatureOfCourseId = message.Payload.Tags.NatureOfCourse?.FirstOrDefault(),
                SearchEngineCategories =
                    message.Payload.Tags.CategoryIds == null || !message.Payload.Tags.CategoryIds.Any()
                        ? null
                        : message.Payload.Tags.CategoryIds
                            .Select(t => new SearchEngineCategory() { CategoryId = t })
                            .ToList(),
                SearchEngineDevelopmentRoles =
                    message.Payload.Tags.DevelopmentalRoleIds == null ||
                    !message.Payload.Tags.DevelopmentalRoleIds.Any()
                        ? null
                        : message.Payload.Tags.DevelopmentalRoleIds
                            .Select(t => new SearchEngineDevelopmentRole() { DevelopmentRoleId = t })
                            .ToList(),
                SearchEngineLearningAreas =
                    message.Payload.Tags.LearningAreaIds == null || !message.Payload.Tags.LearningAreaIds.Any()
                        ? null
                        : message.Payload.Tags.LearningAreaIds
                            .Select(t => new SearchEngineLearningArea() { LearningAreaId = t })
                            .ToList(),
                SearchEngineLearningDimensions = message.Payload.Tags.LearningDimensionIds == null ||
                                                 !message.Payload.Tags.LearningDimensionIds.Any()
                    ? null
                    : message.Payload.Tags.LearningDimensionIds
                        .Select(t => new SearchEngineLearningDimension() { LearningDimensionId = t })
                        .ToList(),
                SearchEngineLearningFrameworks = message.Payload.Tags.LearningFrameworkIds == null ||
                                                 !message.Payload.Tags.LearningFrameworkIds.Any()
                    ? null
                    : message.Payload.Tags.LearningFrameworkIds
                        .Select(t => new SearchEngineLearningFramework() { LearningFrameworkId = t })
                        .ToList(),
                SearchEngineLearningSubAreas = message.Payload.Tags.LearningSubAreaIds == null ||
                                               !message.Payload.Tags.LearningSubAreaIds.Any()
                    ? null
                    : message.Payload.Tags.LearningSubAreaIds
                        .Select(t => new SearchEngineLearningSubArea() { LearningSubAreaId = t })
                        .ToList(),
                SearchEngineServiceSchemes = message.Payload.Tags.ServiceSchemeIds == null ||
                                             !message.Payload.Tags.ServiceSchemeIds.Any()
                    ? null
                    : message.Payload.Tags.ServiceSchemeIds
                        .Select(t => new SearchEngineServiceScheme() { ServiceSchemeId = t })
                        .ToList(),
                SearchEngineSubjects = message.Payload.Tags.SubjectAreaIds == null ||
                                       !message.Payload.Tags.SubjectAreaIds.Any()
                    ? null
                    : message.Payload.Tags.SubjectAreaIds
                        .Select(t => new SearchEngineSubject() { SubjectId = t })
                        .ToList(),
                SearchEngineTeachingLevels = message.Payload.Tags.TeachingLevels == null ||
                                             !message.Payload.Tags.TeachingLevels.Any()
                    ? null
                    : message.Payload.Tags.TeachingLevels
                        .Select(t => new SearchEngineTeachingLevel() { TeachingLevelId = t })
                        .ToList()
            };

            await _searchEngineRepository.InsertAsync(searchEngine);
        }
    }
}
