using cxPlatform.Client.ConexusBase;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace cxOrganization.Domain.Services
{
    public interface ISuspendOrDeactiveUserBackgroundJob
    {
        Task SuspendUserStatus();
        Task DeActiveUserStatus();
    }
}
