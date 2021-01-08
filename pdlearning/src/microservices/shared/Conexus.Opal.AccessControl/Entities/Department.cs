using System.ComponentModel.DataAnnotations.Schema;
using Thunder.Platform.Core.Domain.Entities;

namespace Conexus.Opal.AccessControl.Entities
{
    /// <summary>
    /// This entity is used to mapping between SAM and others.
    /// We will keep the origin column name such as HDID, ID (with uppercase D).
    /// </summary>
    public class Department : Entity
    {
        [Column("DepartmentID")]
        public int DepartmentId { get; set; }
    }
}
