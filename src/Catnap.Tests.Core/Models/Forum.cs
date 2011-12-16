using System;
using System.Collections.Generic;

namespace Catnap.Tests.Core.Models
{
    public class Forum : EntityGuid
    {
        private IEnumerable<Post> posts = new List<Post>();

        public string Name { get; set; }
        public TimeSpan? TimeOfDayLastUpdated { get; set; }
        public IEnumerable<Post> Posts
        {
            get { return posts;  }
            set { posts = value;  }
        }
    }

    public class ForumInt : Entity
    {
        private IEnumerable<PostInt> posts = new List<PostInt>();

        public string Name { get; set; }
        public TimeSpan? TimeOfDayLastUpdated { get; set; }
        public IEnumerable<PostInt> Posts
        {
            get { return posts; }
            set { posts = value; }
        }
    }
}