using System;
using Catnap.Adapters.Sqlite;
using Machine.Specifications;
using Should.Fluent;
using It=Machine.Specifications.It;

namespace Catnap.UnitTests
{
    public class when_converting_to_nullable_timespan
    {
        const long dbValue = 320000000;
        static TimeSpan? expected;
        static SqliteTypeConverter target;
        static TimeSpan? actual;

        Establish context = () =>
        {
            target = new SqliteTypeConverter();
            expected = new TimeSpan(dbValue);
        };

        Because of = () =>
        {
            actual = (TimeSpan?)target.ConvertFromDbType(dbValue, typeof(TimeSpan?));
        };

        It should_convert = () => actual.Value.Ticks.Should().Equal(expected.Value.Ticks);
    }
}