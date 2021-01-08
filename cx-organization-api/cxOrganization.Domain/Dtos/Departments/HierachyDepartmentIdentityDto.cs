using cxPlatform.Client.ConexusBase;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace cxOrganization.Client.Departments
{
    public class HierachyDepartmentIdentityDto : ConexusBaseDto
    {
        public HierachyDepartmentIdentityDto()
        {
            Identity = new IdentityDto();
        }
        public int ParentDepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentDescription { get; set; }       
        public string OrganizationNumber { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string Tag { get; set; }
        public int? LanguageId { get; set; }
        public int? CountryCode { get; set; }
        public string Path { get; set; }
        public string PathName { get; set; }
        public IDictionary<string, object> JsonDynamicAttributes { get; set; }
        public int? ChildrenCount { get; set; }
        public int? UserCount { get; set; }
        [JsonExtensionData]
        public Dictionary<string, object> CustomData { get; set; }
        public bool? IsCurrentDepartment { get; set; }

        public T Cast<T>() where T:HierachyDepartmentIdentityDto, new()
        {
            return new T
            {
                Identity = this.Identity,
                Path = this.Path,
                Address = this.Address,
                ChildrenCount = this.ChildrenCount,
                City = this.City,
                CountryCode = this.CountryCode,
                CustomData = this.CustomData,
                DepartmentDescription = this.DepartmentDescription,
                DepartmentName = this.DepartmentName,
                IsCurrentDepartment = this.IsCurrentDepartment,
                JsonDynamicAttributes = this.JsonDynamicAttributes,
                LanguageId = this.LanguageId,
                OrganizationNumber = this.OrganizationNumber,
                ParentDepartmentId = this.ParentDepartmentId,
                PathName = this.PathName,
                PostalCode = this.PostalCode,
                Tag = this.Tag,
                UserCount = this.UserCount,
                DynamicAttributes = this.DynamicAttributes,
                EntityStatus = this.EntityStatus
            };
        }
    }
}
