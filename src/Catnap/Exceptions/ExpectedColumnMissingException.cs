using System;
using Catnap.Mapping;

namespace Catnap.Exceptions
{
    public class ExpectedColumnMissingException<T> : Exception where T : class, new()
    {
        public ExpectedColumnMissingException(IPropertyMapWithColumn<T> map, string tableName)
            : base(string.Format("Property '{0}.{1}' is mapped to '{2}.{3}' but no such column was in the results set. Check your mapping for that property.", typeof(T), map.PropertyName, tableName, map.ColumnName)) { }
    }
}