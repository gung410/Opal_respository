using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace cxOrganization.Domain.Entities
{
    /// <summary>
    /// Class PropValue.
    /// </summary>
    [Serializable]
    public class PropValueEntity
    {
        /// <summary>
        /// Gets or sets the property value Identifier.
        /// </summary>
        /// <value>The property value Identifier.</value>
        public int PropValueId { get; set; }
        /// <summary>
        /// Gets or sets the property Identifier.
        /// </summary>
        /// <value>The property Identifier.</value>
        public int PropertyId { get; set; }
        /// <summary>
        /// Gets or sets the property option Identifier.
        /// </summary>
        /// <value>The property option Identifier.</value>
        public int? PropOptionId { get; set; }
        /// <summary>
        /// Gets or sets the item Identifier.
        /// </summary>
        /// <value>The item Identifier.</value>
        public int ItemId { get; set; }
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public string Value { get; set; }
        /// <summary>
        /// Gets or sets the property file Identifier.
        /// </summary>
        /// <value>The property file Identifier.</value>
        public int? PropFileId { get; set; }
        /// <summary>
        /// Gets or sets the no.
        /// </summary>
        /// <value>The no.</value>
        public short No { get; set; }
        /// <summary>
        /// Gets or sets the created.
        /// </summary>
        /// <value>The created.</value>
        public DateTime Created { get; set; }
        /// <summary>
        /// Gets or sets the created by.
        /// </summary>
        /// <value>The created by.</value>
        public int? CreatedBy { get; set; }
        /// <summary>
        /// Gets or sets the updated.
        /// </summary>
        /// <value>The updated.</value>
        public DateTime Updated { get; set; }
        /// <summary>
        /// Gets or sets the updated by.
        /// </summary>
        /// <value>The updated by.</value>
        public int? UpdatedBy { get; set; }
        /// <summary>
        /// Gets or sets the CustomerId).
        /// </summary>
        /// <value>The CustomerId).</value>
        public int? CustomerId { get; set; }
    }
}
