using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.Commands.Abstracts;
using Microservice.Course.Application.Events.Todos;
using Microservice.Course.Common.Extensions;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Cqrs;
using Thunder.Service.Authentication;

namespace Microservice.Course.Application.Commands.CommandHandlers
{
    public class SendCoursePublicityCommandHandler : BaseCommandHandler<SendCoursePublicityCommand>
    {
        private readonly IReadOnlyRepository<CourseUser> _readUserRepository;
        private readonly IReadOnlyRepository<CourseEntity> _readCourseRepository;
        private readonly IThunderCqrs _thunderCqrs;

        public SendCoursePublicityCommandHandler(
            IReadOnlyRepository<CourseUser> readUserRepository,
            IReadOnlyRepository<CourseEntity> readCourseRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IThunderCqrs thunderCqrs,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readCourseRepository = readCourseRepository;
            _readUserRepository = readUserRepository;
            _thunderCqrs = thunderCqrs;
        }

        protected override async Task HandleAsync(SendCoursePublicityCommand command, CancellationToken cancellationToken)
        {
            var userQuery = _readUserRepository.GetAll()
                           .Where(x => x.Status == CourseUserStatus.Active);

            if (command.SpecificTargetAudience == false)
            {
                userQuery = userQuery.Where(CourseUser.HasRoleExpr(new List<string> { UserRoles.Learner }));
            }
            else
            {
                Expression<Func<CourseUser, bool>> specificExp = courseUser => command.UserIds.Any(x => x == courseUser.Id);
                Expression<Func<CourseUser, bool>> teachingLevelExp = courseUser => courseUser.UserMetadatas.Any(_ => _.Type == UserMetadataValueType.TeachingLevel && _.Value != null && command.TeachingLevels.Contains(_.Value));
                Expression<Func<CourseUser, bool>> teachingSubjectExp = courseUser => courseUser.UserMetadatas.Any(_ => _.Type == UserMetadataValueType.TeachingSubject && _.Value != null && command.TeachingSubjectIds.Contains(_.Value));

                specificExp = command.TeachingLevels.Any()
                    ? specificExp.Or(teachingLevelExp.AndAlsoIf(command.TeachingSubjectIds.Any(), teachingSubjectExp))
                    : specificExp.OrIf(command.TeachingSubjectIds.Any(), teachingSubjectExp);

                userQuery = userQuery
                    .Where(CourseUser.HasRoleExpr(new List<string> { UserRoles.Learner }))
                    .Where(specificExp);
            }

            var receivers = await userQuery.ToListAsync(cancellationToken);
            var course = await _readCourseRepository.GetAsync(command.CourseId);

            EnsureBusinessLogicValid(course.ValidateNotArchived());

            if (!string.IsNullOrEmpty(command.Base64Message) && receivers != null)
            {
                await _thunderCqrs.SendEvents(
                    receivers.Select(p =>
                        new SendCoursePublicityNotifyLearnerEvent(
                            CurrentUserIdOrDefault,
                            new SendCoursePublicityNotifyLearnerPayload
                            {
                                CourseTitle = course.CourseName,
                                UserName = p.FullName(),
                            },
                            new List<Guid> { p.Id },
                            command.GetReplacedTagsMessage(course.CourseName, p.FullName()))),
                    cancellationToken);
            }
        }
    }
}
