using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using cxOrganization.Business.Common;
using cxPlatform.Client.ConexusBase;

namespace cxOrganization.Business.MoveOrganization.MoveDepartment
{
    [DataContract]
    [Serializable]
    public class MoveDepartmentsResultDto
    {
        [DataMember]
        public List<MoveDepartmentResultDto> DepartmentResults { get; set; }

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
    public class MoveDepartmentResultDto
    {
        [DataMember]
        public IdentityDto Identity { get; set; }
        [DataMember]
        public MessageStatus Status { get; set; }  

        //For internal handlingHierarchyDepartmentEntity
        [IgnoreDataMember]
        public int OldParentHierarchyDepartmentId { get; set; }
        [IgnoreDataMember]
        public int HierarchyDepartmentId { get; set; }

        public static MoveDepartmentResultDto Create(IdentityDto identity, MessageStatus messageStatus)
        {
            return new MoveDepartmentResultDto
            {
                Identity = identity,
                Status = messageStatus

            };
        }
    
        public static MoveDepartmentResultDto CreateNotFound(IdentityDto identity)
        {
            return new MoveDepartmentResultDto
            {
                Identity = identity,
                Status = MessageStatus.CreateNotFound(identity)

            };
        }
    }
}