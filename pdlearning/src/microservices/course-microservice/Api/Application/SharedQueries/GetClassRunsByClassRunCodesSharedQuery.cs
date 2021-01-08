using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Course.Application.Models;
using Microservice.Course.Application.SharedQueries.Abstractions;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Extensions;

namespace Microservice.Course.Application.SharedQueries
{
    public class GetClassRunsByClassRunCodesSharedQuery : BaseSharedQuery
    {
        private readonly IReadOnlyRepository<ClassRun> _readClassRunRepository;

        public GetClassRunsByClassRunCodesSharedQuery(IReadOnlyRepository<ClassRun> readClassRunRepository)
        {
            _readClassRunRepository = readClassRunRepository;
        }

        public async Task<List<ClassRunModel>> Execute(
            List<string> classRunCodes,
            Guid? courseId,
            List<ClassRunStatus> statuses,
            CancellationToken cancellationToken = default)
        {
            if (classRunCodes == null || !classRunCodes.Any())
            {
                return new List<ClassRunModel>();
            }

            var dbQuery = courseId.HasValue
                ? _readClassRunRepository.GetAll().Where(x => x.CourseId == courseId.Value)
                : _readClassRunRepository.GetAll();

            dbQuery = dbQuery.WhereIf(statuses != null && statuses.Any(), p => statuses.Contains(p.Status));

            var classRuns = await dbQuery
                .Where(x => classRunCodes.Contains(x.ClassRunCode))
                .ToListAsync(cancellationToken);

            return classRuns.Select(x => new ClassRunModel(x)).ToList();
        }
    }
}
