using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.Course.Application.Events;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Consumers
{
    [OpalConsumer("microservice.events.ccpm.has-reference-to-resource")]
    public class ContentFormToCheckReferencesConsumer : ScopedOpalMessageConsumer<ContentFormToCheckReferencesMessage>
    {
        public async Task InternalHandleAsync(
            ContentFormToCheckReferencesMessage message,
            IReadOnlyRepository<Lecture> readLectureRepository,
            IReadOnlyRepository<LectureContent> readLectureContentRepository,
            IReadOnlyRepository<CourseEntity> readCourseRepository,
            IThunderCqrs thunderCqrs)
        {
            var resourceTypeToLectureContentTypeDict = new Dictionary<ResourcesNotReferencedType, LectureContentType>
            {
                { ResourcesNotReferencedType.Content, LectureContentType.DigitalContent },
                { ResourcesNotReferencedType.Form, LectureContentType.Quiz }
            };

            var courseResourceMaps = await readLectureContentRepository.GetAll()
                .Where(
                    p => p.ResourceId != null
                         && message.ObjectIds.Contains(p.ResourceId.Value)
                         && resourceTypeToLectureContentTypeDict.ContainsKey(message.ContentType)
                         && p.Type == resourceTypeToLectureContentTypeDict[message.ContentType])
                .Join(
                    readLectureRepository.GetAll(),
                    p => p.LectureId,
                    p => p.Id,
                    (lectureContent, lecture) => new { lectureContent.ResourceId, lecture.CourseId })
                .Join(
                    readCourseRepository.GetAll(),
                    p => p.CourseId,
                    p => p.Id,
                    (courseResourceMap, course) => courseResourceMap)
                .ToListAsync();
            var courseIdToResourceIdsDict = courseResourceMaps.GroupBy(p => p.ResourceId)
                .ToDictionary(p => p.Key, p => p.ToList());

            var notReferenceResourceIds = message.ObjectIds.Where(x =>
                !courseIdToResourceIdsDict.ContainsKey(x) || !courseIdToResourceIdsDict[x].Any()).ToList();

            if (notReferenceResourceIds.Any())
            {
                await thunderCqrs.SendEvent(
                    new ResourcesNotReferencedEvent(
                        notReferenceResourceIds,
                        message.ContentType));
            }
        }
    }
}
