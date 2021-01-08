using System.Collections.Generic;
using System.Threading.Tasks;

namespace cxOrganization.Domain.ApiClient
{
    public interface ILearningCatalogClientService
    {
        Task<List<CatalogItemDto>> GetCatalogEntries(string correlationId, string parentCode);
        Task<List<CatalogItemDto>> GetOrganizationUnitTypes(string correlationId);
        Task<List<CatalogItemDto>> GetDesignations(string correlationId);
    }

}
