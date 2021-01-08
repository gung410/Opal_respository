using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace cxOrganization.Adapter.Assessment.Data.Entities
{
    /// <summary>
    /// Class Result_Answer.
    /// </summary>

    public partial class AnswerEntity
    {
        /// <summary>
        /// Gets or sets the answer identifier.
        /// </summary>
        /// <value>The answer identifier.</value>
        public long AnswerID { get; set; }
        /// <summary>
        /// Gets or sets the item identifier.
        /// </summary>
        /// <value>The item identifier.</value>
        public int? ItemId { get; set; }
        /// <summary>
        /// Gets or sets the result identifier.
        /// </summary>
        /// <value>The result identifier.</value>
        public long ResultID { get; set; }
        /// <summary>
        /// Gets or sets the question identifier.
        /// </summary>
        /// <value>The question identifier.</value>
        public int QuestionID { get; set; }
        /// <summary>
        /// Gets or sets the alternative identifier.
        /// </summary>
        /// <value>The alternative identifier.</value>
        public int AlternativeID { get; set; }
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public double Value { get; set; }
        /// <summary>
        /// Gets or sets the date value.
        /// </summary>
        /// <value>The date value.</value>
        public DateTime? DateValue { get; set; }

        /// <summary>
        /// Gets or sets the free.
        /// </summary>
        /// <value>The free.</value>
        public string Free { get; set; }
        /// <summary>
        /// Gets or sets the no.
        /// </summary>
        /// <value>The no.</value>
        public short No { get; set; }
        /// <summary>
        /// Gets or sets CustomerID.
        /// </summary>
        /// <value>
        /// The CustomerID.
        /// </value>
        public int? CustomerID { get; set; }
        /// <summary>
        /// Gets or sets LastUpdated.
        /// </summary>
        /// <value>The LastUpdated.</value>
        public DateTime? LastUpdated { get; set; }
        /// <summary>
        /// Gets or sets LastUpdatedBy.
        /// </summary>
        /// <value>
        /// The LastUpdatedBy.
        /// </value>
        public int? LastUpdatedBy { get; set; }
        /// <summary>
        /// Gets or sets the created.
        /// </summary>
        /// <value>
        /// The created.
        /// </value>
        public DateTime? Created { get; set; }

        /// <summary>
        /// Gets or sets the created by.
        /// </summary>
        /// <value>
        /// The created by.
        /// </value>
        public int? CreatedBy { get; set; }
        /// <summary>
        /// Gets or sets the result.
        /// </summary>
        /// <value>The result.</value>
        public virtual ResultEntity Result { get; set; }
        /// <summary>
        /// Gets or sets the result.
        /// </summary>
        /// <value>The result.</value>
        [NotMapped]
        public double CalcValue { get; set; }
     
    }
}

