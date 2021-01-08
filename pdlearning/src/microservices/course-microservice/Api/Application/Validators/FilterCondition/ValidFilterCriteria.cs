using System.Collections.Generic;
using System.Collections.Immutable;

namespace Microservice.Course.Application.Validators.FilterCondition
{
    public class ValidFilterCriteria : IValidFilterCriteria
    {
        public ValidFilterCriteria(Dictionary<string, ImmutableHashSet<string>> validPropertyNames)
        {
            ValidPropertyNames = validPropertyNames;
        }

        public IReadOnlyDictionary<string, ImmutableHashSet<string>> ValidPropertyNames { get; }
    }
}
