using System;

namespace cxOrganization.Domain.HttpClients
{
    public class UserIdentityOtpDto
    {
        public string Otp { get; set; }
        public DateTime? OtpExpiration { get; set; }
    }
}