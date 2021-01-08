using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Course.Application.BusinessLogics.Abstractions;
using Microservice.Course.Application.Events.Todos;
using Microservice.Course.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.BusinessLogics
{
    public class SendRegistrationWithdrawalNotificationLogic : BaseBusinessLogic
    {
        private readonly IReadOnlyRepository<CourseUser> _readUserRepository;
        private readonly IReadOnlyRepository<CourseEntity> _readCourseRepository;
        private readonly IThunderCqrs _thunderCqrs;

        public SendRegistrationWithdrawalNotificationLogic(
            IReadOnlyRepository<CourseUser> readUserRepository,
            IReadOnlyRepository<CourseEntity> readCourseRepository,
            IThunderCqrs thunderCqrs,
            IUserContext userContext) : base(userContext)
        {
            _readUserRepository = readUserRepository;
            _readCourseRepository = readCourseRepository;
            _thunderCqrs = thunderCqrs;
        }

        public async Task Execute(
            IEnumerable<Registration> registrations,
            Guid currentUserId,
            CancellationToken cancellationToken = default)
        {
            var user = await _readUserRepository.GetAsync(currentUserId);
            var courseDict = await _readCourseRepository
                .GetAll()
                .Where(x => registrations.Select(p => p.CourseId).Contains(x.Id))
                .ToDictionaryAsync(x => x.Id, x => x, cancellationToken);
            await _thunderCqrs.SendEvents(
                registrations.Select(x => new ManualWithdrawnLearnerNotifyLearnerEvent(
                    currentUserId,
                    new ManualWithdrawnLearnerNotifyLearnerPayload
                    {
                        CAName = user.FullName(),
                        CAEmail = user.Email,
                        CourseTitle = courseDict.ContainsKey(x.CourseId) ? courseDict[x.CourseId].CourseName : string.Empty
                    },
                    new List<Guid> { x.UserId })),
                cancellationToken);
        }
    }
}
