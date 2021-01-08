using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace cxOrganization.Adapter.Assessment.Data.Entities
{
    /// <summary>
    /// Class Alternative.
    /// </summary>
    
    public class AlternativeEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AlternativeEntity"/> class.
        /// </summary>
        public AlternativeEntity()
        {
            LtAlternativeEntities = new List<LtAlternativeEntity>();
            IsVisible = true;
        }

        /// <summary>
        /// Gets or sets the alternative identifier.
        /// </summary>
        /// <value>The alternative identifier.</value>
        public int AlternativeID { get; set; }
        /// <summary>
        /// Gets or sets the scale identifier.
        /// </summary>
        /// <value>The scale identifier.</value>
        public int ScaleID { get; set; }
        /// <summary>
        /// Gets or sets the agid.
        /// </summary>
        /// <value>The agid.</value>
        public int? AGID { get; set; }
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public short Type { get; set; }
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
        /// Gets or sets the inverted value.
        /// </summary>
        /// <value>The inverted value.</value>
        public double InvertedValue { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="AlternativeEntity"/> is calculate.
        /// </summary>
        /// <value><c>true</c> if calculate; otherwise, <c>false</c>.</value>
        public bool Calc { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="AlternativeEntity"/> is sc.
        /// </summary>
        /// <value><c>true</c> if sc; otherwise, <c>false</c>.</value>
        public bool SC { get; set; }
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
        /// Gets or sets the format.
        /// </summary>
        /// <value>The format.</value>
        public string Format { get; set; }
        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        /// <value>The size.</value>
        public short Size { get; set; }
        /// <summary>
        /// Gets or sets the CSS class.
        /// </summary>
        /// <value>The CSS class.</value>
        public string CssClass { get; set; }
        /// <summary>
        /// Gets or sets the created.
        /// </summary>
        /// <value>The created.</value>
        public DateTime Created { get; set; }
        /// <summary>
        /// Gets or sets the default value.
        /// </summary>
        /// <value>The default value.</value>
        public string DefaultValue { get; set; }
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
        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        public string Width { get; set; }
        /// <summary>
        /// Gets or sets the parent identifier.
        /// </summary>
        /// <value>The parent identifier.</value>
        public int? ParentID { get; set; }
        /// <summary>
        /// Gets or sets the owner color identifier.
        /// </summary>
        /// <value>The owner color identifier.</value>
        public int? OwnerColorID { get; set; }
        /// <summary>
        /// Gets or sets the default type of the calculate.
        /// </summary>
        /// <value>The default type of the calculate.</value>
        public short DefaultCalcType { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [use encryption].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [use encryption]; otherwise, <c>false</c>.
        /// </value>
        public bool UseEncryption { get; set; }
        /// <summary>
        /// Gets or sets the scale.
        /// </summary>
        /// <value>The scale.</value>
        public virtual  ScaleEntity Scale { get; set; }
        public virtual ICollection<LtAlternativeEntity> LtAlternativeEntities { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is visible.
        /// </summary>
        /// <value><c>true</c> if this instance is visible; otherwise, <c>false</c>.</value>
        [NotMapped]
        public bool IsVisible { get; set; }
    }
}
