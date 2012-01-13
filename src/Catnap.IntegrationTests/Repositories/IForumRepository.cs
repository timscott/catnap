using Catnap.Tests.Core.Models;

namespace Catnap.IntegrationTests.Repositories
{
    public interface IForumRepository : IRepository<ForumGuid>
    {
    }

    public interface IForumIntRepository : IRepository<Forum>
    {
    }
}