using System;

namespace Catnap.Mapping.Impl
{
    public class GuidGenerator : IIdValueGenerator
    {
        public object Generate()
        {
            return Guid.NewGuid();
        }
    }
}