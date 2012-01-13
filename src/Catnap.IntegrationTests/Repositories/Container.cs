namespace Catnap.IntegrationTests.Repositories
{
    public class ContainerGuid
    {
        private static IForumRepository forumRepository;
        private static IPersonRepository personRepository;

        public static IForumRepository ForumRepository
        {
            get { return forumRepository ?? (forumRepository = new ForumRepository()); }
        }

        public static IPersonRepository PersonRepository
        {
            get { return personRepository ?? (personRepository = new PersonRepository()); }
        }
    }

    public class Container
    {
        private static IForumIntRepository forumRepository;
        private static IPersonIntRepository personRepository;

        public static IForumIntRepository ForumRepository
        {
            get { return forumRepository ?? (forumRepository = new ForumIntRepository()); }
        }

        public static IPersonIntRepository PersonRepository
        {
            get { return personRepository ?? (personRepository = new PersonIntRepository()); }
        }
    }
}