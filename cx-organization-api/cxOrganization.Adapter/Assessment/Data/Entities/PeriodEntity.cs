using System;
using System.Collections.Generic;

namespace cxOrganization.Adapter.Assessment.Data.Entities
{
    /// <summary>
    /// Class Period.
    /// </summary>
    
    public class PeriodEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PeriodEntity"/> class.
        /// </summary>
        public PeriodEntity()
        {
            Surveys = new List<SurveyEntity>();
        }

        /// <summary>
        /// Gets or sets the period identifier.
        /// </summary>
        /// <value>The period identifier.</value>
        public int PeriodID { get; set; }
        /// <summary>
        /// Gets or sets the ownerid.
        /// </summary>
        /// <value>The ownerid.</value>
        public int Ownerid { get; set; }
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
        public DateTime? StartDate { get; set; }
        /// <summary>
        /// Gets or sets the enddate.
        /// </summary>
        /// <value>The enddate.</value>
        public DateTime? EndDate { get; set; }
        /// <summary>
        /// Gets or sets the ext identifier.
        /// </summary>
        /// <value>The ext identifier.</value>
        public string ExtID { get; set; }
        /// <summary>
        /// Gets or sets the surveys.
        /// </summary>
        /// <value>The surveys.</value>
        public virtual ICollection<SurveyEntity> Surveys { get; set; }
    }
}