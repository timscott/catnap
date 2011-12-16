using System;

namespace Catnap.Tests.Core.Models
{
    public class Person : EntityGuid
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class PersonInt : Entity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool Active { get; set; }
        public DateTime MemberSince { get; set; }
    }
}