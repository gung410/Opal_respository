using System.ComponentModel.DataAnnotations.Schema;
using Thunder.Platform.Core.Domain.Entities;

namespace Conexus.Opal.AccessControl.Entities
{
    /// <summary>
    /// This entity is used to mapping between SAM and others.
    /// We will keep the origin column name such as HDID, ID (with uppercase D).
    /// </summary>
    public class DepartmentType : Entity
    {
        [Column("DepartmentTypeID")]
        public int DepartmentTypeId { get; set; }

        /// <summary>
        /// Fixed ID such as: dataowner, wing, division, branch, cluster, school, ministry, etc.
        /// Use to check type of department.
        /// </summary>
        [Column("ExtID")]
        public string ExtId { get; set; }
    }
}
