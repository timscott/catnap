using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Catnap.Database
{
    public abstract class BaseDbAdapter : IDbAdapter
    {
        protected readonly Type connectionType;
        protected readonly string parameterPrefix;
        protected readonly string openQuote;
        protected readonly string closeQuote;

        protected BaseDbAdapter() : this("@", "\"", "\"") { }

        protected BaseDbAdapter(Type connectionType) : this()
        {
            this.connectionType = connectionType;
        }

        protected BaseDbAdapter(Type connectionType, string parameterPrefix, string openQuote, string closeQuote) 
            : this(parameterPrefix, openQuote, closeQuote)
        {
            this.connectionType = connectionType;
        }

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

        protected static Type ResolveConnectionType(string connectionTypeAssemblyName, string connectionTypeName)
        {
            var connectionType = Type.GetType(string.Format("{0},{1}", connectionTypeName, connectionTypeAssemblyName));
            if (connectionType != null)
            {
                return connectionType;
            }
            var assembly = GetAssembly(connectionTypeAssemblyName);
            return assembly.GetType(connectionTypeName, true);
        }

        protected static Type ResolveConnectionType(string connectionTypeAssemblyName)
        {
            var assembly = Assembly.Load(connectionTypeAssemblyName);
            if (assembly == null)
            {
                throw new TypeLoadException(string.Format("Could not load assembly: {0}.  Are you missing a reference?",
                    connectionTypeAssemblyName));
            }
            var type = assembly.GetTypes().FirstOrDefault(x => typeof(IDbConnection).IsAssignableFrom(x));
            if (type == null)
            {
                throw new TypeLoadException(string.Format("Could not find type of IDbConnection in assembly: {0}.  Are you missing a reference?",
                    connectionTypeAssemblyName));
            }
            return type;
        }

        private static Assembly GetAssembly(string name)
        {
            var assembly = Assembly.Load(name);
            if (assembly == null)
            {
                throw new TypeLoadException(string.Format("Could not load assembly: {0}.  Are you missing a reference?", name));
            }
            return assembly;
        }

        public virtual IDbConnection CreateConnection(string connectionString)
        {
            if (connectionType == null)
            {
                throw new InvalidOperationException("Connection type is unknown.");
            }
            return (IDbConnection)Activator.CreateInstance(connectionType, new object[] { connectionString });
        }

        public abstract object ConvertFromDb(object value, Type toType);
        public abstract IDbCommand CreateLastInsertIdCommand(string tableName, IDbCommandFactory commandFactory);
        public abstract IDbCommand CreateGetTableMetadataCommand(string tableName, IDbCommandFactory commandFactory);
        public abstract object ConvertToDb(object value);
        public abstract string GetGeneralStringType();
    }
}