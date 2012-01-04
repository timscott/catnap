namespace Catnap.Database
{
    public abstract class BaseDbAdapter
    {
        protected readonly string parameterPrefix;
        protected readonly string openQuote;
        private readonly string closeQuote;

        protected BaseDbAdapter() : this("@", "\"", "\"") { }

        protected BaseDbAdapter(string parameterPrefix, string openQuote, string closeQuote)
        {
            this.parameterPrefix = parameterPrefix;
            this.openQuote = openQuote;
            this.closeQuote = closeQuote;
        }

        public virtual string FormatParameterName(string name)
        {
            return name.StartsWith(parameterPrefix)
                ? name
                : parameterPrefix + name;
        }

        public virtual string Quote(string name)
        {
            return
                (name.StartsWith(openQuote) ? null : openQuote) +
                name +
                (name.EndsWith(closeQuote) ? null : closeQuote);
        }
    }
}