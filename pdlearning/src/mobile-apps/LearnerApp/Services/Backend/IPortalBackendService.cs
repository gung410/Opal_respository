using System.Collections.Generic;
using System.Threading.Tasks;
using LearnerApp.Models;
using LearnerApp.Models.Permission;
using Refit;

namespace LearnerApp.Services.Backend
{
    public interface IPortalBackendService
    {
        [Get("/menus")]
        Task<object> GetAllowedSites();

        [Get("/sites")]
        Task<Sites> GetSiteInformation();

        [Get("/me/accessRights/?modules=LearnerSite")]
        Task<List<AccessRight>> GetAccessRights();
    }
}
