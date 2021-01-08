using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.Commands.Abstracts;
using Microservice.Course.Application.Constants;
using Microservice.Course.Application.Events.Todos;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Microservice.Course.Settings;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Exceptions;
using Thunder.Platform.Cqrs;
using Thunder.Service.Authentication;

namespace Microservice.Course.Application.Commands.CommandHandlers
{
    public class SendCourseNominationAnnouncementCommandHandler : BaseCommandHandler<SendCourseNominationAnnouncementCommand>
    {
        private readonly IReadOnlyRepository<CourseUser> _readUserRepository;
        private readonly IReadOnlyRepository<CourseEntity> _readCourseRepository;
        private readonly WebAppLinkBuilder _webAppLinkBuilder;
        private readonly IThunderCqrs _thunderCqrs;

        public SendCourseNominationAnnouncementCommandHandler(
            IReadOnlyRepository<CourseUser> readUserRepository,
            IReadOnlyRepository<CourseEntity> readCourseRepository,
            WebAppLinkBuilder webAppLinkBuilder,
            IUnitOfWorkManager unitOfWorkManager,
            IThunderCqrs thunderCqrs,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readCourseRepository = readCourseRepository;
            _readUserRepository = readUserRepository;
            _webAppLinkBuilder = webAppLinkBuilder;
            _thunderCqrs = thunderCqrs;
        }

        protected override async Task HandleAsync(SendCourseNominationAnnouncementCommand command, CancellationToken cancellationToken)
        {
            var userQuery = _readUserRepository
                .GetAll()
                .Where(x => x.Status == CourseUserStatus.Active);

            // NOTE: In SAM module, Divisional Learning Coordinator display text 'Divisional Learning Coordinator' but value display 'divisiontrainingcoordinator'.
            // So role Divisional Learning Coordinator will be 'DivisionalTrainingCoordinator' in CAM module.
            if (command.SpecificOrganisation == false)
            {
                userQuery = userQuery.Where(CourseUser.HasRoleExpr(new List<string> { UserRoles.DivisionalTrainingCoordinator, UserRoles.SchoolStaffDeveloper }));
            }
            else
            {
                userQuery = userQuery
                    .Where(CourseUser.HasRoleExpr(new List<string> { UserRoles.DivisionalTrainingCoordinator, UserRoles.SchoolStaffDeveloper }))
                    .Where(p => command.Organisations.Any(x => x == p.DepartmentId));
            }

            var receivers = await userQuery.ToListAsync(cancellationToken);
            var course = await _readCourseRepository.GetAsync(command.CourseId);

            EnsureBusinessLogicValid(course.ValidateNotArchived());

            await _thunderCqrs.SendEvents(
                receivers.Select(p =>
                    new SendCourseNominationAnnouncementNotifyEvent(
                        CurrentUserIdOrDefault,
                        new SendCourseNominationAnnouncementNotifyPayload
                        {
                            CourseTitle = course.CourseName,
                            UserName = p.FullName(),
                            ActionUrl = _webAppLinkBuilder.GetAdhocNominationLinkForCAMModule()
                        },
                        new List<Guid> { p.Id },
                        command.GetReplacedTagsMessage(course.CourseName, p.FullName()))),
                cancellationToken);
        }
    }
}
