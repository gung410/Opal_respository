using System;
using System.Collections.Generic;

namespace cxOrganization.Adapter.Assessment.Data.Entities
{
    public class ScaleEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScaleEntity"/> class.
        /// </summary>
        public ScaleEntity()
        {
            Alternatives = new List<AlternativeEntity>();
        }

        /// <summary>
        /// Gets or sets the scale identifier.
        /// </summary>
        /// <value>The scale identifier.</value>
        public int ScaleID { get; set; }
        /// <summary>
        /// Gets or sets the activity identifier.
        /// </summary>
        /// <value>The activity identifier.</value>
        public int? ActivityID { get; set; }
        /// <summary>
        /// Gets or sets the minimum select.
        /// </summary>
        /// <value>The minimum select.</value>
        public short MinSelect { get; set; }
        /// <summary>
        /// Gets or sets the maximum select.
        /// </summary>
        /// <value>The maximum select.</value>
        public short MaxSelect { get; set; }
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public short Type { get; set; }
        /// <summary>
        /// Gets or sets the minimum value.
        /// </summary>
        /// <value>The minimum value.</value>
        public double MinValue { get; set; }
        /// <summary>
        /// Gets or sets the maximum value.
        /// </summary>
        /// <value>The maximum value.</value>
        public double MaxValue { get; set; }
        /// <summary>
        /// Gets or sets the created.
        /// </summary>
        /// <value>The created.</value>
        public DateTime Created { get; set; }
        /// <summary>
        /// Gets or sets the tag.
        /// </summary>
        /// <value>The tag.</value>
        public string Tag { get; set; }
        /// <summary>
        /// Gets or sets the ext identifier.
        /// </summary>
        /// <value>The ext identifier.</value>
        public string ExtID { get; set; }
        public virtual ICollection<AlternativeEntity> Alternatives { get; set; }
      
    }
}
