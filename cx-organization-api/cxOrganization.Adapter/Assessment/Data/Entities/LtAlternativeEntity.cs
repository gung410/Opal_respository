namespace cxOrganization.Adapter.Assessment.Data.Entities
{
    public class LtAlternativeEntity
    {
        /// <summary>
        /// Gets or sets the language identifier.
        /// </summary>
        /// <value>The language identifier.</value>
        public int LanguageID { get; set; }
        /// <summary>
        /// Gets or sets the alternative identifier.
        /// </summary>
        /// <value>The alternative identifier.</value>
        public int AlternativeID { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        /// <value>The label.</value>
        public string Label { get; set; }
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }
        /// <summary>
        /// Gets or sets the alternative.
        /// </summary>
        /// <value>The alternative.</value>
        public virtual AlternativeEntity Alternative { get; set; }
    }
}
