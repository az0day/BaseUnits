using System;
using System.Linq;
using System.Globalization;
using Xunit;
using Xunit.Abstractions;

namespace ZZXUnit.Tests
{
    public class UnitTest : BaseTester
    {
        public UnitTest(ITestOutputHelper outputHelper) : base(outputHelper)
        {

        }

        [Fact]
        public void DateTime_ToString_Test()
        {
            var date = DateTime.Now;

            WriteLine("en-US yyyyMMdd: {0}", date.ToString("yyyyMMdd", new CultureInfo("en-US")));
            WriteLine("zh-CN yyyyMMdd: {0}", date.ToString("yyyyMMdd", new CultureInfo("zh-CN")));
            WriteLine("th-TH yyyyMMdd: {0}", date.ToString("yyyyMMdd", new CultureInfo("th-TH")));

            Assert.True(true);
        }

        [Theory]
        [InlineData(10, 10)]
        [InlineData(20, 20)]
        public void Int_Equal(int input, int expected)
        {
            Assert.Equal(input, expected);
            WriteLine("Done.");
        }

        [Fact]
        public void Bools_Test()
        {
            var falses = new bool[]
            {
                false,
                false
            };

            var shouldFalse = falses.Any(o => o);
            WriteLine("Should be False: {0}", shouldFalse);
            Assert.False(shouldFalse);

            var existTrues = new bool[]
            {
                false,
                true
            };

            var shouldTrue = existTrues.Any(o => o);
            WriteLine("Should be True: {0}", shouldTrue);

            Assert.True(true);
        }

        [Theory]
        [InlineData("sss")]
        public void Equals_Empty_Test(string username)
        {
            var eq = username.Equals("");
            Assert.False(eq);
            WriteLine("Done.");
        }
    }
}
