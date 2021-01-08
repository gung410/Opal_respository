using System.ComponentModel.DataAnnotations;

namespace Communication.Business.Models
{
    public class NotificationReadModel
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public string NotificationId { get; set; }
    }
}