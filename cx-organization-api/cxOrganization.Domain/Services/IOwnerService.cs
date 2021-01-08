using cxOrganization.Domain.Entities;

namespace cxOrganization.Domain.Services
{
    public interface IOwnerService
    {
        /// <summary>
        /// Gets the owner by identifier.
        /// </summary>
        /// <param name="ownerId">The owner identifier.</param>
        /// <returns>Owner.</returns>
        OwnerEntity GetOwnerById(int ownerId);
    }
}
