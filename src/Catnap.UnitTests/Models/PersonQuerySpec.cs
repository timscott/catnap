using System;
using Catnap.Common;

namespace Catnap.UnitTests.Models
{
    public class PersonQuerySpec : IQuerySpec<Person>
    {
        private FindCommandBuilder<Person> commandBuilder = new FindCommandBuilder<Person>();

        public PersonQuerySpec FirstName(string value)
        {
            commandBuilder.AddCondition(x => x.FirstName == value);
            return this;
        }

        public PersonQuerySpec LastName(string value)
        {
            commandBuilder.AddCondition(x => x.LastName == value);
            return this;
        }

        public PersonQuerySpec Active(bool value)
        {
            commandBuilder.AddCondition(x => x.Active == value);
            return this;
        }

        public PersonQuerySpec MemberSince(DateTime? from, DateTime? through)
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