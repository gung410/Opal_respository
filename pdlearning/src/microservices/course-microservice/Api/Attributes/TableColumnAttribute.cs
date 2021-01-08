using System;

namespace Microservice.Course.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class TableColumnAttribute : Attribute
    {
        public TableColumnAttribute(int order, string columnName = null)
        {
            ColumnName = columnName;
            Order = order;
        }

        public string ColumnName { get; }

        public int Order { get; }
    }
}
