using System;

namespace cxOrganization.Domain.HttpClients
{
    public class UserIdentityResponseDto
    {
        public UserIdentityDto User { get; set; }
        public string Otp { get; set; }
        public DateTime? OtpExpiration { get; set; }
    }
}