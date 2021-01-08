using System;
using cxOrganization.Business.Common;

namespace cxOrganization.Business.Validations
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]

    public class UserGroupArchetypeIdentityValidateAttribute : ArchetypeIdentityValidateAttribute
    {
        public UserGroupArchetypeIdentityValidateAttribute()
            : base(DomainDefinition.UserGroupArchetypes)
        {

        }
        protected override string GetArchetypeType()
        {
            return "user group archetype";
        }
    }
}