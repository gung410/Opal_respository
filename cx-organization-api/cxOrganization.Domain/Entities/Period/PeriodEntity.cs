using System;
using System.Collections.Generic;

namespace cxOrganization.Domain.Entities
{
    /// <summary>
    /// Class Period.
    /// </summary>
     [Serializable]
    public class PeriodEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Period"/> class.
        /// </summary>
        public PeriodEntity()
        {
            LtPeriods = new List<LtPeriodEntity>();
        }

        /// <summary>
        /// Gets or sets the period Identifier.
        /// </summary>
        /// <value>The period Identifier.</value>
        public int PeriodId { get; set; }
        /// <summary>
        /// Gets or sets the ownerId).
        /// </summary>
        /// <value>The ownerId).</value>
        public int OwnerId { get; set; }
        /// <summary>
        /// Gets or sets the created.
        /// </summary>
        /// <value>The created.</value>
        public DateTime Created { get; set; }
        /// <summary>
        /// Gets or sets the no.
        /// </summary>
        /// <value>The no.</value>
        public short No { get; set; }
        /// <summary>
        /// Gets or sets the startdate.
        /// </summary>
        /// <value>The startdate.</value>
        public DateTime? Startdate { get; set; }
        /// <summary>
        /// Gets or sets the enddate.
        /// </summary>
        /// <value>The enddate.</value>
        public DateTime? Enddate { get; set; }
        /// <summary>
        /// Gets or sets the ext Identifier.
        /// </summary>
        /// <value>The ext Identifier.</value>
        public string ExtId { get; set; }
        /// <summary>
        /// Gets or sets the l t_ period.
        /// </summary>
        /// <value>The l t_ period.</value>
        public virtual ICollection<LtPeriodEntity> LtPeriods { get; set; }
    }
}