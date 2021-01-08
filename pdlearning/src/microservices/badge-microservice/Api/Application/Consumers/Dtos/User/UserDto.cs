using System;

namespace Microservice.Badge.Application.Consumers.Dtos
{
    public class UserDto
    {
        public int Id { get; set; }

        public Guid Guid { get; set; }

        public string Email { get; set; }
    }
}
