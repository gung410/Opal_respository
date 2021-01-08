using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Course.Application.SharedQueries.Abstractions;
using Microservice.Course.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;

// ReSharper disable once CheckNamespace
namespace Microservice.Course.Application.SharedQueries.GetHaveRegistrationsClassRuns
{
    public class GetHaveRegistrationsClassRunsSharedQuery : BaseSharedQuery
    {
        private readonly IReadOnlyRepository<Registration> _readRegistrationRepository;

        public GetHaveRegistrationsClassRunsSharedQuery(IReadOnlyRepository<Registration> readRegistrationRepository)
        {
            _readRegistrationRepository = readRegistrationRepository;
        }

        public async Task<List<Result>> ByClassRuns(List<ClassRun> classRuns, CancellationToken cancellationToken)
        {
            if (classRuns.Count == 0)
            {
                return Result.Empty();
            }

            var classRunIds = classRuns.Select(x => x.Id);
            var registrationList = await _readRegistrationRepository
                .GetAll()
                .Where(p => classRunIds.Contains(p.ClassRunId))
                .ToListAsync(cancellationToken);
            var classRunIdToRegistrationsDic = registrationList
                .GroupBy(p => p.ClassRunId)
                .ToDictionary(p => p.Key, p => p.ToList());

            return classRuns
                .Where(p => classRunIdToRegistrationsDic.ContainsKey(p.Id))
                .Select(p => new Result(
                    p,
                    classRunIdToRegistrationsDic[p.Id]))
                .ToList();
        }
    }

    public class Result
    {
        public Result(ClassRun classRun, List<Registration> registrations)
        {
            ClassRun = classRun;
            Registrations = registrations;
        }

        public ClassRun ClassRun { get; }

        public List<Registration> Registrations { get; }

        public static List<Result> Empty()
        {
            return new List<Result>();
        }
    }
}
