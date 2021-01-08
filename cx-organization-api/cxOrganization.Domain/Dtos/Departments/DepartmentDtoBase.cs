using System;
using cxPlatform.Client.ConexusBase;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace cxOrganization.Client.Departments
{
    [Serializable]
    public abstract class DepartmentDtoBase : ConexusBaseDto
    {
        public DepartmentDtoBase()
        {
            var comparer = StringComparer.OrdinalIgnoreCase;
            CustomData = new Dictionary<string, object>(comparer);
        }
        /// <summary>
        /// Parent department Id
        /// </summary>
        [Required]
        public int? ParentDepartmentId { get; set; }
        /// <summary>
        /// The name of the department 
        /// </summary>
        [Required]
        [MaxLength(256, ErrorMessage = "Name max length is 256")]
        public string Name { get; set; }
        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Tag
        /// </summary>
        public string Tag { get; set; }
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
        public int? LanguageId { get; set; }
        /// <summary>
        /// Numeric value of country code (example: 47)
        /// </summary>
        public int? CountryCode { get; set; }

        public IDictionary<string, dynamic> JsonDynamicAttributes { get; set; }
        [JsonExtensionData]
        public Dictionary<string, object> CustomData { get; set; }
    }
}
