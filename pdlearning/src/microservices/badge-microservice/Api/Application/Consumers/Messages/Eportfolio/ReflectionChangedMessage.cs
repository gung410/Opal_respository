using System;
using Microservice.Badge.Application.Consumers.Dtos;

namespace Microservice.Badge.Application.Consumers.Messages
{
    public class ReflectionChangedMessage
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public ReflectionDto Reflection { get; set; }

        public EPortfolioDto EPortfolio { get; set; }
    }
}
