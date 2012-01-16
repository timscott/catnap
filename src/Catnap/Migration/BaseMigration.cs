using System;
using System.Linq;

namespace Catnap.Migration
{
    public class BaseMigration : IDatabaseMigration
    {
        private readonly string[] sqls;

        public BaseMigration(params string[] sqls)
        {
            this.sqls = sqls;
        }

        public string Name
        {
            get { return GetType().Name; }
        }

        public Action<ISession> Action
        {
            get { return session => sqls.Select(x => new DbCommandSpec().SetCommandText(x)).ToList().ForEach(x => session.ExecuteNonQuery(x)); }
        }
    }
}