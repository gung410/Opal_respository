using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.SqlClient;

namespace cxOrganization.Domain.Extensions
{
    public static class SqlParameterExtension
    {
        public static List<SqlParameter> AddSingleValueParameter<T>(this List<SqlParameter> parameters, string name, T value)
        {
            if (value != null)
            {
                parameters.Add(new SqlParameter(name, value));
            }

            return parameters;
        }
        public static List<SqlParameter> AddMultiValuesParameter(this List<SqlParameter> parameters, string name, List<int> values, string valueSeparator=",")
        {
            if (!values.IsNullOrEmpty())
            {
                parameters.Add(new SqlParameter(name, string.Join(valueSeparator, values)));
            }

            return parameters;
        }

        public static List<SqlParameter> AddMultiValuesParameter(this List<SqlParameter> parameters, string name,
            List<string> values, bool putValueInQuote, string valueSeparator = ",")
        {
            if (!values.IsNullOrEmpty())
            {
                if (putValueInQuote)
                {
                    var valuesWIthQuote = values.Select(v => $"'{v}'");
                    parameters.Add(new SqlParameter(name, string.Join(valueSeparator, valuesWIthQuote)));

                }
                else
                {
                    parameters.Add(new SqlParameter(name, string.Join(valueSeparator, values)));

                }

            }

            return parameters;
        }


        public static List<SqlParameter> AddMultiValuesParameter(this List<SqlParameter> parameters, string name, List<List<int>> multiValues, string valueSeparator = ",", string andSeparator = "&&")
        {
            if (!multiValues.IsNullOrEmpty())
            {

                var multiFilters = new List<string>();
                foreach (var values in multiValues)
                {
                    if (!values.IsNullOrEmpty())
                    {
                        multiFilters.Add(string.Join(valueSeparator, values));
                    }
                }

                if (multiFilters.Count > 0)
                {
                    parameters.Add(new SqlParameter(name, string.Join(andSeparator, multiFilters)));
                }
            }

            return parameters;
        }
        public static List<SqlParameter> AddMultiValuesParameter(this List<SqlParameter> parameters, string name, List<List<string>> multiValues, string valueSeparator = ",", string andSeparator = "&&")
        {
            if (!multiValues.IsNullOrEmpty())
            {

                var multiFilters = new List<string>();
                foreach (var values in multiValues)
                {
                    if (!values.IsNullOrEmpty())
                    {
                        var valuesWIthQuote = values.Select(v => $"'{v}'");
                        multiFilters.Add(string.Join(valueSeparator, valuesWIthQuote));
                    }
                }

                if (multiFilters.Count > 0)
                {
                    parameters.Add(new SqlParameter(name, string.Join(andSeparator, multiFilters)));
                }
            }

            return parameters;
        }
    }
}
