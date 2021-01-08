using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Microservice.Calendar.Application.RequestDtos
{
    public class GetSharedTeamMemberEventsRequest
    {
        [Required]
        [FromRoute(Name = "memberId")]
        public Guid LearnerId { get; set; }

        [Required]
        public DateTime? RangeStart { get; set; }

        [Required]
        public DateTime? RangeEnd { get; set; }

        [Required]
        [FromRoute(Name = "accessShareId")]
        public Guid AccessShareId { get; set; }
    }
}
