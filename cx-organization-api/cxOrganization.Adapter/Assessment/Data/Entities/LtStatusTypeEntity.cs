using System;

namespace cxOrganization.Adapter.Assessment.Data.Entities
{
    /// <summary>
    /// Class LT_StatusType.
    /// </summary>
     [Serializable]
    public class LtStatusTypeEntity
    {
        /// <summary>
        /// Gets or sets the language identifier.
        /// </summary>
        /// <value>The language identifier.</value>
        public int LanguageID { get; set; }
        /// <summary>
        /// Gets or sets the status type identifier.
        /// </summary>
        /// <value>The status type identifier.</value>
        public int StatusTypeID { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the default action name.
        /// </summary>
        /// <value>The default action name.</value>
        public string DefaultActionName { get; set; }
        /// <summary>
        /// Gets or sets the default action description.
        /// </summary>
        /// <value>The default action description.</value>
        public string DefaultActionDescription { get; set; }
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }
        //public virtual Language Language { get; set; }
        /// <summary>
        /// Gets or sets the type of the status.
        /// </summary>
        /// <value>The type of the status.</value>
        public virtual StatusTypeEntity StatusType { get; set; }
    }
}