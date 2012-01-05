using System;
using System.Collections.Generic;
using Catnap.Database.Types;
using Catnap.Extensions;
using Catnap.Logging;

namespace Catnap.Database
{
    public class ValueConverter
    {
        private readonly IType defaultType;
        private readonly IDictionary<Type, IType> types;

        public ValueConverter(IType defaultType, IDictionary<Type, IType> types)
        {
            this.defaultType = defaultType;
            this.types = types;
        }

        public object ConvertToDb(object value)
        {
            if (value == null)
            {
                return null;
            }
            var underlyingType = value.GetType().GetUnderlyingGenericType();
            Log.Debug("Converting '{0}' from type {1}", value, underlyingType.FullName);
            var type = ResolveType(underlyingType);
            return type.ToDb(value);
        }

        public object ConvertFromDb(object value, Type toType)
        {
            Log.Debug("Converting '{0}' to type {1}", value, toType.FullName);
            var underlyingType = toType.GetUnderlyingGenericType();
            var type = ResolveType(underlyingType);
            return type.FromDb(value, underlyingType);
        }

        private IType ResolveType(Type underlyingType)
        {
            return types.ContainsKey(underlyingType)
                ? types[underlyingType]
                : defaultType;
        }
    }
}