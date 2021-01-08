using System;
using System.Collections.Generic;

namespace cxOrganization.Adapter.Assessment.Data.Entities
{
    /// <summary>
    /// Class Survey.
    /// </summary>
    
    public class SurveyEntity 
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SurveyEntity"/> class.
        /// </summary>
        public SurveyEntity()
        {
        }

        /// <summary>
        /// Gets or sets the survey identifier.
        /// </summary>
        /// <value>The survey identifier.</value>
        public int SurveyID { get; set; }
        /// <summary>
        /// Gets or sets the activity identifier.
        /// </summary>
        /// <value>The activity identifier.</value>
        public int ActivityID { get; set; }
        /// <summary>
        /// Gets or sets the hierarchy identifier.
        /// </summary>
        /// <value>The hierarchy identifier.</value>
        public int? HierarchyID { get; set; }
        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        /// <value>The start date.</value>
        public DateTime StartDate { get; set; }
        /// <summary>
        /// Gets or sets the end date.
        /// </summary>
        /// <value>The end date.</value>
        public DateTime EndDate { get; set; }
        /// <summary>
        /// Gets or sets the anonymous.
        /// </summary>
        /// <value>The anonymous.</value>
        public short Anonymous { get; set; }
        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>The status.</value>
        public short Status { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [show back].
        /// </summary>
        /// <value><c>true</c> if [show back]; otherwise, <c>false</c>.</value>
        public bool ShowBack { get; set; }
        /// <summary>
        /// Gets or sets the language identifier.
        /// </summary>
        /// <value>The language identifier.</value>
        public int LanguageID { get; set; }
        /// <summary>
        /// Gets or sets the button placement.
        /// </summary>
        /// <value>The button placement.</value>
        public short ButtonPlacement { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [use page no].
        /// </summary>
        /// <value><c>true</c> if [use page no]; otherwise, <c>false</c>.</value>
        public bool UsePageNo { get; set; }
        /// <summary>
        /// Gets or sets the link URL.
        /// </summary>
        /// <value>The link URL.</value>
        public string LinkURL { get; set; }
        /// <summary>
        /// Gets or sets the finish URL.
        /// </summary>
        /// <value>The finish URL.</value>
        public string FinishURL { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [create result].
        /// </summary>
        /// <value><c>true</c> if [create result]; otherwise, <c>false</c>.</value>
        public bool CreateResult { get; set; }
        /// <summary>
        /// Gets or sets the report database.
        /// </summary>
        /// <value>The report database.</value>
        public string ReportDB { get; set; }
        /// <summary>
        /// Gets or sets the report server.
        /// </summary>
        /// <value>The report server.</value>
        public string ReportServer { get; set; }
        /// <summary>
        /// Gets or sets the style sheet.
        /// </summary>
        /// <value>The style sheet.</value>
        public string StyleSheet { get; set; }
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public short Type { get; set; }
        /// <summary>
        /// Gets or sets the created.
        /// </summary>
        /// <value>The created.</value>
        public DateTime Created { get; set; }
        /// <summary>
        /// Gets or sets the last processed.
        /// </summary>
        /// <value>The last processed.</value>
        public DateTime LastProcessed { get; set; }
        /// <summary>
        /// Gets or sets the re process olap.
        /// </summary>
        /// <value>The re process olap.</value>
        public short ReProcessOLAP { get; set; }
        /// <summary>
        /// Gets or sets the no.
        /// </summary>
        /// <value>The no.</value>
        public short No { get; set; }
        /// <summary>
        /// Gets or sets the olap server.
        /// </summary>
        /// <value>The olap server.</value>
        public string OLAPServer { get; set; }
        /// <summary>
        /// Gets or sets the olapdb.
        /// </summary>
        /// <value>The olapdb.</value>
        public string OLAPDB { get; set; }
        /// <summary>
        /// Gets or sets the period identifier.
        /// </summary>
        /// <value>The period identifier.</value>
        public int? PeriodID { get; set; }
        /// <summary>
        /// Gets or sets the process categorys.
        /// </summary>
        /// <value>The process categorys.</value>
        public short ProcessCategorys { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [delete result on user delete].
        /// </summary>
        /// <value><c>true</c> if [delete result on user delete]; otherwise, <c>false</c>.</value>
        public bool DeleteResultOnUserDelete { get; set; }
        /// <summary>
        /// Gets or sets last processed result id.
        /// </summary>
        /// <value>last processed result identifier.</value>
        public long? LastProcessedResultID { get; set; }
        /// <summary>
        /// Gets or sets Last processed answer id.
        /// </summary>
        /// <value>Last processed answer identifier.</value>
        public long? LastProcessedAnswerID { get; set; }
        /// <summary>
        /// Gets or sets ext id.
        /// </summary>
        /// <value>Ext id.</value>
        public string ExtId { get; set; }
        /// <summary>
        /// Gets or sets tag.
        /// </summary>
        /// <value>Tag.</value>
        public string Tag { get; set; }
        /// <summary>
        /// Survey Languages
        /// </summary>
        public virtual ICollection<LtSurveyEntity> LtSurvey { get; set; }
        /// <summary>
        /// survey period
        /// </summary>
        public virtual PeriodEntity Period { get; set; }
        /// <summary>
        /// reference activity
        /// </summary>
        public virtual ActivityEntity Activity{ get; set; }
    }
}
