using System;

namespace Catnap.Exceptions
{
    public class LazyLoadException : Exception
    {
        public override string Message
        {
            get { return "Failed to load the collection.  Did you dispose the session between getting the parent and and accessing the collection?"; }
        }
    }
}