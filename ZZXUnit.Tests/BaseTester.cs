using Xunit.Abstractions;

namespace ZZXUnit.Tests
{
    public abstract class BaseTester
    {
        private readonly ITestOutputHelper _output;

        protected BaseTester(ITestOutputHelper outputHelper)
        {
            _output = outputHelper;
        }

        protected void WriteLine(string format, params object[] args)
        {
            _output.WriteLine(format, args);
        }
    }
}
