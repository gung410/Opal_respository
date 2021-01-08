using System;
using cxOrganization.Business.Common;

namespace cxOrganization.Business.Validations
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]

    public class DepartmentArchetypeIdentityValidateAttribute : ArchetypeIdentityValidateAttribute
    {
        public DepartmentArchetypeIdentityValidateAttribute()
            : base(DomainDefinition.DepartmentArchetypes)
        {
            
        }
        protected override string GetArchetypeType()
        {
            return "department archetype";
        }
    }
}