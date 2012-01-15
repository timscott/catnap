using System;

namespace Catnap.Extensions
{
    public static class ObjectExtensions
    {
        public static void GuardArgumentNull(this object arg, string argName)
        {
            if (arg == null)
            {
                throw new ArgumentNullException(argName);
            }
        }
    }
}