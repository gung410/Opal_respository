using System;

namespace Microservice.Webinar.Application.Models
{
    public class UserModel
    {
        public Guid Id { get; set; }

        public int UserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FullName
        {
            get
            {
                return (FirstName ?? string.Empty) + " " + (LastName ?? string.Empty).Trim();
            }
        }
    }
}
