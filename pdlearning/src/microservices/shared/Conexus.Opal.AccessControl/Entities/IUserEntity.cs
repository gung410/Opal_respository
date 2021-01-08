using System;
using Thunder.Platform.Core.Domain.Entities;

namespace Conexus.Opal.AccessControl.Entities
{
    public interface IUserEntity : IEntity<Guid>
    {
        int DepartmentId { get; set; }

        string Email { get; set; }

        string FirstName { get; set; }

        string LastName { get; set; }

        int OriginalUserId { get; set; }

        string FullName();
    }
}
