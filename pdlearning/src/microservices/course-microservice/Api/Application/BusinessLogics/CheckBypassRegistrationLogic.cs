using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Course.Application.BusinessLogics.Abstractions;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Course.Application.BusinessLogics
{
    public class CheckBypassApprovalRegistrationLogic : BaseBusinessLogic
    {
        private readonly IReadOnlyRepository<CourseEntity> _readCourseRepository;
        private readonly IReadOnlyRepository<Registration> _readRegistrationRepository;

        public CheckBypassApprovalRegistrationLogic(
            IReadOnlyRepository<CourseEntity> readCourseRepository,
            IReadOnlyRepository<Registration> readRegistrationRepository,
            IUserContext userContext) : base(userContext)
        {
            _readCourseRepository = readCourseRepository;
            _readRegistrationRepository = readRegistrationRepository;
        }

        public async Task<Dictionary<Guid, bool>> Execute(List<Guid> registrationsIds, CancellationToken cancellationToken = default)
        {
            var publicCoursesQuery = _readCourseRepository.GetAll().Where(x => x.RegistrationMethod == RegistrationMethod.Public);
            var bypassRegistrationIdList = await _readRegistrationRepository.GetAll()
                .Where(x => registrationsIds.Contains(x.Id))
                .Join(
                    publicCoursesQuery,
                    x => x.CourseId,
                    p => p.Id,
                    (registration, course) => registration.Id)
                .ToListAsync(cancellationToken);

            var bypassRegistrationIdHashSet = bypassRegistrationIdList.ToHashSet();

            return registrationsIds.ToDictionary(x => x, x => bypassRegistrationIdHashSet.Contains(x));
        }
    }
}
