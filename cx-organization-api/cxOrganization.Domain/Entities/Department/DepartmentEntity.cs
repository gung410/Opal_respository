using System;
using cxPlatform.Core.Entities;
using System.Collections.Generic;

namespace cxOrganization.Domain.Entities
{
    /// <summary>
    /// Class Department.
    /// </summary>
    [Serializable]
    public class DepartmentEntity : EntityBase, IDynamicAttributes
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DepartmentEntity"/> class.
        /// </summary>
        public DepartmentEntity()
        {
            H_D = new List<HierarchyDepartmentEntity>();
            Users = new List<UserEntity>();
            DT_Ds = new List<DTDEntity>();
            DG_Ds = new List<DGDEntity>();
        }

        /// <summary>
        /// Gets or sets the department Identifier.
        /// </summary>
        /// <value>The department Identifier.</value>
        public int DepartmentId { get; set; }
        /// <summary>
        /// Gets or sets the language Identifier.
        /// </summary>
        /// <value>The language Identifier.</value>
        public int LanguageId { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }
        /// <summary>
        /// Gets or sets the adress.
        /// </summary>
        /// <value>The adress.</value>
        public string Adress { get; set; }
        /// <summary>
        /// Gets or sets the postal code.
        /// </summary>
        /// <value>The postal code.</value>
        public string PostalCode { get; set; }
        /// <summary>
        /// Gets or sets the city.
        /// </summary>
        /// <value>The city.</value>
        public string City { get; set; }
        /// <summary>
        /// Gets or sets the org no.
        /// </summary>
        /// <value>The org no.</value>
        public string OrgNo { get; set; }
        /// <summary>
        /// Gets or sets the created.
        /// </summary>
        /// <value>The created.</value>
        public DateTime Created { get; set; }
        /// <summary>
        /// Gets or sets the ext Identifier.
        /// </summary>
        /// <value>The ext Identifier.</value>
        public string ExtId { get; set; }
        /// <summary>
        /// Gets or sets the tag.
        /// </summary>
        /// <value>The tag.</value>
        public string Tag { get; set; }
        /// <summary>
        /// Gets or sets the locked.
        /// </summary>
        /// <value>The locked.</value>
        public short Locked { get; set; }
        /// <summary>
        /// Gets or sets the country code.
        /// </summary>
        /// <value>The country code.</value>
        public int CountryCode { get; set; }
        /// <summary>
        /// Gets or sets the last updated.
        /// </summary>
        /// <value>The last updated.</value>
        public DateTime LastUpdated { get; set; }
        /// <summary>
        /// Gets or sets last updated by.
        /// </summary>
        /// <value>The last updated by.</value>
        public int? LastUpdatedBy { get; set; }
        /// <summary>
        /// Gets or sets the last synchronized.
        /// </summary>
        /// <value>The last synchronized.</value>
        public DateTime LastSynchronized { get; set; }
        /// <summary>
        /// Gets or sets the archetype Identifier.
        /// </summary>
        /// <value>The archetype Identifier.</value>
        public int? ArchetypeId { get; set; }
        /// Get or set the dymanic attributes in json format
        /// </summary>
        public string DynamicAttributes { get; set; }
        /// <summary>
        /// Gets or sets the h_ d.
        /// </summary>
        /// <value>The h_ d.</value>
        public ICollection<HierarchyDepartmentEntity> H_D { get; set; }
        /// <summary>
        /// Gets or sets the user groups.
        /// </summary>
        /// <value>The user groups.</value>
        public ICollection<UserGroupEntity> UserGroups { get; set; }
        /// <summary>
        /// Gets or sets the users.
        /// </summary>
        /// <value>The users.</value>
        public ICollection<UserEntity> Users { get; set; }
        public ICollection<DTDEntity> DT_Ds { get; set; }
        public ICollection<DGDEntity> DG_Ds { get; set; }
    }
}
