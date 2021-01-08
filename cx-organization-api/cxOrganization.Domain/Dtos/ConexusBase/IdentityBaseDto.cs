namespace cxPlatform.Client.ConexusBase
{
    public class IdentityBaseDto
    {
        /// <summary>
        /// The type of the DTO based on which concept its was used for
        /// </summary>
        public ArchetypeEnum Archetype { get; set; }
        /// <summary>
        /// The identifier of the DTO in database
        /// </summary>
        public long? Id { get; set; }
    }
}
