using System;
using System.Collections.Generic;

namespace cxOrganization.Adapter.Assessment.Data.Entities
{
    /// <summary>
    /// Class StatusType.
    /// </summary>
     [Serializable]
    public class StatusTypeEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StatusTypeEntity"/> class.
        /// </summary>
        public StatusTypeEntity()
        {
            LtStatusType = new List<LtStatusTypeEntity>();
        }

        /// <summary>
        /// Gets or sets the status type identifier.
        /// </summary>
        /// <value>The status type identifier.</value>
        public int StatusTypeID { get; set; }
        /// <summary>
        /// Gets or sets the owner identifier.
        /// </summary>
        /// <value>The owner identifier.</value>
        public int? OwnerID { get; set; }
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public int Type { get; set; }
        /// <summary>
        /// Gets or sets the no.
        /// </summary>
        /// <value>The no.</value>
        public short No { get; set; }
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public double Value { get; set; }
        /// <summary>
        /// Gets or sets the type of the l t_ status.
        /// </summary>
        /// <value>The type of the l t_ status.</value>
        public virtual ICollection<LtStatusTypeEntity> LtStatusType { get; set; }

        /// <summary>
        /// Gets or sets the no.
        /// </summary>
        /// <value>The no.</value>
        public string CodeName { get; set; }
    }
}
