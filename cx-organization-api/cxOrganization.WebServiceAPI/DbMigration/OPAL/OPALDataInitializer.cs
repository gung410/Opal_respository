using cxOrganization.Domain;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Repositories;

namespace cxOrganization.WebServiceAPI.DbMigration.OPAL
{
    /// <summary>
    /// This class will responsible for initializing the default identity server config data for MOE only
    /// </summary>
    [Name("opal")]
    public class OPALDataInitializer : IDataInitializer
    {
        private readonly IUserRepository _userRepository;
        private readonly OrganizationDbContext _organizationDbContext;

        public OPALDataInitializer(IUserRepository userRepository,
            OrganizationDbContext organizationDbContext)
        {
            _userRepository = userRepository;
            _organizationDbContext = organizationDbContext;
        }

        public void Run()
        {
        }
    }
}
