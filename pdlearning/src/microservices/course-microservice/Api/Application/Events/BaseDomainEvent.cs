using System;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Events
{
    public abstract class BaseDomainEvent<TAction> : BaseThunderEvent where TAction : Enum
    {
        public TAction Action { get; set; }
    }
}
