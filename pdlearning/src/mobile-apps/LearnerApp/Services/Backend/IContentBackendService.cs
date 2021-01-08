using System.Collections.Generic;
using System.Threading.Tasks;
using LearnerApp.Models.Course;
using LearnerApp.Models.MyLearning;
using Refit;

namespace LearnerApp.Services.Backend
{
    public interface IContentBackendService
    {
        [Get("/api/contents/{digitalContentId}")]
        Task<MyDigitalContentDetails> GetDigitalContentDetails(string digitalContentId);

        [Post("/api/contents/getByIds")]
        Task<List<MyDigitalContentDetails>> GetDigitalContentDetails([Body] string[] digitalContentIds);

        [Post("/api/content/broken-links/report")]
        Task ReportBrokenLink([Body] object param);

        [Get("/api/contents/{resourceId}")]
        Task<LectureContentDetails> GetLectureContents(string resourceId);
    }
}
