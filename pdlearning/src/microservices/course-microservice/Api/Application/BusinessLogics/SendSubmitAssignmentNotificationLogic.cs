using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Course.Application.AggregatedEntityModels;
using Microservice.Course.Application.BusinessLogics.Abstractions;
using Microservice.Course.Application.Constants;
using Microservice.Course.Application.Events.Todos;
using Microservice.Course.Application.SharedQueries;
using Microservice.Course.Settings;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.BusinessLogics
{
    public class SendSubmitAssignmentNotificationLogic : BaseBusinessLogic
    {
        private readonly IThunderCqrs _thunderCqrs;
        private readonly WebAppLinkBuilder _webAppLinkBuilder;
        private readonly GetAggregatedRegistrationSharedQuery _getAggregatedRegistrationSharedQuery;

        public SendSubmitAssignmentNotificationLogic(
            WebAppLinkBuilder webAppLinkBuilder,
            GetAggregatedRegistrationSharedQuery getAggregatedRegistrationSharedQuery,
            IThunderCqrs thunderCqrs,
            IUserContext userContext) : base(userContext)
        {
            _webAppLinkBuilder = webAppLinkBuilder;
            _getAggregatedRegistrationSharedQuery = getAggregatedRegistrationSharedQuery;
            _thunderCqrs = thunderCqrs;
        }

        public async Task Execute(List<ParticipantAssignmentTrackAggregatedEntityModel> aggregatedParticipantAssignmentTracks, CancellationToken cancellationToken = default)
        {
            var aggregatedRegistrations =
                (await _getAggregatedRegistrationSharedQuery
                    .WithClassAndCourseByRegistrationIds(aggregatedParticipantAssignmentTracks.Select(x => x.ParticipantAssignmentTrack.RegistrationId).ToList(), cancellationToken))
                .ToDictionary(p => p.Registration.Id);

            var events = aggregatedParticipantAssignmentTracks
                  .Where(p => aggregatedRegistrations.ContainsKey(p.ParticipantAssignmentTrack.RegistrationId))
                  .Select(p =>
                  {
                      var course = aggregatedRegistrations[p.ParticipantAssignmentTrack.RegistrationId].Course;
                      var classRun = aggregatedRegistrations[p.ParticipantAssignmentTrack.RegistrationId].ClassRun;

                      var assignmentDetailLink = _webAppLinkBuilder.GetAssignmentDetailLinkForLMMModule(
                           LMMTabConfigurationConstant.CoursesTab,
                           LMMTabConfigurationConstant.CourseInfoTab,
                           LMMTabConfigurationConstant.AllClassRunsTab,
                           CourseDetailModeConstant.View,
                           LMMTabConfigurationConstant.ClassRunInfoTab,
                           ClassRunDetailModeConstant.View,
                           LMMTabConfigurationConstant.AssigneesTab,
                           AssignmentDetailModeConstant.View,
                           course.Id,
                           classRun.Id,
                           p.ParticipantAssignmentTrack.AssignmentId);

                      return new SubmittedAssignmentNotifyCourseFacilitatorEvent(
                        Guid.Empty,
                        new SubmittedAssignmentNotifyCourseFacilitatorPayload
                        {
                            CourseName = course.CourseName,
                            ClassRunTitle = classRun.ClassTitle,
                            ActionUrl = assignmentDetailLink
                        },
                        course.GetAssignmentManagementUserIds().Concat(classRun.GetAssignmentManagementUserIds()).ToList());
                  });

            await _thunderCqrs.SendEvents(events);
        }
    }
}
