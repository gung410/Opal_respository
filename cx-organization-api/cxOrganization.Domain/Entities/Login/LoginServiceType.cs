using System;
using System.Collections.Generic;
using System.Text;

namespace cxOrganization.Domain.Entities.Login
{
    public enum LoginServiceType
    {
        NativeLogin = 1,
        AADB2C = 2,
        ADFS = 3,
        cxIdentityServer = 4
    }
}
