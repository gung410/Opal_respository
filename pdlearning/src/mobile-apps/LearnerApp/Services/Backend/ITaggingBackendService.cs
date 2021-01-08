using System.Threading.Tasks;
using Refit;

namespace LearnerApp.Services.Backend
{
    public interface ITaggingBackendService
    {
        [Post("/api/me/suggestion/courses")]
        Task<string[]> GetMySuggestedCourseIdentifiers([Body] string[] userTagIds);
    }
}
