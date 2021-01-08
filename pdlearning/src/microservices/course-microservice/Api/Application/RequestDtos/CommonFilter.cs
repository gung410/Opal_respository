using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microservice.Course.Application.Validators.FilterCondition;
using Microservice.Course.Common.Extensions;

namespace Microservice.Course.Application.RequestDtos
{
#pragma warning disable SA1402 // File may only contain a single type

    public interface IHasFilterByField
    {
        public string Field { get; set; }

        public bool IsValidFieldName(ImmutableHashSet<string> validFieldNames);
    }

    public abstract class FilterByField : IHasFilterByField
    {
        public string Field { get; set; }

        public virtual bool IsValidFieldName(ImmutableHashSet<string> validFieldNames)
        {
            return validFieldNames == null || validFieldNames.Contains(Field);
        }
    }

    public class CommonFilter
    {
        // These filters are used for filter some specific values of a field own by an entity
        // They come from model created by client. Therefore this property has to have get set. When used, it must be check null or not
        public List<ContainFilter> ContainFilters { get; set; }

        // These filters are used for filter values in a specific range of a field own by an entity
        // They come from model created by client. Therefore this property has to have get set. When used, it must be check null or not
        public List<FromToFilter> FromToFilters { get; set; }

        public bool ValidateWith(string entityName, IValidFilterCriteria filterCriteria)
        {
            if (filterCriteria?.ValidPropertyNames == null || !filterCriteria.ValidPropertyNames.ContainsKey(entityName))
            {
                return true;
            }

            var validPropertyNames = filterCriteria.ValidPropertyNames[entityName];
            var filters = new List<FilterByField>().Concat(ContainFilters.EmptyIfNull()).Concat(FromToFilters.EmptyIfNull());

            return filters.All(x => x.IsValidFieldName(validPropertyNames));
        }
    }

    public class ContainFilter : FilterByField
    {
        public string[] Values { get; set; }

        public bool NotContain { get; set; }
    }

    public class FromToFilter : FilterByField
    {
        public string FromValue { get; set; }

        public string ToValue { get; set; }

        public bool EqualFrom { get; set; }

        public bool EqualTo { get; set; }
    }

#pragma warning restore SA1402 // File may only contain a single type
}
