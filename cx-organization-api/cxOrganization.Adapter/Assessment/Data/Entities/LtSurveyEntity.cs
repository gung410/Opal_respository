
using System;

namespace cxOrganization.Adapter.Assessment.Data.Entities
{
    /// <summary>
    /// Class LT_Survey.
    /// </summary>
     [Serializable]
    public class LtSurveyEntity
    {
        /// <summary>
        /// Gets or sets the language identifier.
        /// </summary>
        /// <value>The language identifier.</value>
        public int LanguageID { get; set; }
        /// <summary>
        /// Gets or sets the survey identifier.
        /// </summary>
        /// <value>The survey identifier.</value>
        public int SurveyID { get; set; }
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
        /// <summary>
        /// Gets or sets the information.
        /// </summary>
        /// <value>The information.</value>
        public string Info { get; set; }
        /// <summary>
        /// Gets or sets the finish text.
        /// </summary>
        /// <value>The finish text.</value>
        public string FinishText { get; set; }
        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        /// <value>The display name.</value>
        public string DisplayName { get; set; }
        //public virtual Language Language { get; set; }
        /// <summary>
        /// Gets or sets the survey.
        /// </summary>
        /// <value>The survey.</value>
        public virtual SurveyEntity Survey { get; set; }
    }
}