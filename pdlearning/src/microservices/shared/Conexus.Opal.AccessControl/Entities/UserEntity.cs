using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using Thunder.Platform.Core.Domain.Entities;

namespace Conexus.Opal.AccessControl.Entities
{
    [Table("Users")]
    public class UserEntity : Entity, IUserEntity
    {
        /// <summary>
        /// Use to map user ID from original database.
        /// </summary>
        [Column("UserID")]
        public int OriginalUserId { get; set; }

        [MaxLength(100)]
        public string FirstName { get; set; }

        [MaxLength(100)]
        public string LastName { get; set; }

        [MaxLength(256)]
        public string Email { get; set; }

        [Column("DepartmentID")]
        public int DepartmentId { get; set; }

        public static Expression<Func<UserEntity, bool>> IsBelongToDepartment(int departmentId)
        {
            return x => x.DepartmentId == departmentId;
        }

        public static Expression<Func<UserEntity, bool>> IsBelongToDepartments(IEnumerable<int> departmentIds)
        {
            return x => departmentIds.Contains(x.DepartmentId);
        }

        public string FullName()
        {
            return (FirstName ?? string.Empty) + " " + (LastName ?? string.Empty);
        }
    }
}
