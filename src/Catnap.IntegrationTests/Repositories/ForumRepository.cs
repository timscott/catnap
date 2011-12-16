using Catnap.Tests.Core.Models;

namespace Catnap.IntegrationTests.Repositories
{
    public class ForumRepository : Repository<Forum>, IForumRepository
    {
    }

    public class ForumIntRepository : Repository<ForumInt>, IForumIntRepository
    {
    }
}