using System;
using System.ComponentModel.DataAnnotations;

namespace Microservice.Calendar.Application.RequestDtos
{
    public class GetTeamMemberEventOverviewRequest
    {
        [Required]
        public DateTime? RangeStart { get; set; }

        [Required]
        public DateTime? RangeEnd { get; set; }
    }
}
