using Catnap.Impl;
using Catnap.Tests.Core.Models;

namespace Catnap.IntegrationTests.Repositories
{
    public class ForumRepository : Repository<ForumGuid>, IForumRepository
    {
    }

    public class ForumIntRepository : Repository<Forum>, IForumIntRepository
    {
    }
}