using System.Collections.Generic;
using System.Linq;
using Catnap.Common.Database;
using Catnap.Find;
using Catnap.Find.Conditions;
using Catnap.IntegrationTests.Models;

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

        public int GetTotalPostCount(int personId)
        {
            var command = new DbCommandSpec()
                .SetCommandText("select count(*) from Post p where p.PosterId = @personId")
                .AddParameter("@personId", personId);
            return UnitOfWork.Current.Session.ExecuteScalar<int>(command);
        }
    }
}