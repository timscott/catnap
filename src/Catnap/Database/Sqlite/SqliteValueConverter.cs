using System;
using System.Collections.Generic;
using Catnap.Database.Types;

namespace Catnap.Database.Sqlite
{
    public class SqliteValueConverter : ValueConverter
    {
        //NOTE: other conversions needed?
        public SqliteValueConverter() : base(new DefaultType(),  new Dictionary<Type, IType>
        {
            { typeof(bool), new BooleanIntType() },
            { typeof(Guid), new GuidStringType() },
            { typeof(DateTime), new DateTimeTicksType() },
            { typeof(TimeSpan), new TimespanTicksType() }
        }) { }
    }
}