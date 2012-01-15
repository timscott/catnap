using System;
using Catnap.Citeria.Conditions;
using Catnap.Configuration;
using Catnap.Database;
using Catnap.Logging;
using Catnap.Tests.Core;
using Catnap.Tests.Core.Models;
using Machine.Specifications;
using Should.Fluent;

namespace Catnap.UnitTests
{
    public class behaves_like_unit_test_requiring_domain_context
    {
        protected static ISessionFactory sessionFactory;

        Establish context = () =>
        {
            Log.Level = LogLevel.Off;
            sessionFactory = Fluently.Configure
                .Domain(DomainMapping.Get())
                .DatabaseAdapter(new NullDbAdapter())
                .Build();
        };
    }

    public class when_creating_criteria_from_expression : behaves_like_unit_test_requiring_domain_context
    {
        static IDbCommandSpec spec;
        static bool? isActive = true;
        static DateTime? joinedBy = new DateTime(2008, 1, 1);

        Because of = () =>
        {
            var isActiveLocalScope = isActive;
            var joinedByLocalScope = joinedBy;
            var memberBeforeLocalScope = DateTime.Today.AddDays(-10);
            var criteria = Criteria.For<Person>()
                .Where(x => x.Active == isActiveLocalScope.Value)
                .Where(x => x.MemberSince >= joinedByLocalScope && x.MemberSince <= memberBeforeLocalScope)
                .Where(x => x.FirstName == "Tim")
                .Where(x => x.LastName == "Scott");
            spec = criteria.Build(sessionFactory.Create());
        };

        It should_have_correct_command_text = () => spec.CommandText.Should()
            .Equal("((Active = @0) and ((MemberSince >= @1) and (MemberSince <= @2)) and (FirstName = @3) and (LastName = @4))");

        It should_have_correct_parameters = () =>
        {
            spec.Parameters.Should().Count.Exactly(5);
            spec.Parameters.Should().Contain.One(x => x.Name == "@0" && x.Value.Equals(1));
            spec.Parameters.Should().Contain.One(x => x.Name == "@1" && x.Value.Equals(joinedBy));
            spec.Parameters.Should().Contain.One(x => x.Name == "@2" && x.Value.Equals(DateTime.Today.AddDays(-10)));
            spec.Parameters.Should().Contain.One(x => x.Name == "@3" && x.Value.Equals("Tim"));
            spec.Parameters.Should().Contain.One(x => x.Name == "@4" && x.Value.Equals("Scott"));
        };
    }
}