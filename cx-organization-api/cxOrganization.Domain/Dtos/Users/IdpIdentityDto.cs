using cxPlatform.Client.ConexusBase;

namespace cxOrganization.Domain.Dtos.Users
{
    public class IdpIdentityDto
    {
        public string Id { get; set; }
        /// <summary>
        /// The identifier of the owner who that the DTO belong to
        /// </summary>
        public int OwnerId { get; set; }
        /// <summary>
        /// The identifier of the customer that the DTO belong to
        /// </summary>
        public int CustomerId { get; set; }

        /// The type of the DTO based on which concept its was used for
        /// </summary>
        public ArchetypeEnum Archetype { get; set; }
        /// <summary>
        /// The identifier of the DTO in database
        /// </summary>
    }
}