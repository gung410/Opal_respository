using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cxOrganization.Domain.Entities
{
    /// <summary>
    /// Class LT_MemberRoleEntity.
    /// </summary>
    [Serializable]
    public class LtMemberRoleEntity
    {
        /// <summary>
        /// Gets or sets MemberRoleId)
        /// </summary>
        public int MemberRoleId { get; set; }
        /// <summary>
        /// Gets or sets the language Identifier.
        /// </summary>
        /// <value>The language Identifier.</value>
        public int LanguageId { get; set; }
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
        /// Gets or sets the type of the user.
        /// </summary>
        /// <value>The type of the user.</value>
        public virtual MemberRoleEntity MemberRole { get; set; }
    }
}
