using System;

namespace Catnap.Tests.Core.Models
{
    public class PersonGuid : EntityGuid
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool Active { get; set; }
        public DateTime MemberSince { get; set; }
    }

    public class Person : EntityInt
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool Active { get; set; }
        public DateTime MemberSince { get; set; }
    }
}