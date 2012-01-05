using System;
using Catnap.Database.Sqlite;
using Machine.Specifications;
using Should.Fluent;
using It=Machine.Specifications.It;

namespace Catnap.UnitTests
{
    public class beahves_like_sqlite_value_converter_from_db_tests<T>
    {
        protected static SqliteValueConverter target;
        protected static T expected;
        protected static T actual;
        Establish context = () => target = new SqliteValueConverter();
    }

    public class beahves_like_sqlite_value_converter_to_db_tests
    {
        protected static SqliteValueConverter target;
        protected static object actual;
        Establish context = () => target = new SqliteValueConverter();
    }

    public class when_converting_from_timespan : beahves_like_sqlite_value_converter_from_db_tests<TimeSpan>
    {
        const long dbValue = 320000000;
        Establish context = () => expected = new TimeSpan(dbValue);
        Because of = () => actual = (TimeSpan)target.ConvertFromDb(dbValue, typeof(TimeSpan));
        It should_convert = () => actual.Ticks.Should().Equal(expected.Ticks);
    }

    public class when_converting_from_nullable_timespan : beahves_like_sqlite_value_converter_from_db_tests<TimeSpan?>
    {
        const long dbValue = 320000000;
        Establish context = () => expected = new TimeSpan(dbValue);
        Because of = () => actual = target.ConvertFromDb(dbValue, typeof(TimeSpan?)) as TimeSpan?;
        It should_convert = () => actual.Value.Ticks.Should().Equal(expected.Value.Ticks);
    }

    public class when_converting_from_bool : beahves_like_sqlite_value_converter_from_db_tests<bool>
    {
        const long dbValue = 1;
        Establish context = () => expected = true;
        Because of = () => actual = (bool)target.ConvertFromDb(expected, typeof(bool));
        It should_convert = () => actual.Should().Be.True();
    }

    public class when_converting_from_nullable_bool : beahves_like_sqlite_value_converter_from_db_tests<bool?>
    {
        const long dbValue = 1;
        Establish context = () => expected = true;
        Because of = () => actual = target.ConvertFromDb(expected, typeof(bool?)) as bool?;
        It should_convert = () => actual.Should().Be.True();
    }

    public class when_converting_from_datetime : beahves_like_sqlite_value_converter_from_db_tests<DateTime>
    {
        Establish context = () => expected = new DateTime(2000, 1, 1);
        Because of = () => actual = (DateTime)target.ConvertFromDb(expected.Ticks, typeof(DateTime));
        It should_convert = () => actual.Ticks.Should().Equal(expected.Ticks);
    }

    public class when_converting_from_nullable_datetime : beahves_like_sqlite_value_converter_from_db_tests<DateTime?>
    {
        Establish context = () => expected = new DateTime(2000, 1, 1);
        Because of = () => actual = (DateTime)target.ConvertFromDb(expected.Value.Ticks, typeof(DateTime));
        It should_convert = () => actual.Value.Ticks.Should().Equal(expected.Value.Ticks);
    }

    public class when_converting_from_guid : beahves_like_sqlite_value_converter_from_db_tests<Guid>
    {
        Establish context = () => expected = Guid.NewGuid();
        Because of = () => actual = (Guid)target.ConvertFromDb(expected.ToString(), typeof(Guid));
        It should_convert = () => actual.Should().Equal(expected);
    }

    public class when_converting_from_nullable_guid : beahves_like_sqlite_value_converter_from_db_tests<Guid?>
    {
        Establish context = () => expected = Guid.NewGuid();
        Because of = () => actual = target.ConvertFromDb(expected.ToString(), typeof(Guid?)) as Guid?;
        It should_convert = () => actual.Should().Equal(expected);
    }

    public class when_converting_to_bool : beahves_like_sqlite_value_converter_to_db_tests
    {
        Because of = () => actual = target.ConvertToDb(true);
        It should_convert = () => actual.Should().Equal(1);
    }

    public class when_converting_to_nullable_bool : beahves_like_sqlite_value_converter_to_db_tests
    {
        Because of = () => actual = target.ConvertToDb((bool?)true);
        It should_convert = () => actual.Should().Equal(1);
    }

    public class when_converting_to_enum : beahves_like_sqlite_value_converter_to_db_tests
    {
        Because of = () => actual = target.ConvertToDb(TestEnum.One);
        It should_convert = () => actual.Should().Equal(1);
    }

    public class when_converting_to_nullable_enum : beahves_like_sqlite_value_converter_to_db_tests
    {
        Because of = () => actual = target.ConvertToDb((TestEnum?)TestEnum.One);
        It should_convert = () => actual.Should().Equal(1);
    }

    public class when_converting_to_date : beahves_like_sqlite_value_converter_to_db_tests
    {
        static readonly DateTime value = new DateTime(2000, 1, 1);
        Because of = () => actual = target.ConvertToDb(value);
        It should_convert = () => actual.Should().Equal(value.Ticks);
    }

    public class when_converting_to_nullable_date : beahves_like_sqlite_value_converter_to_db_tests
    {
        static readonly DateTime? value = new DateTime(2000, 1, 1);
        Because of = () => actual = target.ConvertToDb(value);
        It should_convert = () => actual.Should().Equal(value.Value.Ticks);
    }

    public class when_converting_to_timespan : beahves_like_sqlite_value_converter_to_db_tests
    {
        static readonly TimeSpan value = TimeSpan.FromTicks(320000000);
        Because of = () => actual = target.ConvertToDb(value);
        It should_convert = () => actual.Should().Equal(value.Ticks);
    }

    public class when_converting_to_nullable_timespan : beahves_like_sqlite_value_converter_to_db_tests
    {
        static readonly TimeSpan? value = TimeSpan.FromTicks(320000000);
        Because of = () => actual = target.ConvertToDb(value);
        It should_convert = () => actual.Should().Equal(value.Value.Ticks);
    }
    
    public class when_converting_to_guid : beahves_like_sqlite_value_converter_to_db_tests
    {
        static readonly Guid value = Guid.NewGuid();
        Because of = () => actual = target.ConvertToDb(value);
        It should_convert = () => actual.Should().Equal(value.ToString());
    }

    public class when_converting_to_nullable_guid : beahves_like_sqlite_value_converter_to_db_tests
    {
        static readonly Guid? value = Guid.NewGuid();
        Because of = () => actual = target.ConvertToDb(value);
        It should_convert = () => actual.Should().Equal(value.ToString());
    }

    public enum TestEnum
    {
        Zero,
        One,
        Two
    }
}