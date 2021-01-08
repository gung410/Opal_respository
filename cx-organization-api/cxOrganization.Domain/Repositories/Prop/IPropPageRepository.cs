using cxOrganization.Domain.Entities;
using cxPlatform.Core;
using System.Collections.Generic;

namespace cxOrganization.Domain.Repositories
{
    /// <summary>
    /// Interface IPropPageRepository
    /// </summary>
    public interface IPropPageRepository : IRepository<PropPageEntity>
    {
        /// <summary>
        /// Gets the property pages by table type identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>List{PropPage}.</returns>
        List<PropPageEntity> GetPropPagesByTableTypeId(int Id);
    }
}
