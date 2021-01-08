using System.Collections.Generic;
using System.Threading.Tasks;
using cxOrganization.Adapter.Assessment.Data.Entities;
using cxPlatform.Core;

namespace cxOrganization.Adapter.Assessment.Data.Repositories
{
    /// <summary>
    /// IEventRepository
    /// </summary>
    public interface IStatusTypeRepository : IRepository<StatusTypeEntity>
    {
        List<StatusTypeEntity> GetStatusTypes();
        Task<List<int>> GetStatusTypeIdsByCodeNames(List<string> statusTypeCodeNames);
    }
}
