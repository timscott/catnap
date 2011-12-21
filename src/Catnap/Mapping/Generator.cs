using Catnap.Mapping.Impl;

namespace Catnap.Mapping
{
    public static class Generator
    {
        public static IIdValueGenerator GuidNew = new GuidGenerator();
        public static IIdValueGenerator GuidComb = new GuidCombGenerator();
    }
}