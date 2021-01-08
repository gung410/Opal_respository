using System;
using cxOrganization.Business.Common;

namespace cxOrganization.Business.Validations
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]

    public class UserArchetypeIdentityValidateAttribute : ArchetypeIdentityValidateAttribute
    {
        public UserArchetypeIdentityValidateAttribute()
           : base(DomainDefinition.UserArchetypes)
        {

        }
        protected override string GetArchetypeType()
        {
            return "user archetype";
        }
    }
}