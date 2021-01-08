using System;
using System.Collections.Generic;
using System.Linq;
using cxPlatform.Client.ConexusBase;

namespace cxOrganization.Domain.Extensions
{
    public static class ListExtension
    {
        public static bool IsNotNullOrEmpty<T>(this List<T> filterList)
        {
            return filterList != null && filterList.Count > 0;
        }

        public static bool IsNullOrEmpty<T>(this List<T> filterList)
        {
            return filterList == null || filterList.Count == 0;
        }

        public static IEnumerable<List<T>> Split<T>(this List<T> source, int size)
        {
            if (size <= 0) size = source.Count;
            for (int i = 0; i < source.Count; i += size)
            {
                yield return source.GetRange(i, Math.Min(size, source.Count - i));
            }
        }
        public static string ToEqualOrContainSqlExpression<T>(this List<T> values, string sqlField, bool putValueInQuote = false)
        {
            return ToSqlExpression(values, sqlField, "=", "IN", putValueInQuote);
        }
        public static string ToNotEqualOrNotContainSqlExpression<T>(this List<T> values, string sqlField, bool putValueInQuote = false)
        {
            return ToSqlExpression(values, sqlField, "!=", "NOT IN", putValueInQuote);
        }
        public static string ToSqlExpression<T>(this List<T> values, string sqlField, string singleValueOperator, string multiValueOperator, bool putValueInQuote = false)
        {
            if (values.IsNullOrEmpty()) return null;

            if (putValueInQuote)
            {
                var valuesWIthQuote = values.Select(v => $"'{v}'").ToList();
                if (valuesWIthQuote.Count == 1)
                {
                    return $"{sqlField} {singleValueOperator} {valuesWIthQuote[0]}";
                }
                return $"{sqlField} {multiValueOperator} ({string.Join(",", valuesWIthQuote)})";
            }

            if (values.Count == 1)
            {
                return $"{sqlField} {singleValueOperator} {values[0]}";
            }
            return $"{sqlField} {multiValueOperator} ({string.Join(",", values)})";

        }
        public static string ToEqualOrContainSqlExpression(this List<EntityStatusEnum> values, string sqlField, params EntityStatusEnum[] defaultEntityStatusEnums)
        {
            if (values.IsNullOrEmpty())
            {
                if (defaultEntityStatusEnums != null && defaultEntityStatusEnums.Length > 0)
                {
                    values = defaultEntityStatusEnums.ToList();
                }
                else
                {
                    return null;
                }
            }

            if (values.Contains(EntityStatusEnum.All)) return null;
            var entityStatusIds = values.Select(v => (int)v).ToList();
            if (entityStatusIds.Count == 1)
            {
                return $"{sqlField}={entityStatusIds[0]}";
            }
            return $"{sqlField} in ({string.Join(",", entityStatusIds)})";

        }
      
        public static string ToEqualOrContainSqlExpression(this List<ArchetypeEnum> values, string sqlField)
        {
            if (values.IsNullOrEmpty())
            {
                return null;
            }

            var archetypeIds = values.Select(v => (int)v).ToList();
            if (archetypeIds.Count == 1)
            {
                return $"{sqlField}={archetypeIds[0]}";
            }
            return $"{sqlField} in ({string.Join(",", archetypeIds)})";

        }
    }
}
