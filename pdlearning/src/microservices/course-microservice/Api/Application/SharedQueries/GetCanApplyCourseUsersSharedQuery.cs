using System;
using System.Linq;
using Microservice.Course.Application.SharedQueries.Abstractions;
using Microservice.Course.Common.Extensions;
using Microservice.Course.Common.Helpers;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Extensions;
using Thunder.Service.Authentication;

namespace Microservice.Course.Application.SharedQueries
{
    public class GetCanApplyCourseUsersSharedQuery : BaseSharedQuery
    {
        private readonly IReadOnlyRepository<Registration> _readRegistrationRepository;

        public GetCanApplyCourseUsersSharedQuery(IReadOnlyRepository<Registration> readRegistrationRepository)
        {
            _readRegistrationRepository = readRegistrationRepository;
        }

        public IQueryable<CourseUser> FromQuery(
            IQueryable<CourseUser> usersQuery,
            CourseEntity course,
            bool followCourseTargetParticipant)
        {
            usersQuery = usersQuery.Where(x => x.Status == CourseUserStatus.Active);
            usersQuery = usersQuery.WhereIf(
                followCourseTargetParticipant && course.PlaceOfWork == PlaceOfWorkType.ApplicableForUsersInSpecificOrganisation,
                CourseUser.IsUserApplicableInCourseOrganisationExpr(course));
            usersQuery = usersQuery.WhereIf(
                followCourseTargetParticipant,
                CourseUser.HasRoleExpr(F.List(UserRoles.Learner))
                    .AndAlso(CourseUser.IsUserApplicableInCourseMetaDataExpr(course)));

            var courseParticipantUserIdsQuery = _readRegistrationRepository.GetAll()
                .Where(Registration.IsParticipantExpr())
                .Where(Registration.IsLearningFinishedExpr().Not())
                .Where(p => p.CourseId == course.Id)
                .Select(p => (Guid?)p.UserId).Distinct();

            var exceedMaxLearningTimeUserIds = _readRegistrationRepository.GetAll()
                .Where(p => p.CourseId == course.Id)
                .Where(Registration.IsParticipantExpr().AndAlso(Registration.IsLearningFinishedExpr()))
                .GroupBy(x => x.UserId, x => x.Id)
                .Where(x => x.Count() >= course.MaxReLearningTimes)
                .Select(x => (Guid?)x.Key);

            var unavailableUserIds = courseParticipantUserIdsQuery.Union(exceedMaxLearningTimeUserIds).Distinct();

            var notParticipantUserIdsQuery =
                (from user in usersQuery
                 join participantId in unavailableUserIds on user.Id equals participantId into gj
                 from subUser in gj.DefaultIfEmpty()
                 select new
                 {
                     user.Id,
                     NotParticipant = subUser == null
                 })
                .Where(_ => _.NotParticipant)
                .Select(t => t.Id);

            return usersQuery.Join(notParticipantUserIdsQuery, p => p.Id, p => p, (user, userId) => user);
        }
    }
}
