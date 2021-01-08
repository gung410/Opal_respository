using System;
using Microservice.Course.Domain.Entities;

namespace Microservice.Course.Application.AggregatedEntityModels.Abstractions
{
    public interface IAggregatedContentEntityModel
    {
        Guid Id { get; }

        Guid ForTargetId { get; }

        Guid OwnerId { get; }

        Guid CourseId { get; }

        Guid? ClassRunId { get; }

        CourseUser Owner { get; }

        string Title { get; }

        string CombinedRichText();
    }
}
