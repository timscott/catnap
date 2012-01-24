using System;
using System.Collections.Generic;
using Catnap.Database.Types;

namespace Catnap.Database.SqlServerCe
{
    public class SqlServerCeValueConverter : ValueConverter
    {
        public SqlServerCeValueConverter() : base(new DefaultType(), new Dictionary<Type, IType>
        {
            { typeof(bool), new BooleanIntType() },
            { typeof(TimeSpan), new TimespanTicksType() }
        }) { }
    }
}