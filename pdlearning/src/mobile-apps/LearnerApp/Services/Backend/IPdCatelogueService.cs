using System.Threading.Tasks;
using LearnerApp.Models.PdCatelogue;
using Refit;

namespace LearnerApp.Services.Backend
{
    public interface IPdCatelogueService
    {
        [Post("/api/PDCatalogue/search")]
        Task<PdCatelogueSearch> Search([Body] object param);

        [Post("/api/PDCatalogue/searchv2")]
        Task<PdCatelogueSearch> SearchV2([Body] object param);

        [Post("/api/PDCatalogue/newlyResourceAdded")]
        Task<PdCatelogueSearch> GetNewlyAdded([Body] object param);

        [Post("/api/PDCatalogue/search/recommendationv2")]
        Task<PdCatelogueSearch> GetRecommendation([Body] object param);

        [Get("/catalogentries/explorer/DESIGNATION")]
        Task<JobDesignation[]> GetJobDesignation();
    }
}
