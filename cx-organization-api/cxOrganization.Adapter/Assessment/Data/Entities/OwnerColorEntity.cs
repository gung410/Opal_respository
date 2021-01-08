using System;
using System.Collections.Generic;

namespace cxOrganization.Adapter.Assessment.Data.Entities
{
    /// <summary>
    /// Class OwnerColor.
    /// </summary>
    [Serializable]
    public class OwnerColorEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OwnerColor"/> class.
        /// </summary>
        public OwnerColorEntity()
        {
            LT_OwnerColor = new List<LtOwnerColorEntity>();
            LevelLimits = new List<LevelLimitEntity>();
        }

        /// <summary>
        /// Gets or sets the owner color identifier.
        /// </summary>
        /// <value>The owner color identifier.</value>
        public int OwnerColorId { get; set; }
        /// <summary>
        /// Gets or sets the theme identifier.
        /// </summary>
        /// <value>The theme identifier.</value>
        public int? ThemeID { get; set; }
        /// <summary>
        /// Gets or sets the owner identifier.
        /// </summary>
        /// <value>The owner identifier.</value>
        public int? OwnerID { get; set; }
        /// <summary>
        /// Gets or sets the customer identifier.
        /// </summary>
        /// <value>The customer identifier.</value>
        public int? CustomerID { get; set; }
        /// <summary>
        /// Gets or sets the name of the friendly.
        /// </summary>
        /// <value>The name of the friendly.</value>
        public string FriendlyName { get; set; }
        /// <summary>
        /// Gets or sets the color of the fill.
        /// </summary>
        /// <value>The color of the fill.</value>
        public string FillColor { get; set; }
        /// <summary>
        /// Gets or sets the color of the text.
        /// </summary>
        /// <value>The color of the text.</value>
        public string TextColor { get; set; }
        /// <summary>
        /// Gets or sets the color of the border.
        /// </summary>
        /// <value>The color of the border.</value>
        public string BorderColor { get; set; }
        /// <summary>
        /// Gets or sets the no.
        /// </summary>
        /// <value>The no.</value>
        public int No { get; set; }
        /// <summary>
        /// Gets or sets the created.
        /// </summary>
        /// <value>The created.</value>
        public DateTime Created { get; set; }
        /// <summary>
        /// Gets or sets the color of the l t_ owner.
        /// </summary>
        /// <value>The color of the l t_ owner.</value>
        public virtual ICollection<LtOwnerColorEntity> LT_OwnerColor { get; set; }
        /// <summary>
        /// Gets or sets the color of the l t_ owner.
        /// </summary>
        /// <value>The color of the l t_ owner.</value>
        public virtual ICollection<LevelLimitEntity> LevelLimits { get; set; }
    }
}
