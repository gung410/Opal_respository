using System;

namespace cxOrganization.Domain.Entities
{
    /// <summary>
    /// Class LoginService_User.
    /// </summary>
     [Serializable]
    public class LoginServiceUserEntity
    {
        /// <summary>
        /// Gets or sets the login service Identifier.
        /// </summary>
        /// <value>The login service Identifier.</value>
        public int LoginServiceId { get; set; }
        /// <summary>
        /// Gets or sets the user Identifier.
        /// </summary>
        /// <value>The user Identifier.</value>
        public int UserId { get; set; }
        /// <summary>
        /// Gets or sets the primary claim value.
        /// </summary>
        /// <value>The primary claim value.</value>
        public string PrimaryClaimValue { get; set; }
        /// <summary>
        /// Gets or sets the created.
        /// </summary>
        /// <value>The created.</value>
        public DateTime Created { get; set; }
      
        /// <summary>
        /// Gets or sets the LoginService 
        /// </summary>
        /// <value>The created.</value>
        public virtual LoginServiceEntity LoginService { get; set; }
        /// <summary>
        /// Gets or sets the User 
        /// </summary>
        /// <value>The created.</value>
        public virtual UserEntity User { get; set; }
    }
}
