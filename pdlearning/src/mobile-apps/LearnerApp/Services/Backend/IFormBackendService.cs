using System.Collections.Generic;
using System.Threading.Tasks;
using LearnerApp.Models.Search;
using Refit;

namespace LearnerApp.Services.Backend
{
    public interface IFormBackendService
    {
        [Post("/api/form-participant/form-ids")]
        Task<List<FormSearch>> GetFormsByIds([Body] object param);
    }
}
