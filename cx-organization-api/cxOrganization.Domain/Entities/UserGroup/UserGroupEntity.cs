using System;
using System.Collections.Generic;
using cxPlatform.Core.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace cxOrganization.Domain.Entities
{
    /// <summary>
    /// Class UserGroup.
    /// </summary>
    [Serializable]
    public class UserGroupEntity : OwnerEntityBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserGroupEntity"/> class.
        /// </summary>
        public UserGroupEntity()
        {
            //DepartmentTypes = new List<DepartmentTypeEntity>();
            UGMembers = new List<UGMemberEntity>();
            ReferrerToken = string.Empty;
            ReferrerResource = string.Empty;
            Tag = string.Empty;
        }
        /// <summary>
        /// Gets or sets the user group Identifier.
        /// </summary>
        /// <value>The user group Identifier.</value>
        public int UserGroupId { get; set; }
        [NotMapped]
        public int? CustomerId { get; set; }
        /// <summary>
        /// Gets or sets the department Identifier.
        /// </summary>
        /// <value>The department Identifier.</value>
        public int? DepartmentId { get; set; }
        /// <summary>
        /// Gets or sets the survey Identifier.
        /// </summary>
        /// <value>The survey Identifier.</value>
        public int? SurveyId { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }
        /// <summary>
        /// Gets or sets the created.
        /// </summary>
        /// <value>The created.</value>
        public DateTime Created { get; set; }
        /// <summary>
        /// Gets or sets the ext Identifier.
        /// </summary>
        /// <value>The ext Identifier.</value>
        public string ExtId { get; set; }
        /// <summary>
        /// Gets or sets the period Identifier.
        /// </summary>
        /// <value>The period Identifier.</value>
        public int? PeriodId { get; set; }
        /// <summary>
        /// Gets or sets the user Identifier.
        /// </summary>
        /// <value>The user Identifier.</value>
        public int? UserId { get; set; }
        /// <summary>
        /// Gets or sets the tag.
        /// </summary>
        /// <value>The tag.</value>
        public string Tag { get; set; }
        /// <summary>
        /// Gets or sets the last updated.
        /// </summary>
        /// <value>The last updated.</value>
        public DateTime LastUpdated { get; set; }
        /// <summary>
        /// Gets or sets last updated by.
        /// </summary>
        /// <value>The last updated by.</value>
        public int? LastUpdatedBy { get; set; }
        /// <summary>
        /// Gets or sets the last synchronized.
        /// </summary>
        /// <value>The last synchronized.</value>
        public DateTime LastSynchronized { get; set; }
        /// <summary>
        /// Gets or sets the archetype Identifier.
        /// </summary>
        /// <value>The archetype Identifier.</value>
        public int? ArchetypeId { get; set; }
        /// <summary>
        /// Gets or sets the department.
        /// </summary>
        /// <value>The department.</value>
        public virtual DepartmentEntity Department { get; set; }

        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        /// <value>The department.</value>
        public virtual UserEntity User { get; set; }
        /// <summary>
        /// Gets or sets the department types.
        /// </summary>
        /// <value>The department types.</value>
        //public virtual ICollection<DepartmentTypeEntity> DepartmentTypes { get; set; }
        /// <summary>
        /// Gets or sets the user group type Identifier.
        /// </summary>
        /// <value>The user group type Identifier.</value>
        public int? UserGroupTypeId { get; set; }
        /// <summary>
        /// Get or set the User group users.
        /// </summary>
        public ICollection<UGMemberEntity> UGMembers { get; set; }
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
        public ICollection<DTUGEntity> DT_UGs { get; set; }
    }
}
