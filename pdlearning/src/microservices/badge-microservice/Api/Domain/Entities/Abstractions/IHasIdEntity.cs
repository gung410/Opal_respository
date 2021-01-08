using System;

namespace Microservice.Badge.Domain.Entities
{
    public interface IHasIdEntity
    {
        public Guid Id { get; init; }
    }
}
