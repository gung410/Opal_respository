using System;
using System.ComponentModel.DataAnnotations;
using cxPlatform.Client.ConexusBase;

namespace cxOrganization.Client.Customers
{
    public class CustomerDto : ConexusBaseDto
    {
        /// <summary>
        /// The name of the customer
        /// </summary>
        [Required]
        [MaxLength(256, ErrorMessage = "Name max length is 256")]
        public string Name { get; set; }
        /// <summary>
        /// List of examples of languages:
        /// 1	nb-NO
        /// 2	en-US
        /// 3	DE
        /// 4	SA
        /// 5	AR
        /// 6	nn-NO
        /// 10	da-DK
        /// </summary>
        public int LanguageId { get; set; }
        /// <summary>
        /// Root menu ID
        /// </summary>
        public int RootMenuId { get; set; }
        /// <summary>
        /// Code name
        /// </summary>
        [MaxLength(256, ErrorMessage = "Code Name max length is 256")]
        public string CodeName { get; set; }
        /// <summary>
        /// Logo URL
        /// </summary>
        public string LogoUrl { get; set; }
        /// <summary>
        /// Css variables
        /// </summary>
        public string CssVariables { get; set; }
        public DateTime Created { get; set; }
        /// <summary>
        /// Status
        /// </summary>
        public short Status { get; set; }
        /// <summary>
        /// Has user integration
        /// </summary>
        public bool HasUserIntegration { get; set; }
        /// <summary>
        /// External ID
        /// </summary>
        [MaxLength(64, ErrorMessage = "ExtId max length is 64")]
        public string ExtId { get; set; }
    }
}
