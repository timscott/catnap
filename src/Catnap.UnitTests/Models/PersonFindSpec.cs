using System;
using Catnap.Common;

namespace Catnap.UnitTests.Models
{
    public class PersonFindSpec : IFindSpec<Person>
    {
        private FindCommandBuilder<Person> commandBuilder = new FindCommandBuilder<Person>();

        public PersonFindSpec FirstName(string value)
        {
            commandBuilder.AddCondition(x => x.FirstName == value);
            return this;
        }

        public PersonFindSpec LastName(string value)
        {
            commandBuilder.AddCondition(x => x.LastName == value);
            return this;
        }

        public PersonFindSpec Active(bool value)
        {
            commandBuilder.AddCondition(x => x.Active == value);
            return this;
        }

        public PersonFindSpec MemberSince(DateTime? from, DateTime? through)
        {
            commandBuilder.AddCondition(x => x.MemberSince >= from && x.MemberSince <= through);
            return this;
        }

        public DbCommandSpec ToCommand()
        {
            return commandBuilder.Build();
        }
    }
}