using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LearnerApp.Common;
using LearnerApp.Controls.LearnerObservableCollection;
using LearnerApp.Converters;
using LearnerApp.Helper;
using LearnerApp.Models;
using LearnerApp.Models.Course;
using LearnerApp.Models.Learner;
using LearnerApp.Resources.Texts;
using LearnerApp.Services.Backend;
using LearnerApp.Services.Navigation;
using LearnerApp.ViewModels.Base;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace LearnerApp.ViewModels
{
    public class LearningOpportunityClassRunCardViewModel : BaseViewModel
    {
        public LearnerObservableCollection<ClassRun> ClassRunCollection = new LearnerObservableCollection<ClassRun>();

        private const string Applied = "✓ Applied";
        private const string Apply = "Apply";
        private const string ApplyAgain = "Apply Again";
        private const string Nominated = "✓ Nominated";
        private const string Changed = "Changed class run";

        private readonly IPDPMBackendService _pdpmBackendService;
        private readonly ICourseBackendService _courseBackendService;
        private readonly ILearnerBackendService _learnerBackendService;
        private readonly IOrganizationBackendService _organizationBackendService;
        private readonly IWebinarBackendService _webinarBackendService;
        private readonly IIdmBackendService _idmBackendService;

        private readonly List<string> _removeStatusList = new List<string>
        {
            RegistrationStatus.Rejected.ToString(),
            RegistrationStatus.RejectedByCA.ToString(),
            RegistrationStatus.OfferRejected.ToString(),
            RegistrationStatus.WaitlistRejected.ToString()
        };

        private readonly List<string> _changeClassList = new List<string>
        {
            MyCourseDisplayStatus.Approved.ToString(),
            MyCourseDisplayStatus.ConfirmedByCA.ToString(),
            MyCourseDisplayStatus.OfferConfirmed.ToString(),
            MyCourseDisplayStatus.ClassRunChangeConfirmedByCA.ToString(),
            MyCourseDisplayStatus.WaitlistPendingApprovalByLearner.ToString(),
            MyCourseDisplayStatus.WaitlistConfirmed.ToString(),
            MyCourseDisplayStatus.OfferPendingApprovalByLearner.ToString(),
            MyCourseDisplayStatus.WithdrawalRejectedByCA.ToString(),
            RegistrationStatus.WaitlistUnsuccessful.ToString(),
            MyCourseDisplayStatus.ConfirmedBeforeStartDate.ToString(),
            MyCourseDisplayStatus.WithdrawalRejected.ToString(),
            MyCourseDisplayStatus.ClassRunChangeRejected.ToString(),
            MyCourseDisplayStatus.ClassRunChangeRejectedByCA.ToString()
        };

        // Status
        private readonly List<string> _courseStartedList = new List<string>
        {
            StatusLearning.InProgress.ToString(),
            StatusLearning.Completed.ToString(),
            StatusLearning.Passed.ToString(),
            StatusLearning.Failed.ToString()
        };

        // Withdraw
        private readonly List<string> _withdrawStatusList = new List<string>
        {
            WithdrawalStatus.PendingConfirmation.ToString(),
            WithdrawalStatus.Approved.ToString()
        };

        // Up-coming
        private readonly List<string> _upcomingList = new List<string>
        {
            MyCourseDisplayStatus.ConfirmedByCA.ToString(),
            MyCourseDisplayStatus.OfferConfirmed.ToString(),
            MyCourseDisplayStatus.AddedByCAConfirmedByCA.ToString(),
            MyCourseDisplayStatus.AddedByCAOfferConfirmed.ToString(),
            MyCourseDisplayStatus.AddedByCARescheduled.ToString(),
            MyCourseDisplayStatus.Rescheduled.ToString()
        };

        // Withdraw Reject
        private readonly List<string> _withdrawRejectList = new List<string>
        {
            WithdrawalStatus.Rejected.ToString(),
            WithdrawalStatus.RejectedByCA.ToString(),
            WithdrawalStatus.WithdrawalRejected.ToString()
        };

        private int _totalCount;

        private List<AttendanceTracking> _attendanceTrackings;

        public LearningOpportunityClassRunCardViewModel()
        {
            _pdpmBackendService = CreateRestClientFor<IPDPMBackendService>(GlobalSettings.BackendServicePDPM);
            _courseBackendService = CreateRestClientFor<ICourseBackendService>(GlobalSettings.BackendServiceCourse);
            _learnerBackendService = CreateRestClientFor<ILearnerBackendService>(GlobalSettings.BackendServiceLearner);
            _organizationBackendService = CreateRestClientFor<IOrganizationBackendService>(GlobalSettings.BackendServiceOrganization);
            _webinarBackendService = CreateRestClientFor<IWebinarBackendService>(GlobalSettings.BackendServiceWebinar);
            _idmBackendService = CreateRestClientFor<IIdmBackendService>(GlobalSettings.BackendServiceIdm);
        }

        public async Task<ObservableCollection<ClassRun>> GetClassRunCollection(LearningOpportunityClassRunCardTransfer classRunCardTransfer)
        {
            var classRunRequest = new ClassRunRequest
            {
                CourseId = classRunCardTransfer.CourseId
            };

            var courseClassRunsResponse = await ExecuteBackendService(() => _courseBackendService.GetClassRuns(classRunRequest));

            if (courseClassRunsResponse.HasEmptyResult())
            {
                return ClassRunCollection;
            }

            using (DialogService.DisplayLoadingIndicator())
            {
                var classRunInCourseAdmins = courseClassRunsResponse.Payload.Items;

                classRunInCourseAdmins.Reverse();

                _totalCount = courseClassRunsResponse.Payload.TotalCount;

                if (classRunInCourseAdmins.IsNullOrEmpty())
                {
                    return ClassRunCollection;
                }

                classRunInCourseAdmins = FilterClassRunList(classRunInCourseAdmins, classRunCardTransfer, classRunCardTransfer.MyClassRuns);

                var classRunIds = classRunInCourseAdmins.Select(run => run.Id).ToArray();

                // Get participant
                classRunInCourseAdmins = await GetParticipantClassRun(classRunIds, classRunInCourseAdmins);

                // Get session
                var sessionResponse = await ExecuteBackendService(() => _courseBackendService.GetSessionsByClassRunIds(classRunIds));

                if (!sessionResponse.HasEmptyResult())
                {
                    foreach (var run in classRunInCourseAdmins)
                    {
                        run.Sessions = sessionResponse.Payload.Where(ses => ses.ClassRunId == run.Id).OrderBy(ses => ses.StartDateTime).ToList();
                        run.IsVisibleSessions = run.Sessions.IsNullOrEmpty();
                    }
                }
                else
                {
                    foreach (var run in classRunInCourseAdmins)
                    {
                        run.IsVisibleSessions = true;
                    }
                }

                await GetUpComingSession(classRunCardTransfer.MyCourseStatus, classRunCardTransfer.MyClassRun, classRunInCourseAdmins);

                bool classRunInProgress = true;

                var isExistedStatusOrWithdraw = CheckExistedStatusOrWithdrawInCourse(classRunCardTransfer.MyCourseStatus, classRunCardTransfer.MyClassRun);

                if (isExistedStatusOrWithdraw)
                {
                    classRunInProgress = classRunCardTransfer.MyCourseStatus.MyRegistrationStatus switch
                    {
                        nameof(RegistrationStatus.Rejected) => false,
                        nameof(RegistrationStatus.RejectedByCA) => false,
                        nameof(RegistrationStatus.WaitlistRejected) => false,
                        nameof(RegistrationStatus.OfferRejected) => false,
                        _ => true,
                    };
                }

                if (classRunInProgress)
                {
                    var disableApplyAll = !isExistedStatusOrWithdraw || IsDisableAllApplyButton(classRunCardTransfer.MyCourseStatus);

                    if (disableApplyAll && classRunCardTransfer.MyClassRuns.Exists(IsClassRunInprogress))
                    {
                        foreach (var cr in classRunInCourseAdmins)
                        {
                            cr.IsVisibleApplyButton = cr.Status != ClassRunStatus.Cancelled;
                            cr.CanApply = false;
                        }
                    }

                    classRunInCourseAdmins = SetTextForAllApplyButton(classRunCardTransfer, classRunInCourseAdmins);

                    // Show withdraw again
                    if (_withdrawRejectList.Contains(classRunCardTransfer.MyClassRun.WithdrawalStatus))
                    {
                        var cr = classRunInCourseAdmins.FirstOrDefault(c => c.Id == classRunCardTransfer.MyClassRun.ClassRunId);

                        if (cr != null)
                        {
                            var isChangeClassProgress = (classRunCardTransfer.MyClassRun.ClassRunChangeStatus == ClassRunChangeStatus.PendingConfirmation
                                                           || classRunCardTransfer.MyClassRun.ClassRunChangeStatus == ClassRunChangeStatus.Approved)
                                                          && cr.Id != classRunCardTransfer.MyClassRun.ClassRunChangeId;
                            if (isChangeClassProgress)
                            {
                                cr.IsVisibleWithdrawButton = false;
                                cr.IsVisibleApplyButton = true;
                                cr.IsVisibleChangeClassRun = false;
                            }
                            else
                            {
                                cr.IsVisibleWithdrawButton = true;
                                cr.IsVisibleApplyButton = false;
                                cr.IsVisibleChangeClassRun = false;
                                cr.WithdrawText = "Withdraw Again";
                            }
                        }
                    }

                    // Show withdrawal reason
                    foreach (var cr in classRunInCourseAdmins)
                    {
                        var mcr = classRunCardTransfer.MyClassRuns.FirstOrDefault(p => p.ClassRunId == cr.Id);

                        if (!string.IsNullOrEmpty(mcr?.WithdrawalStatus))
                        {
                            cr.IsVisibleWithdrawalMessage = true;
                        }

                        if (string.IsNullOrEmpty(mcr?.Status) && classRunCardTransfer.WithdrawnMyClassRuns?.FirstOrDefault(p => p.ClassRunId == cr.Id) != null)
                        {
                            cr.IsVisibleWithdrawalMessage = true;
                        }
                    }

                    // In case my class run finish
                    if (classRunCardTransfer.MyClassRuns.Exists(IsClassRunFinish) && !classRunCardTransfer.IsReachMaximumComplete)
                    {
                        classRunInCourseAdmins = SetTextInClassRunFinish(classRunCardTransfer, classRunInCourseAdmins);
                    }

                    // In case in-progress
                    if (IsCourseInProgress(classRunCardTransfer.MyClassRun))
                    {
                        foreach (var cr in classRunInCourseAdmins)
                        {
                            cr.IsVisibleChangeClassRun = cr.IsVisibleWithdrawButton = false;
                            cr.IsVisibleApplyButton = true;
                            cr.CanApply = false;

                            if (cr.Id != classRunCardTransfer.MyClassRun.ClassRunId)
                            {
                                continue;
                            }

                            if (cr.ApplyText == StatusLearning.Incomplete.ToString() ||
                                cr.ApplyText == StatusLearning.Completed.ToString())
                            {
                                continue;
                            }

                            if (classRunCardTransfer.MyClassRun.LearningStatus == StatusLearning.Passed)
                            {
                                if (classRunCardTransfer.MyClassRun.PostCourseEvaluationFormCompleted != null &&
                                    classRunCardTransfer.MyClassRun.PostCourseEvaluationFormCompleted.Value)
                                {
                                    cr.ApplyText = StatusLearning.Completed.ToString();
                                }
                            }
                            else
                            {
                                cr.ApplyText =
                                    classRunCardTransfer.MyClassRun.RegistrationType == RegistrationType.Nominated ||
                                    classRunCardTransfer.MyClassRun.RegistrationType == RegistrationType.AddedByCA
                                        ? Nominated
                                        : Applied;
                            }
                        }
                    }
                }
                else
                {
                    foreach (var cr in classRunInCourseAdmins)
                    {
                        cr.CanApply = true;
                        cr.ApplyText = IsClassRunApplyAgain(classRunCardTransfer.WithdrawnMyClassRuns, classRunCardTransfer.RejectedMyClassRuns, cr.Id) ? ApplyAgain : Apply;
                        cr.IsVisibleApplyButton = !cr.Status.Equals(ClassRunStatus.Cancelled);
                        cr.IsVisibleWithdrawButton = false;

                        // Set status class run: published/unpublished/rescheduled/cancelled
                        cr.Status = SetStatusClassRun(cr);

                        // Registration method: private and none
                        cr.IsVisibleApplyButton =
                            classRunCardTransfer.RegistrationMethod != RegistrationMethod.Private &&
                            classRunCardTransfer.RegistrationMethod != RegistrationMethod.None;
                    }

                    // In case my class run finish
                    if (classRunCardTransfer.MyClassRuns.Exists(IsClassRunFinish) && !classRunCardTransfer.IsReachMaximumComplete)
                    {
                        classRunInCourseAdmins = SetTextInClassRunFinish(classRunCardTransfer, classRunInCourseAdmins);
                    }
                }

                // Update the list of class run
                classRunInCourseAdmins = await GetFacilitator(classRunInCourseAdmins);
                ClassRunCollection.AddRange(classRunInCourseAdmins);

                // Visible show more class run
                ShowMoreButton();
                return ClassRunCollection;
            }
        }

        public Tuple<LearnerObservableCollection<ClassRun>, ClassRun> ShowClassRunDetail(string classRunId)
        {
            var classRun = ClassRunCollection.FirstOrDefault(run => run.Id.Equals(classRunId));

            if (classRun == null)
            {
                return null;
            }

            classRun.IsExpand = !classRun.IsExpand;

            return Tuple.Create(ClassRunCollection, classRun);
        }

        public Tuple<LearnerObservableCollection<ClassRun>, ClassRun> ShowClassRunInformationDetail(string classRunId)
        {
            var classRun = ClassRunCollection.FirstOrDefault(run => run.Id.Equals(classRunId));

            if (classRun == null)
            {
                return null;
            }

            classRun.IsClassRunExpand = !classRun.IsClassRunExpand;

            return Tuple.Create(ClassRunCollection, classRun);
        }

        public Tuple<LearnerObservableCollection<ClassRun>, ClassRun> ShowSessionDetail(string sessionId)
        {
            foreach (var run in ClassRunCollection)
            {
                var session = run.Sessions.FirstOrDefault(ses => ses.Id.Equals(sessionId));

                if (session == null)
                {
                    continue;
                }

                session.IsExpand = !session.IsExpand;

                return Tuple.Create(ClassRunCollection, run);
            }

            return null;
        }

        public async Task<ObservableCollection<ClassRun>> ApplyClassRun(LearningOpportunityClassRunCardTransfer classRunCardTransfer, string classRunId, EventHandler onSetLearningStatus)
        {
            // Check course violate
            var courseViolationResponse = await ExecuteBackendService(() => _courseBackendService.GetCourseViolation(classRunCardTransfer.CourseId, classRunId));

            if (courseViolationResponse.IsError)
            {
                return ClassRunCollection;
            }

            if (!courseViolationResponse.HasEmptyResult())
            {
                var preRequisite = courseViolationResponse.Payload?.ViolationDetail.PreRequisiteCourses?.FirstOrDefault(per => per.ViolationType != "NotViolate");

                if (preRequisite != null)
                {
                    await this.DialogService.ShowAlertAsync(TextsResource.COURSE_UNABLE_APPLY, "OK");
                    return ClassRunCollection;
                }
            }

            var classRun = ClassRunCollection.FirstOrDefault(run => run.Id.Equals(classRunId));

            if (classRun == null)
            {
                return null;
            }

            await DialogService.ConfirmAsync(TextsResource.COURSE_APPLY, "No", "Yes", async (confirm) =>
            {
                if (!confirm)
                {
                    return;
                }

                var myCourseSummaryInfo = await ExecuteBackendService(() => _learnerBackendService.GetMyCourseSummary(classRunCardTransfer.CourseId));

                if (myCourseSummaryInfo.HasEmptyResult())
                {
                    return;
                }

                if (myCourseSummaryInfo.Payload.MyCourseInfo == null)
                {
                    await PDPlanMethod.AddCourseToPDPlan(classRunCardTransfer.CourseId, classRunCardTransfer.UserId, (pdo) => ExecuteBackendService(() => _pdpmBackendService.AddCourseToPDPlan(pdo)));
                    await DialogService.ShowAlertAsync(TextsResource.COURSE_ADDED_PDPLAN, "OK", isVisibleIcon: false);
                }

                await ApplyClassRun(classRunCardTransfer.CourseId, classRunCardTransfer.ApprovingOfficer, classRunCardTransfer.AlternativeApprovingOfficer, classRun, onSetLearningStatus);
            });

            return ClassRunCollection;
        }

        public async Task OpenCheckInPopup()
        {
            await NavigationService.NavigateToAsync<CheckInViewModel>();
        }

        public async Task OpenCannotParticipatePopup(string sessionId)
        {
            var parameters = new NavigationParameters();
            parameters.SetParameter("sessionId", sessionId);
            await NavigationService.NavigateToAsync<CannotParticipateViewModel>(parameters);
        }

        public async Task OpenAbsenceMessagePopup(string sessionId)
        {
            if (_attendanceTrackings.IsNullOrEmpty())
            {
                return;
            }

            var attendanceTracking = _attendanceTrackings.FirstOrDefault(att => att.SessionId == sessionId);

            if (attendanceTracking == null)
            {
                return;
            }

            string attachment = attendanceTracking.Attachment.IsNullOrEmpty() ? string.Empty : attendanceTracking.Attachment[0];

            await DialogService.AbsenceMessageAsync("Reason for Absence", attendanceTracking.ReasonForAbsence, attachment);
        }

        public async Task WithdrawClassRun(MyClassRun myClassRun, string classRunId, RegistrationMethod registrationMethod, EventHandler onSetLearningStatus)
        {
            var classRun = ClassRunCollection.FirstOrDefault(run => run.Id.Equals(classRunId));

            if (classRun == null)
            {
                return;
            }

            var localNow = DateTime.Now.ToUniversalTime().Date;
            var classRunStartDate = classRun.GetStartDateTime().Date;

            if (classRunStartDate.CompareTo(localNow) > 0 && (classRunStartDate - localNow).TotalDays < 10)
            {
                await this.DialogService.ShowAlertAsync(TextsResource.WARNING_WITHDRAWAL_LESS_THAN_10_DAYS, "OK", async (confirmed) =>
                {
                    if (!confirmed)
                    {
                        return;
                    }

                    await WithdrawClassRunWithReason(myClassRun, classRun, registrationMethod, onSetLearningStatus);
                });
            }
            else
            {
                await WithdrawClassRunWithReason(myClassRun, classRun, registrationMethod, onSetLearningStatus);
            }
        }

        public async Task ChangeClassRun(MyClassRun myClassRun, string classRunChangeId, RegistrationMethod registrationMethod, EventHandler onSetLearningStatus)
        {
            var classRun = ClassRunCollection.FirstOrDefault(run => run.Id.Equals(classRunChangeId));

            if (classRun == null)
            {
                return;
            }

            var classRunInProgress = ClassRunCollection.FirstOrDefault(run => run.Id.Equals(myClassRun.ClassRunId));

            if (classRunInProgress == null)
            {
                return;
            }

            var localNow = DateTime.Now.ToUniversalTime().Date;
            var classRunStartDate = classRunInProgress.GetStartDateTime().Date;

            var warningMessage = string.Empty;
            var isClassFullResponse = await ExecuteBackendService(() => _courseBackendService.CheckClassFull(classRunChangeId));
            bool isClassFull = false;

            if (!isClassFullResponse.HasEmptyResult())
            {
                isClassFull = isClassFullResponse.Payload;
            }

            if (classRunStartDate.CompareTo(localNow) > 0 && (classRunStartDate - localNow).TotalDays < 10)
            {
                warningMessage = isClassFull ? $"{TextsResource.WARNING_CHANGE_CLASS_LESS_THAN_10_DAYS}\n\n{TextsResource.WARNING_CLASS_IS_FULL}" : TextsResource.WARNING_CHANGE_CLASS_LESS_THAN_10_DAYS;
            }
            else if (isClassFull)
            {
                warningMessage = TextsResource.WARNING_CLASS_IS_FULL;
            }

            if (!string.IsNullOrEmpty(warningMessage))
            {
                await this.DialogService.ShowAlertAsync(warningMessage, "OK", async (confirmed) =>
                {
                    if (confirmed)
                    {
                        await ChangeClassRunWithReason(myClassRun, classRun, classRunChangeId, registrationMethod, onSetLearningStatus);
                    }
                });
            }
            else
            {
                await ChangeClassRunWithReason(myClassRun, classRun, classRunChangeId, registrationMethod, onSetLearningStatus);
            }
        }

        public async Task JoinWebinar(string sessionId)
        {
            using (DialogService.DisplayLoadingIndicator())
            {
                var joinUrlResponse = await ExecuteBackendService(() => _webinarBackendService.GetWebinarJoinUrl("Course", sessionId));
                if (joinUrlResponse.HasEmptyResult())
                {
                    return;
                }

                var redirectUrl = joinUrlResponse.Payload.JoinUrl;
                if (!string.IsNullOrEmpty(redirectUrl))
                {
                    await Browser.OpenAsync(redirectUrl, BrowserLaunchMode.External);
                }
            }
        }

        public async Task OpenClassCommunity(string classRunId)
        {
            var classRunResponse = await ExecuteBackendService(() => _courseBackendService.GetClassRunById(classRunId));
            if (classRunResponse.IsError || classRunResponse.HasEmptyResult())
            {
                return;
            }

            var classRun = classRunResponse.Payload;
            string redirectUrl = classRun.Status == ClassRunStatus.Unpublished ? $"{GlobalSettings.WebViewUrlSocial}/s/class-{classRunId}" : $"{GlobalSettings.WebViewUrlSocial}/s/{classRun.ClassRunCode}";

            var apiResult = await ExecuteBackendService(() => _idmBackendService.GetLoginWithTokenUrl(redirectUrl));
            if (apiResult.IsError || apiResult.HasEmptyResult())
            {
                return;
            }

            var parameters = new NavigationParameters();
            parameters.SetParameter("IdmResponse", apiResult.Payload);
            parameters.SetParameter("ReturnUrl", redirectUrl);
            await this.NavigationService.NavigateToAsync<CommunityViewModel>(parameters);
        }

        private void ShowMoreButton()
        {
            // Show more icon when reach last item
            if (ClassRunCollection.Count < _totalCount)
            {
                ClassRunCollection.ForEach(run =>
                {
                    if (run.IsLastItem)
                    {
                        run.IsLastItem = false;
                    }
                });

                var lastItem = ClassRunCollection.LastOrDefault();

                if (lastItem != null)
                {
                    lastItem.IsLastItem = true;
                }
            }
            else
            {
                ClassRunCollection.ForEach(run => run.IsLastItem = false);
            }
        }

        private List<ClassRun> FilterClassRunList(List<ClassRun> classRunInCourseAdmins, LearningOpportunityClassRunCardTransfer transferData, List<MyClassRun> myClassRuns)
        {
            var myClassRun = transferData.MyClassRun;
            if (classRunInCourseAdmins.IsNullOrEmpty() || myClassRun == null)
            {
                return new List<ClassRun>();
            }

            // Filter class run by application start date >= now =< application end date
            DateTime nowDate = DateTime.Now.ToUniversalTime().Date;

            string myClassRunId = string.Empty;

            if (myClassRun.Status != null && !_removeStatusList.Contains(myClassRun.Status))
            {
                myClassRunId = myClassRun.ClassRunId;
            }

            int removeCount;

            var myClassRunIds = new List<string>();

            var myClassRunFinishList = myClassRuns.Where(IsClassRunFinish).ToList();

            if (!myClassRunFinishList.IsNullOrEmpty())
            {
                myClassRunIds = myClassRunFinishList.Select(p => p.ClassRunId).ToList();
            }

            if (string.IsNullOrEmpty(myClassRunId))
            {
                removeCount = classRunInCourseAdmins.RemoveAll(classRun => classRun.ApplicationEndDate != null && (nowDate.CompareTo(classRun.GetAplicationStartDateTime().Date) < 0 || nowDate.CompareTo(classRun.GetAplicationEndDateTime().Date) > 0));
            }
            else
            {
                removeCount = classRunInCourseAdmins.RemoveAll(classRun => classRun.ApplicationEndDate != null && !myClassRunIds.Contains(classRun.Id) && (!classRun.Id.Equals(myClassRunId) && (nowDate.CompareTo(classRun.GetAplicationStartDateTime().Date) < 0 || nowDate.CompareTo(classRun.GetAplicationEndDateTime().Date) > 0)));
                var courseCommunityVisibleConverter = (CourseCommunityVisibleConverter)Application.Current.Resources["CourseCommunityVisibleConverter"];
                var classRun = classRunInCourseAdmins.FirstOrDefault(run => run.Id.Equals(myClassRunId));
                if (classRun != null)
                {
                    classRun.IsVisibleCommunity = (bool)courseCommunityVisibleConverter.Convert(transferData.MyCourseStatus, null, null, null);
                }
            }

            classRunInCourseAdmins.ForEach(classRun =>
            {
                if (string.IsNullOrEmpty(myClassRunId) || classRun.Id != myClassRunId)
                {
                    var refClassRun = myClassRuns.FirstOrDefault(cr => (cr.ClassRunId == classRun.Id));
                    if (refClassRun != null)
                    {
                        classRun.IsVisibleCommunity = refClassRun.LearningStatus == StatusLearning.Completed || refClassRun.LearningStatus == StatusLearning.Failed;
                    }
                }
            });

            _totalCount -= removeCount;

            return classRunInCourseAdmins;
        }

        private async Task ApplyClassRun(string courseId, string approvingOfficer, string alternativeApprovingOfficer, ClassRun classRun, EventHandler onSetLearningStatus)
        {
            using (DialogService.DisplayLoadingIndicator())
            {
                var registration = new Registration
                {
                    CourseId = courseId,
                    ClassRunId = classRun.Id
                };

                var registrations = new List<Registration> { registration };

                ClassRunRegistration classRunRegistration = new ClassRunRegistration
                {
                    ApprovingOfficer = approvingOfficer,
                    AlternativeApprovingOfficer = string.IsNullOrEmpty(alternativeApprovingOfficer) ? null : alternativeApprovingOfficer,
                    Registrations = registrations
                };

                var registrationIdResponse = await ExecuteBackendService(() => _courseBackendService.ApplyClassRun(classRunRegistration));

                if (registrationIdResponse.HasEmptyResult())
                {
                    return;
                }

                classRun.ApplyText = Applied;
                classRun.IsVisibleWithdrawalMessage = false;

                foreach (var cr in ClassRunCollection)
                {
                    cr.CanApply = false;
                }

                if (!registrationIdResponse.Payload.IsNullOrEmpty())
                {
                    onSetLearningStatus?.Invoke(registrationIdResponse.Payload[0].Status, null);
                }
            }

            MessagingCenter.Unsubscribe<LearningOpportunityClassRunCardViewModel>(this, "reload-toc");
            MessagingCenter.Send(this, "reload-toc", classRun.Id);
        }

        private List<ClassRun> SetTextForAllApplyButton(LearningOpportunityClassRunCardTransfer classRunCardTransfer, List<ClassRun> classRunInCourseAdmins)
        {
            if (classRunCardTransfer.MyCourseStatus == null || classRunCardTransfer.MyClassRun == null)
            {
                return classRunInCourseAdmins;
            }

            // Reach maximum complete course
            if (classRunCardTransfer.IsReachMaximumComplete)
            {
                foreach (var cr in classRunInCourseAdmins)
                {
                    // Set status class run: published/unpublished/rescheduled/cancelled
                    cr.Status = SetStatusClassRun(cr);

                    var myCr = classRunCardTransfer.MyClassRuns.FirstOrDefault(p => IsClassRunFinish(p) && p.ClassRunId == cr.Id);

                    if (myCr != null)
                    {
                        cr.ApplyText = myCr.LearningStatus.ToString();
                        cr.CanApply = false;
                        cr.IsVisibleApplyButton = true;
                    }
                    else
                    {
                        cr.IsVisibleApplyButton = false;
                    }

                    cr.IsVisibleChangeClassRun = false;
                    cr.IsVisibleWithdrawButton = false;
                }

                return classRunInCourseAdmins;
            }

            // Set status class run: published/unpublished/rescheduled/cancelled
            foreach (var cr in classRunInCourseAdmins)
            {
                cr.IsVisibleApplyButton = !cr.Status.Equals(ClassRunStatus.Cancelled);
                cr.Status = SetStatusClassRun(cr);
            }

            // In case apply again
            classRunInCourseAdmins.ForEach(cr =>
            {
                cr.ApplyText = IsClassRunApplyAgain(classRunCardTransfer.WithdrawnMyClassRuns, classRunCardTransfer.RejectedMyClassRuns, cr.Id) ? ApplyAgain : cr.ApplyText;
            });

            bool canApply = classRunCardTransfer.MyClassRun.Status switch
            {
                nameof(RegistrationStatus.Rejected) => true,
                nameof(RegistrationStatus.RejectedByCA) => true,
                nameof(RegistrationStatus.WaitlistRejected) => true,
                nameof(RegistrationStatus.OfferRejected) => true,
                _ => false,
            };

            if (canApply)
            {
                return classRunInCourseAdmins;
            }

            var classRun = classRunInCourseAdmins.FirstOrDefault(run => run.Id == classRunCardTransfer.MyClassRun.ClassRunId);

            if (classRun == null)
            {
                foreach (var run in classRunInCourseAdmins)
                {
                    // Registration method: private and none
                    run.IsVisibleApplyButton = run.IsVisibleApplyButton &&
                                               classRunCardTransfer.RegistrationMethod != RegistrationMethod.Private &&
                                               classRunCardTransfer.RegistrationMethod != RegistrationMethod.None;
                }

                return classRunInCourseAdmins;
            }

            classRun.ApplyText = classRunCardTransfer.MyClassRun.RegistrationType == RegistrationType.Nominated || classRunCardTransfer.MyClassRun.RegistrationType == RegistrationType.AddedByCA ? Nominated : Applied;

            foreach (var run in classRunInCourseAdmins)
            {
                if (classRunCardTransfer.MyClassRun.ClassRunChangeStatus == ClassRunChangeStatus.PendingConfirmation
                    || classRunCardTransfer.MyClassRun.ClassRunChangeStatus == ClassRunChangeStatus.Approved
                    || classRunCardTransfer.MyClassRun.ClassRunChangeStatus == ClassRunChangeStatus.ConfirmedByCA)
                {
                    if (run.Id == classRunCardTransfer.MyClassRun.ClassRunChangeId)
                    {
                        run.ChangeClassRunText = Changed;
                        run.CanChangeClassRun = false;
                        run.IsVisibleChangeClassRun = true;
                        run.IsVisibleApplyButton = false;
                        run.IsVisibleWithdrawButton = false;

                        continue;
                    }

                    run.ApplyText = run.Id == classRunCardTransfer.MyClassRun.ClassRunId ? Applied : Apply;

                    run.CanApply = false;
                    run.IsVisibleApplyButton = true;
                    run.IsVisibleChangeClassRun = false;
                    run.IsVisibleWithdrawButton = false;
                }
                else
                {
                    var localNow = DateTime.Now.ToUniversalTime();

                    if (run.Id == classRunCardTransfer.MyClassRun.ClassRunId && classRunCardTransfer.MyCourseStatus.MyRegistrationStatus != RegistrationStatus.ConfirmedBeforeStartDate.ToString())
                    {
                        classRun = SetEnableWithdrawn(classRun, classRunCardTransfer, localNow);

                        // Set can apply by Status
                        run.CanApply = SetCanApplyByCourseStatus(run.CanApply, classRunCardTransfer);

                        continue;
                    }

                    // Set change class request
                    if (_changeClassList.Contains(classRunCardTransfer.MyCourseStatus.MyCourseDisplayStatus)
                        && classRunCardTransfer.MyClassRun.RegistrationType != RegistrationType.Nominated)
                    {
                        run.IsVisibleApplyButton = false;
                        run.IsVisibleChangeClassRun = true;
                    }

                    // We will visible button change class run when class run rejected
                    // Class run start date <= now
                    if (classRunCardTransfer.MyClassRun.RegistrationType == RegistrationType.AddedByCA || (run.GetStartDateTime().Date.CompareTo(localNow.Date) == 0))
                    {
                        run.IsVisibleApplyButton = true;
                        run.IsVisibleChangeClassRun = false;
                    }

                    if (run.IsVisibleChangeClassRun && classRun.GetStartDateTime().Date.CompareTo(localNow.Date) <= 0 && !IsClassRunFinish(classRunCardTransfer.MyClassRun))
                    {
                        run.IsVisibleApplyButton = true;
                        run.CanApply = false;
                        run.IsVisibleChangeClassRun = false;
                    }

                    classRun = SetEnableWithdrawn(classRun, classRunCardTransfer, localNow);

                    // Set can apply by Status
                    run.CanApply = SetCanApplyByCourseStatus(run.CanApply, classRunCardTransfer);

                    // Registration method: private and none
                    run.IsVisibleApplyButton = run.IsVisibleApplyButton &&
                        classRunCardTransfer.RegistrationMethod != RegistrationMethod.Private &&
                        classRunCardTransfer.RegistrationMethod != RegistrationMethod.None;
                }
            }

            return classRunInCourseAdmins;
        }

        private bool SetCanApplyByCourseStatus(bool canApply, LearningOpportunityClassRunCardTransfer classRunCardTransfer)
        {
            if (!string.IsNullOrEmpty(classRunCardTransfer.MyCourseStatus.Status) && classRunCardTransfer.MyCourseStatus.Status == StatusLearning.InProgress.ToString())
            {
                canApply = false;
            }

            if (!string.IsNullOrEmpty(classRunCardTransfer.MyCourseStatus.Status) && _courseStartedList.Contains(classRunCardTransfer.MyCourseStatus.Status) && classRunCardTransfer.MyClassRun.PostCourseEvaluationFormCompleted != null && !classRunCardTransfer.MyClassRun.PostCourseEvaluationFormCompleted.Value)
            {
                canApply = false;
            }

            if (!string.IsNullOrEmpty(classRunCardTransfer.MyCourseStatus.Status) && _courseStartedList.Contains(classRunCardTransfer.MyCourseStatus.Status) && classRunCardTransfer.MyClassRun.PostCourseEvaluationFormCompleted == null)
            {
                canApply = false;
            }

            return canApply;
        }

        private ClassRun SetEnableWithdrawn(ClassRun classRun, LearningOpportunityClassRunCardTransfer classRunCardTransfer, DateTime localNow)
        {
            // Set enable withdraw
            if (classRun.Status == ClassRunStatus.Cancelled
                || !CheckExistedStatusOrWithdrawInCourse(classRunCardTransfer.MyCourseStatus, classRunCardTransfer.MyClassRun)
                || !string.IsNullOrEmpty(classRunCardTransfer.MyClassRun.WithdrawalStatus)
                || classRun.GetStartDateTime().Date.CompareTo(localNow.Date) <= 0)
            {
                classRun.IsVisibleWithdrawButton = false;
                return classRun;
            }

            if (string.IsNullOrEmpty(classRunCardTransfer.MyCourseStatus.MyRegistrationStatus))
            {
                return classRun;
            }

            classRun.IsVisibleWithdrawButton = classRunCardTransfer.MyCourseStatus.MyRegistrationStatus switch
            {
                nameof(RegistrationStatus.Approved) => true,
                nameof(RegistrationStatus.WaitlistPendingApprovalByLearner) => true,
                nameof(RegistrationStatus.WaitlistConfirmed) => true,
                nameof(RegistrationStatus.OfferPendingApprovalByLearner) => true,
                nameof(RegistrationStatus.WaitlistUnsuccessful) => true,
                nameof(RegistrationStatus.ConfirmedBeforeStartDate) => true,
                _ => false,
            };

            // Disable apply button
            if (!classRun.IsVisibleWithdrawButton)
            {
                return classRun;
            }

            classRun.IsVisibleApplyButton = false;
            classRun.IsVisibleChangeClassRun = false;

            return classRun;
        }

        private List<ClassRun> SetTextInClassRunFinish(LearningOpportunityClassRunCardTransfer classRunCardTransfer, List<ClassRun> classRunInCourseAdmins)
        {
            if (classRunCardTransfer.MyClassRuns.IsNullOrEmpty() || classRunInCourseAdmins.IsNullOrEmpty())
            {
                return classRunInCourseAdmins;
            }

            var localNow = DateTime.Now.ToUniversalTime();

            foreach (var cr in classRunInCourseAdmins)
            {
                var mcr = classRunCardTransfer.MyClassRuns.FirstOrDefault(p => p.ClassRunId == cr.Id);

                if (mcr == null)
                {
                    cr.ApplyText = cr.ApplyText == ApplyAgain ? ApplyAgain : Apply;
                    cr.CanApply = SetCanApplyByCourseStatus(cr.CanApply, classRunCardTransfer)
                                  && cr.GetStartDateTime().Date.CompareTo(localNow.Date) >= 0
                                  && (classRunCardTransfer.MyCourseStatus.MyRegistrationStatus != MyCourseDisplayStatus.ConfirmedBeforeStartDate.ToString()
                                    || classRunCardTransfer.MyCourseStatus.Status == StatusLearning.Completed.ToString()
                                    || classRunCardTransfer.MyCourseStatus.Status == StatusLearning.Failed.ToString());
                    cr.IsVisibleApplyButton = cr.CanApply;
                    cr.IsVisibleWithdrawButton = false;
                    cr.IsVisibleChangeClassRun = !cr.IsVisibleApplyButton;

                    continue;
                }

                if (!IsClassRunFinish(mcr))
                {
                    if (classRunCardTransfer.MyCourseStatus.MyRegistrationStatus == MyCourseDisplayStatus.ConfirmedBeforeStartDate.ToString())
                    {
                        cr.CanApply = false;
                        cr.IsVisibleWithdrawButton = cr.IsVisibleWithdrawButton;
                        cr.IsVisibleApplyButton = !cr.IsVisibleWithdrawButton;
                        cr.IsVisibleChangeClassRun = false;
                    }

                    continue;
                }

                cr.ApplyText = mcr.LearningStatus.ToString();
                cr.CanApply = false;
                cr.IsVisibleApplyButton = true;
                cr.IsVisibleChangeClassRun = false;
                cr.IsVisibleWithdrawButton = false;
            }

            return classRunInCourseAdmins;
        }

        private async Task<List<ClassRun>> GetFacilitator(List<ClassRun> classRunCollection)
        {
            if (classRunCollection.IsNullOrEmpty())
            {
                return classRunCollection;
            }

            foreach (var classRun in classRunCollection)
            {
                var userCxIds = new
                {
                    UserCxIds = classRun.FacilitatorIds
                };

                var userResponse = await ExecuteBackendService(() => _organizationBackendService.GetUserInfomation(userCxIds));

                if (userResponse.HasEmptyResult())
                {
                    return classRunCollection;
                }

                var facilitators = userResponse.Payload.Select(user => user.FullName).ToList();
                classRun.Facilitators = StringExtension.GetInformationFromList(facilitators);
            }

            return classRunCollection;
        }

        private bool IsDisableAllApplyButton(MyCourseStatus myCourseStatus)
        {
            if (string.IsNullOrEmpty(myCourseStatus.MyRegistrationStatus)
                || myCourseStatus.MyCourseDisplayStatus == MyCourseDisplayStatus.Cancelled.ToString()
                || myCourseStatus.MyCourseDisplayStatus == MyCourseDisplayStatus.WithdrawalWithdrawn.ToString())
            {
                return false;
            }

            return myCourseStatus.MyRegistrationStatus switch
            {
                nameof(RegistrationStatus.PendingConfirmation) => true,
                nameof(RegistrationStatus.Approved) => true,
                nameof(RegistrationStatus.ConfirmedByCA) => true,
                nameof(RegistrationStatus.WaitlistPendingApprovalByLearner) => true,
                nameof(RegistrationStatus.WaitlistConfirmed) => true,
                nameof(RegistrationStatus.OfferPendingApprovalByLearner) => true,
                nameof(RegistrationStatus.OfferConfirmed) => true,
                nameof(RegistrationStatus.WaitlistUnsuccessful) => true,
                nameof(RegistrationStatus.Cancelled) => true,
                nameof(RegistrationStatus.ConfirmedBeforeStartDate) => true,
                _ => false,
            };
        }

        private bool CheckExistedStatusOrWithdrawInCourse(MyCourseStatus myCourseStatus, MyClassRun myClassRun)
        {
            if (myCourseStatus == null)
            {
                return false;
            }

            // Status
            if (!string.IsNullOrEmpty(myCourseStatus.Status) && _courseStartedList.Contains(myCourseStatus.Status))
            {
                return false;
            }

            if (!string.IsNullOrEmpty(myCourseStatus.Status) && _courseStartedList.Contains(myCourseStatus.Status) && myClassRun.PostCourseEvaluationFormCompleted != null && !myClassRun.PostCourseEvaluationFormCompleted.Value)
            {
                return false;
            }

            if (!string.IsNullOrEmpty(myCourseStatus.Status) && _courseStartedList.Contains(myCourseStatus.Status) && myClassRun.PostCourseEvaluationFormCompleted == null)
            {
                return false;
            }

            // Withdraw
            return string.IsNullOrEmpty(myCourseStatus.MyWithdrawalStatus) || !_withdrawStatusList.Contains(myCourseStatus.MyWithdrawalStatus);
        }

        private async Task GetUpComingSession(MyCourseStatus myCourseStatus, MyClassRun myClassRun, List<ClassRun> classRunInCourseAdmins)
        {
            // Get session in attendance tracking
            if (_upcomingList.Contains(myCourseStatus.MyCourseDisplayStatus))
            {
                var attendanceTrackingResponse = await ExecuteBackendService(() =>
                    _courseBackendService.GetUserAttendanceTrackingByClassRunId(myClassRun.ClassRunId));

                if (attendanceTrackingResponse.HasEmptyResult())
                {
                    return;
                }

                _attendanceTrackings = attendanceTrackingResponse.Payload;

                var classRunSession =
                    classRunInCourseAdmins.FirstOrDefault(run => run.Id == myClassRun.ClassRunId);

                if (classRunSession == null)
                {
                    return;
                }

                DateTime localNow = DateTime.Now.ToUniversalTime();

                bool isUpComingSession = false;

                foreach (var session in classRunSession.Sessions)
                {
                    var attendanceTracking = _attendanceTrackings.FirstOrDefault(att => att.SessionId == session.Id);

                    UpComingTransferData upComingTransferData = new UpComingTransferData
                    {
                        Session = session,
                        AttendanceTracking = attendanceTracking
                    };

                    if (session.StartDateTime == null)
                    {
                        continue;
                    }

                    var sessionStartTime = session.GetStartDateTime();

                    if (attendanceTracking == null)
                    {
                        // show session status
                        session.Status = $"{sessionStartTime:dd/MM/yyyy}";

                        // show upcoming session
                        if (!isUpComingSession && sessionStartTime.Date.CompareTo(localNow.Date) >= 0)
                        {
                            isUpComingSession = true;
                            MessagingCenter.Unsubscribe<LearningOpportunityClassRunCardViewModel, UpComingTransferData>(
                                this, "upcoming-session");
                            MessagingCenter.Send(this, "upcoming-session", upComingTransferData);
                        }

                        // show checkin button
                        session.CanCheckin = sessionStartTime.Date.CompareTo(localNow.Date) == 0
                            && PermissionHelper.GetPermissionForCheckinDoAssignmentDownloadContentDoPostCourse();
                    }
                    else
                    {
                        // show session status
                        if (attendanceTracking.Status == AttendanceTrackingStatus.Absent)
                        {
                            session.Status = SessionStatus.Incomplete.ToString();

                            // show button unable to participate
                            session.CanAbsent = string.IsNullOrEmpty(attendanceTracking.ReasonForAbsence);

                            // show reasons for absence
                            session.ShowAbsenceMessage = !string.IsNullOrEmpty(attendanceTracking.ReasonForAbsence);
                        }
                        else if (attendanceTracking.Status == AttendanceTrackingStatus.Present || attendanceTracking.IsCodeScanned)
                        {
                            session.Status = SessionStatus.Completed.ToString();
                        }
                        else
                        {
                            session.Status = $"{sessionStartTime:dd/MM/yyyy}";

                            // show checkin button
                            session.CanCheckin = sessionStartTime.Date.CompareTo(localNow.Date) == 0
                                && PermissionHelper.GetPermissionForCheckinDoAssignmentDownloadContentDoPostCourse();
                        }

                        // show upcoming session
                        if (!isUpComingSession && attendanceTracking.Status == null &&
                            !attendanceTracking.IsCodeScanned && sessionStartTime.Date.CompareTo(localNow.Date) >= 0)
                        {
                            isUpComingSession = true;
                            MessagingCenter.Unsubscribe<LearningOpportunityClassRunCardViewModel, UpComingTransferData>(
                                this, "upcoming-session");
                            MessagingCenter.Send(this, "upcoming-session", upComingTransferData);
                        }
                    }
                }

                foreach (var classRun in classRunInCourseAdmins)
                {
                    if (classRun == classRunSession)
                    {
                        continue;
                    }

                    SetStatusSessionInClassRun(classRun);
                }
            }
            else
            {
                foreach (var classRun in classRunInCourseAdmins)
                {
                    SetStatusSessionInClassRun(classRun);
                }
            }
        }

        private void SetStatusSessionInClassRun(ClassRun classRun)
        {
            foreach (var session in classRun.Sessions)
            {
                var sessionStartTime = session.GetStartDateTime();

                // show session status
                session.Status = $"{sessionStartTime:dd/MM/yyyy}";
            }
        }

        private async Task ChangeClassRunWithReason(MyClassRun myClassRun, ClassRun classRun, string classRunChangeId, RegistrationMethod registrationMethod, EventHandler onSetLearningStatus)
        {
            await DialogService.ConfirmMessageAsync(TextsResource.CHANGE_CLASS_REASON, "Close", "Confirm", true, async (confirm, reason) =>
            {
                if (!confirm)
                {
                    return;
                }

                var classRunChangeRequest = new ClassRunChangeRequest
                {
                    RegistrationId = myClassRun.RegistrationId,
                    ClassRunChangeId = classRunChangeId,
                    Reason = reason
                };

                await ExecuteBackendService(() => _courseBackendService.ChangeClassRun(classRunChangeRequest));

                classRun.IsVisibleChangeClassRun = true;
                classRun.CanChangeClassRun = false;
                classRun.ChangeClassRunText = Changed;

                foreach (var cr in ClassRunCollection)
                {
                    if (cr.Id == classRunChangeId)
                    {
                        continue;
                    }

                    cr.ApplyText = cr.Id == myClassRun.ClassRunId ? Applied : Apply;

                    cr.CanApply = false;
                    cr.IsVisibleApplyButton = true;
                    cr.IsVisibleChangeClassRun = false;
                    cr.IsVisibleWithdrawButton = false;
                }

                if (registrationMethod == RegistrationMethod.Public)
                {
                    onSetLearningStatus?.Invoke(ClassRunChangeStatus.Approved, null);
                    return;
                }

                onSetLearningStatus?.Invoke(ClassRunChangeStatus.PendingConfirmation, null);
            });
        }

        private async Task WithdrawClassRunWithReason(MyClassRun myClassRun, ClassRun classRun, RegistrationMethod registrationMethod, EventHandler onSetLearningStatus)
        {
            await DialogService.ConfirmMessageAsync(TextsResource.WITHDRAWAL_REASON, checkValidate: true, onConfirmed: async (confirm, comment) =>
            {
                if (!confirm)
                {
                    return;
                }

                var registrationIds = new List<string> { myClassRun.RegistrationId };

                var withdrawalItem = new Withdrawal
                {
                    Ids = registrationIds,
                    WithdrawalStatus = WithdrawalStatus.PendingConfirmation.ToString(),
                    Comment = comment
                };

                await ExecuteBackendService(() => _courseBackendService.Withdraw(withdrawalItem));

                classRun.IsVisibleWithdrawButton = false;
                classRun.CanApply = false;
                classRun.IsVisibleApplyButton = true;
                classRun.IsVisibleWithdrawalMessage = true;

                foreach (var cr in ClassRunCollection)
                {
                    if (cr == classRun)
                    {
                        continue;
                    }

                    cr.IsVisibleWithdrawButton = false;
                    cr.IsVisibleChangeClassRun = false;
                    cr.IsVisibleApplyButton = registrationMethod != RegistrationMethod.None && registrationMethod != RegistrationMethod.Private;
                    cr.CanApply = false;
                }

                if (registrationMethod == RegistrationMethod.Public)
                {
                    onSetLearningStatus?.Invoke(WithdrawalStatus.Approved, null);
                    return;
                }

                onSetLearningStatus?.Invoke(WithdrawalStatus.PendingConfirmation, null);
            });
        }

        private bool IsClassRunFinish(MyClassRun myClassRun)
        {
            if (myClassRun == null)
            {
                return false;
            }

            return myClassRun.LearningStatus == StatusLearning.Completed || myClassRun.LearningStatus == StatusLearning.Failed;
        }

        private bool IsClassRunInprogress(MyClassRun myClassRun)
        {
            return myClassRun.LearningStatus == StatusLearning.NotStarted || myClassRun.LearningStatus == StatusLearning.InProgress;
        }

        private bool IsCourseInProgress(MyClassRun myClassRun)
        {
            return myClassRun.LearningStatus != null && myClassRun.LearningStatus != StatusLearning.NotStarted && myClassRun.LearningStatus != StatusLearning.Completed && myClassRun.LearningStatus != StatusLearning.Failed;
        }

        private bool IsClassRunApplyAgain(List<MyClassRun> withdrawnMyClassRuns, List<MyClassRun> rejectedMyClassRuns, string classRunId)
        {
            return withdrawnMyClassRuns?.FirstOrDefault(cr => classRunId == cr.ClassRunId) != null || rejectedMyClassRuns?.FirstOrDefault(cr => classRunId == cr.ClassRunId) != null;
        }

        private ClassRunStatus SetStatusClassRun(ClassRun classRun)
        {
            if (classRun.Status == ClassRunStatus.Published &&
                classRun.RescheduleStatus == ClassRunRescheduleStatus.Approved &&
                classRun.RescheduleStartDateTime.HasValue)
            {
                return ClassRunStatus.Rescheduled;
            }

            return classRun.Status;
        }

        private async Task<List<ClassRun>> GetParticipantClassRun(string[] classRunIds, List<ClassRun> classRunInCourseAdmins)
        {
            if (classRunIds.IsNullOrEmpty())
            {
                return classRunInCourseAdmins;
            }

            var participantResponse = await ExecuteBackendService(() => _courseBackendService.GetTotalParticipantInClassRun(new { ClassRunIds = classRunIds }));

            if (participantResponse.HasEmptyResult())
            {
                return classRunInCourseAdmins;
            }

            return classRunInCourseAdmins.Join(
                participantResponse.Payload,
                classRun => classRun.Id,
                participant => participant.ClassRunId,
                (classRun, participant) =>
                    new ClassRun
                    {
                        Id = classRun.Id,
                        IsBusy = classRun.IsBusy,
                        ClassTitle = classRun.ClassTitle,
                        ApplicationEndDate = classRun.ApplicationEndDate,
                        ApplicationStartDate = classRun.ApplicationStartDate,
                        CurrentCapacity = participant.ParticipantTotal,
                        ApplyText = classRun.ApplyText,
                        CanApply = classRun.CanApply,
                        CanChangeClassRun = classRun.CanChangeClassRun,
                        ChangeClassRunText = classRun.ChangeClassRunText,
                        ChangedBy = classRun.ChangedBy,
                        ClassRunCode = classRun.ClassRunCode,
                        CoFacilitatorIds = classRun.CoFacilitatorIds,
                        CourseId = classRun.CourseId,
                        CreatedBy = classRun.CreatedBy,
                        EndDateTime = classRun.EndDateTime,
                        FacilitatorIds = classRun.FacilitatorIds,
                        Facilitators = classRun.Facilitators,
                        IsClassRunExpand = classRun.IsClassRunExpand,
                        IsExpand = classRun.IsExpand,
                        IsLastItem = classRun.IsLastItem,
                        IsVisibleApplyButton = classRun.IsVisibleApplyButton,
                        IsVisibleChangeClassRun = classRun.IsVisibleChangeClassRun,
                        IsVisibleSessions = classRun.IsVisibleSessions,
                        IsVisibleWithdrawButton = classRun.IsVisibleWithdrawButton,
                        IsVisibleWithdrawalMessage = classRun.IsVisibleWithdrawalMessage,
                        MaxClassSize = classRun.MaxClassSize,
                        MinClassSize = classRun.MinClassSize,
                        PlanningEndTime = classRun.PlanningEndTime,
                        PlanningStartTime = classRun.PlanningStartTime,
                        RegistrationEndDate = classRun.RegistrationEndDate,
                        RegistrationStartDate = classRun.RegistrationStartDate,
                        RescheduleEndDateTime = classRun.RescheduleEndDateTime,
                        RescheduleStartDateTime = classRun.RescheduleStartDateTime,
                        RescheduleStatus = classRun.RescheduleStatus,
                        Sessions = classRun.Sessions,
                        StartDateTime = classRun.StartDateTime,
                        Status = classRun.Status,
                        WithdrawText = classRun.WithdrawText,
                        IsVisibleCommunity = classRun.IsVisibleCommunity
                    }).ToList();
        }
    }
}
