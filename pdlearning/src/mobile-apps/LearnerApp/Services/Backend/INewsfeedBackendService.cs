using System.Threading.Tasks;
using LearnerApp.Models;
using LearnerApp.Models.Newsfeed;
using Refit;

namespace LearnerApp.Services.Backend
{
    public interface INewsfeedBackendService
    {
        [Post("/api/newsFeed")]
        public Task<ListResultDto<Feed>> GetNewsfeed([Body] object param);
    }
}
