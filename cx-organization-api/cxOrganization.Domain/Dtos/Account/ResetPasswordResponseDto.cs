
using System;

namespace cxOrganization.Client.Account
{
    public class ResetPasswordResponseDto : AccountBaseDto
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool UserOneTimePassword { get; set; }
        public string OneTimePassword { get; set; }
        public DateTime OneTimeExpireTime { get; set; }
        public int UserId { get; set; }
    }
}
