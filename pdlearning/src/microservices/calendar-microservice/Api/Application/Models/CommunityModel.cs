using System;
using System.Collections.Generic;

namespace Microservice.Calendar.Application.Models
{
    public class CommunityModel
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public List<CommunityModel> SubCommunities { get; set; }
    }
}
