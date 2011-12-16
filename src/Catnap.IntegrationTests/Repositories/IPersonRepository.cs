using System;
using System.Collections.Generic;
using Catnap.Tests.Core.Models;

namespace Catnap.IntegrationTests.Repositories
{
    public interface IPersonRepository : IRepository<Person>
    {
        IEnumerable<Person> FindByFirstName(string firstName);
        IEnumerable<Person> GetPesonsWhoHavePosted();
        long GetTotalPostCount(Guid personId);
    }

    public interface IPersonIntRepository : IRepository<PersonInt>
    {
        IEnumerable<PersonInt> FindByFirstName(string firstName);
        IEnumerable<PersonInt> GetPesonsWhoHavePosted();
        long GetTotalPostCount(int personId);
    }
}