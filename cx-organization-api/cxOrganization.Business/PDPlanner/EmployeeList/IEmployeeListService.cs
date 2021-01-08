using System.Threading.Tasks;

namespace cxOrganization.Business.PDPlanner.EmployeeList
{
    public interface IEmployeeListService
    {
        Task<IdpEmployeeListDto> GetIdpEmployeeListAsync(IdpEmployeeListArguments idpEmployeeListArgument);
    }
}