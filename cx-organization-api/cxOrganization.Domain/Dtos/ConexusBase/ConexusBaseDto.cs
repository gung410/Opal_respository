using System;
using System.Collections.Generic;
using System.Dynamic;

namespace cxPlatform.Client.ConexusBase
{
    [Serializable]
    public abstract class ConexusBaseDto : IConexusBaseDto
    {
        public ConexusBaseDto()
        {
            Identity = new IdentityDto();
            EntityStatus = new EntityStatusDto();
        }
        /// <summary>
        /// The information of DTO indentty: Owner, customer, Identifier, and External Identifier
        /// </summary>
        public IdentityDto Identity { get; set; }
        /// <summary>
        /// Addtional information for the DTO
        /// </summary>
        public List<EntityKeyValueDto> DynamicAttributes { get; set; }
        /// <summary>
        /// The information about datetime/version of data changes and status of the entity prensent for DTO in database
        /// </summary>
        public EntityStatusDto EntityStatus { get; set; }
    }
}
