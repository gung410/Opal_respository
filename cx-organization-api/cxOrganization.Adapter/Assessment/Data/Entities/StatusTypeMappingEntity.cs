using System;

namespace cxOrganization.Adapter.Assessment.Data.Entities
{
    /// <summary>
    /// Class StatusTypeMapping.
    /// </summary>
    [Serializable]
    public class StatusTypeMappingEntity
    {
        /// <summary>
        /// Gets or sets the status type mapping identifier.
        /// </summary>
        /// <value>The status type mapping identifier.</value>
        public int StatusTypeMappingID { get; set; }
        /// <summary>
        /// Gets or sets the activity identifier.
        /// </summary>
        /// <value>The activity identifier.</value>
        public int ActivityID { get; set; }
        /// <summary>
        /// Gets or sets the customer identifier.
        /// </summary>
        /// <value>The customer identifier.</value>
        public int? CustomerID { get; set; }
        /// <summary>
        /// Gets or sets from status type identifier.
        /// </summary>
        /// <value>From status type identifier.</value>
        public int? FromStatusTypeID { get; set; }
        /// <summary>
        /// Gets or sets to status type identifier.
        /// </summary>
        /// <value>To status type identifier.</value>
        public int ToStatusTypeID { get; set; }

        /// <summary>
        /// Gets or sets to ShowActionInUI identifier
        /// </summary>
        public bool ShowActionInUI { get; set; }

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
        /// Gets or sets the settings.
        /// </summary>
        /// <value>The settings.</value>
        public string Settings { get; set; }
    }
}
