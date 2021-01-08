using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Extensions;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.AggregatedEntityModels;
using Microservice.Course.Application.BusinessLogics;
using Microservice.Course.Application.BusinessLogics.EntityCud;
using Microservice.Course.Application.Commands.Abstracts;
using Microservice.Course.Application.Constants;
using Microservice.Course.Application.SharedQueries;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Exceptions;
using Thunder.Platform.Core.Extensions;
using Thunder.Platform.Core.Timing;

namespace Microservice.Course.Application.Commands.CommandHandlers
{
    public class ChangeAnnouncementStatusCommandHandler : BaseCommandHandler<ChangeAnnouncementStatusCommand>
    {
        private readonly IReadOnlyRepository<Announcement> _readAnnouncementRepository;
        private readonly IReadOnlyRepository<CourseEntity> _readCourseRepository;
        private readonly AnnouncementCudLogic _announcementCudLogicCudLogic;
        private readonly GetAggregatedAnnouncementSharedQuery _getAggregatedAnnouncementSharedQuery;
        private readonly SendAnnouncementNotifyLearnerLogic _sendAnnouncementNotifyLearnerLogic;

        public ChangeAnnouncementStatusCommandHandler(
            IUnitOfWorkManager unitOfWorkManager,
            IUserContext userContext,
            IReadOnlyRepository<Announcement> readAnnouncementRepository,
            AnnouncementCudLogic announcementCudLogicCudLogic,
            IAccessControlContext<CourseUser> accessControlContext,
            GetAggregatedAnnouncementSharedQuery getAggregatedAnnouncementSharedQuery,
            IReadOnlyRepository<CourseEntity> readCourseRepository,
            SendAnnouncementNotifyLearnerLogic sendAnnouncementNotifyLearnerLogic) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readAnnouncementRepository = readAnnouncementRepository;
            _announcementCudLogicCudLogic = announcementCudLogicCudLogic;
            _getAggregatedAnnouncementSharedQuery = getAggregatedAnnouncementSharedQuery;
            _readCourseRepository = readCourseRepository;
            _sendAnnouncementNotifyLearnerLogic = sendAnnouncementNotifyLearnerLogic;
        }

        protected override async Task HandleAsync(ChangeAnnouncementStatusCommand command, CancellationToken cancellationToken)
        {
            var announcementsQuery = _readAnnouncementRepository.GetAll()
               .WhereIf(command.ForAnnouncements.HasSpecificAnnouncementIds(), p => command.ForAnnouncements.Ids.Contains(p.Id))
               .WhereIf(command.ForAnnouncements.ScheduleDateBefore != null, p => p.ScheduleDate < command.ForAnnouncements.ScheduleDateBefore)
               .WhereIf(command.ForAnnouncements.ScheduleDateFrom != null, p => p.ScheduleDate >= command.ForAnnouncements.ScheduleDateFrom);
            var aggregatedAnnouncements =
                (await _getAggregatedAnnouncementSharedQuery.FullByQuery(announcementsQuery, cancellationToken))
                .WhereIf(!command.ForAnnouncements.HasSpecificAnnouncementIds(), p => p.Announcement.ValidateCanModify(p.Course))
                .ToList();

            EnsureHasCudAnnouncementPermission(aggregatedAnnouncements);

            if (command.ForAnnouncements.HasSpecificAnnouncementIds() && aggregatedAnnouncements.Any(p => !p.Announcement.ValidateCanModify(p.Course)))
            {
                EnsureBusinessLogicValid(aggregatedAnnouncements, p => p.Announcement.ValidateCanModify(p.Course));
            }

            foreach (var announcement in aggregatedAnnouncements)
            {
                announcement.Announcement.Status = command.Status;
                announcement.Announcement.ChangedDate = Clock.Now;
                announcement.Announcement.ChangedBy = CurrentUserId;
                if (command.Status == AnnouncementStatus.Sent)
                {
                    announcement.Announcement.SentDate = Clock.Now;
                }
            }

            await _announcementCudLogicCudLogic.UpdateMany(aggregatedAnnouncements.Select(p => p.Announcement).ToList(), cancellationToken);

            await _sendAnnouncementNotifyLearnerLogic.Execute(aggregatedAnnouncements, CurrentUserId, cancellationToken);
        }

        private void EnsureHasCudAnnouncementPermission(List<AnnouncementAggregatedEntityModel> aggregatedAnnouncements)
        {
            var announcementCourseIds = aggregatedAnnouncements.Select(p => p.Course.Id).Distinct().ToList();
            var hasAdminRightForAnnouncementCourseChecker =
                _readCourseRepository.GetHasAdminRightChecker(p => announcementCourseIds.Contains(p.Id), AccessControlContext);
            EnsureValidPermission(
                aggregatedAnnouncements,
                announcement => Announcement.HasCudPermission(
                    CurrentUserId,
                    CurrentUserRoles,
                    announcement.Course,
                    announcement.ClassRun,
                    hasAdminRightForAnnouncementCourseChecker));
        }
    }
}
