using System.Collections.Generic;

namespace Catnap.IntegrationTests.Models
{
    public class Forum : Entity
    {
        private IEnumerable<Post> posts = new List<Post>();

        public string Name { get; set; }
        public IEnumerable<Post> Posts
        {
            get { return posts;  }
            set { posts = value;  }
        }
    }
}