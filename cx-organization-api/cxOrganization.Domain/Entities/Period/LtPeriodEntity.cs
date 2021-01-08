using System;

namespace cxOrganization.Domain.Entities
{
    /// <summary>
    /// Class LT_Period.
    /// </summary>
     [Serializable]
    public class LtPeriodEntity
    {
        /// <summary>
        /// Gets or sets the language Identifier.
        /// </summary>
        /// <value>The language Identifier.</value>
        public int LanguageId { get; set; }
        /// <summary>
        /// Gets or sets the period Identifier.
        /// </summary>
        /// <value>The period Identifier.</value>
        public int PeriodId { get; set; }
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
        //public virtual Language Language { get; set; }
        /// <summary>
        /// Gets or sets the period.
        /// </summary>
        /// <value>The period.</value>
        public virtual PeriodEntity Period { get; set; }
    }
}