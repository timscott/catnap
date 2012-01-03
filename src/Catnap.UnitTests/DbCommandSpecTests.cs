using Catnap.Database;
using Machine.Specifications;
using Should.Fluent;

namespace Catnap.UnitTests
{
    public class beahves_like_dbcommandspec_test
    {
        protected static DbCommandSpec spec;
        Establish context = () => spec = new DbCommandSpec();
    }

    public class when_adding_parameters_to_a_dbcommandspec : beahves_like_dbcommandspec_test
    {
        Because of = () => spec.AddParameters(new[]
        {
            new Parameter("theAnswer", 42),
            new Parameter("foo", "bar") 
        });

        It should_have_the_parameters = () => spec.Parameters
            .Should().Count.Exactly(2)
            .Should().Contain.One(x => x.Name == "theAnswer" && x.Value.Equals(42))
            .Should().Contain.One(x => x.Name == "foo" && x.Value.Equals("bar"));			
    }

    public class when_adding_parameters_as_name_value_pairs_to_a_dbcommandspec : beahves_like_dbcommandspec_test
    {
        Because of = () => spec.AddParameter("theAnswer", 42).AddParameter("foo", "bar");

        It should_have_the_parameters = () => spec.Parameters
            .Should().Count.Exactly(2)
            .Should().Contain.One(x => x.Name == "theAnswer" && x.Value.Equals(42))
            .Should().Contain.One(x => x.Name == "foo" && x.Value.Equals("bar"));
    }

    public class when_adding_parameters_as_separate_lambdas_to_a_dbcommandspec : beahves_like_dbcommandspec_test
    {
        Because of = () => spec.AddParameters(theAnswer => 42).AddParameter(foo => "bar");

        It should_have_the_parameters = () => spec.Parameters
            .Should().Count.Exactly(2)
            .Should().Contain.One(x => x.Name == "theAnswer" && x.Value.Equals(42))
            .Should().Contain.One(x => x.Name == "foo" && x.Value.Equals("bar"));
    }

    public class when_adding_parameters_as_lambdas_to_a_dbcommandspec : beahves_like_dbcommandspec_test
    {
        Because of = () => spec.AddParameters(theAnswer => 42, foo => "bar");

        It should_have_the_parameters = () => spec.Parameters
            .Should().Count.Exactly(2)
            .Should().Contain.One(x => x.Name == "theAnswer" && x.Value.Equals(42))
            .Should().Contain.One(x => x.Name == "foo" && x.Value.Equals("bar"));
    }

    public class when_adding_parameters_as_an_anonymous_object_to_a_dbcommandspec : beahves_like_dbcommandspec_test
    {
        Because of = () => spec.AddParameters(new { theAnswer = 42, foo = "bar" });

        It should_have_the_parameters = () => spec.Parameters
            .Should().Count.Exactly(2)
            .Should().Contain.One(x => x.Name == "theAnswer" && x.Value.Equals(42))
            .Should().Contain.One(x => x.Name == "foo" && x.Value.Equals("bar"));
    }
}