using System;

namespace cxOrganization.Domain.HttpClients
{
    public class UserIdentityDto
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public DateTime? LastLogoutDate { get; set; }
        public DateTime? MobileLastLoginDate { get; set; }
        public string EmailAddress { get; set; }
        public bool IsActive { get; set; }
        public string Role { get; set; }
        public string[] Roles { get; set; }
        public string[] Orgs { get; set; }
        public string[] AccessPlans { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string CountryCode { get; set; }
        public string Country { get; set; }
        public int? Gender { get; set; }
        public bool IsLocked { get; set; }
        public int? Status { get; set; }
        public string EncryptedNric { get; set; }
        public string OrganizationUnitName { get; set; }
        public string PersonId { get; set; }
        public string OTPValue { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public DateTime? ActiveDate { get; set; }
        public bool? UnlockUser { get; set; }
        public bool LockoutEnabled { get; set; }
        public bool IsRequestOTP { get; set; }
        public string ServiceScheme { get; set; }
        public string Category { get; set; }
        public string Reason { get; set; }
        public string Designation { get; set; }
        public bool KeepLogin { get; set; }
    }
}