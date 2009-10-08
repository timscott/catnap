using System;

namespace Catnap.UnitTests.Models
{
    public class Person : Entity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? MemberSince { get; set; }
        public bool Active { get; set; }
    }
}