using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cx.datahub.scheduling.jobs.shared;
using Microservice.Course.Application.Constants;
using Microservice.Course.Application.Enums;
using Microservice.Course.Application.Events.Todos;
using Microservice.Course.Application.SharedQueries;
using Microservice.Course.Infrastructure;
using Microservice.Course.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Timing;
using Thunder.Platform.Cqrs;
using Thunder.Service.Authentication;

namespace Microservice.Course.Application.HangfireJob.RecurringJob
{
    public class CoursePlanningCycleCheckingJob : BaseHangfireJob, ICoursePlanningCycleCheckingJob
    {
        private readonly GetUsersSharedQuery _getUsersSharedQuery;
        private readonly GetCoursePlanningCycleSharedQuery _getCoursePlanningCycleSharedQuery;
        private readonly WebAppLinkBuilder _webAppLinkBuilder;

        public CoursePlanningCycleCheckingJob(
            IThunderCqrs thunderCqrs,
            GetUsersSharedQuery getUsersSharedQuery,
            GetCoursePlanningCycleSharedQuery getCoursePlanningCycleSharedQuery,
            WebAppLinkBuilder webAppLinkBuilder,
            IUnitOfWorkManager unitOfWorkManager,
            ILoggerFactory loggerFactory) : base(thunderCqrs, unitOfWorkManager, loggerFactory)
        {
            _getUsersSharedQuery = getUsersSharedQuery;
            _getCoursePlanningCycleSharedQuery = getCoursePlanningCycleSharedQuery;
            _webAppLinkBuilder = webAppLinkBuilder;
        }

        protected override async Task InternalHandleAsync()
        {
            await NotifyCoursePlanningCycleStarted();
        }

        private async Task NotifyCoursePlanningCycleStarted()
        {
            var coursePlannigCycles = await _getCoursePlanningCycleSharedQuery.ByStartDate(Clock.Now, Clock.Now.AddDays(1));

            if (coursePlannigCycles.Any())
            {
                var coursePlanningCycleStartedToday = coursePlannigCycles.First();
                var divisionCourseContentCreator = await _getUsersSharedQuery.ByDepartmentTypeAndRoles(DepartmentTypeEnum.Division, new List<string> { UserRoles.CourseContentCreator });
                var coursePlanningCoordinator = await _getUsersSharedQuery.ByIds(new List<Guid> { coursePlanningCycleStartedToday.CreatedBy }).FirstOrDefaultAsync();

                await ThunderCqrs.SendEvent(
                      new CoursePlanningCycleStartedNotifyCCCEvent(
                          new CoursePlanningCycleStartedNotifyCCCPayload
                          {
                              CoursePlanningCyclePeriod = coursePlanningCycleStartedToday.YearCycle.ToString(),
                              PlanningStartDate = TimeHelper.ConvertTimeFromUtc(coursePlanningCycleStartedToday.StartDate.Value).ToString(DateTimeFormatConstant.OnlyDate),
                              PlanningEndDate = TimeHelper.ConvertTimeFromUtc(coursePlanningCycleStartedToday.EndDate.Value).ToString(DateTimeFormatConstant.OnlyDate),
                              CPCName = coursePlanningCoordinator.FullName(),
                              CPCEmail = coursePlanningCoordinator.Email,
                              ActionUrl = _webAppLinkBuilder.GetCoursePlanningCycleDetailForCAMModule(
                                       CAMTabConfigurationConstant.PlanningCycleTab,
                                       CAMTabConfigurationConstant.CoursePlanningCycleInfoTab,
                                       CAMTabConfigurationConstant.AllCoursesOfPlanningCycleTab,
                                       CoursePlanningCycleDetailModeConstant.View,
                                       coursePlanningCycleStartedToday.Id)
                          },
                          divisionCourseContentCreator.Select(p => p.Id).ToList()));
            }
        }
    }
}
