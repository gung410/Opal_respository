using Communication.Business.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Communication.Business.HttpClients
{
    public interface IOrganizationClientService
    {
        Task<OrgnanizationResponseDto> GetUsers(string cxToken, ISet<string> userIds = null,
            ISet<string> emails = null, 
            ISet<string> departmentIds = null,
            ISet<string> roles = null,
            ISet<string> userGroupIds = null,
            int pageIndex = 1, int pageSize = 100,
            bool? forHrmsUsers = null, bool? forExternalUsers = null);
    }
}
