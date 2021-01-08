using System.Collections.Generic;
using System.Collections.Immutable;

namespace Microservice.Course.Application.Validators.FilterCondition
{
    public interface IValidFilterCriteria
    {
        /* This dictionary contains key as entity name and values as property names of that entity which are allowed to filter.
        For example:
        new Dictionary<string, ImmutableHashSet<string>>
        {
            {
                nameof(CourseEntity),
                ImmutableHashSet.Create(
                    nameof(CourseEntity.CreatedBy),
                    nameof(CourseEntity.DepartmentId),
                    nameof(CourseEntity.CreatedDate),
                    nameof(CourseEntity.ChangedDate),
                    nameof(CourseEntity.Status),
                    nameof(CourseEntity.PDActivityType),
                    nameof(CourseEntity.CategoryIds),
                    nameof(CourseEntity.ServiceSchemeIds),
                    nameof(CourseEntity.DevelopmentalRoleIds),
                    nameof(CourseEntity.TeachingLevels),
                    nameof(CourseEntity.CourseLevel),
                    nameof(CourseEntity.SubjectAreaIds),
                    nameof(CourseEntity.LearningFrameworkIds),
                    nameof(CourseEntity.LearningDimensionIds),
                    nameof(CourseEntity.LearningAreaIds),
                    nameof(CourseEntity.LearningSubAreaIds))
            }
        }*/
        public IReadOnlyDictionary<string, ImmutableHashSet<string>> ValidPropertyNames { get; }
    }
}
