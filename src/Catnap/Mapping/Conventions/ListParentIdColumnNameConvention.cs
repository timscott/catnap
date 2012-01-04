using System;

namespace Catnap.Mapping.Conventions
{
    public class ListParentIdColumnNameConvention
    {
        private readonly Func<IListPropertyMapDescriptor, string> convention;

        public ListParentIdColumnNameConvention(Func<IListPropertyMapDescriptor, string> convention)
        {
            this.convention = convention;
        }

        public string GetColumnName(IListPropertyMapDescriptor map)
        {
            return convention(map);
        }
    }
}