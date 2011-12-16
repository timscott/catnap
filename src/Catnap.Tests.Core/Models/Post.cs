using System;

namespace Catnap.Tests.Core.Models
{
    public class Post : EntityGuid
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public Person Poster { get; set; }
        public DateTime? DatePosted { get; set; }
    }

    public class PostInt : Entity
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public PersonInt Poster { get; set; }
        public DateTime? DatePosted { get; set; }
    }
}