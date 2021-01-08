using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Extensions;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.BusinessLogics;
using Microservice.Course.Application.BusinessLogics.EntityCud;
using Microservice.Course.Application.Commands.Abstracts;
using Microservice.Course.Application.Constants;
using Microservice.Course.Application.Events.Todos;
using Microservice.Course.Domain.Constants;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Microservice.Course.Settings;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Exceptions;
using Thunder.Platform.Core.Timing;
using Thunder.Platform.Core.Validation;
using Thunder.Platform.Cqrs;
using Thunder.Service.Authentication;

namespace Microservice.Course.Application.Commands.CommandHandlers
{
    public class ChangeCourseStatusCommandHandler : BaseCommandHandler<ChangeCourseStatusCommand>
    {
        private readonly IReadOnlyRepository<CourseEntity> _readCourseRepository;
        private readonly IReadOnlyRepository<CoursePlanningCycle> _readCoursePlanningCycleRepository;
        private readonly IReadOnlyRepository<CourseUser> _readUserRepository;
        private readonly IThunderCqrs _thunderCqrs;
        private readonly SendCoursePlanningCycleInfoApprovalNotificationLogic _sendCoursePlanningCycleInfoApprovalNotificationLogic;
        private readonly ArchiveCourseLogic _archiveCourseLogic;
        private readonly WebAppLinkBuilder _webAppLinkBuilder;
        private readonly CourseCudLogic _courseCudLogic;
        private readonly ProcessCoursePdAreaThemeCodeLogic _processCoursePdAreaThemeCodeLogic;

        public ChangeCourseStatusCommandHandler(
            IUnitOfWorkManager unitOfWorkManager,
            IReadOnlyRepository<CourseEntity> readCourseRepository,
            IReadOnlyRepository<CourseUser> readUserRepository,
            IReadOnlyRepository<CoursePlanningCycle> readCoursePlanningCycleRepository,
            IThunderCqrs thunderCqrs,
            IUserContext userContext,
            SendCoursePlanningCycleInfoApprovalNotificationLogic sendCoursePlanningCycleInfoApprovalNotificationLogic,
            ArchiveCourseLogic archiveCourseLogic,
            CourseCudLogic courseCudLogic,
            IAccessControlContext<CourseUser> accessControlContext,
            WebAppLinkBuilder webAppLinkBuilder,
            ProcessCoursePdAreaThemeCodeLogic processCoursePdAreaThemeCodeLogic) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readCourseRepository = readCourseRepository;
            _readUserRepository = readUserRepository;
            _readCoursePlanningCycleRepository = readCoursePlanningCycleRepository;
            _thunderCqrs = thunderCqrs;
            _sendCoursePlanningCycleInfoApprovalNotificationLogic = sendCoursePlanningCycleInfoApprovalNotificationLogic;
            _archiveCourseLogic = archiveCourseLogic;
            _webAppLinkBuilder = webAppLinkBuilder;
            _processCoursePdAreaThemeCodeLogic = processCoursePdAreaThemeCodeLogic;
            _courseCudLogic = courseCudLogic;
        }

        protected override async Task HandleAsync(ChangeCourseStatusCommand command, CancellationToken cancellationToken)
        {
            var courses = await _readCourseRepository.GetAllListAsync(course => command.Ids.Contains(course.Id));
            var coursesHasAdminRightChecker = _readCourseRepository.GetHasAdminRightChecker(courses, AccessControlContext);
            switch (command.Status)
            {
                case CourseStatus.PendingApproval:
                    {
                        await SubmitCourseForApproval(courses, command, coursesHasAdminRightChecker, cancellationToken);
                        break;
                    }

                case CourseStatus.Approved:
                case CourseStatus.Rejected:
                    {
                        await ApproveOrRejectCourse(courses, command, coursesHasAdminRightChecker, cancellationToken);
                        break;
                    }

                case CourseStatus.PlanningCycleVerified:
                case CourseStatus.VerificationRejected:
                    {
                        await VerifyOrRejectCourse(courses, command, coursesHasAdminRightChecker, cancellationToken);
                        break;
                    }

                case CourseStatus.Published:
                    {
                        await PublishCourse(courses, coursesHasAdminRightChecker, cancellationToken);
                        break;
                    }

                case CourseStatus.Unpublished:
                    {
                        UnpublishCourse(courses, coursesHasAdminRightChecker);
                        break;
                    }

                case CourseStatus.Draft:
                    {
                        MakeCourseDraft(courses, coursesHasAdminRightChecker);
                        break;
                    }

                case CourseStatus.PlanningCycleCompleted:
                    {
                        CompletePlanningCourse(courses, coursesHasAdminRightChecker);
                        break;
                    }

                case CourseStatus.Completed:
                    {
                        CompleteCourse(courses);
                        break;
                    }

                case CourseStatus.Archived:
                    {
                        await ArchiveCourses(courses, coursesHasAdminRightChecker, cancellationToken);
                        break;
                    }
            }

            await _courseCudLogic.UpdateMany(courses, cancellationToken);
        }

        private async Task ArchiveCourses(List<CourseEntity> courses, Func<CourseEntity, bool> haveFullRight, CancellationToken cancellationToken)
        {
            EnsureValidPermission(
                courses,
                course => course.HasArchivalPermission(CurrentUserId, CurrentUserRoles, haveFullRight));

            await _archiveCourseLogic.EnsureCanArchiveCourses(courses, cancellationToken);

            await _archiveCourseLogic.ArchiveCourses(courses, CurrentUserIdOrDefault, cancellationToken);
        }

        private void MakeCourseDraft(List<CourseEntity> courses, Func<CourseEntity, bool> haveFullRight)
        {
            EnsureValidPermission(
                courses,
                course => course.HasUpdatePermission(CurrentUserId, CurrentUserRoles, haveFullRight, p => HasPermissionPrefix(p)));

            EnsureBusinessLogicValid(courses, course => course.IsEditable(course.CourseType));

            foreach (var course in courses)
            {
                course.Status = CourseStatus.Draft;
                course.ChangedBy = CurrentUserId ?? Guid.Empty;
            }
        }

        private void CompletePlanningCourse(List<CourseEntity> courses, Func<CourseEntity, bool> haveFullRight)
        {
            EnsureValidPermission(
                courses,
                course => course.HasContentCreatorPermission(CurrentUserId, CurrentUserRoles, haveFullRight));

            EnsureBusinessLogicValid(courses, course => course.CanCompletePlanning());

            foreach (var course in courses)
            {
                course.Status = CourseStatus.PlanningCycleCompleted;
                course.ChangedBy = CurrentUserId ?? Guid.Empty;
            }
        }

        private void CompleteCourse(List<CourseEntity> courses)
        {
            EnsureBusinessLogicValid(courses, course => course.CanCompleteCourse());

            foreach (var course in courses)
            {
                course.Status = CourseStatus.Completed;
            }
        }

        private async Task PublishCourse(List<CourseEntity> courses, Func<CourseEntity, bool> haveFullRight, CancellationToken cancellationToken)
        {
            EnsureValidPermission(
                courses,
                course => course.HasPublishUnpublishPermission(CurrentUserId, CurrentUserRoles, haveFullRight, p => HasPermissionPrefix(p)));

            EnsureBusinessLogicValid(courses, course => course.ValidateCanPublish());

            var missingPdAreaThemeCodeCourses = courses
                .Where(p => string.IsNullOrEmpty(p.PDAreaThemeCode))
                .ToList();
            await _processCoursePdAreaThemeCodeLogic.ForCourses(missingPdAreaThemeCodeCourses, cancellationToken);

            var pDAreaThemeCodes = courses.Select(x => x.PDAreaThemeCode).Distinct().ToList();
            var existedCourseCodes = await _readCourseRepository.GetAll()
                .Where(x => !string.IsNullOrEmpty(x.CourseCode) && !string.IsNullOrEmpty(x.PDAreaThemeCode) && pDAreaThemeCodes.Contains(x.PDAreaThemeCode))
                .Select(x => new { x.PDAreaThemeCode, x.CourseCode })
                .ToListAsync(cancellationToken);

            var existedCourseCodeSets = existedCourseCodes.Select(p => p.CourseCode).ToHashSet();
            var existedPdAreaThemeToMaxCodeNumberDic = existedCourseCodes
                .GroupBy(x => x.PDAreaThemeCode)
                .Select(x => new { x.Key, Total = x.Count() })
                .ToDictionary(x => x.Key, x => x.Total);

            foreach (var course in courses)
            {
                course.VersionNo = course.NeedUpdateVersionNo(CourseStatus.Published) ? course.VersionNo + 1 : course.VersionNo;
                course.Status = CourseStatus.Published;
                course.PublishDate = Clock.Now;
                course.ChangedBy = CurrentUserId ?? Guid.Empty;

                // [TODO]: Need to remove if there is NOT any course has IsMigrated = true in production database
                course.IsMigrated = false;

                if (string.IsNullOrEmpty(course.CourseCode))
                {
                    course.CourseCode = GenerateCourseCode(course.PDAreaThemeCode, existedPdAreaThemeToMaxCodeNumberDic, existedCourseCodeSets);
                }
            }
        }

        private async Task ApproveOrRejectCourse(
            List<CourseEntity> courses,
            ChangeCourseStatusCommand command,
            Func<CourseEntity, bool> haveFullRight,
            CancellationToken cancellationToken)
        {
            EnsureValidPermission(courses.TrueForAll(course =>
                course.HasApprovalPermission(CurrentUserId, CurrentUserRoles, haveFullRight)));

            EnsureBusinessLogicValid(courses, p => p.ValidateCanBeApproval());

            foreach (var course in courses)
            {
                course.Status = command.Status;
                course.ApprovalDate = Clock.Now;
                course.ChangedBy = CurrentUserId ?? Guid.Empty;
            }

            await SendApprovalNotificationToCoursePlanningCoordinator(courses.Where(x => x.CoursePlanningCycleId.HasValue && x.Status == CourseStatus.Approved), cancellationToken);
            await SendApprovalNotificationToOwner(command.Status, command.Comment, courses, cancellationToken);
            await SendApprovalCourseMluNotificationToCollaborator(command.Status, courses, cancellationToken);
        }

        private async Task VerifyOrRejectCourse(List<CourseEntity> courses, ChangeCourseStatusCommand command, Func<CourseEntity, bool> haveFullRight, CancellationToken cancellationToken)
        {
            EnsureValidPermission(
                courses,
                course => course.HasVerifyPlanningCycleOrRejectVerificationPermission(CurrentUserId, CurrentUserRoles, haveFullRight));

            EnsureBusinessLogicValid(courses, course => course.CanVerifyPlanningCycleOrRejectVerification());

            foreach (var course in courses)
            {
                course.Status = command.Status;
                course.VerifiedDate = command.Status == CourseStatus.PlanningCycleVerified ? Clock.Now : course.VerifiedDate;
                course.ChangedBy = CurrentUserId ?? Guid.Empty;
            }

            await _sendCoursePlanningCycleInfoApprovalNotificationLogic.Execute(command.Status, courses, command.Comment, CurrentUserIdOrDefault, cancellationToken);
        }

        private void UnpublishCourse(List<CourseEntity> courses, Func<CourseEntity, bool> haveFullRight)
        {
            EnsureValidPermission(
                courses,
                course => course.HasPublishUnpublishPermission(CurrentUserId, CurrentUserRoles, haveFullRight, p => HasPermissionPrefix(p)));

            EnsureBusinessLogicValid(courses, course => course.ValidateCanUnpublishCourse());

            foreach (var course in courses)
            {
                course.Status = CourseStatus.Unpublished;
                course.ChangedBy = CurrentUserId ?? Guid.Empty;
            }
        }

        private async Task SubmitCourseForApproval(
            List<CourseEntity> courses,
            ChangeCourseStatusCommand command,
            Func<CourseEntity, bool> haveFullRight,
            CancellationToken cancellationToken)
        {
            EnsureValidPermission(
                courses,
                course => course.HasSubmitCourseForApprovalPermission(CurrentUserId, CurrentUserRoles, haveFullRight));

            EnsureBusinessLogicValid(courses, course => course.CanSubmitForApproval());

            foreach (var course in courses)
            {
                course.Status = command.Status;
                course.SubmittedDate = Clock.Now;
                course.ChangedBy = CurrentUserIdOrDefault;
            }

            await SendPendingApprovalNotification(courses, cancellationToken);
        }

        private string GenerateCourseCode(string pDAreaThemeCode, Dictionary<string, int> existedPdAreaThemeToMaxCodeNumberDic, HashSet<string> existedCourseCodeSets)
        {
            if (!existedPdAreaThemeToMaxCodeNumberDic.ContainsKey(pDAreaThemeCode))
            {
                existedPdAreaThemeToMaxCodeNumberDic.Add(pDAreaThemeCode, 0);
            }

            var nextPdAreaThemeNumber = ++existedPdAreaThemeToMaxCodeNumberDic[pDAreaThemeCode];
            var newCode = $"{pDAreaThemeCode}-{nextPdAreaThemeNumber.ToString(EntitiesConstants.UniqueCodeLength)}";
            while (existedCourseCodeSets.Contains(newCode))
            {
                nextPdAreaThemeNumber = ++existedPdAreaThemeToMaxCodeNumberDic[pDAreaThemeCode];
                newCode = $"{pDAreaThemeCode}-{nextPdAreaThemeNumber.ToString(EntitiesConstants.UniqueCodeLength)}";
            }

            existedCourseCodeSets.Add(newCode);
            return newCode;
        }

        private async Task SendApprovalNotificationToCoursePlanningCoordinator(IEnumerable<CourseEntity> courses, CancellationToken cancellationToken)
        {
            var ownerFullNameDict = await _readUserRepository.GetAll().Where(p => courses.Select(x => x.CreatedBy).Contains(p.Id)).ToDictionaryAsync(
                x => x.Id,
                x => x.FullName(),
                cancellationToken);
            var coursePlanningCoordinatorIds = await _readUserRepository
                .GetAll()
                .Where(x => x.Status == CourseUserStatus.Active)
                .Where(CourseUser.HasRoleExpr(new List<string> { UserRoles.CoursePlanningCoordinator, UserRoles.SystemAdministrator }))
                .Select(x => x.Id)
                .ToListAsync(cancellationToken);
            var coursePlanningCycleDict = await _readCoursePlanningCycleRepository.GetAll()
                .Where(p => courses.Select(x => x.CoursePlanningCycleId.Value).Contains(p.Id))
                .ToDictionaryAsync(x => x.Id, x => x, cancellationToken);
            await _thunderCqrs.SendEvents(
                courses.Select(p => new ApprovedCourseInPlanningCycleNotifyCPCEvent(
                    CurrentUserIdOrDefault,
                    new ApprovedCourseInPlanningCycleNotifyCPCPayload
                    {
                        OwnerName = ownerFullNameDict[p.CreatedBy],
                        PlanningCyclePeriod = coursePlanningCycleDict[p.CoursePlanningCycleId.GetValueOrDefault()].YearCycle.ToString(),
                        ActionUrl = _webAppLinkBuilder.GetCourseDetailLinkInPlanningCycleForCAMModule(
                            CAMTabConfigurationConstant.PlanningCycleTab,
                            CAMTabConfigurationConstant.CoursesOfPlanningCycleTab,
                            CAMTabConfigurationConstant.CoursePendingVerifyOfPlanningCycleTab,
                            CoursePlanningCycleDetailModeConstant.View,
                            p.CoursePlanningCycleId.GetValueOrDefault(),
                            CAMTabConfigurationConstant.CourseInfoTab,
                            CAMTabConfigurationConstant.AllClassRunsTab,
                            CourseDetailModeConstant.ForVerifier,
                            p.Id)
                    },
                    coursePlanningCoordinatorIds)),
                cancellationToken);
        }

        private async Task SendApprovalNotificationToOwner(CourseStatus status, string comment, IEnumerable<CourseEntity> courses, CancellationToken cancellationToken)
        {
            switch (status)
            {
                case CourseStatus.Approved:
                    await _thunderCqrs.SendEvents(
                        courses.Select(p => new ApprovedCourseNotifyOwnerEvent(
                            CurrentUserIdOrDefault,
                            new ApprovedCourseNotifyOwnerPayload
                            {
                                CourseTitle = p.CourseName,
                                Comment = comment,
                                ActionUrl = _webAppLinkBuilder.GetCourseDetailLinkForCAMModule(
                                    CAMTabConfigurationConstant.HasCoursePendingApprovalTab,
                                    CAMTabConfigurationConstant.CourseInfoTab,
                                    CAMTabConfigurationConstant.AllClassRunsTab,
                                    CourseDetailModeConstant.ForApprover,
                                    p.Id)
                            },
                            new List<Guid> { p.CreatedBy })),
                        cancellationToken);

                    break;
                case CourseStatus.Rejected:
                    await _thunderCqrs.SendEvents(
                        courses.Select(p => new RejectedCourseNotifyOwnerEvent(
                            CurrentUserIdOrDefault,
                            new RejectedCourseNotifyOwnerPayload
                            {
                                CourseTitle = p.CourseName,
                                Comment = comment,
                                ActionUrl = _webAppLinkBuilder.GetCourseDetailLinkForCAMModule(
                                    CAMTabConfigurationConstant.HasCoursePendingApprovalTab,
                                    CAMTabConfigurationConstant.CourseInfoTab,
                                    CAMTabConfigurationConstant.AllClassRunsTab,
                                    CourseDetailModeConstant.ForApprover,
                                    p.Id)
                            },
                            new List<Guid> { p.CreatedBy })),
                        cancellationToken);
                    break;
            }
        }

        private async Task SendApprovalCourseMluNotificationToCollaborator(CourseStatus status, IEnumerable<CourseEntity> courses, CancellationToken cancellationToken)
        {
            if (status == CourseStatus.Approved)
            {
                var events = courses
                    .Where(course => course.IsMicroLearning() && course.CollaborativeContentCreatorIds.Any())
                    .Select(course => new ApprovedCourseMLUNotifyCollaboratorEvent(
                        CurrentUserIdOrDefault,
                        new ApprovedCourseMLUNotifyCollaboratorPayload()
                        {
                            ActionUrl = _webAppLinkBuilder.GetCourseDetailLinkForLMMModule(
                                CAMTabConfigurationConstant.CoursesTab,
                                LMMTabConfigurationConstant.CourseInfoTab,
                                LMMTabConfigurationConstant.AllClassRunsTab,
                                CourseDetailModeConstant.View,
                                course.Id),
                            CourseTitle = course.CourseName,
                            CoursePDArea = course.PDAreaThemeId
                        },
                        course.CollaborativeContentCreatorIds.ToList()))
                    .ToList();

                if (events.Any())
                {
                    await _thunderCqrs.SendEvents(events, cancellationToken);
                }
            }
        }

        private async Task SendPendingApprovalNotification(List<CourseEntity> courses, CancellationToken cancellationToken)
        {
            var events = new List<BaseThunderEvent>();
            foreach (var course in courses)
            {
                var approvingOfficerId = course.PrimaryApprovingOfficerId.HasValue ? new List<Guid> { course.PrimaryApprovingOfficerId.Value } : new List<Guid>();

                if (approvingOfficerId.Count > 0)
                {
                    if (course.ApprovalDate != null)
                    {
                        events.Add(new UpdatedCourseNotifyApproverEvent(
                            CurrentUserIdOrDefault,
                            new UpdatedCourseNotifyApproverPayload
                            {
                                ActionUrl = _webAppLinkBuilder.GetCourseDetailLinkForCAMModule(
                                    CAMTabConfigurationConstant.HasCoursePendingApprovalTab,
                                    CAMTabConfigurationConstant.CourseInfoTab,
                                    CAMTabConfigurationConstant.AllClassRunsTab,
                                    CourseDetailModeConstant.ForApprover,
                                    course.Id)
                            },
                            approvingOfficerId));
                    }
                    else
                    {
                        events.Add(new CreatedCourseNotifyApproverEvent(
                            CurrentUserIdOrDefault,
                            new CreatedCourseNotifyApproverPayload
                            {
                                ActionUrl = _webAppLinkBuilder.GetCourseDetailLinkForCAMModule(
                                    CAMTabConfigurationConstant.HasCoursePendingApprovalTab,
                                    CAMTabConfigurationConstant.CourseInfoTab,
                                    CAMTabConfigurationConstant.AllClassRunsTab,
                                    CourseDetailModeConstant.ForApprover,
                                    course.Id)
                            },
                            approvingOfficerId));
                    }
                }
            }

            await _thunderCqrs.SendEvents(events, cancellationToken);
        }
    }
}
