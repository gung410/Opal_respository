using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LearnerApp.Models.PDPM;
using Refit;

namespace LearnerApp.Services.Backend
{
    public interface IPDPMBackendService
    {
        [Post("/idp/actionitems/results")]
        Task<PDOResponse> AddCourseToPDPlan([Body] PDO pdo);

        [Get("/idp/actionitems/{query}")]
        [QueryUriFormat(UriFormat.Unescaped)]
        Task<List<PDOResponse>> GetResultPDPlan(string query);

        [Get("/idp/plans/pdos?courseId={courseId}")]
        Task<PDOSInfoResponse> GetPDOSInfo(string courseId);

        [Post("/idp/actionitems/results/deactivate")]
        Task<List<PDORemoveResponse>> DeactivateCourseInPDPlan([Body] PDORemove pdo);

        [Get("/idp/recommendationsByOU?pageIndex={pageIndex}&pageSize={pageSize}")]
        Task<PDORecommendation> GetRecommendationByOrganization(int pageIndex = 1, int pageSize = GlobalSettings.MaxResultPerPage);
    }
}
