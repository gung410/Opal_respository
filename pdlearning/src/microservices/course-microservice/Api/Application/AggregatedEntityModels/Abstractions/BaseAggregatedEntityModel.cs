using Thunder.Platform.Core.Domain.Entities;

#pragma warning disable SA1402 // File may only contain a single type
namespace Microservice.Course.Application.AggregatedEntityModels.Abstractions
{
    public abstract class BaseAggregatedEntityModel
    {
    }

    public abstract class BaseAggregatedEntityModel<TAssociatedEntity> where TAssociatedEntity : Entity
    {
        public abstract TAssociatedEntity ToAssociatedEntity();
    }
}
#pragma warning restore SA1402 // File may only contain a single type
