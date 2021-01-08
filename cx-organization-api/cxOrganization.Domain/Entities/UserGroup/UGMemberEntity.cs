using System;
using System.ComponentModel.DataAnnotations;

namespace cxOrganization.Domain.Entities
{
    /// <summary>
    /// Class UserGroupUser.
    /// </summary>
    [Serializable]
    public class UGMemberEntity
    {
        /// <summary>
        /// User group user Id)
        /// </summary>
        public long UGMemberId { get; set; }
        /// <summary>
        /// User group reference Id)
        /// </summary>
        public int UserGroupId { get; set; }
        /// <summary>
        /// Referenced user group
        /// </summary>
        public UserGroupEntity UserGroup { get; set; }
        /// <summary>
        /// User reference Id)
        /// </summary>
        public int? UserId { get; set; }
        /// <summary>
        /// Referenced user
        /// </summary>
        public UserEntity User { get; set; }
        /// <summary>
        /// Created date time
        /// </summary>
        public DateTime? Created { get; set; }
        /// <summary>
        /// Created by Id)
        /// </summary>
        public int? CreatedBy { get; set; }
        /// <summary>
        /// Member role Id)
        /// </summary>
        public int? MemberRoleId { get; set; }
        /// <summary>
        /// ValId) from
        /// </summary>
        public DateTime? validFrom { get; set; }
        /// <summary>
        /// ValId) to
        /// </summary>
        public DateTime? ValidTo { get; set; }
        /// <summary>
        /// Entity Version
        /// </summary>
        [Timestamp]
        public byte[] EntityVersion { get; set; }
        /// <summary>
        /// Last updated
        /// </summary>
        public DateTime? LastUpdated { get; set; }
        /// <summary>
        /// Last updated by
        /// </summary>
        public int? LastUpdatedBy { get; set; }
        /// <summary>
        /// Last synchronized
        /// </summary>
        public DateTime? LastSynchronized { get; set; }
        /// <summary>
        /// Deleted
        /// </summary>
        public DateTime? Deleted { get; set; }
        /// <summary>
        /// Entity status Id)
        /// </summary>
        public int? EntityStatusId { get; set; }
        /// <summary>
        /// Entity status reason Id)
        /// </summary>
        public int? EntityStatusReasonId { get; set; }
        /// <summary>
        /// Customer Id)
        /// </summary>
        public int? CustomerId { get; set; }
        /// <summary>
        /// Period Id)
        /// </summary>
        public int? PeriodId { get; set; }
        /// <summary>
        /// External Id)
        /// </summary>
        public string ExtId { get; set; }
        /// <summary>
        /// Member role
        /// </summary>
        public virtual MemberRoleEntity MemberRole { get; set; }
        /// <summary>
        /// Display name
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// Referrer Token of external system
        /// </summary>
        public string ReferrerToken { get; set; }
        /// <summary>
        /// ReferrerResource link to external instance
        /// </summary>
        public string ReferrerResource { get; set; }
        /// <summary>
        /// Archetype of external instance
        /// </summary>
        public int? ReferrerArchetypeId { get; set; }
    }
}
