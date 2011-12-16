using System;
using System.Collections.Generic;
using Catnap.Tests.Core.Models;

namespace Catnap.IntegrationTests.Repositories
{
    public interface IPersonRepository : IRepository<PersonGuid>
    {
        IEnumerable<PersonGuid> FindByFirstName(string firstName);
        IEnumerable<PersonGuid> GetPesonsWhoHavePosted();
        long GetTotalPostCount(Guid personId);
    }

    public interface IPersonIntRepository : IRepository<Person>
    {
        IEnumerable<Person> FindByFirstName(string firstName);
        IEnumerable<Person> GetPesonsWhoHavePosted();
        long GetTotalPostCount(int personId);
    }
}