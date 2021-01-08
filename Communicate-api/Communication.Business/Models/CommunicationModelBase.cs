using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Communication.Business.Models
{
    public class CommunicationModelBase
    {
        public string Body { get; set; }
        
        [Required]
        public string Subject { get; set; }

    }
}