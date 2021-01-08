using System;
using System.ComponentModel.DataAnnotations;
using cxOrganization.Business.Connection;

namespace cxOrganization.Business.Validations
{

    [AttributeUsage(AttributeTargets.Class |AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public class ConnectionMemberValidateAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var connectionMember = value as ConnectionMemberDto;
            if (connectionMember != null)
            {
                return ValidateConnectionMember(connectionMember);
            }
       
            return true;

        }
        private bool ValidateConnectionMember(ConnectionMemberDto connectionMember)
        {
            if (connectionMember.UserIdentity == null
                && string.IsNullOrEmpty(connectionMember.ReferrerToken)
                && string.IsNullOrEmpty(connectionMember.ReferrerResource) && connectionMember.ReferrerArchetype == null)
            {
                ErrorMessage = "It requires value on UserIdentity or all referrer informations (ReferrerToken, ReferrerResource, ReferrerArchetype)";
                return false;
            }
            return true;
        }


    
    }
}