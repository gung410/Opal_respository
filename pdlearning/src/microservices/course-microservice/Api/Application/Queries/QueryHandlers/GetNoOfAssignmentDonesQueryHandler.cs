using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.Models;
using Microservice.Course.Application.Queries.Abstracts;
using Microservice.Course.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Course.Application.Queries.QueryHandlers
{
    public class GetNoOfAssignmentDonesQueryHandler : BaseQueryHandler<GetNoOfAssignmentDonesQuery, IEnumerable<NoOfAssignmentDoneInfoModel>>
    {
        private readonly IReadOnlyRepository<Assignment> _readAssignmentRepository;
        private readonly IReadOnlyRepository<ParticipantAssignmentTrack> _readParticipantAssignmentTrackRepository;
        private readonly IReadOnlyRepository<ClassRun> _readClassRunRepository;

        public GetNoOfAssignmentDonesQueryHandler(
            IReadOnlyRepository<ClassRun> readClassRunRepository,
            IReadOnlyRepository<Assignment> readAssignmentRepository,
            IReadOnlyRepository<ParticipantAssignmentTrack> readParticipantAssignmentTrackRepository,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            IUnitOfWorkManager unitOfWorkManager) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readClassRunRepository = readClassRunRepository;
            _readAssignmentRepository = readAssignmentRepository;
            _readParticipantAssignmentTrackRepository = readParticipantAssignmentTrackRepository;
        }

        protected override async Task<IEnumerable<NoOfAssignmentDoneInfoModel>> HandleAsync(GetNoOfAssignmentDonesQuery query, CancellationToken cancellationToken)
        {
            var classRun = await _readClassRunRepository.GetAll().FirstOrDefaultAsync(x => x.Id == query.ClassRunId);

            var totalAssignments = await _readAssignmentRepository.GetAll()
                .Where(x => x.ClassRunId == classRun.Id && x.CourseId == classRun.CourseId)
                .CountAsync(cancellationToken);

            if (totalAssignments == 0)
            {
                totalAssignments = await _readAssignmentRepository.GetAll()
                    .Where(x => x.CourseId == classRun.CourseId && x.ClassRunId == null)
                    .CountAsync(cancellationToken);
            }

            var registrationAssignmentDonesDic = (await _readParticipantAssignmentTrackRepository.GetAll()
                .Where(ParticipantAssignmentTrack.IsDoneExpr())
                .Where(p => query.RegistrationIds.Contains(p.RegistrationId))
                .GroupBy(x => x.RegistrationId)
                .Select(group => new
                {
                    RegistrationId = group.Key,
                    DoneAssignments = group.Count()
                })
                .ToListAsync(cancellationToken))
                .ToDictionary(p => p.RegistrationId, p => p.DoneAssignments);

            return query.RegistrationIds.Select(p => new NoOfAssignmentDoneInfoModel
            {
                RegistrationId = p,
                TotalAssignments = totalAssignments,
                DoneAssignments = registrationAssignmentDonesDic.ContainsKey(p) ? registrationAssignmentDonesDic[p] : 0
            });
        }
    }
}
