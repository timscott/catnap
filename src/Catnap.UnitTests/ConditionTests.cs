using System;
using Catnap.Find.Conditions;
using Catnap.Logging;
using Catnap.Mapping;
using Catnap.Tests.Core.Models;
using Machine.Specifications;
using Should.Fluent;
using It=Machine.Specifications.It;

namespace Catnap.UnitTests
{
    public class when_creating_an_complex_condition
    {
        static ICriteria<Person> target;
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
                        e.Property(x => x.MemberSince);
                    }))
                 .Build();
            target = Criteria.For<Person>()
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
                });
        };

        Because of = () => sessionFactory.New().Build(target);

        It should_render_correct_sql = () => target.Sql
            .Should().Equal("((Bar < @0) and (Bar >= @1) and ((FirstName != @2) or ((Foo = @3) and (Baz = @4))) and ((MemberSince <= @5) and (MemberSince > @6)))");

        It should_contain_expected_parameters = () =>
        {
            target.Parameters.Should().Count.Exactly(7);
            target.Parameters.Should().Contain.One(x => x.Name == "@0" && x.Value.Equals(1000));
            target.Parameters.Should().Contain.One(x => x.Name == "@1" && x.Value.Equals(300));
            target.Parameters.Should().Contain.One(x => x.Name == "@2" && x.Value.Equals("Tim"));
            target.Parameters.Should().Contain.One(x => x.Name == "@3" && x.Value.Equals(25));
            target.Parameters.Should().Contain.One(x => x.Name == "@4" && x.Value.Equals(500));
            target.Parameters.Should().Contain.One(x => x.Name == "@5" && x.Value.Equals(new DateTime(2000, 1, 1)));
            target.Parameters.Should().Contain.One(x => x.Name == "@6" && x.Value.Equals(new DateTime(1980, 1, 1)));
        };
    }
}