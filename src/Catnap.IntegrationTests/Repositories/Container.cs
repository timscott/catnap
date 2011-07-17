namespace Catnap.IntegrationTests.Repositories
{
    public class Container
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
}