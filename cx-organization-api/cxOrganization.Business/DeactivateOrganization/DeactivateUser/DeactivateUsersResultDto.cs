using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using cxOrganization.Business.Common;
using cxPlatform.Client.ConexusBase;

namespace cxOrganization.Business.DeactivateOrganization.DeactivateUser
{
    public class DeactivateUsersResultDto
    {
        public List<DeactivateUserResultDto> UserResults { get; set; }
        public IdentityDto UpdatedByIdentity { get; set; }

        public int MaxStatus()
        {
            if (UserResults.Count == 0) return 0;
            return UserResults.Max(r => r.StatusCode);

        }
    }
    public class DeactivateUserResultDto
    {
        public IdentityWithClaimDto Identity { get; set; }

        public DeactivateUserDetailResult DetailtResult { get; set; }

        public int StatusCode
        {
            get
            {
                if (DetailtResult != null)
                {
                    var deactiveUserStatus = DetailtResult.User != null ? DetailtResult.User.StatusCode : 0;
                    var deactivateLoginServicStatus = DetailtResult.LoginService != null ? DetailtResult.LoginService.StatusCode : 0;
                    return Math.Max(deactiveUserStatus, deactivateLoginServicStatus);
                }
                return 0;
            }
        }
    }

    public class DeactivateUserDetailResult
    {
        public MessageStatus User { get; set; }
        public MessageStatus LoginService { get; set; }
        public MessageStatus Membership { get; set; }

        public static DeactivateUserDetailResult Create(MessageStatus userResult, MessageStatus loginServiceResult, MessageStatus membership)
        {
            return new DeactivateUserDetailResult
            {
                User = userResult,
                LoginService = loginServiceResult,
                Membership = membership
            };
        }
    }
}