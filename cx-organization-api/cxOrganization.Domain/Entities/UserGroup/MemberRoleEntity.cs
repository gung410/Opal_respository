using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cxOrganization.Domain.Entities
{
    /// <summary>
    /// Class MemberRole.
    /// </summary>
    [Serializable]
    public class MemberRoleEntity
    {
        public MemberRoleEntity()
        {
            LT_MemberRoles = new List<LtMemberRoleEntity>();
            UGMembers = new List<UGMemberEntity>();
        }
        /// <summary>
        /// Gets or sets MemberRoleId)
        /// </summary>
        public int MemberRoleId { get; set; }
        /// <summary>
        /// Gets or sets EntityStatusId)
        /// </summary>
        public int? EntityStatusId { get; set; }
        /// <summary>
        /// Gets or sets EntityStatusReasonId)
        /// </summary>
        public int? EntityStatusReasonId { get; set; }
        /// <summary>
        /// Gets or sets ExtId)
        /// </summary>
        public string ExtId { get; set; }
        /// <summary>
        /// Gets or sets No
        /// </summary>
        public int No { get; set; }
        // <summary>
        /// Gets or sets the created.
        /// </summary>
        /// <value>The created.</value>
        public DateTime Created { get; set; }
        /// <summary>
        /// Gets or sets OwnerId)
        /// </summary>
        public int OwnerId { get; set; }
        /// <summary>
        /// Gets or sets CodeName
        /// </summary>
        public string CodeName { get; set; }
        /// <summary>
        /// Gets or sets the type of the l t_ user.
        /// </summary>
        /// <value>The type of the l t_ user.</value>
        public virtual ICollection<LtMemberRoleEntity> LT_MemberRoles { get; set; }
        /// <summary>
        /// Gets or sets the type of the UserGroupUserEntities.
        /// </summary>
        /// <value>The type of the UserGroupUserEntities.</value>
        public virtual ICollection<UGMemberEntity> UGMembers { get; set; }
    }
}
