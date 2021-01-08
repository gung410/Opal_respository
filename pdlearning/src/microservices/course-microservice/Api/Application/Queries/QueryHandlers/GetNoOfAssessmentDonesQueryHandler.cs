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
    public class GetNoOfAssessmentDonesQueryHandler : BaseQueryHandler<GetNoOfAssessmentDonesQuery, IEnumerable<NoOfAssessmentDoneInfoModel>>
    {
        private readonly IReadOnlyRepository<AssessmentAnswer> _readAssessmentAnswerRepository;

        public GetNoOfAssessmentDonesQueryHandler(
            IReadOnlyRepository<AssessmentAnswer> readAssessmentAnswerRepository,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            IUnitOfWorkManager unitOfWorkManager) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readAssessmentAnswerRepository = readAssessmentAnswerRepository;
        }

        protected override async Task<IEnumerable<NoOfAssessmentDoneInfoModel>> HandleAsync(GetNoOfAssessmentDonesQuery query, CancellationToken cancellationToken)
        {
            var peerAssessmentDonesDic = (await _readAssessmentAnswerRepository.GetAll()
                .Where(AssessmentAnswer.IsPeerAssessmentExpr())
                .Where(p => query.ParticipantAssignmentTrackIds.Contains(p.ParticipantAssignmentTrackId))
                .GroupBy(x => x.ParticipantAssignmentTrackId)
                .Select(group => new
                {
                    ParticipantAssignmentTrackId = group.Key,
                    TotalAssessments = group.Count(),
                    DoneAssessments = group.AsQueryable().Where(AssessmentAnswer.IsDoneExpr()).Count()
                })
                .ToListAsync(cancellationToken))
                .ToDictionary(p => p.ParticipantAssignmentTrackId, p => p);

            return query.ParticipantAssignmentTrackIds.Select(p => new NoOfAssessmentDoneInfoModel
            {
                ParticipantAssignmentTrackId = p,
                TotalAssessments = peerAssessmentDonesDic.ContainsKey(p) ? peerAssessmentDonesDic[p].TotalAssessments : 0,
                DoneAssessments = peerAssessmentDonesDic.ContainsKey(p) ? peerAssessmentDonesDic[p].DoneAssessments : 0
            });
        }
    }
}
