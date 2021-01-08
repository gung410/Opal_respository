using System;
using System.Collections.Generic;

namespace cxOrganization.Domain.Entities
{
    /// <summary>
    /// Class PropOption.
    /// </summary>
    [Serializable]
    public class PropOptionEntity
    {
        /// <summary>
        /// Gets or sets the property option Identifier.
        /// </summary>
        /// <value>The property option Identifier.</value>
        public int PropOptionId { get; set; }
        /// <summary>
        /// Gets or sets the property Identifier.
        /// </summary>
        /// <value>The property Identifier.</value>
        public int PropertyId { get; set; }
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public double Value { get; set; }
        /// <summary>
        /// Gets or sets the l t_ property option.
        /// </summary>
        /// <value>The l t_ property option.</value>
        public virtual ICollection<LtPropOptionEntity> LtPropOptions { get; set; }
    }
}
