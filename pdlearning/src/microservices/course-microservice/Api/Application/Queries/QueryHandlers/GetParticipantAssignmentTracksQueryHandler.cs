using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.Models;
using Microservice.Course.Application.Queries.Abstracts;
using Microservice.Course.Application.SharedQueries;
using Microservice.Course.Common.Extensions;
using Microservice.Course.Domain.Constants;
using Microservice.Course.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Extensions;

namespace Microservice.Course.Application.Queries.QueryHandlers
{
    public class GetParticipantAssignmentTracksQueryHandler : BaseQueryHandler<GetParticipantAssignmentTracksQuery, PagedResultDto<ParticipantAssignmentTrackModel>>
    {
        private readonly IReadOnlyRepository<Registration> _readRegistrationRepository;
        private readonly IReadOnlyRepository<ParticipantAssignmentTrack> _readParticipantAssignmentTrackRepository;
        private readonly GetFullTextFilteredEntitiesSharedQuery _getFullTextFilteredEntitiesSharedQuery;
        private readonly IReadOnlyRepository<CourseUser> _readUserRepository;
        private readonly GetUsersSharedQuery _getUsersSharedQuery;

        public GetParticipantAssignmentTracksQueryHandler(
            IReadOnlyRepository<Registration> readRegistrationRepository,
            IReadOnlyRepository<ParticipantAssignmentTrack> readParticipantAssignmentTrackRepository,
            GetFullTextFilteredEntitiesSharedQuery getFullTextFilteredEntitiesSharedQuery,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            IUnitOfWorkManager unitOfWorkManager,
            GetUsersSharedQuery getUsersSharedQuery,
            IReadOnlyRepository<CourseUser> readUserRepository) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readRegistrationRepository = readRegistrationRepository;
            _readParticipantAssignmentTrackRepository = readParticipantAssignmentTrackRepository;
            _getFullTextFilteredEntitiesSharedQuery = getFullTextFilteredEntitiesSharedQuery;
            _getUsersSharedQuery = getUsersSharedQuery;
            _readUserRepository = readUserRepository;
        }

        protected override async Task<PagedResultDto<ParticipantAssignmentTrackModel>> HandleAsync(GetParticipantAssignmentTracksQuery query, CancellationToken cancellationToken)
        {
            var filteredParticipantsQuery = _readRegistrationRepository
                .GetAll()
                .Where(Registration.IsParticipantExpr())
                .WhereIf(query.CourseId != null, x => x.CourseId == query.CourseId)
                .WhereIf(query.ClassRunId != null, x => x.ClassRunId == query.ClassRunId)
                .WhereIf(query.RegistrationIds != null && query.RegistrationIds.Any(), p => query.RegistrationIds.Contains(p.Id));

            IQueryable<CourseUser> filteredUserQuery = null;
            if (!string.IsNullOrEmpty(query.SearchText) || query.Filter?.ContainFilters != null || query.Filter?.FromToFilters != null)
            {
                filteredUserQuery = _getUsersSharedQuery.BySearchText(_readUserRepository.GetAll(), query.SearchText);
                filteredUserQuery = query.Filter?.ContainFilters?.Aggregate(
                filteredUserQuery,
                (current, containFilter) =>
                    current.BuildContainFilter(
                        containFilter.Field,
                        containFilter.Values,
                        NotApplicableMetadataConstants.UserNotApplicableMapping,
                        containFilter.NotContain,
                        (values, fieldName, currentQuery) => _getFullTextFilteredEntitiesSharedQuery.ByFilter(
                            currentQuery,
                            fieldName,
                            values,
                            NotApplicableMetadataConstants.UserNotApplicableMapping.ContainsKey(fieldName) ? NotApplicableMetadataConstants.UserNotApplicableMapping[fieldName] : null))) ?? filteredUserQuery;

                filteredUserQuery = query.Filter?.FromToFilters?.Aggregate(
                    filteredUserQuery,
                    (current, fromToFilter) =>
                        current.BuildFromToFilter(
                            fromToFilter.Field,
                            fromToFilter.FromValue,
                            fromToFilter.ToValue,
                            fromToFilter.EqualFrom,
                            fromToFilter.EqualTo)) ?? filteredUserQuery;
            }

            var dbQuery = _readParticipantAssignmentTrackRepository
                .GetAll()
                .WhereIf(query.AssignmentId != null, x => x.AssignmentId == query.AssignmentId)
                .WhereIf(query.ForCurrentUser == true, x => x.UserId == CurrentUserId)
                .Join(
                    filteredParticipantsQuery,
                    p => p.RegistrationId,
                    p => p.Id,
                    (participantAssignmentTrack, filteredParticipant) => participantAssignmentTrack)
                .JoinIf(
                    filteredUserQuery != null,
                    filteredUserQuery,
                    p => p.UserId,
                    p => p.Id);

            var totalCount = await dbQuery.CountAsync(cancellationToken);
            dbQuery = dbQuery.OrderByDescending(p => p.CreatedDate);
            dbQuery = ApplyPaging(dbQuery, query.PageInfo);

            var data = await GetAssignmentTrackHasAnwser(dbQuery, query.IncludeQuizAssignmentFormAnswer, cancellationToken);

            return new PagedResultDto<ParticipantAssignmentTrackModel>(totalCount, data);
        }

        private async Task<List<ParticipantAssignmentTrackModel>> GetAssignmentTrackHasAnwser(IQueryable<ParticipantAssignmentTrack> dbQuery, bool includeQuizAssignmentFormAnswer, CancellationToken cancellationToken)
        {
            var participantAssignmentTracks = await dbQuery.ToListAsync(cancellationToken);

            if (includeQuizAssignmentFormAnswer)
            {
                return participantAssignmentTracks.Select(participantAssignmentTrack => new ParticipantAssignmentTrackModel(
                         participantAssignmentTrack,
                         participantAssignmentTrack.QuizAnswer)).ToList();
            }
            else
            {
                return participantAssignmentTracks.Select(p => new ParticipantAssignmentTrackModel(p, null)).ToList();
            }
        }
    }
}
