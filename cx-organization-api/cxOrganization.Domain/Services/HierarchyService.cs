using cxOrganization.Domain.AdvancedWorkContext;
using cxOrganization.Domain.Repositories;
using cxPlatform.Core;

namespace cxOrganization.Domain.Services
{
    public class HierarchyService : IHierarchyService
    {
        private readonly IHierarchyRepository _hierarchyRepository;
        private readonly IAdvancedWorkContext _workContext;

        public HierarchyService(IHierarchyRepository hierarchyRepository,
            IAdvancedWorkContext workContext)
        {
            _hierarchyRepository = hierarchyRepository;
            _workContext = workContext;
        }
    }
}
