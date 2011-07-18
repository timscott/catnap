using System;
using Catnap.Common.Logging;
using Catnap.Database;
using Catnap.Find;
using Catnap.Maps;
using Catnap.UnitTests.Models;
using Machine.Specifications;
using Should.Fluent;

namespace Catnap.UnitTests
{
    public class behaves_like_unit_test_requiring_domain_context
    {
        private Establish context = () =>
        {
            Log.Level = LogLevel.Off;
            Domain.Configure
            (
                Map.Entity<Person>()
                    .Property(x => x.FirstName)
                    .Property(x => x.LastName)
                    .Property(x => x.Active)
                    .Property(x => x.MemberSince),
                Map.Entity<Forum>()
                    .List(x => x.Posts)
                    .Property(x => x.Name),
                Map.Entity<Post>()
                    .ParentColumn("ForumId")
                    .Property(x => x.Title)
                    .Property(x => x.Body)
                    .BelongsTo(x => x.Poster, "PosterId")
            );
        };
    }

    public class when_creating_sql_command_from_expression : behaves_like_unit_test_requiring_domain_context
    {
        static DbCommandSpec commandSpec;
        static bool? isActive = true;
        static DateTime? joinedBy = new DateTime(2008, 1, 1);

        Because of = () =>
        {
            var isActiveLocalScope = isActive;
            var joinedByLocalScope = joinedBy;
            var memberBeforeLocalScope = DateTime.Today.AddDays(-10);
            commandSpec = new FindCommandBuilder<Person>()
                .AddCondition(x => x.Active == isActiveLocalScope.Value)
                .AddCondition(x => x.MemberSince >= joinedByLocalScope && x.MemberSince <= memberBeforeLocalScope)
                .AddCondition(x => x.FirstName == "Tim")
                .AddCondition(x => x.LastName == "Scott")
                .Build();
        };

        It should_have_correct_command_text = () => commandSpec.CommandText.Should()
            .Equal("select * from Person where (Active = @0) and ((MemberSince >= @1) and (MemberSince <= @2)) and (FirstName = @3) and (LastName = @4)");

        It should_have_correct_parameters = () =>
        {
            commandSpec.Parameters.Should().Count.Exactly(5);
            commandSpec.Parameters.Should().Contain.One(x => x.Name == "@0" && x.Value.Equals(1));
            commandSpec.Parameters.Should().Contain.One(x => x.Name == "@1" && x.Value.Equals(joinedBy));
            commandSpec.Parameters.Should().Contain.One(x => x.Name == "@2" && x.Value.Equals(DateTime.Today.AddDays(-10)));
            commandSpec.Parameters.Should().Contain.One(x => x.Name == "@3" && x.Value.Equals("Tim"));
            commandSpec.Parameters.Should().Contain.One(x => x.Name == "@4" && x.Value.Equals("Scott"));
        };
    }
}