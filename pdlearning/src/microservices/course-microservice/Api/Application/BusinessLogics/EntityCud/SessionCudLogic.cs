using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Course.Application.AggregatedEntityModels;
using Microservice.Course.Application.Constants;
using Microservice.Course.Application.Events;
using Microservice.Course.Application.Events.Todos;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Infrastructure;
using Microservice.Course.Settings;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Timing;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.BusinessLogics.EntityCud
{
    public class SessionCudLogic : BaseEntityCudLogic<Session>
    {
        private readonly IReadOnlyRepository<Registration> _readRegistrationRepository;
        private readonly IWriteOnlyRepository<ClassRun> _classRunWriteOnlyRepository;
        private readonly WebAppLinkBuilder _webAppLinkBuilder;

        public SessionCudLogic(
            IWriteOnlyRepository<Session> repository,
            IThunderCqrs thunderCqrs,
            IReadOnlyRepository<Registration> readRegistrationRepository,
            IUserContext userContext,
            WebAppLinkBuilder webAppLinkBuilder,
            IWriteOnlyRepository<ClassRun> classRunWriteOnlyRepository) : base(repository, thunderCqrs, userContext)
        {
            _readRegistrationRepository = readRegistrationRepository;
            _webAppLinkBuilder = webAppLinkBuilder;
            _classRunWriteOnlyRepository = classRunWriteOnlyRepository;
        }

        public async Task Insert(SessionAggregatedEntityModel aggregatedSession, CancellationToken cancellationToken = default)
        {
            await RootRepository.InsertAsync(aggregatedSession.Session);

            aggregatedSession.ClassRun.ChangedDate = Clock.Now;
            aggregatedSession.ClassRun.ChangedBy = aggregatedSession.Session.CreatedBy;
            await _classRunWriteOnlyRepository.UpdateAsync(aggregatedSession.ClassRun);

            await ThunderCqrs.SendEvent(
                new SessionChangeEvent(aggregatedSession.ToAssociatedEntity(), SessionChangeType.Created), cancellationToken);
        }

        public async Task Update(SessionAggregatedEntityModel aggregatedSession, CancellationToken cancellationToken = default)
        {
            await RootRepository.UpdateAsync(aggregatedSession.Session);

            aggregatedSession.ClassRun.ChangedDate = Clock.Now;
            aggregatedSession.ClassRun.ChangedBy = aggregatedSession.Session.ChangedBy;
            await _classRunWriteOnlyRepository.UpdateAsync(aggregatedSession.ClassRun);

            await ThunderCqrs.SendEvent(
                new SessionChangeEvent(aggregatedSession.ToAssociatedEntity(), SessionChangeType.Updated), cancellationToken);
        }

        public async Task UpdateMany(List<SessionAggregatedEntityModel> aggregatedSessions, CancellationToken cancellationToken = default)
        {
            await RootRepository.UpdateManyAsync(aggregatedSessions.Select(p => p.Session));

            await _classRunWriteOnlyRepository.UpdateManyAsync(
                aggregatedSessions
                    .Select(p =>
                    {
                        p.ClassRun.ChangedDate = Clock.Now;
                        p.ClassRun.ChangedBy = p.Session.ChangedBy;
                        return p.ClassRun;
                    })
                    .ToList());

            await ThunderCqrs.SendEvents(
                aggregatedSessions.Select(x => new SessionChangeEvent(x.ToAssociatedEntity(), SessionChangeType.Updated)), cancellationToken);
        }

        public async Task Delete(SessionAggregatedEntityModel aggregatedSession, CancellationToken cancellationToken = default)
        {
            await RootRepository.DeleteAsync(aggregatedSession.Session);

            aggregatedSession.ClassRun.ChangedDate = Clock.Now;
            aggregatedSession.ClassRun.ChangedBy = aggregatedSession.Session.CreatedBy;
            await _classRunWriteOnlyRepository.UpdateAsync(aggregatedSession.ClassRun);

            await ThunderCqrs.SendEvent(
                new SessionChangeEvent(aggregatedSession.ToAssociatedEntity(), SessionChangeType.Deleted), cancellationToken);

            var registrations = await _readRegistrationRepository.GetAll().Where(p => p.ClassRunId == aggregatedSession.Session.ClassRunId).ToListAsync(cancellationToken);

            if (registrations.Any())
            {
                await ThunderCqrs.SendEvent(new DeleteSessionNotifyLearnerEvent(
                    CurrentUserIdOrDefault,
                    new DeleteSessionNotifyLearnerPayload
                    {
                        CourseTitle = aggregatedSession.Course.CourseName,
                        ClassrunTitle = aggregatedSession.ClassRun.ClassTitle,
                        SessionDate = aggregatedSession.Session.StartDateTime.HasValue
                            ? TimeHelper.ConvertTimeFromUtc(aggregatedSession.Session.StartDateTime.Value).ToString(DateTimeFormatConstant.OnlyDate)
                            : string.Empty,
                        SessionStartTime = TimeHelper.ConvertTimeFromUtc(aggregatedSession.Session.StartDateTime.GetValueOrDefault()).ToString(DateTimeFormatConstant.OnlyHourMinute),
                        SessionEndTime = TimeHelper.ConvertTimeFromUtc(aggregatedSession.Session.EndDateTime.GetValueOrDefault()).ToString(DateTimeFormatConstant.OnlyHourMinute),
                        SessionVenue = aggregatedSession.Session.Venue,
                        ActionUrl = _webAppLinkBuilder.GetCourseDetailLearnerLinkForCAMModule(aggregatedSession.Course.Id)
                    },
                    registrations.Select(p => p.UserId).ToList()));
            }
        }
    }
}
