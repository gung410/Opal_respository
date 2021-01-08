using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Extensions;
using cxPlatform.Core.Exceptions;

using LinqKit;

namespace cxOrganization.Domain.Repositories.QueryBuilders
{
    public static class QueryBuilderExtension
    {
        //Need to make sure the sign which is has longer length should be ordered first for processing string
        private static readonly Dictionary<ComparisonOperator, string> ComparisonOperatorSignMappings =
            new Dictionary<ComparisonOperator, string>
            {

                {ComparisonOperator.NotEqual, ComparisonOperatorSign.NotEqual},
                {ComparisonOperator.GreaterThanOrEqual,ComparisonOperatorSign.GreaterThanOrEqual},
                {ComparisonOperator.LessThanOrEqual,ComparisonOperatorSign.LessThanOrEqual},
                {ComparisonOperator.Equal, ComparisonOperatorSign.Equal},
                {ComparisonOperator.GreaterThan, ComparisonOperatorSign.GreaterThan},
                {ComparisonOperator.LessThan, ComparisonOperatorSign.LessThan}
            };

        public static IQueryable<T> FilterByJsonValue<T>(this IQueryable<T> source,
            List<string> jsonDynamicData) where T : IDynamicAttributes
        {
            //TODO: notice that (EfJsonExtensions.JsonValue(x.DynamicAttributes, path).CompareTo(value) could be invalid with number without leading zero

            source = source.FilterByJsonValue(jsonDynamicData,
                (path, comparisonOperator, value) =>
                {
                    if (comparisonOperator != ComparisonOperator.Equal)
                    {
                        throw new NotSupportedException($"Do not support filter on json attribute '{path}' with comparison operator '{comparisonOperator}'");
                    }
                    //TODO handle support jsonQuery for various Operator
                    return x => EfJsonQueryExtensions.JsonQuery(x.DynamicAttributes, path).Contains(value);
                },
                (path, comparisonOperator, value) =>
                {
                    switch (comparisonOperator)
                    {

                        case ComparisonOperator.Equal:
                        {
                            var isNull = value == "null";
                            if (isNull)
                            {
                                //JSOn Value could has no value or has value 'null'
                                return x => (EfJsonExtensions.JsonValue(x.DynamicAttributes, path) == null ||
                                             EfJsonExtensions.JsonValue(x.DynamicAttributes, path) == "null");

                            }

                            return x => EfJsonExtensions.JsonValue(x.DynamicAttributes, path) == value;

                        }
                        case ComparisonOperator.GreaterThan:
                        {

                            return x => (EfJsonExtensions.JsonValue(x.DynamicAttributes, path).CompareTo(value) > 0);

                        }
                        case ComparisonOperator.LessThan:
                        {

                            return x => (EfJsonExtensions.JsonValue(x.DynamicAttributes, path).CompareTo(value) < 0);

                        }
                        case ComparisonOperator.GreaterThanOrEqual:
                        {

                            return x => (EfJsonExtensions.JsonValue(x.DynamicAttributes, path).CompareTo(value) >= 0);

                        }
                        case ComparisonOperator.LessThanOrEqual:
                        {

                            return x => (EfJsonExtensions.JsonValue(x.DynamicAttributes, path).CompareTo(value) <= 0);

                        }
                        default: //NotEqual
                        {
                            var isNotNull = value == "null";
                            if (isNotNull)
                            {
                                return x => (EfJsonExtensions.JsonValue(x.DynamicAttributes, path) != null &&
                                             EfJsonExtensions.JsonValue(x.DynamicAttributes, path) != "null");

                            }

                            return x => EfJsonExtensions.JsonValue(x.DynamicAttributes, path) != value;
                        }
                    }

                },
                (jsonValues, comparisonOperator, path) =>
                {

                    var orIsNull = jsonValues.Contains("null");
                    var comparableValues = orIsNull ? jsonValues.Where(v => v != "null").ToList() : jsonValues;

                    if (comparableValues.Count == 1)
                    {
                        var value = comparableValues.First();
                        switch (comparisonOperator)
                        {

                            case ComparisonOperator.Equal:
                                {
                                    if (orIsNull)
                                    {
                                        //JSOn Value could has no value or has value 'null'
                                        return x => (EfJsonExtensions.JsonValue(x.DynamicAttributes, path) == null ||
                                                     EfJsonExtensions.JsonValue(x.DynamicAttributes, path) == "null" ||
                                                     EfJsonExtensions.JsonValue(x.DynamicAttributes, path) == value);

                                    }

                                    return x => EfJsonExtensions.JsonValue(x.DynamicAttributes, path) == value;

                                }
                            case ComparisonOperator.GreaterThan:
                                {
                                    if (orIsNull)
                                    {
                                        //JSOn Value could has no value or has value 'null'
                                        return x => (EfJsonExtensions.JsonValue(x.DynamicAttributes, path) == null ||
                                                     EfJsonExtensions.JsonValue(x.DynamicAttributes, path) == "null" ||
                                                     EfJsonExtensions.JsonValue(x.DynamicAttributes, path).CompareTo(value) > 0);

                                    }
                                    return x => (EfJsonExtensions.JsonValue(x.DynamicAttributes, path).CompareTo(value) > 0);

                                }
                            case ComparisonOperator.LessThan:
                                {
                                    if (orIsNull)
                                    {
                                        //JSOn Value could has no value or has value 'null'
                                        return x => (EfJsonExtensions.JsonValue(x.DynamicAttributes, path) == null ||
                                                     EfJsonExtensions.JsonValue(x.DynamicAttributes, path) == "null" ||
                                                     EfJsonExtensions.JsonValue(x.DynamicAttributes, path).CompareTo(value) < 0);

                                    }
                                    return x => (EfJsonExtensions.JsonValue(x.DynamicAttributes, path).CompareTo(value) < 0);

                                }
                            case ComparisonOperator.GreaterThanOrEqual:
                                {
                                    if (orIsNull)
                                    {
                                        //JSOn Value could has no value or has value 'null'
                                        return x => (EfJsonExtensions.JsonValue(x.DynamicAttributes, path) == null ||
                                                     EfJsonExtensions.JsonValue(x.DynamicAttributes, path) == "null" ||
                                                     EfJsonExtensions.JsonValue(x.DynamicAttributes, path).CompareTo(value) >= 0);

                                    }
                                    return x => (EfJsonExtensions.JsonValue(x.DynamicAttributes, path).CompareTo(value) >= 0);

                                }
                            case ComparisonOperator.LessThanOrEqual:
                                {

                                    if (orIsNull)
                                    {
                                        //JSOn Value could has no value or has value 'null'
                                        return x => (EfJsonExtensions.JsonValue(x.DynamicAttributes, path) == null ||
                                                     EfJsonExtensions.JsonValue(x.DynamicAttributes, path) == "null" ||
                                                     EfJsonExtensions.JsonValue(x.DynamicAttributes, path).CompareTo(value) <= 0);

                                    }
                                    return x => (EfJsonExtensions.JsonValue(x.DynamicAttributes, path).CompareTo(value) <= 0);

                                }
                            default: //NotEqual
                                {
                                    if (orIsNull)
                                    {
                                        //JSOn Value could has no value or has value 'null'
                                        return x => (EfJsonExtensions.JsonValue(x.DynamicAttributes, path) == null ||
                                                     EfJsonExtensions.JsonValue(x.DynamicAttributes, path) == "null" ||
                                                     EfJsonExtensions.JsonValue(x.DynamicAttributes, path) != value);

                                    }
                                    return x => EfJsonExtensions.JsonValue(x.DynamicAttributes, path) != value;
                                }
                        }
                    }
                    else
                    {
                        if (comparisonOperator == ComparisonOperator.Equal)
                        {
                            if (orIsNull)
                            {
                                return (x =>
                                        EfJsonExtensions.JsonValue(x.DynamicAttributes, path) == null
                                        || EfJsonExtensions.JsonValue(x.DynamicAttributes, path) == "null"
                                        || comparableValues.Contains(
                                            EfJsonExtensions.JsonValue(x.DynamicAttributes, path))
                                    );
                            }

                            return x => comparableValues.Contains(
                                EfJsonExtensions.JsonValue(x.DynamicAttributes, path));
                        }

                        if (comparisonOperator == ComparisonOperator.NotEqual)
                        {
                            if (orIsNull)
                            {
                                return (x =>
                                        EfJsonExtensions.JsonValue(x.DynamicAttributes, path) == null
                                        || EfJsonExtensions.JsonValue(x.DynamicAttributes, path) == "null"
                                        || !comparableValues.Contains(
                                            EfJsonExtensions.JsonValue(x.DynamicAttributes, path))
                                    );
                            }

                            return x => !comparableValues.Contains(
                                EfJsonExtensions.JsonValue(x.DynamicAttributes, path));
                        }

                        throw new NotSupportedException(
                            $"Do not support filter on json attribute '{path}' with comparison operator '{comparisonOperator}' against multiple values");

                    }

                });

            return source;
        }
        public static IQueryable<T> FilterByJsonValue<T>(this IQueryable<T> source, 
            List<string> jsonDynamicData,
            Func<string, ComparisonOperator, string, Expression<Func<T,bool>>> jsonQuery,
            Func<string, ComparisonOperator, string, Expression<Func<T, bool>>> jsonValue,
            Func<List<string>, ComparisonOperator, string, Expression<Func<T, bool>>> jsonContain)
        {
            if (jsonDynamicData == null || !jsonDynamicData.Any())
                return source;
            var jsonDynamicFilters = BuildJsonDynamicFilters(jsonDynamicData);

            foreach (var jsonFilter in jsonDynamicFilters)
            {
                var jsonPath = jsonFilter.JsonPath;
                var isArray = jsonPath.EndsWith("[]");
                if (isArray)
                {
                    jsonPath = jsonPath.Replace("[]", "");
                }
                var jsonValues = jsonFilter.Values;
                if (jsonValues.Count == 1)
                {
                    var value = jsonValues[0];
                    if (isArray)
                    {
                        //x => EfJsonQueryExtensions.JsonQuery(x.DynamicAttributes, jsonPath).Contains(value)
                        var predicate = jsonQuery(jsonPath, jsonFilter.Operator, value);
                        source = source.Where(predicate);
                    }
                    else
                    {
                        //x => EfJsonExtensions.JsonValue(x.DynamicAttributes, jsonPath) == value
                        var predicate = jsonValue(jsonPath, jsonFilter.Operator, value);
                        source = source.Where(predicate);
                    }
                }
                else
                {
                    if (isArray)
                    {
                        var predicate = PredicateBuilder.New<T>();
                        foreach (string keyword in jsonValues)
                        {
                            var predicateChild = jsonQuery(jsonPath, jsonFilter.Operator, keyword);
                            predicate = predicate.Or(predicateChild);
                        }
                        source = source.Where(predicate);
                    }
                    else
                    {
                        //x => jsonValues.Contains(EfJsonExtensions.JsonValue(x.DynamicAttributes, jsonPath))
                        var predicate = jsonContain(jsonValues, jsonFilter.Operator, jsonPath);
                        source = source.Where(predicate);
                    }
                }
            }

            return source;
        }

        public static List<JsonFilterInfo> BuildJsonDynamicFilters(List<string> jsonDynamicData)
        {
            List<JsonFilterInfo> jsonDynamicFilters = new List<JsonFilterInfo>();

            foreach (var jsonQuery in jsonDynamicData)
            {
                try
                {
                    string[] jsonInfo = null;
                    var comparisonOperator = ComparisonOperator.Equal;
                    var comparisonOperatorSign = ComparisonOperatorSign.Equal;

                    foreach (var comparisonOperatorSignMap in ComparisonOperatorSignMappings)
                    {
                        jsonInfo = Regex.Split(jsonQuery, comparisonOperatorSignMap.Value,
                            RegexOptions.IgnorePatternWhitespace);
                        if (jsonInfo.Length > 1)
                        {
                            comparisonOperator = comparisonOperatorSignMap.Key;
                            comparisonOperatorSign = comparisonOperatorSignMap.Value;
                            break;
                        }
                    }

                    if (jsonInfo == null || jsonInfo.Length <= 1) continue;

                    var jsonPath = jsonInfo[0];
                    if (!jsonPath.StartsWith("$.")) jsonPath = $"$.{jsonPath}";

                    var jsonValues = jsonInfo[1].Split(',', StringSplitOptions.RemoveEmptyEntries);
                    if (jsonValues.Length == 0) continue;

                    var existingFilter = jsonDynamicFilters.FirstOrDefault(f =>
                        f.JsonPath == jsonPath && f.Operator == comparisonOperator);
                    if (existingFilter != null)
                    {
                        existingFilter.Values.AddRange(jsonValues);
                    }
                    else
                    {
                        jsonDynamicFilters.Add(new JsonFilterInfo
                        {
                            JsonPath = jsonPath,
                            Operator = comparisonOperator,
                            OperatorSign = comparisonOperatorSign,
                            Values = jsonValues.ToList()
                        });
                    }

                }
                catch (Exception ex)
                {
                    throw new cxStudioException($"Json filter Error: {ex.Message}. Received: {jsonQuery}");
                }
            }

            return jsonDynamicFilters;
        }


    }
    public class JsonFilterInfo
    { 
        public string JsonPath { get; set; }
        public ComparisonOperator Operator { get; set; }
        public string OperatorSign { get; set; }

        public List<string> Values { get; set; }

    }
   
    public enum ComparisonOperator 
    {
        [Description("=")] Equal = 1,
        [Description(">")] GreaterThan = 2,
        [Description("<")] LessThan = 3,
        [Description("!=")] NotEqual = 4,
        [Description(">=")] GreaterThanOrEqual = 5,
        [Description("<=")] LessThanOrEqual = 6

    }

    public static class ComparisonOperatorSign
    {
        public const string Equal = "=";
        public const string GreaterThan = ">";
        public const string LessThan =  "<";
        public const string NotEqual = "!=";
        public const string GreaterThanOrEqual =">=";
        public const string LessThanOrEqual = "<=";

    }
}
