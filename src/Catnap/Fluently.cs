namespace Catnap
{
    public static class Fluently
    {
        public static IConfigurator Configure
        {
            get { return new Configurator(); }
        }
    }
}