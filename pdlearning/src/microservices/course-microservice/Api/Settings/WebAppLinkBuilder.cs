using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using Microservice.Course.Application.Constants;
using Microservice.Course.Common.Helpers;
using Microsoft.Extensions.Options;

#pragma warning disable SA1402 // File may only contain a single type
namespace Microservice.Course.Settings
{
    public class WebAppLinkBuilder
    {
        public static readonly string CAMModule = "cam";
        public static readonly string LearnerModule = "learner";
        public static readonly string LMMModule = "lmm";
        public static readonly string PDPMModule = "pdplanner";

        private readonly OpalClientUrlOption _opalClientUrlOption;

        public WebAppLinkBuilder(IOptions<OpalClientUrlOption> opalClientUrlOption)
        {
            _opalClientUrlOption = opalClientUrlOption.Value;
        }

        // CAM Deeplink
        public string GetCourseDetailLinkForCAMModule(
            string courseManagementPageActiveTab,
            string activeTab,
            string subActiveTab,
            string mode,
            Guid id)
        {
            var routeDataLevel1 = WebAppLinkBuilderPageInput.Create(CAMRoutePathsConstant.CourseManagementPage, new WebAppLinkBuilderPageInputData() { ActiveTab = courseManagementPageActiveTab });
            var routeDataLevel2 = WebAppLinkBuilderPageInput.Create(
                CAMRoutePathsConstant.CourseDetailPage,
                new WebAppLinkBuilderPageInputData()
                {
                    ActiveTab = activeTab,
                    SubActiveTab = subActiveTab,
                    Data = new { mode, id }
                },
                routeDataLevel1);
            return $"{_opalClientUrlOption.OpalClientUrl}{CAMModule}{routeDataLevel2.ToNavigationPath()}";
        }

        public string GetCourseDetailLinkInPlanningCycleForCAMModule(
            string coursePlanningPageActiveTab,
            string coursePlanningCycleDetailPageActiveTab,
            string coursePlanningCycleDetailPageSubActiveTab,
            string coursePlanningCycleDetailPageMode,
            Guid planningCycleId,
            string courseDetailPageActiveTab,
            string courseDetailPageSubActiveTab,
            string courseDetailPageMode,
            Guid courseId)
        {
            var routeDataLevel1 = WebAppLinkBuilderPageInput.Create(CAMRoutePathsConstant.CoursePlanningPage, new WebAppLinkBuilderPageInputData() { ActiveTab = coursePlanningPageActiveTab });

            var routeDataLevel2 = WebAppLinkBuilderPageInput.Create(
               CAMRoutePathsConstant.CoursePlanningCycleDetailPage,
               new WebAppLinkBuilderPageInputData()
               {
                   ActiveTab = coursePlanningCycleDetailPageActiveTab,
                   SubActiveTab = coursePlanningCycleDetailPageSubActiveTab,
                   Data = new { mode = coursePlanningCycleDetailPageMode, id = planningCycleId }
               },
               routeDataLevel1);
            var routeDataLevel3 = WebAppLinkBuilderPageInput.Create(
                CAMRoutePathsConstant.CourseDetailPage,
                new WebAppLinkBuilderPageInputData()
                {
                    ActiveTab = courseDetailPageActiveTab,
                    SubActiveTab = courseDetailPageSubActiveTab,
                    Data = new { mode = courseDetailPageMode, id = courseId }
                },
                routeDataLevel2);

            return $"{_opalClientUrlOption.OpalClientUrl}{CAMModule}{routeDataLevel3.ToNavigationPath()}";
        }

        public string GetClassRunDetailLinkForCAMModule(
            string courseManagementPageActiveTab,
            string courseDetailActiveTab,
            string courseDetailSubActiveTab,
            string courseDetailMode,
            string activeTab,
            string mode,
            Guid courseId,
            Guid id)
        {
            var routeDataLevel1 = WebAppLinkBuilderPageInput.Create(CAMRoutePathsConstant.CourseManagementPage, new WebAppLinkBuilderPageInputData() { ActiveTab = courseManagementPageActiveTab });
            var routeDataLevel2 = WebAppLinkBuilderPageInput.Create(
                CAMRoutePathsConstant.CourseDetailPage,
                new WebAppLinkBuilderPageInputData()
                {
                    ActiveTab = courseDetailActiveTab,
                    SubActiveTab = courseDetailSubActiveTab,
                    Data = new { mode = courseDetailMode, id = courseId, courseCriteriaMode = CourseCriteriaDetailModeConstant.View }
                },
                routeDataLevel1);
            var routeDataLevel3 = WebAppLinkBuilderPageInput.Create(
                CAMRoutePathsConstant.ClassRunDetailPage,
                new WebAppLinkBuilderPageInputData()
                {
                    ActiveTab = activeTab,
                    Data = new { mode, id, courseId }
                },
                routeDataLevel2);

            return $"{_opalClientUrlOption.OpalClientUrl}{CAMModule}{routeDataLevel3.ToNavigationPath()}";
        }

        public string GetCoursePlanningCycleDetailForCAMModule(
            string coursePlanningPageActiveTab,
            string coursePlanningCycleDetailPageActiveTab,
            string coursePlanningCycleDetailPageSubActiveTab,
            string coursePlanningCycleDetailPageMode,
            Guid planningCycleId)
        {
            var routeDataLevel1 = WebAppLinkBuilderPageInput.Create(CAMRoutePathsConstant.CoursePlanningPage, new WebAppLinkBuilderPageInputData() { ActiveTab = coursePlanningPageActiveTab });

            var routeDataLevel2 = WebAppLinkBuilderPageInput.Create(
                CAMRoutePathsConstant.CoursePlanningCycleDetailPage,
                new WebAppLinkBuilderPageInputData()
                {
                    ActiveTab = coursePlanningCycleDetailPageActiveTab,
                    SubActiveTab = coursePlanningCycleDetailPageSubActiveTab,
                    Data = new { mode = coursePlanningCycleDetailPageMode, id = planningCycleId }
                },
                routeDataLevel1);

            return $"{_opalClientUrlOption.OpalClientUrl}{CAMModule}{routeDataLevel2.ToNavigationPath()}";
        }

        public string GetCourseManagementPageCAMModule(string courseManagementPageActiveTab)
        {
            var route = WebAppLinkBuilderPageInput.Create(
                CAMRoutePathsConstant.CourseManagementPage,
                new WebAppLinkBuilderPageInputData()
                {
                    ActiveTab = courseManagementPageActiveTab,
                });

            return $"{_opalClientUrlOption.OpalClientUrl}{CAMModule}{route.ToNavigationPath()}";
        }

        // LMM Deeplink
        public string GetCourseDetailLinkForLMMModule(
            string courseManagementPageActiveTab,
            string activeTab,
            string subActiveTab,
            string mode,
            Guid id)
        {
            var routeDataLevel1 = WebAppLinkBuilderPageInput.Create(LMMRoutePathsConstant.CourseManagementPage, new WebAppLinkBuilderPageInputData() { ActiveTab = courseManagementPageActiveTab });
            var routeDataLevel2 = WebAppLinkBuilderPageInput.Create(
                LMMRoutePathsConstant.CourseDetailPage,
                new WebAppLinkBuilderPageInputData()
                {
                    ActiveTab = activeTab,
                    SubActiveTab = subActiveTab,
                    Data = new { mode, id }
                },
                routeDataLevel1);
            return $"{_opalClientUrlOption.OpalClientUrl}{LMMModule}{routeDataLevel2.ToNavigationPath()}";
        }

        public string GetClassRunDetailLinkForLMMModule(
            string courseManagementPageActiveTab,
            string courseDetailActiveTab,
            string courseDetailSubActiveTab,
            string courseDetailMode,
            string activeTab,
            string mode,
            Guid courseId,
            Guid id)
        {
            var routeDataLevel1 = WebAppLinkBuilderPageInput.Create(LMMRoutePathsConstant.CourseManagementPage, new WebAppLinkBuilderPageInputData() { ActiveTab = courseManagementPageActiveTab });
            var routeDataLevel2 = WebAppLinkBuilderPageInput.Create(
                LMMRoutePathsConstant.CourseDetailPage,
                new WebAppLinkBuilderPageInputData()
                {
                    ActiveTab = courseDetailActiveTab,
                    SubActiveTab = courseDetailSubActiveTab,
                    Data = new { mode = courseDetailMode, id = courseId, courseCriteriaMode = CourseCriteriaDetailModeConstant.View }
                },
                routeDataLevel1);
            var routeDataLevel3 = WebAppLinkBuilderPageInput.Create(
                LMMRoutePathsConstant.ClassRunDetailPage,
                new WebAppLinkBuilderPageInputData()
                {
                    ActiveTab = activeTab,
                    Data = new { mode, id, courseId }
                },
                routeDataLevel2);

            return $"{_opalClientUrlOption.OpalClientUrl}{LMMModule}{routeDataLevel3.ToNavigationPath()}";
        }

        public string GetAssignmentDetailLinkForLMMModule(
            string courseManagementPageActiveTab,
            string courseDetailActiveTab,
            string courseDetailSubActiveTab,
            string courseDetailMode,
            string classRunActiveTab,
            string classRunMode,
            string activeTab,
            string mode,
            Guid courseId,
            Guid classRunId,
            Guid id)
        {
            var routeDataLevel1 = WebAppLinkBuilderPageInput.Create(LMMRoutePathsConstant.CourseManagementPage, new WebAppLinkBuilderPageInputData() { ActiveTab = courseManagementPageActiveTab });
            var routeDataLevel2 = WebAppLinkBuilderPageInput.Create(
                LMMRoutePathsConstant.CourseDetailPage,
                new WebAppLinkBuilderPageInputData()
                {
                    ActiveTab = courseDetailActiveTab,
                    SubActiveTab = courseDetailSubActiveTab,
                    Data = new { mode = courseDetailMode, id = courseId }
                },
                routeDataLevel1);
            var routeDataLevel3 = WebAppLinkBuilderPageInput.Create(
                LMMRoutePathsConstant.ClassRunDetailPage,
                new WebAppLinkBuilderPageInputData()
                {
                    ActiveTab = classRunActiveTab,
                    Data = new { mode = classRunMode, id = classRunId, courseId }
                },
                routeDataLevel2);
            var routeDataLevel4 = WebAppLinkBuilderPageInput.Create(
                LMMRoutePathsConstant.AssignmentDetailPage,
                new WebAppLinkBuilderPageInputData()
                {
                    ActiveTab = activeTab,
                    Data = new { mode, assignmentId = id, courseId, classRunId }
                },
                routeDataLevel3);

            return $"{_opalClientUrlOption.OpalClientUrl}{LMMModule}{routeDataLevel4.ToNavigationPath()}";
        }

        public string GetSessionDetailLinkForLMMModule(
            string courseManagementPageActiveTab,
            string courseDetailActiveTab,
            string courseDetailSubActiveTab,
            string courseDetailMode,
            string classRunActiveTab,
            string classRunMode,
            string activeTab,
            string mode,
            Guid courseId,
            Guid classRunId,
            Guid id)
        {
            var routeDataLevel1 = WebAppLinkBuilderPageInput.Create(LMMRoutePathsConstant.CourseManagementPage, new WebAppLinkBuilderPageInputData() { ActiveTab = courseManagementPageActiveTab });
            var routeDataLevel2 = WebAppLinkBuilderPageInput.Create(
                LMMRoutePathsConstant.CourseDetailPage,
                new WebAppLinkBuilderPageInputData()
                {
                    ActiveTab = courseDetailActiveTab,
                    SubActiveTab = courseDetailSubActiveTab,
                    Data = new { mode = courseDetailMode, id = courseId }
                },
                routeDataLevel1);
            var routeDataLevel3 = WebAppLinkBuilderPageInput.Create(
                LMMRoutePathsConstant.ClassRunDetailPage,
                new WebAppLinkBuilderPageInputData()
                {
                    ActiveTab = classRunActiveTab,
                    Data = new { mode = classRunMode, id = classRunId, courseId }
                },
                routeDataLevel2);
            var routeDataLevel4 = WebAppLinkBuilderPageInput.Create(
                LMMRoutePathsConstant.SessionDetailPage,
                new WebAppLinkBuilderPageInputData()
                {
                    ActiveTab = activeTab,
                    Data = new { mode, id = id, courseId, classRunId }
                },
                routeDataLevel3);

            return $"{_opalClientUrlOption.OpalClientUrl}{LMMModule}{routeDataLevel4.ToNavigationPath()}";
        }

        // Learner Deeplink
#pragma warning disable CA1024 // Use properties where appropriate
        public string GetCourseDetailLearnerLinkForCAMModule(Guid courseId)
        {
            return $"{_opalClientUrlOption.OpalClientUrl}{LearnerModule}/detail/Course/{courseId.ToString()}";
        }

        public string GetMyAchievementsLearnerLinkForCAMModule()
        {
            return $"{_opalClientUrlOption.OpalClientUrl}{LearnerModule}/detail/myachievements";
        }
#pragma warning restore CA1024 // Use properties where appropriate

        // PDPM Deeplink
#pragma warning disable CA1024 // Use properties where appropriate
        public string GetClassRegistrationLinkForCAMModule()
        {
            return $"{PDPMModuleClientRootUrl()}/pending-request-idp/{PDPMRoutePathsConstant.ClassRegistrationPage}";
        }

        public string GetClassWithdrawalLinkForCAMModule()
        {
            return $"{PDPMModuleClientRootUrl()}/pending-request-idp/{PDPMRoutePathsConstant.ClassWithDrawalPage}";
        }

        public string GetClassChangeRequestLinkForCAMModule()
        {
            return $"{PDPMModuleClientRootUrl()}/pending-request-idp/{PDPMRoutePathsConstant.ClassChangeRequestPage}";
        }

        public string GetAdhocNominationLinkForCAMModule()
        {
            return $"{PDPMModuleClientRootUrl()}/adhoc-nominations/";
        }

        private string PDPMModuleClientRootUrl()
        {
            return $"{_opalClientUrlOption.OpalClientUrl.Replace("/app/", "/")}{PDPMModule}";
        }
#pragma warning restore CA1024 // Use properties where appropriate
    }

    public class WebAppLinkBuilderPageInputData
    {
        public static WebAppLinkBuilderPageInputData Default => new WebAppLinkBuilderPageInputData();

        public string ActiveTab { get; set; }

        public string SubActiveTab { get; set; }

        public object Data { get; set; }

#pragma warning disable CA1055 // URI-like return values should not be strings
        public virtual string ToEncodedUrlString()
#pragma warning restore CA1055 // URI-like return values should not be strings
        {
            // Return "_" when data is empty. "_" represent for empty page input data in url in web app client
            if (string.IsNullOrEmpty(ActiveTab) && string.IsNullOrEmpty(SubActiveTab) && Data == null)
            {
                return "_";
            }

            var jsonStringFormat = JsonSerializer.Serialize(
                this,
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, IgnoreNullValues = true });
            return WebUtility.UrlEncode(jsonStringFormat);
        }
    }

    public class WebAppLinkBuilderPageInput : WebAppLinkBuilderPageInputData
    {
        public string Path { get; set; }

        public WebAppLinkBuilderPageInput Parent { get; set; }

        public static WebAppLinkBuilderPageInput Create(
            string path,
            WebAppLinkBuilderPageInputData data = null,
            WebAppLinkBuilderPageInput parent = null)
        {
            return new WebAppLinkBuilderPageInput()
            {
                Path = path,
                Data = data?.Data,
                ActiveTab = data?.ActiveTab,
                SubActiveTab = data?.SubActiveTab,
                Parent = parent
            };
        }

        public string ToNavigationPath()
        {
            var parentToChildOrderRoutes = this.FlatRouteTree();

            var path = string.Empty;
            parentToChildOrderRoutes.ForEach(p =>
            {
                if (!string.IsNullOrEmpty(p.Path))
                {
                    path += "/" + p.Path + "/" + p.ExtractPageInputData().ToEncodedUrlString();
                }
            });

            return path;
        }

        public List<WebAppLinkBuilderPageInput> FlatRouteTree()
        {
            var childToParentOrderRoutes = F.List(this);

            var currentPageInput = this;
            while (currentPageInput.Parent != null)
            {
                childToParentOrderRoutes.Add(currentPageInput.Parent);
                currentPageInput = currentPageInput.Parent;
            }

            childToParentOrderRoutes.Reverse();

            return childToParentOrderRoutes;
        }

        public WebAppLinkBuilderPageInputData ExtractPageInputData()
        {
            return new WebAppLinkBuilderPageInputData()
            {
                SubActiveTab = SubActiveTab,
                Data = Data,
                ActiveTab = ActiveTab
            };
        }
    }
}
#pragma warning restore SA1402 // File may only contain a single type
