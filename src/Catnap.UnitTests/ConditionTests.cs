using Catnap.Common.Logging;
using Catnap.Find;
using Catnap.Find.Conditions;
using Catnap.Maps;
using Catnap.UnitTests.Models;
using Machine.Specifications;
using ShouldIt.Clr.Fluent;
using It=Machine.Specifications.It;

namespace Catnap.UnitTests
{
    public class when_creating_an_complex_condition
    {
        static ICriteria target;

        Establish context = () => { }; //to prevent picking up context from previou test run

        Because of = () =>
        {
            Log.Level = LogLevel.Off;
            Domain.Configure(Map.Entity<Person>().Property(x => x.FirstName));
            target = new Criteria
            (
                Condition.Less("Bar", 1000),
                Condition.GreaterOrEqual("Bar", 300),
                Condition.Or
                (
                    Condition.NotEqual<Person>(x => x.FirstName, "Tim"),
                    Condition.And
                    (
                        Condition.Equal("Foo", 25),
                        Condition.Equal("Baz", 500)
                    )
                )
            );
            target.Build();
        };

        It should_render_correct_string = () => target.ToString().Should()
            .Equal("((Bar < @0) and (Bar >= @1) and ((FirstName != @2) or ((Foo = @3) and (Baz = @4))))");

        It should_contain_expected_parameters = () =>
        {
            target.Parameters.Should().Count.Exactly(5);
            target.Parameters.Should().Contain.One(x => x.Name == "@0" && x.Value.Equals(1000));
            target.Parameters.Should().Contain.One(x => x.Name == "@1" && x.Value.Equals(300));
            target.Parameters.Should().Contain.One(x => x.Name == "@2" && x.Value.Equals("Tim"));
            target.Parameters.Should().Contain.One(x => x.Name == "@3" && x.Value.Equals(25));
            target.Parameters.Should().Contain.One(x => x.Name == "@4" && x.Value.Equals(500));
        };
    }
}