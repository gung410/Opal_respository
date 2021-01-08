using System.Collections.Generic;
using System.Threading.Tasks;
using LearnerApp.Models;
using Refit;

namespace LearnerApp.Services.Backend
{
    public interface IMetadataBackendService
    {
        [Get("/api/courses/{courseId}/metadata")]
        Task<Metadata> GetCourseMetadata(string courseId);

        [Get("/api/resource/{digitalContentId}")]
        Task<Resource> GetDigitalContentMetadata(string digitalContentId);

        [Get("/api/metadataTag")]
        Task<List<MetadataTag>> GetMetadata();
    }
}
