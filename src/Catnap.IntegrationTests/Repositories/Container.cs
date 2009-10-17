namespace Catnap.IntegrationTests.Repositories
{
    public class Container
    {
        private static IForumRepository forumRepository;
        private static IPersonRepository personRepository;

        public static IForumRepository ForumRepository
        {
            get
            {
                if (forumRepository == null)
                {
                    forumRepository = new ForumRepository();
                }
                return forumRepository;
            }
        }

        public static IPersonRepository PersonRepository
        {
            get
            {
                if (personRepository == null)
                {
                    personRepository = new PersonRepository();
                }
                return personRepository;
            }
        }
    }
}