using System;
using Conexus.Opal.AccessControl.Entities;

namespace Microservice.Learner.Application.Models
{
    public class UserModel
    {
        public UserModel(UserEntity entity, bool isFollowing = false)
        {
            Id = entity.Id;
            OriginalUserId = entity.OriginalUserId;
            FirstName = entity.FirstName;
            LastName = entity.LastName;
            Email = entity.Email;
            DepartmentId = entity.DepartmentId;
            IsFollowing = isFollowing;
        }

        public Guid Id { get; set; }

        public int OriginalUserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public int DepartmentId { get; set; }

        public bool IsFollowing { get; set; }
    }
}
