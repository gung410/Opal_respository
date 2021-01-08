namespace cxOrganization.Adapter.Assessment.Data.Entities
{
    public class LtActivityEntity
    {
        /// <summary>
        /// Gets or sets the language identifier.
        /// </summary>
        /// <value>The language identifier.</value>
        public int LanguageID { get; set; }
        /// <summary>
        /// Gets or sets the activity identifier.
        /// </summary>
        /// <value>The activity identifier.</value>
        public int ActivityID { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the information.
        /// </summary>
        /// <value>The information.</value>
        public string Info { get; set; }
        /// <summary>
        /// Gets or sets the start text.
        /// </summary>
        /// <value>The start text.</value>
        public string StartText { get; set; }
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }
        /// <summary>
        /// Gets or sets the name of the role.
        /// </summary>
        /// <value>The name of the role.</value>
        public string RoleName { get; set; }
        /// <summary>
        /// Gets or sets the name of the survey.
        /// </summary>
        /// <value>The name of the survey.</value>
        public string SurveyName { get; set; }
        /// <summary>
        /// Gets or sets the name of the batch.
        /// </summary>
        /// <value>The name of the batch.</value>
        public string BatchName { get; set; }
        /// <summary>
        /// Gets or sets the name of the batch.
        /// </summary>
        /// <value>The name of the batch.</value>
        public string DisplayName { get; set; }
        /// <summary>
        /// Gets or sets the name of the short name.
        /// </summary>
        /// <value>The short name of the activity.</value>
        public string ShortName { get; set; }
        /// <summary>
        /// Gets or sets the activity.
        /// </summary>
        /// <value>The activity.</value>
        public virtual ActivityEntity Activity { get; set; }
    }
}
