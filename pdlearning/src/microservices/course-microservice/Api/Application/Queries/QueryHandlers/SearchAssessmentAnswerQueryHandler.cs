using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.Models;
using Microservice.Course.Application.Queries.Abstracts;
using Microservice.Course.Application.SharedQueries;
using Microservice.Course.Common.Extensions;
using Microservice.Course.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Extensions;

namespace Microservice.Course.Application.Queries.QueryHandlers
{
    public class SearchAssessmentAnswerQueryHandler : BaseQueryHandler<SearchAssessmentAnswerQuery, PagedResultDto<AssessmentAnswerModel>>
    {
        private readonly IReadOnlyRepository<AssessmentAnswer> _readAssessmentAnswerRepository;

        public SearchAssessmentAnswerQueryHandler(
            IReadOnlyRepository<AssessmentAnswer> readAssessmentAnswerRepository,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            IUnitOfWorkManager unitOfWorkManager) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readAssessmentAnswerRepository = readAssessmentAnswerRepository;
        }

        protected override async Task<PagedResultDto<AssessmentAnswerModel>> HandleAsync(SearchAssessmentAnswerQuery query, CancellationToken cancellationToken)
        {
            var dbQuery = _readAssessmentAnswerRepository.GetAll();

            dbQuery = dbQuery
                .WhereIf(query.ParticipantAssignmentTrackId != null, p => p.ParticipantAssignmentTrackId == query.ParticipantAssignmentTrackId)
                .WhereIf(query.UserId != null, p => p.UserId == query.UserId);

            var totalCount = await dbQuery.CountAsync(cancellationToken);

            if (query.PageInfo != null && query.PageInfo.MaxResultCount == 0)
            {
                return new PagedResultDto<AssessmentAnswerModel>(totalCount);
            }

            dbQuery = dbQuery.OrderByDescending(p => p.CreatedDate);

            dbQuery = ApplyPaging(dbQuery, query.PageInfo);

            var entities = await dbQuery.Select(p => new AssessmentAnswerModel(p)).ToListAsync();

            return new PagedResultDto<AssessmentAnswerModel>(totalCount, entities);
        }
    }
}
