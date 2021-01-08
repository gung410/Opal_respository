using System.ComponentModel.DataAnnotations;
using cxPlatform.Client.ConexusBase;

namespace cxOrganization.Client.UserGroups
{
    public abstract class UserGroupDtoBase : ConexusBaseDto
    {
        /// <summary>
        /// The name of the user group
        /// </summary>
        [MaxLength(256, ErrorMessage = "Name max length is 256")]
        public string Name { get; set; }

        /// <summary>
        /// The type of the user group
        /// Currently 4 types is available: 
        /// 1. Default 
        /// 2. Historical
        /// 3. Connections
        /// 4. Events
        /// </summary>
        public GrouptypeEnum Type { get; set; }

        /// <summary>
        /// Description for user group
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The period of time that the user group is using on
        /// </summary>
        public PeriodDto Period { get; set; }

        /// <summary>
        /// Referrer Token of external system
        /// </summary>
        public string ReferrerToken { get; set; }
        /// <summary>
        /// ReferrerResource link to external instance
        /// </summary>
        public string ReferrerResource { get; set; }
        /// <summary>
        /// Archetype of external instance
        /// </summary>
        public int? ReferrerArchetypeId { get; set; }

        /// <summary>
        /// Get parent department for usergroup
        /// </summary>
        /// <returns></returns>
        public abstract int? GetParentDepartmentId();

        /// <summary>
        /// Set parent department for usergroup
        /// </summary>
        /// <param name="parentDepartmentId"></param>
        public abstract void SetParentDepartmentId(int? parentDepartmentId);

        /// <summary>
        /// Set parent user for usergroup
        /// </summary>
        /// <returns></returns>
        public abstract int? GetParentUserId();
    }
}
