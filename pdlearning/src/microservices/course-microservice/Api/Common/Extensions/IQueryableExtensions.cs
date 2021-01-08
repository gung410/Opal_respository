using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;

namespace Microservice.Course.Common.Extensions
{
    public static class IQueryableExtensions
    {
        public static IQueryable<TEntity> BuildContainFilter<TEntity>(
            this IQueryable<TEntity> query,
            string field,
            string[] values,
            Dictionary<string, string[]> notApplicableMapping,
            bool notContain = false)
            where TEntity : class
        {
            return BuildContainFilter(query, field, values, notApplicableMapping, notContain, p => p);
        }

        public static IQueryable<TEntity> BuildContainFilter<TEntity>(
            this IQueryable<TEntity> query,
            string field,
            string[] values,
            Dictionary<string, string[]> notApplicableMapping,
            bool notContain,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> complexTypeFieldFilter)
            where TEntity : class
        {
            if (values == null || !values.Any())
            {
                return query;
            }

            var propertyType = typeof(TEntity).GetProperty(field)?.PropertyType ?? throw new NotSupportedException($"Property '{field}' not found.");

            if (propertyType == typeof(string))
            {
                return BuildContainStringQuery(query, field, values, notApplicableMapping, notContain);
            }

            if (propertyType == typeof(char) || propertyType == typeof(char?))
            {
                return BuildContainValueQuery(query, propertyType, field, values, char.Parse, notApplicableMapping, notContain);
            }

            if (propertyType == typeof(bool) || propertyType == typeof(bool?))
            {
                return BuildContainValueQuery(query, propertyType, field, values, bool.Parse, notApplicableMapping, notContain);
            }

            if (propertyType == typeof(short) || propertyType == typeof(short?))
            {
                return BuildContainValueQuery(query, propertyType, field, values, short.Parse, notApplicableMapping, notContain);
            }

            if (propertyType == typeof(int) || propertyType == typeof(int?))
            {
                return BuildContainValueQuery(query, propertyType, field, values, int.Parse, notApplicableMapping, notContain);
            }

            if (propertyType == typeof(long) || propertyType == typeof(long?))
            {
                return BuildContainValueQuery(query, propertyType, field, values, long.Parse, notApplicableMapping, notContain);
            }

            if (propertyType == typeof(float) || propertyType == typeof(float?))
            {
                return BuildContainValueQuery(query, propertyType, field, values, float.Parse, notApplicableMapping, notContain);
            }

            if (propertyType == typeof(double) || propertyType == typeof(double?))
            {
                return BuildContainValueQuery(query, propertyType, field, values, double.Parse, notApplicableMapping, notContain);
            }

            if (propertyType == typeof(decimal) || propertyType == typeof(decimal?))
            {
                return BuildContainValueQuery(query, propertyType, field, values, decimal.Parse, notApplicableMapping, notContain);
            }

            if (propertyType == typeof(Guid) || propertyType == typeof(Guid?))
            {
                return BuildContainValueQuery(query, propertyType, field, values, Guid.Parse, notApplicableMapping, notContain);
            }

            var nullableUnderlyingEnumType = Nullable.GetUnderlyingType(propertyType);
            var isNullableEnum = nullableUnderlyingEnumType != null && nullableUnderlyingEnumType.IsEnum;
            if (propertyType.IsEnum || isNullableEnum)
            {
                var method = typeof(IQueryableExtensions).GetMethod(
                    isNullableEnum ? nameof(BuildContainNullableEnumQuery) : nameof(BuildContainEnumQuery),
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static);

                var generic = method?.MakeGenericMethod(typeof(TEntity), isNullableEnum ? nullableUnderlyingEnumType : propertyType);
                return (IQueryable<TEntity>)generic?.Invoke(null, new object[] { query, field, values, notApplicableMapping, notContain });
            }

            if (complexTypeFieldFilter != null)
            {
                return complexTypeFieldFilter(query);
            }

            throw new NotSupportedException($"Property type '{propertyType}' is not supported.");
        }

        /// <summary>
        /// Build contain filter support complex field type.
        /// </summary>
        /// <typeparam name="TEntity">Entity type.</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="field">Field name to filter.</param>
        /// <param name="values">The field value need to be existed in this values list.</param>
        /// <param name="notApplicableMapping">Support filter data is 'Not Applicable'.</param>
        /// <param name="notContain">Set true to negative the filter condition, which is the field value not existed in the values list.</param>
        /// <param name="complexTypeFieldFilter">The function to filter manually when the field name is matched in values of TComplexTypeFieldNameType enum.</param>
        /// <returns>The filtered query.</returns>
        public static IQueryable<TEntity> BuildContainFilter<TEntity>(
            this IQueryable<TEntity> query,
            string field,
            string[] values,
            Dictionary<string, string[]> notApplicableMapping,
            bool notContain,
            Func<string[], string, IQueryable<TEntity>, IQueryable<TEntity>> complexTypeFieldFilter)
            where TEntity : class
        {
            return BuildContainFilter(query, field, values, notApplicableMapping, notContain, currentQuery => complexTypeFieldFilter(values, field, currentQuery));
        }

        public static IQueryable<TEntity> BuildFromToFilter<TEntity>(
            this IQueryable<TEntity> query,
            string field,
            string fromValue,
            string toValue,
            bool equalFrom = false,
            bool equalTo = false)
            where TEntity : class
        {
            var propertyInfo = typeof(TEntity).GetProperty(field);
            if (propertyInfo == null)
            {
                throw new NotSupportedException($"Property '{field}' not found.");
            }

            var propertyType = typeof(TEntity).GetProperty(field)?.PropertyType ?? throw new NotSupportedException($"Property '{field}' not found.");

            if (propertyType == typeof(short) || propertyType == typeof(short?))
            {
                return BuildFromToQuery(query, field, fromValue, toValue, equalFrom, equalTo, short.Parse);
            }

            if (propertyType == typeof(int) || propertyType == typeof(int?))
            {
                return BuildFromToQuery(query, field, fromValue, toValue, equalFrom, equalTo, int.Parse);
            }

            if (propertyType == typeof(long) || propertyType == typeof(long?))
            {
                return BuildFromToQuery(query, field, fromValue, toValue, equalFrom, equalTo, long.Parse);
            }

            if (propertyType == typeof(float) || propertyType == typeof(float?))
            {
                return BuildFromToQuery(query, field, fromValue, toValue, equalFrom, equalTo, float.Parse);
            }

            if (propertyType == typeof(double) || propertyType == typeof(double?))
            {
                return BuildFromToQuery(query, field, fromValue, toValue, equalFrom, equalTo, double.Parse);
            }

            if (propertyType == typeof(decimal) || propertyType == typeof(decimal?))
            {
                return BuildFromToQuery(query, field, fromValue, toValue, equalFrom, equalTo, decimal.Parse);
            }

            if (propertyType == typeof(DateTime) || propertyType == typeof(DateTime?))
            {
                return BuildFromToQuery(query, field, fromValue, toValue, equalFrom, equalTo, DateTime.Parse);
            }

            throw new NotSupportedException($"Property type '{propertyType}' is not supported.");
        }

        private static IQueryable<TEntity> BuildContainEnumQuery<TEntity, TEnum>(
            this IQueryable<TEntity> query,
            string field,
            string[] values,
            Dictionary<string, string[]> notApplicableMapping,
            bool notContain = false)
            where TEntity : class
            where TEnum : struct
        {
            var s = values.Select(p => (TEnum)Enum.Parse(typeof(TEnum), p)).ToList();
            return BuildContainQuery(query, field, s, notApplicableMapping, notContain);
        }

        private static IQueryable<TEntity> BuildContainNullableEnumQuery<TEntity, TEnum>(
            this IQueryable<TEntity> query,
            string field,
            string[] values,
            Dictionary<string, string[]> notApplicableMapping,
            bool notContain = false)
            where TEntity : class
            where TEnum : struct
        {
            var s = values.Select(p => (TEnum?)Enum.Parse(typeof(TEnum), p)).ToList();
            return BuildContainQuery(query, field, s, notApplicableMapping, notContain);
        }

        private static IQueryable<TEntity> BuildContainStringQuery<TEntity>(
            this IQueryable<TEntity> query,
            string field,
            string[] values,
            Dictionary<string, string[]> notApplicableMapping,
            bool notContain = false)
            where TEntity : class
        {
            return BuildContainQuery(query, field, values.ToList(), notApplicableMapping, notContain);
        }

        private static IQueryable<TEntity> BuildContainValueQuery<TEntity, T>(
            this IQueryable<TEntity> query,
            Type propertyType,
            string field,
            string[] values,
            Func<string, T> parseFunc,
            Dictionary<string, string[]> notApplicableMapping,
            bool notContain = false)
            where TEntity : class
            where T : struct
        {
            if (propertyType == typeof(T))
            {
                return BuildContainQuery(query, field, values.Select(parseFunc).ToList(), notApplicableMapping, notContain);
            }

            return BuildContainQuery(query, field, values.Select(p => p != null ? (T?)parseFunc(p) : null).ToList(), notApplicableMapping, notContain);
        }

        private static IQueryable<TEntity> BuildContainQuery<TEntity, T>(
            this IQueryable<TEntity> query,
            string field,
            List<T> values,
            Dictionary<string, string[]> notApplicableMapping,
            bool notContain = false)
            where TEntity : class
        {
            string[] notApplicableItems = null;
            if (notApplicableMapping != null && notApplicableMapping.ContainsKey(field))
            {
                notApplicableItems = notApplicableMapping[field];
            }

            var notContainStr = notContain ? "!" : string.Empty;

            var predicate = $"{notContainStr}@0.Contains({field})";

            if (notApplicableItems != null)
            {
                return query.Where($"{predicate} OR {notContainStr}@1.Contains({field})", values, notApplicableItems);
            }

            return query.Where($"{predicate}", values);
        }

        private static IQueryable<TEntity> BuildFromToQuery<TEntity, T>(
            this IQueryable<TEntity> query,
            string field,
            string fromValue,
            string toValue,
            bool equalFrom,
            bool equalTo,
            Func<string, T> conversionFunc)
            where TEntity : class
        {
            var queryStrFrom = !string.IsNullOrWhiteSpace(fromValue) ? $"{field} >{(equalFrom ? "=" : string.Empty)} @0" : string.Empty;
            query = !string.IsNullOrWhiteSpace(queryStrFrom) ? query.Where(queryStrFrom, conversionFunc(fromValue)) : query;
            var queryStrTo = !string.IsNullOrWhiteSpace(toValue) ? $"{field} <{(equalTo ? "=" : string.Empty)} @0" : string.Empty;
            query = !string.IsNullOrWhiteSpace(queryStrTo) ? query.Where(queryStrTo, conversionFunc(toValue)) : query;
            return query;
        }
    }
}
