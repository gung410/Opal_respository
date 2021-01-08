using cxOrganization.Domain.Entities.Login;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace cxOrganization.Domain.Entities
{
    /// <summary>
    /// Class LoginService.
    /// </summary>
    [Serializable]
    public class LoginServiceEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoginServiceEntity"/> class.
        /// </summary>
        public LoginServiceEntity()
        {
            LT_LoginServices = new List<LtLoginService>();
        }
        public int LoginServiceID { get; set; }
        public int? SiteID { get; set; }
        public LoginServiceType? LoginServiceType { get; set; }
        public string IconUrl { get; set; }
        public string Authority { get; set; }
        public string MetadataAddress { get; set; }
        public string RedirectUri { get; set; }
        public string SecondaryClaimType { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Scope { get; set; }
        public string ResponseType { get; set; }
        public string PrimaryClaimType { get; set; }
        public string PostLogoutUri { get; set; }
        public bool Disabled { get; set; }
        public DateTime CreatedDate { get; set; }
        public virtual ICollection<LtLoginService> LT_LoginServices { get; set; }
        /// <summary>
        /// Gets or sets the return URL external.
        /// </summary>
        /// <value>The return URL external.</value>
        [NotMapped]
        public string ReturnUrlExternal { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [is successfully login].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [is successfully login]; otherwise, <c>false</c>.
        /// </value>
        [NotMapped]
        public bool IsSuccessfullyLogin { get; set; }
        /// <summary>
        /// get username that logged in service successfully
        /// </summary>
        [NotMapped]
        public string UserName { get; set; }

    }
}

