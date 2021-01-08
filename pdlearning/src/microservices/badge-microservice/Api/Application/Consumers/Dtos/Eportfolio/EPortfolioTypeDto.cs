using System;

namespace Microservice.Badge.Application.Consumers.Dtos
{
    public class EPortfolioTypeDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }

        public bool IsDefault { get; set; }

        public int No { get; set; }
    }
}
