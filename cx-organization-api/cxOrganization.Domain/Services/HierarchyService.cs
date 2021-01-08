using cxOrganization.Domain.Repositories;
using cxPlatform.Core;

namespace cxOrganization.Domain.Services
{
    public class HierarchyService : IHierarchyService
    {
        private readonly IHierarchyRepository _hierarchyRepository;
        private readonly IWorkContext _workContext;

        public HierarchyService(IHierarchyRepository hierarchyRepository,
            IWorkContext workContext)
        {
            _hierarchyRepository = hierarchyRepository;
            _workContext = workContext;
        }
    }
}
