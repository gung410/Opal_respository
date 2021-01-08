using cxOrganization.Domain.Entities;
using cxPlatform.Core;

namespace cxOrganization.Domain.Repositories
{
    public interface IPropOptionRepository : IRepository<PropOptionEntity>
    {
        PropOptionEntity GetPropOptionById(int propOptionId);
    }
}
