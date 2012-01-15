using System;

namespace Catnap.Mapping.Conventions
{
    public class BelongsToColumnNameConvention
    {
        private readonly Func<IBelongsToPropertyMapDescriptor, string> convention;

        public BelongsToColumnNameConvention(Func<IBelongsToPropertyMapDescriptor, string> convention)
        {
            this.convention = convention;
        }

        public string GetColumnName(IBelongsToPropertyMapDescriptor map)
        {
            return convention(map);
        }
    }
}