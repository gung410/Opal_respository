using System;
using Microservice.Badge.Domain.Enums;

namespace Microservice.Badge.Domain.ValueObjects
{
    public class EPortfolioInfo
    {
        public EPortfolioInfo(Guid id, Guid userId, EPortfolioVisibility visibility)
        {
            this.Id = id;
            this.UserId = userId;
            this.Visibility = visibility;
        }

        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public EPortfolioVisibility Visibility { get; set; }
    }
}
