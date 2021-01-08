using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Course.Application.BusinessLogics.Abstractions;
using Microservice.Course.Application.SharedQueries;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Extensions;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.BusinessLogics
{
    /// <summary>
    /// This is logic service to support set Not-Started participants failed.
    /// </summary>
    public class SetAutoFailedRegistrationsLogic : BaseBusinessLogic
    {
        private readonly IReadOnlyRepository<Registration> _readRegistrationRepository;
        private readonly GetAggregatedRegistrationSharedQuery _getAggregatedRegistrationSharedQuery;

        public SetAutoFailedRegistrationsLogic(
            IReadOnlyRepository<Registration> readRegistrationRepository,
            GetAggregatedRegistrationSharedQuery getAggregatedRegistrationSharedQuery,
            IUserContext userContext) : base(userContext)
        {
            _readRegistrationRepository = readRegistrationRepository;
            _getAggregatedRegistrationSharedQuery = getAggregatedRegistrationSharedQuery;
        }

        public async Task<List<Registration>> Execute(
            IQueryable<ClassRun> classrunsQuery,
            CancellationToken cancellationToken = default)
        {
            // Filter need set expired registrations classrun without course checking to reduce amount of classruns
            var classRuns = await classrunsQuery
                .Where(ClassRun.NeedSetAutoFailedRegistrationsClassRunExpr())
                .ToListAsync(cancellationToken);
            return await Execute(classRuns, cancellationToken);
        }

        public async Task<List<Registration>> Execute(
            List<ClassRun> classruns,
            CancellationToken cancellationToken = default)
        {
            var needSetAutoFailedRegistrationsClassRuns = classruns
                .Where(p => p.NeedSetAutoFailedRegistrationsClassRun())
                .Select(p => p.Id)
                .ToList();

            var setAutoFailedRegistrationsQuery = _readRegistrationRepository.GetAll()
                .Where(p => needSetAutoFailedRegistrationsClassRuns.Contains(p.ClassRunId))
                .Where(Registration.CanBeSetAutoFailedExpr());

            var setAutoFailedAggregatedRegistrations = (await _getAggregatedRegistrationSharedQuery
                    .FullByQuery(setAutoFailedRegistrationsQuery, cancellationToken: cancellationToken))
                .SelectList(p =>
                {
                    p.Registration.LearningStatus = LearningStatus.Failed;
                    return p;
                });

            return setAutoFailedAggregatedRegistrations.SelectList(p => p.Registration);
        }
    }
}
