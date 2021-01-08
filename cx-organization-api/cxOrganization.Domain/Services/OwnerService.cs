using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Repositories;

namespace cxOrganization.Domain.Services
{
    public class OwnerService : IOwnerService
    {
        private readonly IOwnerRepository _ownerRepository;

        public OwnerService(IOwnerRepository ownerRepository)
        {
            _ownerRepository = ownerRepository;
        }

        /// <summary>
        /// Get onwer by OwnerID to get information as: R-database,...
        /// </summary>
        /// <param name="ownerId">The owner identifier.</param>
        /// <returns>Owner.</returns>
        public OwnerEntity GetOwnerById(int ownerId)
        {
            return _ownerRepository.GetById(ownerId);
        }
    }
}
