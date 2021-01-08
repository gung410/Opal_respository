using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace Communication.Business.Models
{
 
    public class SubscribeNotificationModel
    {
        [Required]
        public ISet<string> UserIds { get; set; }
        [Required]
        public ISet<string> TopicNames { get; set; }
    }
}