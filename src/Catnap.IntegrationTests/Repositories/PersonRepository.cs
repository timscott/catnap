using System;
using System.Collections.Generic;
using System.Linq;
using Catnap.Database;
using Catnap.Find;
using Catnap.Find.Conditions;
using Catnap.Tests.Core.Models;

namespace Catnap.IntegrationTests.Repositories
{
    public class PersonRepository : Repository<Person>, IPersonRepository
    {
        public IEnumerable<Person> FindByFirstName(string firstName)
        {
            var criteria = new Criteria()
                .Add(Condition.Equal<Person>(x => x.FirstName, firstName));
            return Find(criteria).ToList();
        }

        public IEnumerable<Person> GetPesonsWhoHavePosted()
        {
            var command = new DbCommandSpec()
                .SetCommandText("select p.* from Person p inner join Post on Post.PosterId = p.Id");
            return UnitOfWork.Current.Session.List<Person>(command);
        }

        public long GetTotalPostCount(Guid personId)
        {
            var command = new DbCommandSpec()
                .SetCommandText("select count(*) from Post p where p.PosterId = @personId")
                .AddParameter("@personId", personId);
            return UnitOfWork.Current.Session.ExecuteScalar<long>(command);
        }
    }

    public class PersonIntRepository : Repository<PersonInt>, IPersonIntRepository
    {
        public IEnumerable<PersonInt> FindByFirstName(string firstName)
        {
            var criteria = new Criteria()
                .Add(Condition.Equal<PersonInt>(x => x.FirstName, firstName));
            return Find(criteria).ToList();
        }

        public IEnumerable<PersonInt> GetPesonsWhoHavePosted()
        {
            var command = new DbCommandSpec()
                .SetCommandText("select p.* from PersonInt p inner join PostInt on PostInt.PosterId = p.Id");
            return UnitOfWork.Current.Session.List<PersonInt>(command);
        }

        public long GetTotalPostCount(int personId)
        {
            var command = new DbCommandSpec()
                .SetCommandText("select count(*) from PostInt p where p.PosterId = @personId")
                .AddParameter("@personId", personId);
            return UnitOfWork.Current.Session.ExecuteScalar<long>(command);
        }
    }
}