using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.BusinessLogics;
using Microservice.Course.Application.Models;
using Microservice.Course.Application.Queries.Abstracts;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Course.Application.Queries.QueryHandlers
{
    public class ValidateNominateLearnersQueryHandler : BaseQueryHandler<ValidateNominateLearnersQuery, List<ValidateNominateLearnerResultModel>>
    {
        private readonly IReadOnlyRepository<CourseEntity> _readCourseRepository;
        private readonly IReadOnlyRepository<Registration> _readRegistrationRepository;
        private readonly ValidateNominatedLearnerLogic _validateNominatedLearnerLogic;

        public ValidateNominateLearnersQueryHandler(
            IReadOnlyRepository<CourseEntity> readCourseRepository,
            IReadOnlyRepository<Registration> readRegistrationRepository,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            IUnitOfWorkManager unitOfWorkManager,
            ValidateNominatedLearnerLogic validateNominatedLearnerLogic) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readCourseRepository = readCourseRepository;
            _readRegistrationRepository = readRegistrationRepository;
            _validateNominatedLearnerLogic = validateNominatedLearnerLogic;
        }

        protected override async Task<List<ValidateNominateLearnerResultModel>> HandleAsync(ValidateNominateLearnersQuery query, CancellationToken cancellationToken)
        {
            var courseIds = query.Registrations.Select(x => x.CourseId).ToList();
            var coursesDict = await _readCourseRepository
                .GetAll()
                .Where(x => courseIds.Contains(x.Id) && x.Status == CourseStatus.Published)
                .ToDictionaryAsync(x => x.Id, cancellationToken);

            var courseToUsersToExistedInProgressRegistrationsDic = (await _readRegistrationRepository.GetAll()
                    .Where(x => query.GetRegistrationCourseIds().Contains(x.CourseId))
                    .Where(Registration.InProgressExpr()).ToListAsync(cancellationToken))
                .GroupBy(x => x.CourseId)
                .ToDictionary(x => x.Key, x => x.GroupBy(p => p.UserId).ToDictionary(p => p.Key, p => p.ToList()));

            return query.Registrations
                .Select(nominateLearner =>
                {
                    if (!coursesDict.ContainsKey(nominateLearner.CourseId))
                    {
                        throw new InvalidDataException();
                    }

                    return new ValidateNominateLearnerResultModel
                    {
                        UserId = nominateLearner.UserId,
                        ValidateResultCode = _validateNominatedLearnerLogic.Execute(
                            coursesDict[nominateLearner.CourseId],
                            courseToUsersToExistedInProgressRegistrationsDic.GetValueOrDefault(nominateLearner.CourseId)?.GetValueOrDefault(nominateLearner.UserId))
                    };
                })
                .ToList();
        }
    }
}
