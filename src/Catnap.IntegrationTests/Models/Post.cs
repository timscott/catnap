namespace Catnap.IntegrationTests.Models
{
    public class Post : Entity
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public Person Poster { get; set; }
    }
}