

using System;
using System.Collections.Generic;
using cxOrganization.Domain.Entities;

namespace cxOrganization.Domain.Services.Reports
{
    public class UserAccountDetailsInfo
    {
        public int UserId { get; set; }
        public string ExtId { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string TypeOfOrganization { get; set; }
        public string FullName  { get; set; }
        public List<string> ServiceScheme { get; set; }
        public string Designation { get; set; }
        public string EmailAddress { get; set; }
        public List<string> SystemRoles { get; set; }
        public string OnboardingStatus { get; set; }
        public DateTime? FirstLoginDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public DateTime? DateOnboarded { get; set; }
        public string AccountStatus { get; set; }
        public AccountType AccountType { get; set; }
        public string AccountTypeDisplayText{ get; set; }
    }

    public class UserAccountDetailsEntity
    {
        public int UserId { get; set; }
        public string ExtId { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string TypeOfOrganizationUnitId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DesignationId { get; set; }
        public string JobTitle { get; set; }
        public string EmailAddress { get; set; }
        public string FinishedOnBoarding { get; set; }
        public string LastLoginDate { get; set; }
        public string DateOnboarded { get; set; }
        public string AccountStatus { get; set; }
        public ICollection<UTUEntity> UtuEntities { get; set; }
        public short Locked { get; set; }
      
    }
}
