using System;
using System.Collections.Generic;

using Xunit;
using Xunit.Abstractions;
using ZZXUnit.Tests.Entities.SpiderOdds;

namespace ZZXUnit.Tests.Checking
{
    public class TypeTest
    {
        private readonly ITestOutputHelper _output;

        public TypeTest(ITestOutputHelper outputHelper)
        {
            _output = outputHelper;
        }

        [Fact]
        public void Integer_Test()
        {
            const int VALUE = 1000;
            var type = VALUE.GetType();
            _output.WriteLine("{0}: {1}", VALUE, type);

            Assert.Equal(VALUE, VALUE);
            _output.WriteLine("Done.");
        }

        [Fact]
        public void Long_Test()
        {
            const long VALUE = 1000;
            var type = VALUE.GetType();
            _output.WriteLine("{0}: {1}", VALUE, type);

            Assert.Equal(VALUE, VALUE);
            _output.WriteLine("Done.");
        }

        [Fact]
        public void Array_Test()
        {
            var arrays = new []
            {
                "Hello", "World"
            };

            var type = arrays.GetType();
            _output.WriteLine("{0}: {1}", arrays, type);

            _output.WriteLine("Done.");
        }


        [Fact]
        public void Dictionary_Test()
        {
            var arrays = new Dictionary<int, string>
            {
                {1,  "Hello"},
                {2, "World" }
            };

            var type = arrays.GetType();
            _output.WriteLine("{0}: {1}", arrays, type);

            _output.WriteLine("Done.");
        }

        [Fact]
        public void Object_Test()
        {
            var arrays = new object[]
            {
                "Hello", "World", 1, 3, DateTime.Now
            };

            var type = arrays.GetType();
            _output.WriteLine("{0}: {1}", arrays, type);

            _output.WriteLine("Done.");
        }


        [Fact]
        public void Class_Test()
        {
            var value = new ThirdValueHdp
            {
                Away = 0.98M,
                Home = 0.88M,
                Hdp = -0.25M,
            };

            value.OwnHash = value.CreateOwnHash();

            var type = value.GetType();
            _output.WriteLine("{0}: {1}", value, type);

            _output.WriteLine("Done.");
        }
    }
}
