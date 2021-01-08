using System.ComponentModel.DataAnnotations;
using Conexus.Opal.AccessControl.Entities;

namespace Microservice.Content.Domain.Entities
{
    public class WebinarUser : UserEntity
    {
        [MaxLength(500)]
        public string AvatarUrl { get; set; }
    }
}
