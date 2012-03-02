using System;
using System.Collections.Generic;
using Catnap.Database.Types;

namespace Catnap.Database.MySql
{
    public class MySqlValueConverter : ValueConverter
    {
        public MySqlValueConverter() : base(new DefaultType(), new Dictionary<Type, IType>
        {
            { typeof(bool), new BooleanIntType() },
            { typeof(TimeSpan), new TimespanTicksType() },
            { typeof(Guid), new GuidStringType() }
        }) { }
    }
}