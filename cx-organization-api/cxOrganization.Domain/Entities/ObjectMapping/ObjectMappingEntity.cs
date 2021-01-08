using System;

namespace cxOrganization.Domain.Entities
{
    /// <summary>
    /// Class ObjectMapping.
    /// </summary>
    [Serializable]
    public class ObjectMappingEntity
    {
        /// <summary>
        /// Gets or sets the omid.
        /// </summary>
        /// <value>The omid.</value>
        public int OMId { get; set; }
        /// <summary>
        /// Gets or sets the owner identifier.
        /// </summary>
        /// <value>The owner identifier.</value>
        public int OwnerId { get; set; }
        /// <summary>
        /// Gets or sets from table type identifier.
        /// </summary>
        /// <value>From table type identifier.</value>
        public short FromTableTypeId { get; set; }
        /// <summary>
        /// Gets or sets from identifier.
        /// </summary>
        /// <value>From identifier.</value>
        public int FromId { get; set; }
        /// <summary>
        /// Gets or sets to table type identifier.
        /// </summary>
        /// <value>To table type identifier.</value>
        public short ToTableTypeId { get; set; }
        /// <summary>
        /// Gets or sets to identifier.
        /// </summary>
        /// <value>To identifier.</value>
        public int ToId { get; set; }
        /// <summary>
        /// Gets or sets the relation type identifier.
        /// </summary>
        /// <value>The relation type identifier.</value>
        public int RelationTypeId { get; set; }
    }
}