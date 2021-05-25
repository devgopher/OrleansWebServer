using System;
using System.Collections.Generic;
using System.Text;

namespace OrleansStatisticsKeeper.Grains.Tests.TestClasses
{
    public class TestExecutionContext
    {
        public double Test(int a, int b)
            => Math.Pow(a * b, 2);
    }
}
