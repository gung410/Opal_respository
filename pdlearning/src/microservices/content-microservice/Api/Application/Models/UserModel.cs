using System;
using Microservice.Content.Domain.Entities;

namespace Microservice.Content.Application.Models
{
    public class UserModel
    {
        public UserModel(WebinarUser entity)
        {
            Id = entity.Id;
            AvatarUrl = entity.AvatarUrl;
            FullName = entity.FullName();
        }

        public Guid Id { get; set; }

        public string FullName { get; set; }

        public string AvatarUrl { get; set; }
    }
}
