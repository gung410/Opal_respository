namespace cxOrganization.Adapter.Assessment.Data.Entities
{
    /// <summary>
    /// Class A_S.
    /// </summary>
    
    public class ActivityStatusTypeEntity
    {
        /// <summary>
        /// Gets or sets the asid.
        /// </summary>
        /// <value>The asid.</value>
        public int ASID { get; set; }
        /// <summary>
        /// Gets or sets the activity identifier.
        /// </summary>
        /// <value>The activity identifier.</value>
        public int ActivityID { get; set; }
        /// <summary>
        /// Gets or sets the status type identifier.
        /// </summary>
        /// <value>The status type identifier.</value>
        public int StatusTypeID { get; set; }
        /// <summary>
        /// Gets or sets the no.
        /// </summary>
        /// <value>The no.</value>
        public short No { get; set; }
        /// <summary>
        /// Gets or sets the type of the status.
        /// </summary>
        /// <value>The type of the status.</value>
    }
}