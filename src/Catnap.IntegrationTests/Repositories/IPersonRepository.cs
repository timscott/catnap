using System.Collections.Generic;
using Catnap.IntegrationTests.Models;

namespace Catnap.IntegrationTests.Repositories
{
    public interface IPersonRepository : IRepository<Person>
    {
        IEnumerable<Person> FindByFirstName(string firstName);
        IEnumerable<Person> GetPesonsWhoHavePosted();
        int GetTotalPostCount(int personId);
    }
}