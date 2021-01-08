using System;

namespace Microservice.Calendar.Application.Models
{
    public class UserAccessSharingModel
    {
        public Guid UserId { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public bool Shared { get; set; }
    }
}
