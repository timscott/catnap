using System;
using Catnap.Citeria.Conditions;
using Catnap.Configuration;
using Catnap.Logging;
using Catnap.Mapping;
using Catnap.Tests.Core.Models;
using Machine.Specifications;
using Should.Fluent;
using It=Machine.Specifications.It;

namespace Catnap.UnitTests
{
    public class when_creating_a_complex_condition
    {
        static ICriteria<Person> criteria;
        static IDbCommandSpec target;
        static ISessionFactory sessionFactory;

        Establish context = () =>
        {
            Log.Level = LogLevel.Off;
            sessionFactory = Fluently.Configure
                .Domain(d =>
                    d.Entity<Person>(e =>
                    {
                        e.Id(x => x.Id).Access(Access.Property);
                        e.Property(x => x.FirstName);
                        e.Property(x => x.LastName);
                        e.Property(x => x.MemberSince);
                    }))
                 .Build();
            criteria = Criteria.For<Person>()
                .Less("Bar", 1000)
                .GreaterOrEqual("Bar", 300)
                .Or(or =>
                {
                    or.NotEqual(x => x.FirstName, "Tim");
                    or.And(and =>
                    {
                        and.Equal("Foo", 25);
                        and.Equal("Baz", 500);
                    });
                })
                .And(and =>
                {
                    and.LessOrEqual(x => x.MemberSince, new DateTime(2000, 1, 1));
                    and.Greater(x => x.MemberSince, new DateTime(1980, 1, 1));
                    and.Where(x => x.LastName == "Scott" || x.LastName == "Jones");
                })
                .Null(x => x.LastName)
                .NotNull("FirstName");
        };

        Because of = () => target = criteria.Build(sessionFactory.Create());

        It should_render_correct_sql = () => target.CommandText
            .Should().Equal("((Bar < @0) and (Bar >= @1) and ((FirstName != @2) or ((Foo = @3) and (Baz = @4))) and ((MemberSince <= @5) and (MemberSince > @6) and ((LastName = @7) or (LastName = @8))) and (LastName is NULL) and (FirstName is not NULL))");

        It should_contain_expected_parameters = () =>
        {
            target.Parameters.Should().Count.Exactly(9);
            target.Parameters.Should().Contain.One(x => x.Name == "@0" && x.Value.Equals(1000));
            target.Parameters.Should().Contain.One(x => x.Name == "@1" && x.Value.Equals(300));
            target.Parameters.Should().Contain.One(x => x.Name == "@2" && x.Value.Equals("Tim"));
            target.Parameters.Should().Contain.One(x => x.Name == "@3" && x.Value.Equals(25));
            target.Parameters.Should().Contain.One(x => x.Name == "@4" && x.Value.Equals(500));
            target.Parameters.Should().Contain.One(x => x.Name == "@5" && x.Value.Equals(new DateTime(2000, 1, 1)));
            target.Parameters.Should().Contain.One(x => x.Name == "@6" && x.Value.Equals(new DateTime(1980, 1, 1)));
            target.Parameters.Should().Contain.One(x => x.Name == "@7" && x.Value.Equals("Scott"));
            target.Parameters.Should().Contain.One(x => x.Name == "@8" && x.Value.Equals("Jones"));
        };
    }
}