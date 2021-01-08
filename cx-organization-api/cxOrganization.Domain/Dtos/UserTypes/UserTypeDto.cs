using System.Collections.Generic;
using cxOrganization.Client.ConexusBase;
using cxPlatform.Client.ConexusBase;

namespace cxOrganization.Client.UserTypes
{
    public class UserTypeDto : ConexusBaseDto, ILocalizedDto
    {
        /// <summary>
        /// The localized data text
        /// </summary>
        public List<LocalizedDataDto> LocalizedData { get; set; }
        /// <summary>
        /// Gets or sets the parent Identifier.
        /// </summary>
        /// <value>The ParentId).</value>
        public int? ParentId { get; set; }
    }
}
