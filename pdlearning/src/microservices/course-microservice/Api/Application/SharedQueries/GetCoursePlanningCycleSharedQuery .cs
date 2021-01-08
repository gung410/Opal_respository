using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Course.Application.Models;
using Microservice.Course.Application.SharedQueries.Abstractions;
using Microservice.Course.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Extensions;

namespace Microservice.Course.Application.SharedQueries
{
    public class GetCoursePlanningCycleSharedQuery : BaseSharedQuery
    {
        private readonly IReadOnlyRepository<CoursePlanningCycle> _readCoursePlanningCycleRepository;

        public GetCoursePlanningCycleSharedQuery(
            IReadOnlyRepository<CoursePlanningCycle> readCoursePlanningCycleRepository)
        {
            _readCoursePlanningCycleRepository = readCoursePlanningCycleRepository;
        }

        public async Task<List<CoursePlanningCycleModel>> ByStartDate(
             DateTime? forCoursePlanningCycleStartAfter,
             DateTime? forCoursePlanningCycleStartBefore,
             CancellationToken cancellationToken = default)
        {
            var coursePlanningCycles = await _readCoursePlanningCycleRepository
                    .GetAll()
                    .Where(p => p.StartDate.HasValue && p.EndDate.HasValue)
                    .WhereIf(forCoursePlanningCycleStartAfter.HasValue, p => p.StartDate >= forCoursePlanningCycleStartAfter)
                    .WhereIf(forCoursePlanningCycleStartBefore.HasValue, p => p.StartDate < forCoursePlanningCycleStartBefore)
                    .ToListAsync(cancellationToken);

            return coursePlanningCycles.Select(p => new CoursePlanningCycleModel(p)).ToList();
        }
    }
}
