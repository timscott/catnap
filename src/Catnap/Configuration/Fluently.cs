using Catnap.Configuration.Impl;

namespace Catnap.Configuration
{
    public static class Fluently
    {
        public static IConfigurator Configure
        {
            get { return new Configurator(); }
        }
    }
}