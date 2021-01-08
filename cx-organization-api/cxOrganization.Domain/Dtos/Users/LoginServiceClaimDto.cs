
namespace cxOrganization.Domain.Dtos.Users
{
    /// <summary>
    /// A definition that contain claim value in login service
    /// </summary>
    public class LoginServiceClaimDto
    {
        /// <summary>
        /// Get or set login service id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Get or set claim value
        /// </summary>
        public string Value { get; set; }
    }
}
