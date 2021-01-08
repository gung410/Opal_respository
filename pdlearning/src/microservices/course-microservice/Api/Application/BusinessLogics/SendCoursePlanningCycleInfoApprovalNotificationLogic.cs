using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Course.Application.BusinessLogics.Abstractions;
using Microservice.Course.Application.Constants;
using Microservice.Course.Application.Events.Todos;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Microservice.Course.Settings;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Cqrs;
using Thunder.Service.Authentication;

namespace Microservice.Course.Application.BusinessLogics
{
    public class SendCoursePlanningCycleInfoApprovalNotificationLogic : BaseBusinessLogic
    {
        private readonly IReadOnlyRepository<CourseUser> _readUserRepository;
        private readonly IReadOnlyRepository<CoursePlanningCycle> _readCoursePlanningCycleRepository;
        private readonly IThunderCqrs _thunderCqrs;
        private readonly WebAppLinkBuilder _webAppLinkBuilder;

        public SendCoursePlanningCycleInfoApprovalNotificationLogic(
            IReadOnlyRepository<CourseUser> readUserRepository,
            IReadOnlyRepository<CoursePlanningCycle> readCoursePlanningCycleRepository,
            IThunderCqrs thunderCqrs,
            WebAppLinkBuilder webAppLinkBuilder,
            IUserContext userContext) : base(userContext)
        {
            _readUserRepository = readUserRepository;
            _readCoursePlanningCycleRepository = readCoursePlanningCycleRepository;
            _thunderCqrs = thunderCqrs;
            _webAppLinkBuilder = webAppLinkBuilder;
        }

        public async Task Execute(
            CourseStatus status,
            List<CourseEntity> courses,
            string comment,
            Guid currentUserId,
            CancellationToken cancellationToken = default)
        {
            var coursePlanningCoordinatorDict = await _readUserRepository
                .GetAll()
                .Where(x => x.Status == CourseUserStatus.Active)
                .Where(CourseUser.HasRoleExpr(new List<string> { UserRoles.CoursePlanningCoordinator, UserRoles.SystemAdministrator }))
                .ToDictionaryAsync(x => x.Id, x => x, cancellationToken);

            var coursePlanningCycleIds = courses
                .Where(x => x.CoursePlanningCycleId.HasValue)
                .Select(x => x.CoursePlanningCycleId.Value)
                .ToList();

            var coursePlanningCycleDict = await _readCoursePlanningCycleRepository.GetAll()
                .Where(p => coursePlanningCycleIds.Contains(p.Id))
                .ToDictionaryAsync(x => x.Id, x => x, cancellationToken);

            var validCourseList = courses
                .Where(x => coursePlanningCoordinatorDict.ContainsKey(currentUserId) && coursePlanningCycleDict.ContainsKey(x.CoursePlanningCycleId.GetValueOrDefault()));

            switch (status)
            {
                case CourseStatus.PlanningCycleVerified:
                    var verifiedCourseInPlanningCycleNotifyOwnerEvents = validCourseList.Select(p =>
                      new VerifiedCourseInPlanningCycleNotifyOwnerEvent(
                          currentUserId,
                          new VerifiedCourseInPlanningCycleNotifyOwnerPayload
                          {
                              CourseTitle = p.CourseName,
                              CPCComment = comment,
                              CPCName = coursePlanningCoordinatorDict[currentUserId].FullName(),
                              PlanningCyclePeriod = coursePlanningCycleDict[p.CoursePlanningCycleId.GetValueOrDefault()].YearCycle.ToString(),
                              ActionUrl = _webAppLinkBuilder.GetCourseDetailLinkInPlanningCycleForCAMModule(
                                  CAMTabConfigurationConstant.PlanningCycleTab,
                                  CAMTabConfigurationConstant.CoursesOfPlanningCycleTab,
                                  CAMTabConfigurationConstant.AllCoursesOfPlanningCycleTab,
                                  CoursePlanningCycleDetailModeConstant.View,
                                  p.CoursePlanningCycleId.GetValueOrDefault(),
                                  CAMTabConfigurationConstant.CourseInfoTab,
                                  CAMTabConfigurationConstant.AllClassRunsTab,
                                  CourseDetailModeConstant.View,
                                  p.Id)
                          },
                          new List<Guid> { p.CreatedBy }));

                    await _thunderCqrs.SendEvents(verifiedCourseInPlanningCycleNotifyOwnerEvents, cancellationToken);
                    break;
                case CourseStatus.VerificationRejected:
                    var rejectedCourseInPlanningCycleNotifyOwnerEvents = validCourseList.Select(p =>
                            new RejectedCourseInPlanningCycleNotifyOwnerEvent(
                                currentUserId,
                                new RejectedCourseInPlanningCycleNotifyOwnerPayload
                                {
                                    CourseTitle = p.CourseName,
                                    CPCComment = comment,
                                    CPCName = coursePlanningCoordinatorDict[currentUserId].FullName(),
                                    PlanningCyclePeriod = coursePlanningCycleDict[p.CoursePlanningCycleId.Value].YearCycle.ToString(),
                                    ActionUrl = _webAppLinkBuilder.GetCourseDetailLinkInPlanningCycleForCAMModule(
                                        CAMTabConfigurationConstant.PlanningCycleTab,
                                        CAMTabConfigurationConstant.CoursesOfPlanningCycleTab,
                                        CAMTabConfigurationConstant.AllCoursesOfPlanningCycleTab,
                                        CoursePlanningCycleDetailModeConstant.View,
                                        p.CoursePlanningCycleId.Value,
                                        CAMTabConfigurationConstant.CourseInfoTab,
                                        CAMTabConfigurationConstant.AllClassRunsTab,
                                        CourseDetailModeConstant.ForVerifier,
                                        p.Id),
                                },
                                new List<Guid> { p.CreatedBy }));
                    await _thunderCqrs.SendEvents(rejectedCourseInPlanningCycleNotifyOwnerEvents, cancellationToken);
                    break;
            }
        }
    }
}
