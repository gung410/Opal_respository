namespace cxOrganization.Domain.Dtos.Auth
{
    /// <summary>
    /// The access right dto.
    /// </summary>
    public class AccessRightDto
    {
        /// <summary>
        /// The identifier which currently is the MenuItemId.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The action of the access right which is unique in the access rights.
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// The type of the object which the access right representing for.
        /// </summary>
        public string ObjectType { get; set; }

        /// <summary>
        /// The module who is the owner of the access right.
        /// </summary>
        public string Module { get; set; }

        /// <summary>
        /// The granted type of the access right.
        /// </summary>
        public string GrantedType { get; set; }
    }
}
