using System;
using System.Collections.Generic;
using cxPlatform.Client.ConexusBase;

namespace cxOrganization.Client.Account
{
    public class ChangePasswordRequestDto : AccountBaseDto
    {
        public string UserName { get; set; }
        public string NewPassword { get; set; }
        public string OneTimePassword { get; set; }
    }
}
