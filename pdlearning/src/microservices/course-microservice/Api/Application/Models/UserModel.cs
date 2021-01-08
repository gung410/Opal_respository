using System;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;

namespace Microservice.Course.Application.Models
{
    public class UserModel
    {
        public UserModel(CourseUser entity)
        {
            Id = entity.Id;
            OriginalUserId = entity.OriginalUserId;
            FirstName = entity.FirstName;
            LastName = entity.LastName;
            EmailAddress = entity.Email;
            entity.DepartmentId = DepartmentId;
            Status = entity.Status;
            AccountType = entity.AccountType;
            PrimaryApprovingOfficerId = entity.PrimaryApprovingOfficerId;
            AlternativeApprovingOfficerId = entity.AlternativeApprovingOfficerId;
            FullName = entity.FullName();
        }

        public Guid Id { get; set; }

        public int OriginalUserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string EmailAddress { get; set; }

        public int DepartmentId { get; set; }

        public CourseUserStatus Status { get; set; }

        public CourseUserAccountType AccountType { get; set; }

        public Guid PrimaryApprovingOfficerId { get; set; }

        public Guid? AlternativeApprovingOfficerId { get; set; }

        public string FullName { get; set; }
    }
}
