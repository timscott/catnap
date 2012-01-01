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
                .DatabaseAdapter(new NullDbAdapter("@"))
                .Build();
        };
    }

    public class when_creating_criteria_from_expression : behaves_like_unit_test_requiring_domain_context
    {
        static ICriteria<Person> criteria;
        static bool? isActive = true;
        static DateTime? joinedBy = new DateTime(2008, 1, 1);

        Because of = () =>
        {
            var isActiveLocalScope = isActive;
            var joinedByLocalScope = joinedBy;
            var memberBeforeLocalScope = DateTime.Today.AddDays(-10);
            criteria = Criteria.For<Person>()
                .Where(x => x.Active == isActiveLocalScope.Value)
                .Where(x => x.MemberSince >= joinedByLocalScope && x.MemberSince <= memberBeforeLocalScope)
                .Where(x => x.FirstName == "Tim")
                .Where(x => x.LastName == "Scott");
            sessionFactory.New().Build(criteria);
        };

        It should_have_correct_command_text = () => criteria.Sql.Should()
            .Equal("((Active = @0) and ((MemberSince >= @1) and (MemberSince <= @2)) and (FirstName = @3) and (LastName = @4))");

        It should_have_correct_parameters = () =>
        {
            criteria.Parameters.Should().Count.Exactly(5);
            criteria.Parameters.Should().Contain.One(x => x.Name == "@0" && x.Value.Equals(1));
            criteria.Parameters.Should().Contain.One(x => x.Name == "@1" && x.Value.Equals(joinedBy));
            criteria.Parameters.Should().Contain.One(x => x.Name == "@2" && x.Value.Equals(DateTime.Today.AddDays(-10)));
            criteria.Parameters.Should().Contain.One(x => x.Name == "@3" && x.Value.Equals("Tim"));
            criteria.Parameters.Should().Contain.One(x => x.Name == "@4" && x.Value.Equals("Scott"));
        };
    }
}