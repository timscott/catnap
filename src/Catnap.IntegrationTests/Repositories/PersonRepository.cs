using System;
using System.Collections.Generic;
using System.Linq;
using Catnap.Database;
using Catnap.Find.Conditions;
using Catnap.Tests.Core.Models;

namespace Catnap.IntegrationTests.Repositories
{
    public class PersonRepository : Repository<PersonGuid>, IPersonRepository
    {
        public IEnumerable<PersonGuid> FindByFirstName(string firstName)
        {
            var criteria = Criteria.For<PersonGuid>()
                .Equal(x => x.FirstName, firstName);
            return Find(criteria).ToList();
        }

        public IEnumerable<PersonGuid> GetPesonsWhoHavePosted()
        {
            var command = new DbCommandSpec()
                .SetCommandText("select p.* from PersonGuid p inner join PostGuid on PostGuid.PosterId = p.Id");
            return UnitOfWork.Current.Session.List<PersonGuid>(command);
        }

        public long GetTotalPostCount(Guid personId)
        {
            var command = new DbCommandSpec()
                .SetCommandText("select count(*) from PostGuid p where p.PosterId = @personId")
                .AddParameter("@personId", personId);
            return UnitOfWork.Current.Session.ExecuteScalar<long>(command);
        }
    }

    public class PersonIntRepository : Repository<Person>, IPersonIntRepository
    {
        public IEnumerable<Person> FindByFirstName(string firstName)
        {
            var criteria = Criteria.For<Person>().Equal(x => x.FirstName, firstName);
            return Find(criteria).ToList();
        }

        public IEnumerable<Person> GetPesonsWhoHavePosted()
        {
            var command = new DbCommandSpec()
                .SetCommandText("select p.* from Person p inner join Post on Post.PosterId = p.Id");
            return UnitOfWork.Current.Session.List<Person>(command);
        }

        public long GetTotalPostCount(int personId)
        {
            var command = new DbCommandSpec()
                .SetCommandText("select count(*) from Post p where p.PosterId = @personId")
                .AddParameter("@personId", personId);
            return UnitOfWork.Current.Session.ExecuteScalar<long>(command);
        }
    }
}