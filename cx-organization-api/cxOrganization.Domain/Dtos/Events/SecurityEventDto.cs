using cxPlatform.Client.ConexusBase;

namespace cxEvent.Client
{
    public class SecurityEventDto : EventDtoBase
    {
        public SecurityEventDto()
        {
            UserIdentity = new IdentityBaseDto();
            DepartmentIdentity = new IdentityBaseDto();
        }

        /// <summary>
        /// IP number from client
        /// </summary>
        /// <value>The ip number.</value>
        public string IPNumber { get; set; }
        /// <summary>
        /// UserName
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// PasswordType
        /// </summary>
        public string PasswordType { get; set; }
        /// <summary>
        /// The login service identifier
        /// </summary>
        public int? LoginServiceId { get; set; }
        /// <summary>
        /// UrlLogon
        /// </summary>
        public string UrlLogon { get; set; }
        /// <summary>
        /// Reason
        /// </summary>
        public string Reason { get; set; }
    }
}
