using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using Thunder.Platform.Core.Domain.Entities;

namespace Conexus.Opal.AccessControl.Entities
{
    /// <summary>
    /// This entity is used to mapping between SAM and others.
    /// We will keep the origin column name such as HDID, ID (with uppercase D).
    /// </summary>
    public class HierarchyDepartment : Entity
    {
        [Column("HDID")]
        public int HierarchyDepartmentId { get; set; }

        [Column("DepartmentID")]
        public int DepartmentId { get; set; }

        [Column("ParentID")]
        public int? ParentId { get; set; }

        [MaxLength(1000)]
        public string Path { get; set; }

        public static Expression<Func<HierarchyDepartment, bool>> FromDepartmentId(int departmentId)
        {
            return x => x.DepartmentId == departmentId;
        }

        public static Expression<Func<HierarchyDepartment, bool>> FromSubsequentDepartment(string path)
        {
            return x => x.Path.StartsWith(path);
        }
    }
}
