using System;
using Thunder.Platform.Core.Timing;

namespace Microservice.Badge.Application.Consumers.Dtos
{
    public class ReflectionDto
    {
        public Guid Id { get; set; }

        public Guid? EPortfolioId { get; set; }

        public Guid EPortfolioTypeId { get; set; }

        public Guid UserId { get; set; }

        public string Content { get; set; }

        public DateTime RecordDate { get; set; }

        public DateTime Created { get; set; } = Clock.Now;

        public Guid? CreatedBy { get; set; }

        public DateTime Updated { get; set; } = Clock.Now;

        public Guid? UpdatedBy { get; set; }
    }
}
