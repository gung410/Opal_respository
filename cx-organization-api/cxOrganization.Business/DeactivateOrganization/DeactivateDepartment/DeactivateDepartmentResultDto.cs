using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using cxOrganization.Business.Common;
using cxPlatform.Client.ConexusBase;

namespace cxOrganization.Business.DeactivateOrganization.DeactivateDepartment
{
    [DataContract]
    [Serializable]
    public class DeactivateDepartmentsResultDto
    {
        [DataMember]
        public List<DeactivateDepartmentResultDto> DepartmentResults { get; set; }

        [DataMember]
        public IdentityDto UpdatedByIdentity { get; set; }

        public int MaxStatus()
        {
            if (DepartmentResults.Count == 0) return 0;
            return DepartmentResults.Max(r => r.Status.StatusCode);

        }
    }
    [DataContract]
    [Serializable]
    public class DeactivateDepartmentResultDto
    {
        [DataMember]
        public IdentityDto Identity { get; set; }

        [DataMember]
        public MessageStatus Status { get; set; }


        [DataMember]
        public List<IdentityDto> DescendantIdentites { get; set; }

        public static DeactivateDepartmentResultDto Create(IdentityDto identity, MessageStatus messageStatus, List<IdentityDto> descendantIdentites=null)
        {
            return new DeactivateDepartmentResultDto
            {
                Identity = identity,
                Status = messageStatus,
                DescendantIdentites = descendantIdentites

            };
        }
    
        public static DeactivateDepartmentResultDto CreateNotFound(IdentityDto identity)
        {
            return new DeactivateDepartmentResultDto
            {
                Identity = identity,
                Status = MessageStatus.CreateNotFound(identity)

            };
        }

        public static DeactivateDepartmentResultDto CreateSuccess(IdentityDto identity, string message = null)
        {
            return new DeactivateDepartmentResultDto
            {
                Identity = identity,
                Status = MessageStatus.CreateSuccess(message)

            };
        }
    }
}