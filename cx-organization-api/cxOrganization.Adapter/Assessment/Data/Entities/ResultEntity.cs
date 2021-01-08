using System;
using System.Collections.Generic;

namespace cxOrganization.Adapter.Assessment.Data.Entities
{
    /// <summary>
    /// Class Result_Result.
    /// </summary>
    
    public class ResultEntity  
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResultEntity"/> class.
        /// </summary>
        public ResultEntity()
        {
            Email = "";
            ResultKey = "";
            Answers = new List<AnswerEntity>();
        }

        /// <summary>
        /// Gets or sets the result identifier.
        /// </summary>
        /// <value>The result identifier.</value>
        public long ResultID { get; set; }
        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        /// <value>The start date.</value>
        public DateTime? StartDate { get; set; }
        /// <summary>
        /// Gets or sets the end date.
        /// </summary>
        /// <value>The end date.</value>
        public DateTime? EndDate { get; set; }
        /// <summary>
        /// Gets or sets the role identifier.
        /// </summary>
        /// <value>The role identifier.</value>
        public int RoleID { get; set; }
        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>The user identifier.</value>
        public int? UserID { get; set; }
        /// <summary>
        /// Gets or sets the user group identifier.
        /// </summary>
        /// <value>The user group identifier.</value>
        public int? UserGroupID { get; set; }
        /// <summary>
        /// Gets or sets the survey identifier.
        /// </summary>
        /// <value>The survey identifier.</value>
        public int SurveyID { get; set; }
        /// <summary>
        /// Gets or sets the batch identifier.
        /// </summary>
        /// <value>The batch identifier.</value>
        public int BatchID { get; set; }
        /// <summary>
        /// Gets or sets the language identifier.
        /// </summary>
        /// <value>The language identifier.</value>
        public int LanguageID { get; set; }
        /// <summary>
        /// Gets or sets the page no.
        /// </summary>
        /// <value>The page no.</value>
        public short PageNo { get; set; }
        /// <summary>
        /// Gets or sets the department identifier.
        /// </summary>
        /// <value>The department identifier.</value>
        public int DepartmentID { get; set; }
        /// <summary>
        /// Gets or sets the anonymous.
        /// </summary>
        /// <value>The anonymous.</value>
        public short Anonymous { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [show back].
        /// </summary>
        /// <value><c>true</c> if [show back]; otherwise, <c>false</c>.</value>
        public bool ShowBack { get; set; }
        /// <summary>
        /// Gets or sets the result key.
        /// </summary>
        /// <value>The result key.</value>
        public string ResultKey { get; set; }
        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>The email.</value>
        public string Email { get; set; }
        /// <summary>
        /// Gets or sets the CHK.
        /// </summary>
        /// <value>The CHK.</value>
        public byte[] chk { get; set; }
        /// <summary>
        /// Gets or sets the created.
        /// </summary>
        /// <value>The created.</value>
        public DateTime Created { get; set; }
        /// <summary>
        /// Gets or sets the created by.
        /// </summary>
        /// <value>
        /// The created by.
        /// </value>
        public int? CreatedBy { get; set; }
        /// <summary>
        /// Gets or sets the last updated.
        /// </summary>
        /// <value>The last updated.</value>
        public DateTime LastUpdated { get; set; }
        /// <summary>
        /// Gets or sets the last updated by.
        /// </summary>
        /// <value>The last updated by.</value>
        public int LastUpdatedBy { get; set; }
        /// <summary>
        /// Gets or sets the status type identifier.
        /// </summary>
        /// <value>The status type identifier.</value>
        public int? StatusTypeID { get; set; }
        /// <summary>
        /// Gets or sets the valid from.
        /// </summary>
        /// <value>
        /// The valid from.
        /// </value>
        public DateTime? validFrom { get; set; }
        /// <summary>
        /// Gets or sets the valid to.
        /// </summary>
        /// <value>
        /// The valid to.
        /// </value>
        public DateTime? ValidTo { get; set; }
        /// <summary>
        /// Gets or sets the due date.
        /// </summary>
        /// <value>
        /// The due date.
        /// </value>
        public DateTime? DueDate { get; set; }
        /// <summary>
        /// Gets or sets the parent result identifier.
        /// </summary>
        /// <value>
        /// The parent result identifier.
        /// </value>
        public long? ParentResultId { get; set; }
        /// <summary>
        /// Gets or sets the parent result survey identifier.
        /// </summary>
        /// <value>
        /// The parent result survey identifier.
        /// </value>
        public int? ParentResultSurveyId { get; set; }
        /// <summary>
        /// Gets or sets CustomerID.
        /// </summary>
        /// <value>
        /// The CustomerID.
        /// </value>
        public int? CustomerID { get; set; }
        /// <summary>
        /// Gets or sets the deleted.
        /// </summary>
        /// <value>The deleted.</value>
        public DateTime? Deleted { get; set; }
        /// <summary>
        /// Gets or sets the status identifier.
        /// </summary>
        /// <value>The last updated by.</value>
        public int? EntityStatusId { get; set; }
        /// <summary>
        /// Gets or sets the status reason identifier.
        /// </summary>
        /// <value>The last updated by.</value>
        public int? EntityStatusReasonId { get; set; }
        /// <summary>
        /// Gets or sets the answers.
        /// </summary>
        /// <value>The answers.</value>
        public virtual ICollection<AnswerEntity> Answers { get; set; }
    }
}

