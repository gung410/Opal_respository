using Communication.Business.Models.Command;
using System;
using System.Collections.Generic;

namespace Communication.Business.Models
{
    public class OrgnanizationResponseDto
    {
        public long TotalItems { get; set; }
        public long PageIndex { get; set; }
        public long PageSize { get; set; }
        public bool HasMoreData { get; set; }
        public List<UserDomainDto> Items { get; set; }



    }

    public class UserDomainDto
    {
        public UserDomainDto()
        {
            JsonDynamicAttributes = new JsonDynamicAttributes();
        }
        public string DepartmentName { get; set; }
        public long DepartmentId { get; set; }
        public string FirstName { get; set; }
        public long MobileCountryCode { get; set; }
        public string EmailAddress { get; set; }
        public long Gender { get; set; }
        public string Tag { get; set; }
        public DateTime? Created { get; set; }
        public bool ForceLoginAgain { get; set; }
        public JsonDynamicAttributes JsonDynamicAttributes { get;set;}
        public DomainIdentity Identity { get; set; }
    }

    public class JsonDynamicAttributes
    {
        public Channel? NotificationReference { get; set; }
    }

    public class DomainIdentity
    {
        public string ExtId { get; set; }
        public int OwnerId { get; set; }
        public int? CustomerId { get; set; }
        public string Archetype { get; set; }
        public long Id { get; set; }
    }

}
