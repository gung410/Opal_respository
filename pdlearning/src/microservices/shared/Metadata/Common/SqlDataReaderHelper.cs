using System;
using System.Collections.Generic;
using System.Data.Common;

namespace Conexus.Opal.Microservice.Metadata.Common
{
    public class SqlDataReaderHelper
    {
        public static List<T> CreateList<T>(DbDataReader reader)
        {
            var results = new List<T>();

            while (reader.Read())
            {
                var item = Activator.CreateInstance<T>();
                foreach (var property in typeof(T).GetProperties())
                {
                    if (!IsPropertyValueNull(reader, property))
                    {
                        Type convertTo = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                        property.SetValue(item, ObjectHelper.ParseObject(reader[property.Name], convertTo));
                    }
                }

                results.Add(item);
            }

            reader.Close();

            return results;
        }

        public static List<T> CreateSingleValueList<T>(DbDataReader reader)
        {
            var results = new List<T>();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    results.Add(ObjectHelper.ParseObject<T>(reader[0]));
                }
            }

            if (!reader.IsClosed)
            {
                reader.Close();
            }

            return results;
        }

        public static int Count(DbDataReader reader)
        {
            int count = 0;
            if (reader.Read())
            {
                count = reader.GetInt32(0);
            }

            if (!reader.IsClosed)
            {
                reader.Close();
            }

            return count;
        }

        private static bool IsPropertyValueNull(DbDataReader reader, System.Reflection.PropertyInfo property)
        {
            try
            {
                return reader.IsDBNull(reader.GetOrdinal(property.Name));
            }
            catch
            {
                return true;
            }
        }
    }
}
