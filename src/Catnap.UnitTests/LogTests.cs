using System;
using Catnap.Common.Logging;
using Machine.Specifications;
using Moq;
using It=Machine.Specifications.It;

namespace Catnap.UnitTests
{
    public abstract class behaves_like_logger_test
    {
        protected static MockFactory mockFactory = new MockFactory(MockBehavior.Strict);
        protected static Mock<ILogger> logger;

        Establish context = () =>
        {
            logger = mockFactory.Create<ILogger>();
            Log.Level = LogLevel.Debug;
            Log.Logger = logger.Object;
        };
    }

    public class when_logging_and_exception : behaves_like_logger_test
    {
        static Exception ex;

        Establish context = () =>
        {
            Log.Level = LogLevel.Error;
            ex = new Exception("Test message.", new Exception("Inner 1.", new Exception("Inner 2")));
            logger.Setup(x => x.LogMessage("Test message. Inner 1. Inner 2\r\n"));
        };

        Because of = () => Log.Error(ex);

        It should_not_throw = () => { };
    }

    public class when_log_level_is_info_then_log_debug : behaves_like_logger_test
    {
        Establish context = () =>  Log.Level = LogLevel.Info;
        Because of = () => Log.Debug("foo");
        It should_not_log = () => { };
    }
}